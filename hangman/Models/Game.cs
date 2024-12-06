using hangman.Data;
using hangman.Services;
using System.Collections.Generic;
using System.Linq;

namespace hangman.Models
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class Game
    {
       

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CurrentWord { get; private set; }
        public HashSet<char> GuessedLetters { get; private set; } = new();
        public List<string> GuessedWords { get; private set; } = new();
        public GameState CurrentGameState { get; set; } = GameState.NotStarted;
        public int MaxAttempts { get; private set; }
        public int RemainingAttempts { get; private set; }
        public string? LastGuesser { get; private set; }
        public int RoundCounter { get; private set; } = 0;
       
        private Room _room;
        private WordValidator _wordValidator;
        
        public string MaskedWord => string.Concat(CurrentWord.Select(c => GuessedLetters.Contains(c) ? c : '_'));


        public Game(Room room, WordValidator wordValidator, int maxAttempts = 6)
        {
            _room = room;
            _wordValidator = wordValidator;
            MaxAttempts = maxAttempts;
            RemainingAttempts = maxAttempts;
        }

        public static readonly string[] HangmanStages = new string[]
{
    @"
     -----
     |   |
         |
         |
         |
         |
    =========",
    @"
     -----
     |   |
     O   |
         |
         |
         |
    =========",
    @"
     -----
     |   |
     O   |
     |   |
         |
         |
    =========",
    @"
     -----
     |   |
     O   |
    /|   |
         |
         |
    =========",
    @"
     -----
     |   |
     O   |
    /|\  |
         |
         |
    =========",
    @"
     -----
     |   |
     O   |
    /|\  |
    /    |
         |
    =========",
    @"
     -----
     |   |
     O   |
    /|\  |
    / \  |
         |
    ========="
};

        public string GetHangmanFigure()
        {
            // megmaradt próbálkozások szerint return hangman stages
            return HangmanStages[Math.Clamp(MaxAttempts - RemainingAttempts, 0, HangmanStages.Length - 1)];
        }

        public void StartGame()
        {
            if (_room.Users == null || !_room.Users.Any())
            {
                throw new InvalidOperationException("The game must have at least one user in the room");
            }

            CurrentWord = _wordValidator.GetRandomValidWord();

            if (string.IsNullOrWhiteSpace(CurrentWord))
            {
                throw new ArgumentException("No valid word could be selected");
            }

            GuessedLetters.Clear();
            RemainingAttempts = MaxAttempts;
            CurrentGameState = GameState.InProgress;
        }

        public void StopGame()
        {
            CurrentGameState = GameState.Completed;
            GuessedWords.Clear();
        }

        public GameState GetGameState()
        {
            return CurrentGameState;
        }

        public string GetCurrentWord()
        {
            return MaskedWord;
        }


        public bool GuessLetter(char letter)
        {
           /* if (CurrentGameState != GameState.InProgress)
            {
                throw new InvalidOperationException("The game is not currently in progress.");
            }
           */

            letter = char.ToUpper(letter);

            if (GuessedLetters.Contains(letter))
            {
                return false; // betű már tippelve lett
            }

            GuessedLetters.Add(letter);

            if (!CurrentWord.Contains(letter))
            {
                RemainingAttempts--;
                if (RemainingAttempts <= 0)
                {
                    StopGame();
                }
                return false;
            }

            bool isCorrect = CurrentWord.Contains(letter); // benne van e a betű a szóban

            if (isCorrect)
            {
                User currentPlayer = GetCurrentPlayer();
                currentPlayer.Score = (currentPlayer.Score ?? 0) + 1;

                new UserService().UpdateUser(currentPlayer);

                
                if (MaskedWord == CurrentWord)
                {
                    StopGame();
                }
            }

            return isCorrect; 
        }


        public bool GuessWord(string guessedWord)
        {
          /*  if (CurrentGameState != GameState.InProgress)
            {
                throw new InvalidOperationException("The game is not currently in progress.");
            }
          */

            guessedWord = guessedWord.ToUpper();

            GuessedWords.Add(guessedWord);

            User currentPlayer = GetCurrentPlayer();

            if (guessedWord == CurrentWord)
            {
                currentPlayer.Score = (currentPlayer.Score ?? 0) + 5;
                LastGuesser = currentPlayer.Username;
                StopGame();
                return true;
            }

            using (var context = new AppDbContext())
            {
                context.Users.Update(currentPlayer);
                context.SaveChanges();
            }


            RemainingAttempts--;
            if (RemainingAttempts <= 0)
            {
                StopGame();
            }

            return false;
        }

        public User? GetCurrentPlayer()
        {
            if (_room.Users.Count == 0)
            {
                return null;
            }
            return _room.Users[RoundCounter % _room.Users.Count];
        }

        public void NextRound()
        {
           
            RoundCounter++;
        }

        public int GetRemainingAttempts()
        {
            return RemainingAttempts;
        }

    }
}