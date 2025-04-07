using System.Collections.ObjectModel;
using System.Windows.Input;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Commands;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Web;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _newUsername;

        private string _selectedImagePath;

        private UserModel _selectedUser;

        public string NewUsername
        {
            get => _newUsername;
            set => SetProperty(ref _newUsername, value);
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
        }

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value) && value != null)
                {
                    SelectedImagePath = value.ImagePath;
                }
            }
        }

        public ObservableCollection<UserModel> Users { get; set; }
        
        public ICommand AddUserCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand PlayGameCommand { get;  }
    
        
        public LoginViewModel()
        {
            Users = new ObservableCollection<UserModel>(FileService.LoadUsers());
            if (Users.Count > 0)
                SelectedUser = Users[0];
            AddUserCommand = new RelayCommand(AddUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
            PlayGameCommand = new RelayCommand(PlayGame, () => SelectedUser != null);

        }

        private void AddUser()
        {
            var addUserDialog = new AddUserView();
            var addUserVM = new AddUserViewModel();
            addUserDialog.DataContext = addUserVM;

            bool? result = addUserDialog.ShowDialog();

            if (result == true && !string.IsNullOrWhiteSpace(addUserVM.Username))
            {
                if (!Users.Any(u => u.Username == addUserVM.Username))
                {
                    var newUser = new UserModel(addUserVM.Username, addUserVM.ImagePath);
                    Users.Add(newUser);
                    FileService.SaveUsers(Users.ToList());
                    SelectedUser = newUser;
                }
                else
                {
                    System.Windows.MessageBox.Show("Username already exists!", "Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void DeleteUser()
        {
            if(SelectedUser != null)
            {
                UserModel userToDelete = SelectedUser;
                SelectedUser = null;
                Users.Remove(userToDelete);
                FileService.DeleteUserData(userToDelete.Username);
                if (Users.Count > 0)
                {
                    SelectedUser = Users[0];
                }
            }
        }


        private void PlayGame()
        {
            if (SelectedUser != null)
            {
                var gameVM = new GameViewModel(SelectedUser.Username);
                var gameView = new Views.GameView
                {
                    DataContext = gameVM
                };

                gameView.Show();

                var currentWindow = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault();
                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
            }
        }
    }


}
