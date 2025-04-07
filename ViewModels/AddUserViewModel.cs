using MemoryGame.Commands;
using Microsoft.Win32;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class AddUserViewModel : BaseViewModel
    {
        private string _username;
        private string _imagePath = "pack://application:,,,/MemoryGame;component/Resources/UserPictures/default-user.png";

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public ICommand BrowseImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddUserViewModel()
        {
            BrowseImageCommand = new RelayCommand(BrowseImage);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void BrowseImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Select user image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Username);
        }

        private void Save()
        { 
            var window = Application.Current.Windows.OfType<Views.AddUserView>().FirstOrDefault();
            if (window != null)
            { 
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel()
        {
            var window = Application.Current.Windows.OfType<Views.AddUserView>().FirstOrDefault();
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

    }
}
