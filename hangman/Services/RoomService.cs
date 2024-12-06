using hangman.Models;
using System.Collections.Generic;

namespace hangman.Services
{
    public class RoomService
    {
        private static readonly Dictionary<string, Room> _rooms = new();
        private readonly WordValidator _wordValidator;  // Add the word validator instance

        // Constructor: initialize the word validator
        public RoomService(WordValidator wordValidator)
        {
            _wordValidator = wordValidator;
        }

        public Room CreateRoom(string name, string password = null)
        {
            string connectionCode;
            
            do
            {
                connectionCode = Room.GenerateConnectionCode();
            } while (_rooms.ContainsKey(connectionCode));

            
            var room = new Room(_wordValidator)
            {
                Name = name,
                Password = password,
                ConnectionCode = connectionCode,
            };

            
            _rooms[connectionCode] = room;

            return room;
        }

        // Get a room by its connection code
        public Room GetRoomByCode(string connectionCode)
        {
            _rooms.TryGetValue(connectionCode, out var room);
            return room;
        }

        // Delete a room by its connection code
        public bool DeleteRoom(string connectionCode)
        {
            return _rooms.Remove(connectionCode);
        }
    }
}
