using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hangman.Models
{
    public class WordValidator
    {
        private readonly List<string> _validWords;

        public WordValidator(string filePath)
        {
            _validWords = new List<string>();
            LoadWordsFromFile(filePath);
        }

        private void LoadWordsFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var words = File.ReadLines(filePath)
                        .Where(word => !string.IsNullOrWhiteSpace(word))
                        .Select(word => word.Trim().ToUpper())
                        .ToList();

                    foreach (var word in words)
                    {
                        _validWords.Add(word); 
                    }
                }
                else
                {
                    throw new FileNotFoundException("The word file was not found.", filePath);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error loading words from file: {ex.Message}");
            }
        }

    
        public string GetRandomValidWord()
        {
            return _validWords[Random.Shared.Next(_validWords.Count)];
        }

        public bool IsValidWord(string word)
        {
            return _validWords.Contains(word.ToUpper());
        }
    }
}
