using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using RePhoto.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace RePhoto {
    public partial class Rephoto : PhoneApplicationPage {
        private readonly CameraViewModel cameraViewModel = new CameraViewModel();
        private readonly PhotoChooserTask photoChooserTask = new PhotoChooserTask();
        private readonly MediaLibrary library = new MediaLibrary();
        private PhotoCamera camera;
        private bool cameraInitialized;

        public Rephoto() {
            InitializeComponent();
            StartCameraService();
            DataContext = cameraViewModel;
            var overlayedPicture = new BitmapImage();
            overlayedPicture.CreateOptions = BitmapCreateOptions.None;
            overlayedPicture.UriSource = new Uri("/SplashScreenImage.jpg", UriKind.Relative);
            cameraViewModel.OverlayedPicture = new WriteableBitmap(overlayedPicture);
            photoChooserTask.Completed += PhotoChooserTaskOnCompleted;
            photoChooserTask.ShowCamera = true;
        }

        private void PhotoChooserTaskOnCompleted(object sender, PhotoResult photoResult) {
            if (photoResult.TaskResult.Equals(TaskResult.OK)) {
                var bitmapImage = new BitmapImage();
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.SetSource(photoResult.ChosenPhoto);
                Deployment.Current.Dispatcher.BeginInvoke(()=>cameraViewModel.OverlayedPicture = new WriteableBitmap(bitmapImage));
            }
            StartCameraService();
        }

        private void StartCameraService() {
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
        private void cam_Initialized(object sender, CameraOperationCompletedEventArgs e) {
            if (e.Succeeded) {
                cameraInitialized = true;
                cameraViewModel.SetImageSize(camera.Resolution);
                Deployment.Current.Dispatcher.BeginInvoke(() => cameraViewModel.SelectedFill.Value.Invoke());
            }
        }

        private void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e) {
            try {
                string fileName = Guid.NewGuid().ToString("N") + ".jpg";
                // Save picture to the library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);
                // Set the position of the stream back to start
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                // Save picture as JPEG to isolated storage.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile("temp.dat", FileMode.Create, FileAccess.Write)) {
                        // Initialize the buffer for 4KB disk pages.
                        var readBuffer = new byte[4096];
                        int bytesRead;

                        // Copy the image to isolated storage. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                        targetStream.Flush();
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(ReadImage);
            } catch (Exception exception) {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.ToString()));
            } finally {
                // Close image stream
                e.ImageStream.Close();
            }
        }
/*
        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
            if (camera != null) {
                // LandscapeRight rotation when camera is on back of device.
                int landscapeRightRotation = 180;
                // Change LandscapeRight rotation for front-facing camera.
                if (camera.CameraType == CameraType.FrontFacing) {
                    landscapeRightRotation = -180;
                }
                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight) {
                    // Rotate for LandscapeRight orientation.
                    viewFinderBrush.RelativeTransform =
                        new CompositeTransform {CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation};
                } else {
                    // Rotate for standard landscape orientation.
                    viewFinderBrush.RelativeTransform =
                        new CompositeTransform {CenterX = 0.5, CenterY = 0.5, Rotation = 0};
                }
            }

            base.OnOrientationChanged(e);
        }*/

        private void ReadImage() {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication()) {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("temp.dat", FileMode.Open, FileAccess.Read)) {
                    var picture = new BitmapImage();
                    picture.SetSource(fileStream);
                    cameraViewModel.Picture = new WriteableBitmap(picture).Rotate(90);
                }
            }
        }

        // Informs when full resolution picture has been taken, saves to local media library and isolated storage.
        private void CameraCaptureImageCompleted(object sender, ContentReadyEventArgs e) {
            Deployment.Current.Dispatcher.BeginInvoke(() => {
                                                          lock (this) {
                                                              //cameraViewModel.CameraInUse = false;
                                                          }
                                                      });
        }

        private void CameraAutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e) {
            Action action = () => {
                                lock (this) {
                                    try {
                                        camera.CaptureImage();
                                    } catch (Exception exception) {
                                        Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.ToString()));
                                    }
                                }
                            };
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        private void CameraCaptureClick(object sender, EventArgs e) {
            cameraViewModel.CameraInUse = true;
            camera.Focus();
        }

        private void SavePhoto(object sender, RoutedEventArgs e) {
            cameraViewModel.SavePhoto();
            cameraViewModel.Picture = null;
        }

        private void RetakePhoto(object sender, RoutedEventArgs e) {
            cameraViewModel.Picture = null;
        }

        private void AcceptSettings(object sender, EventArgs e) {
            cameraViewModel.ConfiguringSettings = false;
            cameraViewModel.CameraInUse = false;
        }

        private void ConfigureSettings(object sender, EventArgs eventArgs) {
            bool configuringSettings = !cameraViewModel.ConfiguringSettings;
            cameraViewModel.ConfiguringSettings = configuringSettings;
            cameraViewModel.CameraInUse = configuringSettings;
        }

        private void CameraCaptureTap(object sender, GestureEventArgs e) {
            cameraViewModel.CameraInUse = true;
            camera.Focus();
        }

        private void ChooseOverlay(object sender, EventArgs e) {
            photoChooserTask.Show();
        }
    }
}