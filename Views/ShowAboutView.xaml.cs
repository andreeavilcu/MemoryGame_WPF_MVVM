using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for ShowAboutView.xaml
    /// </summary>
    public partial class ShowAboutView : Window
    {
        public ShowAboutView()
        {
            InitializeComponent();
            DataContext = new ShowAboutViewModel(this);
        }

        
    }

}
