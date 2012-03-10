namespace RePhoto.ViewModels {
    public class CameraViewModel : ViewModelBase {
        private bool cameraInUse;

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