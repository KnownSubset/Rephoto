using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using RePhoto.ViewModels;

namespace RePhoto
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> actionItems = new ObservableCollection<ItemViewModel>();

        public ObservableCollection<ItemViewModel> ActionItems
        {
            get { return actionItems; }
            set
            {
                if (actionItems != null && actionItems != value)
                {
                    actionItems = value;
                }
                NotifyPropertyChanged("actionItems");
            }
        }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the actionItems collection.
        /// </summary>
        public override void LoadData()
        {
            // Sample data; replace with real data
            this.ActionItems.Add(new ItemViewModel() { LineOne = "view projects", LineTwo = "find citizen Rephoto projects near you", NavigationUri = "/ProjectsPage.xaml" });
            this.ActionItems.Add(new ItemViewModel() { LineOne = "personal rephoto", LineTwo = "take some personal Rehoto pictures", NavigationUri = "/Rephoto.xaml"});

            this.IsDataLoaded = true;
        }

    }
}