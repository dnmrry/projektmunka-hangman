﻿namespace hangman.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public int? Score { get; set; }

    }
}
