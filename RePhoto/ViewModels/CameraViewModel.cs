using System.Windows.Media;

namespace RePhoto.ViewModels {
    public class CameraViewModel : ViewModelBase {
        private bool cameraInUse;
        private ImageSource picture;

        public ImageSource Picture {
            get { return picture; }
            set {
                picture = value;
                NotifyPropertyChanged("picture");
            }
        }

        public bool CameraInUse {
            get { return cameraInUse; }
            set {
                cameraInUse = value;
                NotifyPropertyChanged("CameraInUse");
            }
        }

        public override void LoadData() {}
    }
}