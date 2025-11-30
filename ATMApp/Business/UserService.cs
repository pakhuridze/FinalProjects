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

        public User Register(string first, string last, string pid)
        {
            var existing = _userRepo.FindByPersonalId(pid);
            if (existing != null)
                return null;

            var users = _userRepo.GetAll();
            int newId = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;

            var randomPassword = new Random().Next(1000, 9999).ToString();

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
            var user = _userRepo.FindByPersonalId(pid);
            if (user == null) return null;
            if (user.Password != password) return null;

            return user;
        }

        public void Update(User user)
        {
            _userRepo.Update(user);
        }
    }
}
