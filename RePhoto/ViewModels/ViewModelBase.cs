using System;
using System.ComponentModel;

namespace RePhoto.ViewModels {
    public abstract class ViewModelBase : INotifyPropertyChanged {
        protected ViewModelBase() {
            LoadData();
        }

        public bool IsDataLoaded { get; protected set; }
        public abstract void LoadData();

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}