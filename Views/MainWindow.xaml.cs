﻿using MemoryGame.ViewModels;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}