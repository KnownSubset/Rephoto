using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using RePhoto.ViewModels;

namespace RePhoto {
    public partial class ProjectsPage : PhoneApplicationPage {
        private readonly ProjectsViewModel projectsViewModel = new ProjectsViewModel();

        public ProjectsPage() {
            InitializeComponent();
            // Set the data context of the listbox control to the sample data
            DataContext = projectsViewModel;
            this.Loaded += MainPage_Loaded;
        }

        // Load data for the ViewModel Projects
        private void MainPage_Loaded(object sender, RoutedEventArgs e) {
            if (!projectsViewModel.IsDataLoaded) {
                projectsViewModel.LoadData();
            }
        }
    }
}