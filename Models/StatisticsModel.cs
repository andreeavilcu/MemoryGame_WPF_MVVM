using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class StatisticsModel
    {
        public string Username { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        
        public double WinPercentage => GamesPlayed > 0 ? 
            ((double)GamesWon / GamesPlayed ) * 100 : 0;

        //public StatisticsModel(string username, int gamesPlayed = 0, int gamesWon = 0)
        //{
        //    Username = username;
        //    GamesPlayed = gamesPlayed;
        //    GamesWon = gamesWon;
        //}

    }

}
