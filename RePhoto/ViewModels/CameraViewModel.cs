using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RePhoto.ViewModels
{
    public class CameraViewModel : ViewModelBase
    {
        private ImageSource _opacityMask;
        private bool cameraInUse;
        private bool configuringSettings;
        private double maskOpacityLevel = .5;
        private int maskSizeLevel = 25;
        private ImageSource overlayedPicture;
        private ImageSource picture;
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


        public ImageSource Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                NotifyPropertyChanged("Picture");
            }
        }

        public ImageSource OverlayedPicture
        {
            get { return overlayedPicture; }
            set
            {
                overlayedPicture = value;
                NotifyPropertyChanged("OverlayedPicture");
            }
        }

        public bool CameraInUse
        {
            get { return cameraInUse; }
            set
            {
                cameraInUse = value;
                NotifyPropertyChanged("CameraInUse");
            }
        }

        public bool ConfiguringSettings
        {
            get { return configuringSettings; }
            set
            {
                configuringSettings = value;
                NotifyPropertyChanged("ConfiguringSettings");
            }
        }

        public ImageSource OpacityMask
        {
            get { return _opacityMask; }
            set
            {
                _opacityMask = value;
                NotifyPropertyChanged("OpacityMask");
            }
        }

        public double MaskOpacityLevel
        {
            get { return maskOpacityLevel; }
            set
            {
                maskOpacityLevel = value;
                NotifyPropertyChanged("MaskOpacityLevel");
            }
        }

        public int MaskSizeLevel
        {
            get { return maskSizeLevel; }
            set
            {
                maskSizeLevel = value;
                NotifyPropertyChanged("MaskSizeLevel");
                selectedFill.Value.Invoke();
            }
        }

        public KeyValuePair<string, Action> SelectedFill
        {
            get { return selectedFill; }
            set { selectedFill = value;
                value.Value.Invoke();
            }
        }

        public IDictionary<string, Action> Fills { get; set; }

        public void CheckerBoard()
        {
            var opacityMask = new WriteableBitmap(480, 800);
            for (int i = 0; i < 480/MaskSizeLevel; i += 2)
            {
                for (int j = 0; j < 800/MaskSizeLevel; j += 2)
                {
                    opacityMask.FillRectangle(i*MaskSizeLevel, j*MaskSizeLevel, (i + 1)*MaskSizeLevel, (j + 1)*MaskSizeLevel,
                                              Colors.Black);
                }
            }            
            for (int i = 1; i < 480/MaskSizeLevel; i += 2)
            {
                for (int j = 1; j < 800/MaskSizeLevel; j += 2)
                {
                    opacityMask.FillRectangle(i*MaskSizeLevel, j*MaskSizeLevel, (i + 1)*MaskSizeLevel, (j + 1)*MaskSizeLevel,
                                              Colors.Black);
                }
            }
            OpacityMask = opacityMask;
        }

        private void VerticalLines()
        {
            var opacityMask = new WriteableBitmap(480, 800);
            for (int i = 0; i < 480 / MaskSizeLevel; i += 2)
            {
                opacityMask.FillRectangle(i*MaskSizeLevel, 0, (i + 1)*MaskSizeLevel, 800, Colors.Black);
            }
            OpacityMask = opacityMask;
        }

        private void HorizontalLines()
        {
            var opacityMask = new WriteableBitmap(480, 800);
            for (int j = 0; j < 800 / MaskSizeLevel; j += 2)
            {
                opacityMask.FillRectangle(0, j*MaskSizeLevel, 480, (j + 1)*MaskSizeLevel, Colors.Black);
            }
            OpacityMask = opacityMask;
        }

        private void FullScreen()
        {
            var writeableBitmap = new WriteableBitmap(480, 800);
            writeableBitmap.Clear(Colors.Black);
            OpacityMask = writeableBitmap;
        }


        public override void LoadData() {}
    }
}