using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;


namespace MemoryGame.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly string _currentUser;
        private CardViewModel _firstCard;
        private CardViewModel _secondCard;
        private DispatcherTimer _gameTimer;
        private string _category = "Animals";

        private int _timeLeft;
        private int _rows = 4;
        private int _columns = 4;
        private bool _isCustomMode = false;
        private int _totalGameTimeSeconds = 120;
        private int _remainingTimeSeconds = 120;
        private bool _isGameActive = false;
        private bool _isGameWon = false;
        private bool _canFlipCards = true;

        public ObservableCollection<CardViewModel> Cards { get; } = new ObservableCollection<CardViewModel>();

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>
        {
            "Animals",
            "Fruits",
            "Vegetables"
        };

        public string Category
        {
            get => _category;
            set
            {
                if (SetProperty(ref _category, value))
                {
                    if (IsGameActive)
                    {
                        StartNewGame();
                    }
                }
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                if (value >= 2 && value <= 6 && SetProperty(ref _rows, value))
                {
                    if (Rows * Columns % 2 != 0)
                    {
                        if (Columns < 6)
                            Columns++;
                        else
                            Columns = 2;
                    }
                    if (IsGameActive)
                    {
                       
                        StartNewGame();
                    }
                }
            }
        }
        public int Columns
        {
            get => _columns;
            set
            {
                if (value >= 2 && value <= 6 && SetProperty(ref _columns, value))
                {
                    if (Rows * Columns % 2 != 0)
                    {
                        if (Rows < 6)
                            Rows++;
                        else
                            Rows = 2;
                    }
                    if (IsGameActive)
                    {
                        
                        StartNewGame();
                    }
                }
            }
        }

        public bool IsCustomMode
        {
            get => _isCustomMode;
            set
            {
                if (SetProperty(ref _isCustomMode, value))
                {
                    if (IsGameActive)
                    {
                        StartNewGame();
                    }
                }
            }
        }

        public int TotalGameTimeSeconds
        {
            get => _totalGameTimeSeconds;
            set
            {
                if (value >= 30 && SetProperty(ref _totalGameTimeSeconds, value))
                {
                    RemainingTimeSeconds = value;
                }
            }

        }

        public int RemainingTimeSeconds
        {
            get => _remainingTimeSeconds;
            set => SetProperty(ref _remainingTimeSeconds, value);
        }

        public string RemainingTimeDisplay => TimeSpan.FromSeconds(RemainingTimeSeconds).ToString(@"mm\:ss");

        public bool IsGameActive
        {
            get => _isGameActive;
            set => SetProperty(ref _isGameActive, value);
        }

        public bool IsGameWon
        {
            get => _isGameWon;
            set => SetProperty(ref _isGameWon, value);
        }

        public bool CanFlipCards
        {
            get => _canFlipCards;
            set => SetProperty(ref _canFlipCards, value);
        }


        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand ShowStatisticsCommand { get; }

        public ICommand ExitCommand { get; }
        public ICommand SetStandardModeCommand { get; }
        public ICommand SetCustomModeCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public GameViewModel(string username)
        {
            _currentUser = username;
            
            NewGameCommand = new RelayCommand(StartNewGame);
            SaveGameCommand = new RelayCommand(SaveGame, () => IsGameActive);
            OpenGameCommand = new RelayCommand(LoadGame);
            ShowStatisticsCommand = new RelayCommand(ShowStatistics);
            ExitCommand = new RelayCommand(ExitGame);
            SetStandardModeCommand = new RelayCommand(() =>{
                IsCustomMode = false;
                Rows = 4;
                Columns = 4;
                if (IsGameActive)
                {
                    StartNewGame();
                }
            });
            SetCustomModeCommand = new RelayCommand(() => IsCustomMode = true);
            ShowAboutCommand = new RelayCommand(ShowAbout);

            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _gameTimer.Tick += GameTimer_Tick;
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                StartNewGame();
            }));

        }

        private void StartNewGame()
        {
            try
            {
                StopTimer();
                IsGameActive = true;
                IsGameWon = false;
                CanFlipCards = true;
                Cards.Clear();
                _firstCard = null;
                _secondCard = null;

                if (!IsCustomMode)
                {
                    Rows = 4;
                    Columns = 4;
                }

                RemainingTimeSeconds = TotalGameTimeSeconds;

                GenerateCards();
                StartTimer();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error starting new game: " + ex.Message + "\n\n" + ex.StackTrace,
                    "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void SaveGame()
        {
            if (!IsGameActive)
                return;

            StopTimer();

            GameModel game = new GameModel
            {
                Category = Category,
                Rows = Rows,
                Columns = Columns,
                IsCustomMode = IsCustomMode,
                TotalGameTimeSeconds = TotalGameTimeSeconds,
                RemainingTimeSeconds = RemainingTimeSeconds,
                SavedAt = DateTime.Now,
                Cards = Cards.Select(c => new GameModel.CardModel
                {
                    Id = c.Id,
                    ImagePath = c.ImagePath,
                    IsFlipped = c.IsFlipped,
                    IsMatched = c.IsMatched,
                    Row = c.Row,
                    Column = c.Column
                }).ToList()
            };

            FileService.SaveGame(_currentUser, game);
        }

        private void LoadGame()
        {
            try
            {
                GameModel game = FileService.LoadGame(_currentUser);
                if(game == null)
                {
                    System.Windows.MessageBox.Show("No saved game found for this user.",
                "Load Game", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return;
                }
                StopTimer();
                Cards.Clear();

                Category = game.Category;
                Rows = game.Rows;
                Columns = game.Columns;
                IsCustomMode = game.IsCustomMode;
                TotalGameTimeSeconds = game.TotalGameTimeSeconds;
                RemainingTimeSeconds = game.RemainingTimeSeconds;
                IsGameActive = true;
                IsGameWon = false;
                CanFlipCards = true;
                _firstCard = null;
                _secondCard = null;

                foreach (var cardModel in game.Cards)
                {
                    var card = new CardViewModel
                    {
                        Id = cardModel.Id,
                        ImagePath = cardModel.ImagePath,
                        IsFlipped = cardModel.IsFlipped,
                        IsMatched = cardModel.IsMatched,
                        Row = cardModel.Row,
                        Column = cardModel.Column
                    };

                    
                    card.CardClicked += Card_CardClicked;
                    Cards.Add(card);
                }

                if (Cards.All(c => c.IsMatched))
                {
                    GameWon();
                    return;
                }

                StartTimer();
                System.Windows.MessageBox.Show("Game loaded successfully.",
                    "Load Game", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading game: {ex.Message}",
                    "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private void ShowStatistics()
        {
            var statisticsVM = new StatisticsViewModel();
            var statisticsView = new Views.StatisticsView
            {
                DataContext = statisticsVM
            };

            statisticsView.ShowDialog();
        }

        private void ExitGame()
        {
            StopTimer();

            var loginVM = new LoginViewModel();
            var loginView = new Views.LoginView
            {
                DataContext = loginVM
            };

            var currentWindow = System.Windows.Application.Current.Windows.OfType<Views.GameView>().FirstOrDefault();
            if (currentWindow != null)
            {
                loginView.Show();
                currentWindow.Close();
            }
        }

        private void ShowAbout()
        {
            var aboutView = new Views.ShowAboutView();
            aboutView.ShowDialog();
        }
        
        private void GenerateCards()
        {
            int pairsCount = Rows * Columns / 2;
            var imagePaths = GetImagePaths(Category, pairsCount);

            List<CardViewModel> tempCards = new List<CardViewModel>();
            int id = 0;

            for (int i = 0; i < pairsCount; i++)
            {
                string image = imagePaths[i];

                for (int j = 0; j < 2; j++)
                {
                    var card = new CardViewModel
                    {
                        Id = i, 
                        ImagePath = image,
                        IsFlipped = false,
                        IsMatched = false
                    };
                    card.CardClicked += Card_CardClicked;
                    tempCards.Add(card);
                }
            }

            Random rng = new Random();
            tempCards = tempCards.OrderBy(x => rng.Next()).ToList();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int index = i * Columns + j;
                    if (index < tempCards.Count)
                    {
                        tempCards[index].Row = i;
                        tempCards[index].Column = j;
                        Cards.Add(tempCards[index]);
                    }
                }
            }
        }

        private List<string> GetImagePaths(string category, int count)
        {
            Dictionary<string, List<string>> categoryImages = new Dictionary<string, List<string>>
    {
        {
            "Animals", new List<string>
            {
                "arici.png", "cat.png", "dog.png", "elephant.png", "fox.png", "dog2.png", "giraffe.png",
                "frog.png", "guineapig.png", "horse.png", "mouse.png", "rabbit.png", "turtle.png",
                "bear.png", "bird.png", "chicken.png", "cow.png", "deer.png", "dolphin.png"
            }
        },
        {
            "Fruits", new List<string>
            {
                "apple.png", "banana.png", "strawberry.png", "grape.png", "orange.png", "watermelon.png", 
                "peach.png", "kiwi.png", "mango.png", "pineapple.png", "blueberry.png", "pear.png",
                "cherry.png", "plum.png", "papaya.png", "pomegranate.png", "coconut.png", "lemon.png"
            }
        },
        {
            "Vegetables", new List<string>
            {
                "carrot.png", "cucumber.png", "pepper.png", "tomato.png", "broccoli.png", "onion.png",
                "lettuce.png", "cauliflower.png", "eggplant.png", "potato.png", "zucchini.png", "spinach.png",
                "pumpkin.png", "radish.png", "beetroot.png", "asparagus.png", "celery.png", "corn.png"
            }
        }
    };

            if (!categoryImages.ContainsKey(category))
                return new List<string>();

            var allImages = categoryImages[category];

            Random random = new Random();
            var selectedImages = allImages.OrderBy(x => random.Next()).Take(count).ToList();

            return selectedImages.Select(img => $"pack://application:,,,/MemoryGame;component/Resources/{category}/{img}").ToList();
        }

        private void StartTimer()
        {
            _gameTimer.Start();
        }

        private void StopTimer()
        {
            _gameTimer.Stop();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (RemainingTimeSeconds > 0)
            {
                RemainingTimeSeconds--;
                OnPropertyChanged(nameof(RemainingTimeDisplay));
            }
            else if (IsGameActive)
            {
                GameOver();
            }
        }

        private void GameWon()
        {
            IsGameActive = false;
            IsGameWon = true;
            StopTimer();

            FileService.UpdateStatistics(_currentUser, true);

            System.Windows.MessageBox.Show("Congratulations! You've matched all the cards!",
                "You Win!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void GameOver()
        {
            IsGameActive = false;
            IsGameWon = false;
            StopTimer();

            FileService.UpdateStatistics(_currentUser, false);
            System.Windows.MessageBox.Show(
                "Time's up! Game over.",
                "Game Over",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }

        private void Card_CardClicked(object sender, CardViewModel card)
        {
            if (!CanFlipCards || !IsGameActive)
                return;

            if (_firstCard == null)
            {
                _firstCard = card;
            }

            else if (_secondCard == null && _firstCard != card)
            {
                _secondCard = card;
                CanFlipCards = false;

                if (_firstCard.Id == _secondCard.Id)
                {
                    _firstCard.IsMatched = true;
                    _secondCard.IsMatched = true;
                    ResetCardSelection();
                    if (Cards.All(c => c.IsMatched))
                    {
                        GameWon();
                    }
                }
                else
                {
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1)
                    };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        _firstCard.IsFlipped = false;
                        _secondCard.IsFlipped = false;
                        ResetCardSelection();
                    };
                    timer.Start();
                }
            }
        }

        private void ResetCardSelection()
        {
            _firstCard = null;
            _secondCard = null;
            CanFlipCards = true;
        }
    }
}
