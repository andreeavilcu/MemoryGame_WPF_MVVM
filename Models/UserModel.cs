using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }

        public UserModel(string username, string imagePath)
        {
            Username = username;
            ImagePath = imagePath;
        }
    }
}
