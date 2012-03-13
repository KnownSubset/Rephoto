using System.Collections.Generic;
using System.Windows.Media;

namespace RePhoto.ViewModels
{
    public class CameraViewModel : ViewModelBase
    {
        private ImageSource _opacityMask;
        private bool cameraInUse;
        private bool configuringSettings;
        private IList<string> fills = new List<string>(){"squares", "lines", "half"};
        private int maskOpacityLevel;
        private int maskSizeLevel;
        private ImageSource overlayedPicture;
        private ImageSource picture;

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

        public int MaskOpacityLevel
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
            }
        }

        public IList<string> Fills
        {
            get { return fills; }
            set
            {
                fills = value;
                NotifyPropertyChanged("Fills");
            }
        }

        public override void LoadData() {}
    }
}