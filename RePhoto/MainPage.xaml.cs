﻿using System;
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

namespace RePhoto
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel actionItems
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedItem != null)
            {
                var itemViewModel = listBox.SelectedItem as ItemViewModel;
                NavigationService.Navigate(new Uri(itemViewModel.NavigationUri, UriKind.Relative));
                listBox.SelectedItem = null;
            }
        }

    }
}