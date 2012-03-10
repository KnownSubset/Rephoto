using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RePhoto.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> projects = new ObservableCollection<ItemViewModel>();

        public ObservableCollection<ItemViewModel> Projects {
            get { return projects; }
            set {
                if (projects != value) {
                    projects = value;
                }
                NotifyPropertyChanged("Projects");
            }
        }

        public override void LoadData() {
            Projects.Add(new ItemViewModel() { ImageUri = "Images\\logo.jpg", LineOne = "Historic WashU", LineTwo = "1850's era" });
            Projects.Add(new ItemViewModel() { ImageUri = "Images\\logo.jpg", LineOne = "Near Historic WashU", LineTwo = "1900's era" });
            Projects.Add(new ItemViewModel() { ImageUri = "Images\\logo.jpg", LineOne = "Hippy WashU", LineTwo = "1960's era" });
            this.Projects.Add(new ItemViewModel() { ImageUri = "Images\\logo.jpg", LineOne = "Current WashU", LineTwo = "2000's era" });
        }
    }
}
