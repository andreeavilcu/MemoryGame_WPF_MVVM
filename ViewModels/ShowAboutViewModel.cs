using MemoryGame.Commands;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MemoryGame.ViewModels
{
    public class ShowAboutViewModel : BaseViewModel
    {
        private readonly Window _window;
        private string _emailAddress = "mailto:andreea.vilcu@student.unitbv.ro";

        public string EmailAddress
        {
            get => _emailAddress;
            set => SetProperty(ref _emailAddress, value);
        }
        public ICommand CloseCommand { get; }
        public ICommand NavigateCommand { get; }
        public ShowAboutViewModel(Window window)
        {
            _window = window;
            CloseCommand = new RelayCommand(Close);
            NavigateCommand = new RelayCommand(HandleHyperlinkNavigation);
        }

        private void Close()
        {
            _window?.Close();
        }

        private void HandleHyperlinkNavigation()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = EmailAddress,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open email client: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

