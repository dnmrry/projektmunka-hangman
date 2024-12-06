using hangman.Services;
using System;
using System.Collections.Generic;

namespace hangman.Models
{
    public class Room
    {
        public string ConnectionCode { get; init; }
        public string? Name { get; set; }
        public string? Password { get; set; }

        public Game _game { get; private set; }
        public List<User> Users { get;init; } = new List<User>();

        private readonly WordValidator _wordValidator;


        public Room(WordValidator wordValidator)
        {
            _wordValidator = wordValidator;
            ConnectionCode = GenerateConnectionCode();
            _game = new Game(this, _wordValidator, 6); 
        }

        public event EventHandler<Room> RoomUpdated = delegate { };


        public bool AddUser(User user)
        {
            if (Users.Exists(x => x.UserId == user.UserId)) return false;
            Users.Add(user);
            RoomUpdated.Invoke(this, this);
            return true; 

           
        }

        public bool RemoveUser(int userId)
        {
            var userToRemove = Users.Find(u => u.UserId == userId);
            if (userToRemove == null) return false;
            Users.Remove(userToRemove);
            return true;
        }

        public void StartGame()
        {
            _game.StartGame();
            RoomUpdated.Invoke(this, this);
        }

        public static string GenerateConnectionCode()
        {
            int length = 6;
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            string allChars = uppercase + numbers;

            string code = "";
            for (int i = 0; i < length; i++)
            {
                code += allChars[Random.Shared.Next(allChars.Length)];
            }
           
            return code;
        }
        public void TriggerUpdate()
        {
            RoomUpdated.Invoke(this, this);
        }
    }
}
