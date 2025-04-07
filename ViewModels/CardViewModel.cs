using MemoryGame.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MemoryGame.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private int _id;
        private string _imagePath;
        private bool _isFlipped;
        private bool _isMatched;
        private bool _isEnabled = true;
        private BitmapImage _image;
        private BitmapImage _backImage;

        public int Row { get; set; }
        public int Column { get; set; }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (SetProperty(ref _imagePath, value))
                {
                    LoadImage();
                }
            }
        }

        //public string ImagePath
        //{
        //    get => _imagePath;
        //    set
        //    {
        //        if (SetProperty(ref _imagePath, value))
        //        {
        //            LoadImageFromResource(value);
        //        }
        //    }
        //}


        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (SetProperty(ref _isFlipped, value))
                {
                    OnPropertyChanged(nameof(DisplayImage));
                }
            }
        }

        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                if (SetProperty(ref _isMatched, value))
                {
                    IsEnabled = !value;
                }
            }

        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public BitmapImage Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
        }

        public BitmapImage BackImage
        {
            get => _backImage;
            set => SetProperty(ref _backImage, value);
        }


        public BitmapImage DisplayImage => IsFlipped ? Image : BackImage;

        public ICommand FlipCommand { get; }

        public event EventHandler<CardViewModel> CardClicked;

        public CardViewModel()
        {
            FlipCommand = new RelayCommand(OnCardClicked, () => IsEnabled && !IsMatched);
            LoadBackImage();
        }

        private void OnCardClicked()
        {
            if(!IsEnabled || IsMatched || IsFlipped)
            {
                return;
            }

            IsFlipped = true;
            CardClicked?.Invoke(this, this);
        }

        //private void LoadImageFromResource(string resourcePath)
        //{
        //    try
        //    {
        //        var uri = new Uri($"pack://application:,,,/MemoryGame;component/Resources/Animals/{resourcePath}", UriKind.Absolute);
        //        BitmapImage image = new BitmapImage();
        //        image.BeginInit();
        //        image.UriSource = uri;
        //        image.CacheOption = BitmapCacheOption.OnLoad;
        //        image.EndInit();
        //        Image = image;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Failed to load image from resource: {resourcePath}. Error: {ex.Message}");
        //    }
        //}


        private void LoadImage()
        {
            if (string.IsNullOrEmpty(ImagePath))
            {
                System.Diagnostics.Debug.WriteLine("ImagePath is null or empty");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading image from: {ImagePath}");
                BitmapImage image = new BitmapImage();
                image.BeginInit();

                // Abordare simplificată - tratează calea ca pe un fișier direct
                try
                {
                    // Încearcă să încarce ca fișier local
                    image.UriSource = new Uri(ImagePath);
                }
                catch
                {
                    // Dacă nu funcționează, încearcă ca URI de tip pack
                    image.UriSource = new Uri("pack://application:,,,/MemoryGame;component/Resources/default.png");
                }

                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                Image = image;
                System.Diagnostics.Debug.WriteLine("Image loaded successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void LoadBackImage()
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("pack://application:,,,/MemoryGame;component/Resources/card-back.png", UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                BackImage = image;
            }
            catch
            {
                BackImage = new BitmapImage(new Uri("pack://application:,,,/MemoryGame;component/Resources/default-back.png", UriKind.Absolute));
            }
        }
        
    }
}