using hangman.Data;
using hangman.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace hangman.Services

{
    public class UserService
    {

        public void AddUser(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

        }

        public List<User>GetAllUsers()
        {
            using (var context = new AppDbContext()) 
                { 
                   return context.Users.ToList();
                }
        }

        public User? GetUser(int userId)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.Find(userId);
            }
        }

        public User? GetUserByUsername(string username)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.FirstOrDefault(u => u.Username == username);
            }
        }


        public void UpdateUser(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }
    }
}