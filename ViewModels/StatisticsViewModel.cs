using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private ObservableCollection<StatisticsModel> _statistics;
        
        public ObservableCollection<StatisticsModel> Statistics
        {
            get => _statistics;
            set => SetProperty(ref _statistics, value);
        }

        public ICommand CloseCommand { get; }

        public StatisticsViewModel()
        {
            var stats = FileService.LoadStatistics();
            Statistics = new ObservableCollection<StatisticsModel>(stats.OrderByDescending(s => s.WinPercentage));
            
            CloseCommand = new RelayCommand(Close);
        }

        private void Close()
        {
            var currentWindow = System.Windows.Application.Current.Windows.OfType<Views.StatisticsView>().FirstOrDefault();
            currentWindow?.Close();
        }
    }
}
