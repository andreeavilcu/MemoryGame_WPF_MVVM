using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class GameModel
    {
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool IsCustomMode { get; set; } 
        public int TotalGameTimeSeconds { get; set; }
        public int RemainingTimeSeconds { get; set; }
        public DateTime SavedAt { get; set; }

        public List<CardModel> Cards { get; set; } = new List<CardModel>();


        public class CardModel
        {
            public int Id { get; set; }
            public string ImagePath { get; set; }
            public bool IsFlipped { get; set; }
            public bool IsMatched { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }

        public double CompletionPercentage => Cards.Count > 0 ?
            ((double)Cards.Count(c => c.IsMatched) / Cards.Count) * 100 : 0;
    
        public bool IsComplete => Cards.All(c => c.IsMatched);
    }
}
