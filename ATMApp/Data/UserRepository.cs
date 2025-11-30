using ATM_Project.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ATM_Project.Data
{
    public class UserRepository
    {
        private readonly string _filePath = "../../../DataStorage/users.json";

        public UserRepository()
        {
            // Create folder if not exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create file if not exists
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public List<User> GetAll()
        {
            try
            {
                var json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<User>();

                return JsonSerializer.Deserialize<List<User>>(json)
                       ?? new List<User>();
            }
            catch
            {
                // If JSON is corrupted, return empty list, do not crash program
                return new List<User>();
            }
        }

        public void SaveAll(List<User> users)
        {
            var json = JsonSerializer.Serialize(users,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_filePath, json);
        }

        public void Add(User user)
        {
            var users = GetAll();
            users.Add(user);
            SaveAll(users);
        }

        public void Update(User updatedUser)
        {
            var users = GetAll();
            var index = users.FindIndex(x => x.Id == updatedUser.Id);

            if (index != -1)
            {
                users[index] = updatedUser;
                SaveAll(users);
            }
        }

        public User? FindByPersonalId(string pid)
        {
            return GetAll().FirstOrDefault(x => x.PersonalId == pid);
        }
    }
}
