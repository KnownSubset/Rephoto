using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace RePhoto.ViewModels {
    public class CameraViewModel : ViewModelBase {
        private bool cameraInUse;
        private bool configuringSettings;
        private double maskOpacityLevel = .5;
        private int maskSizeLevel = 25;
        private WriteableBitmap opacityMask;
        private ImageSource overlayedPicture;
        private BitmapImage picture;
        private KeyValuePair<string, Action> selectedFill;

        public CameraViewModel()
        {
            IDictionary<string, Action> fills = new Dictionary<string, Action>();
            SelectedFill = new KeyValuePair<string, Action>("checkerboard", CheckerBoard);
            fills.Add(selectedFill);
            fills.Add("vertical lines", VerticalLines);
            fills.Add("horizontal lines", HorizontalLines);
            fills.Add("full screen", FullScreen);
            Fills = fills;
        }

        public BitmapImage Picture
        {
            get { return picture; }
            set {
                picture = value;
                NotifyPropertyChanged("Picture");
            }
        }

        public ImageSource OverlayedPicture {
            get { return overlayedPicture; }
            set {
                overlayedPicture = value;
                NotifyPropertyChanged("OverlayedPicture");
            }
        }

        public bool CameraInUse {
            get { return cameraInUse; }
            set {
                cameraInUse = value;
                NotifyPropertyChanged("CameraInUse");
            }
        }

        public bool ConfiguringSettings {
            get { return configuringSettings; }
            set {
                configuringSettings = value;
                NotifyPropertyChanged("ConfiguringSettings");
            }
        }

        public WriteableBitmap OpacityMask
        {
            get { return opacityMask; }
            set {
                opacityMask = value;
                NotifyPropertyChanged("OpacityMask");
            }
        }

        public double MaskOpacityLevel {
            get { return maskOpacityLevel; }
            set {
                maskOpacityLevel = value;
                NotifyPropertyChanged("MaskOpacityLevel");
            }
        }

        public int MaskSizeLevel {
            get { return maskSizeLevel; }
            set {
                maskSizeLevel = value;
                NotifyPropertyChanged("MaskSizeLevel");
                selectedFill.Value.Invoke();
            }
        }

        public KeyValuePair<string, Action> SelectedFill {
            get { return selectedFill; }
            set {
                selectedFill = value;
                value.Value.Invoke();
            }
        }

        public IDictionary<string, Action> Fills { get; set; }
        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }

        public void CheckerBoard() {
            var opacityMask = new WriteableBitmap((int)ImageWidth, (int)ImageHeight);
            for (int i = 0; i < ImageWidth/MaskSizeLevel; i += 2) {
                for (int j = 0; j < ImageHeight/MaskSizeLevel; j += 2) {
                    opacityMask.FillRectangle(i*MaskSizeLevel, j*MaskSizeLevel, (i + 1)*MaskSizeLevel, (j + 1)*MaskSizeLevel,Colors.Black);
                }
            }
            for (int i = 1; i < ImageWidth/MaskSizeLevel; i += 2) {
                for (int j = 1; j < ImageHeight/MaskSizeLevel; j += 2) {
                    opacityMask.FillRectangle(i*MaskSizeLevel, j*MaskSizeLevel, (i + 1)*MaskSizeLevel, (j + 1)*MaskSizeLevel,Colors.Black);
                }
            }
            OpacityMask = opacityMask;
        }

        private void VerticalLines() {
            var opacityMask = new WriteableBitmap((int)ImageWidth, (int)ImageHeight);
            for (int i = 0; i < ImageWidth/MaskSizeLevel; i += 2) {
                opacityMask.FillRectangle(i * MaskSizeLevel, 0, (i + 1) * MaskSizeLevel, (int)ImageHeight, Colors.Black);
            }
            OpacityMask = opacityMask;
        }

        private void HorizontalLines() {
            var opacityMask = new WriteableBitmap((int)ImageWidth, (int)ImageHeight);
            for (int j = 0; j < ImageHeight/MaskSizeLevel; j += 2) {
                opacityMask.FillRectangle(0, j * MaskSizeLevel, (int)ImageWidth, (j + 1) * MaskSizeLevel, Colors.Black);
            }
            OpacityMask = opacityMask;
        }

        private void FullScreen() {
            var writeableBitmap = new WriteableBitmap((int) ImageWidth,(int) ImageHeight);
            writeableBitmap.Clear(Colors.Black);
            OpacityMask = writeableBitmap;
        }

        public override void LoadData() {}

        public void SavePhoto() {
            var writeableBitmap = new WriteableBitmap(picture);
            writeableBitmap.ForEach(Fill);
              // Create a virtual store and file stream. Check for duplicate tempJPEG files.
            string tempJPEG = CreateFileName();
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(tempJPEG)){
                myStore.DeleteFile(tempJPEG);
            }
            IsolatedStorageFileStream myFileStream = myStore.CreateFile(tempJPEG);
            writeableBitmap.SaveJpeg(myFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
            myFileStream.Close();

            // Create a new stream from isolated storage, and save the JPEG file to the media library on Windows Phone.
            myFileStream = myStore.OpenFile(tempJPEG, FileMode.Open, FileAccess.Read);
            string fileName = CreateFileName();
            new MediaLibrary().SavePictureToCameraRoll(fileName, myFileStream);
        }

        private Color Fill(int x, int y, Color originalColor) {
            Color maskColor = OpacityMask.GetPixel(x, y);
            bool isBlank = maskColor.A == 0;
            return isBlank ? originalColor : maskColor;
        }

        private string CreateFileName() {
            return Guid.NewGuid().ToString() + ".jpg";
        }
    }
}