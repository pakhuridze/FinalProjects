using ATM_Project.Data;
using ATM_Project.Models;
using System;
using System.Linq;

namespace ATM_Project.Business
{
    public class UserService
    {
        private readonly UserRepository _userRepo;

        public UserService()
        {
            _userRepo = new UserRepository();
        }

        // Register new user
        public User Register(string first, string last, string pid)
        {
            if (string.IsNullOrWhiteSpace(first) || first.Length < 2)
            {
                Console.WriteLine("❌ First name is not valid.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(last) || last.Length < 2)
            {
                Console.WriteLine("❌ Last name is not valid.");
                return null;
            }

            if (!Validators.IsValidPersonalId(pid))
            {
                Console.WriteLine("❌ Personal ID must be 11 digits.");
                return null;
            }

            var existing = _userRepo.FindByPersonalId(pid);
            if (existing != null)
            {
                Console.WriteLine("❌ User with this Personal ID already exists.");
                return null;
            }

            var users = _userRepo.GetAll();
            int newId = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;

            string randomPassword = new Random().Next(1000, 9999).ToString();

            var user = new User
            {
                Id = newId,
                FirstName = first,
                LastName = last,
                PersonalId = pid,
                Password = randomPassword,
                Balance = 0
            };

            _userRepo.Add(user);

            return user;
        }

        public User Login(string pid, string password)
        {
            // validate inputs

            if (!Validators.IsValidPersonalId(pid))
            {
                Console.WriteLine("❌ Invalid Personal ID format.");
                return null;
            }

            if (!Validators.IsValidPassword(password))
            {
                Console.WriteLine("❌ Password must be 4 digits.");
                return null;
            }

            // check if user exists
            var user = _userRepo.FindByPersonalId(pid);
            if (user == null)
            {
                Console.WriteLine("❌ No user found with this ID.");
                return null;
            }

            // check password
            if (user.Password != password)
            {
                Console.WriteLine("❌ Incorrect password.");
                return null;
            }

            return user;
        }

        public void Update(User user)
        {
            _userRepo.Update(user);
        }
    }
}
