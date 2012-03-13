using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using RePhoto.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace RePhoto
{
    public partial class Rephoto : PhoneApplicationPage
    {
        private readonly CameraViewModel cameraViewModel = new CameraViewModel();
        private readonly MediaLibrary library = new MediaLibrary();
        private PhotoCamera camera;
        private bool cameraInitialized;

        public Rephoto()
        {
            InitializeComponent();
            StartCameraService();
            DataContext = cameraViewModel;
            var overlayedPicture = new BitmapImage(new Uri("/SplashScreenImage.jpg", UriKind.Relative));
            cameraViewModel.OverlayedPicture = overlayedPicture;
            var opacityMask = new WriteableBitmap(480, 800);
            opacityMask.Clear(Color.FromArgb(0, 0,0,0));
            opacityMask.FillRectangle(100, 300, 300, 600, Colors.Black);
            cameraViewModel.OpacityMask = opacityMask;
        }


        private void StartCameraService()
        {
            camera = new PhotoCamera(CameraType.Primary);
            // Event is fired when the PhotoCamera object has been initialized.
            camera.Initialized += cam_Initialized;

            // Event is fired when the capture sequence is complete and an image is available.
            camera.CaptureImageAvailable += cam_CaptureImageAvailable;
            camera.CaptureImageAvailable += CameraCaptureImageCompleted;

            // The event is fired when auto-focus is complete.
            camera.AutoFocusCompleted += CameraAutoFocusCompleted;

            cameraInitialized = false;
            viewFinderBrush.SetSource(camera);
        }

        // Update the UI if initialization succeeds.
        private void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                cameraInitialized = true;
            }
        }


        private void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N") + ".jpg";
                // Save picture to the library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);
                // Set the position of the stream back to start
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                // Save picture as JPEG to isolated storage.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication()){
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile("temp.dat", FileMode.Create, FileAccess.Write)){
                        // Initialize the buffer for 4KB disk pages.
                        var readBuffer = new byte[4096];
                        int bytesRead;

                        // Copy the image to isolated storage. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0){
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(() => {
                                                              var picture =
                                                                  new BitmapImage(new Uri("/SplashScreenImage.jpg", UriKind.Relative));
                                                              /*using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("temp.dat", FileMode.Open, FileAccess.Read))
                        {
                            picture.SetSource(fileStream);
                        }
                    }*/
                        
                                                              cameraViewModel.Picture = picture;
                                                          });
            } catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.ToString()));
            } finally
            {
                // Close image stream
                e.ImageStream.Close();
            }
        }

        // Informs when full resolution picture has been taken, saves to local media library and isolated storage.
        private void CameraCaptureImageCompleted(object sender, ContentReadyEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => {
                                                          lock (this)
                                                          {
                                                              //cameraViewModel.CameraInUse = false;
                                                          }
                                                      });
        }

        private void CameraAutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            Action action = () => {
                                lock (this)
                                {
                                    try
                                    {
                                        camera.CaptureImage();
                                    } catch (Exception exception)
                                    {
                                        Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.ToString()));
                                    }
                                }
                            };
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        private void CameraCaptureClick(object sender, EventArgs e)
        {
            cameraViewModel.CameraInUse = true;
            camera.Focus();
        }


        private void SavePhoto(object sender, RoutedEventArgs e) {}

        private void RetakePhoto(object sender, RoutedEventArgs e)
        {
            cameraViewModel.Picture = null;
        }

        private void AcceptSettings(object sender, EventArgs e)
        {
            cameraViewModel.ConfiguringSettings = false;
            cameraViewModel.CameraInUse = false;
        }

        private void ConfigureSettings(object sender, RoutedEventArgs e)
        {
            cameraViewModel.ConfiguringSettings = true;
            cameraViewModel.CameraInUse = true;
        }

        private void CameraCaptureTap(object sender, GestureEventArgs e)
        {
            cameraViewModel.CameraInUse = true;
            camera.Focus();
        }
    }
}