using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GuessNumber
{
    internal class Program
    {
        private static readonly Random _random = new();
        private static string _currentUser = "Guest";

        private const string FilePath = "../../../game_history.csv";

        public enum DifficultyLevel
        {
            Easy = 1,
            Medium = 2,
            Hard = 3
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Hello! Welcome to the game 'Guess the Number'");

            LoadUsers();
            HandleUserLogin();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n===== Main Menu ({_currentUser}) =====");
                Console.ResetColor();

                Console.WriteLine("1) Start Game");
                Console.WriteLine("2) TOP 10 Ranking");
                Console.WriteLine("3) Exit");

                Console.Write("Choose an option: ");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        DisplayDifficultySelection();
                        break;
                    case "2":
                        ShowTop10();
                        break;
                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Try again.");
                        break;
                }
            }
        }

        // სირთულის არჩევა
        private static void DisplayDifficultySelection()
        {
            while (true)
            {
                Console.WriteLine("\nSelect difficulty: 1-Easy | 2-Medium | 3-Hard");
                string? input = Console.ReadLine();

                if (Enum.TryParse(input, out DifficultyLevel level) &&
                    Enum.IsDefined(typeof(DifficultyLevel), level))
                {
                    PlayGame(level);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid difficulty. Try again.");
                }
            }
        }

        // აბრუნებს შემთხვევით რიცხვს შესაბამის დიაპაზონში
        private static int GetRandomNumber(DifficultyLevel level)
        {
            return level switch
            {
                DifficultyLevel.Easy => _random.Next(1, 16),
                DifficultyLevel.Medium => _random.Next(1, 26),
                DifficultyLevel.Hard => _random.Next(1, 51),
                _ => 1
            };
        }

        // მთავარი თამაშის ლოგიკა
        private static void PlayGame(DifficultyLevel level)
        {
            int randomNumber = GetRandomNumber(level);
            int maxAttempts = 10;    
            int attemptsUsed = 0;    
            bool isWin = false;

            Console.WriteLine($"\nGame started! Guess the number. Difficulty: {level}");
            Console.WriteLine($"You have {maxAttempts} attempts.");

            while (attemptsUsed < maxAttempts)
            {
                Console.Write($"\nAttempt {attemptsUsed + 1}/{maxAttempts}. Enter a number: ");

                // თუ ტექსტია და არა რიცხვი
                if (!int.TryParse(Console.ReadLine(), out int guess))
                {
                    Console.WriteLine("Please enter only numeric values!");
                    continue;
                }

                attemptsUsed++;

                if (guess == randomNumber)
                {
                    isWin = true;
                    break;
                }

                if (guess < randomNumber)
                    Console.WriteLine("The number is higher.");
                else
                    Console.WriteLine("The number is lower.");
            }

            // მოგება
            if (isWin)
            {
                int score = CalculateScore(maxAttempts, attemptsUsed, level);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nCongratulations! You guessed the number in {attemptsUsed} attempts!");
                Console.WriteLine($"Your score: {score}");
                Console.ResetColor();

                SaveToCsv(_currentUser, score, level);
            }
            else
            {
                // წაგება
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAttempts exhausted! The correct number was: {randomNumber}");
                Console.ResetColor();
            }
        }

        private static int CalculateScore(int maxAttempts, int usedAttempts, DifficultyLevel level)
        {
            int baseScore = (maxAttempts - usedAttempts) + 1;
            int multiplier = (int)level;
            return baseScore * multiplier * 10;
        }

        private static void SaveToCsv(string username, int score, DifficultyLevel level)
        {
            try
            {
                if (!File.Exists(FilePath))
                    File.WriteAllText(FilePath, "Username,Score,Difficulty,Date\n");

                string record = $"{username},{score},{level},{DateTime.Now:yyyy-MM-dd HH:mm}\n";
                File.AppendAllText(FilePath, record);

                Console.WriteLine("Result saved to history.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing to file: {ex.Message}");
            }
        }

        private static void ShowTop10()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("History is empty.");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(FilePath).Skip(1);
                var records = new List<PlayerRecord>();

                // CSV-ის თითო ხაზის დამუშავება
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 4 && int.TryParse(parts[1], out int score))
                    {
                        records.Add(new PlayerRecord
                        {
                            Username = parts[0],
                            Score = score,
                            Difficulty = parts[2],
                            Date = parts[3]
                        });
                    }
                }

                var top10 = records.OrderByDescending(r => r.Score).Take(10).ToList();

                Console.WriteLine("\nTOP 10 Players");
                Console.WriteLine("{0,-15} | {1,-5} | {2,-10} | {3,-20}", "Name", "Score", "Difficulty", "Date");
                Console.WriteLine(new string('-', 60));

                // თითო მოთამაშის გასწორებული ბეჭდვა
                foreach (var rec in top10)
                    Console.WriteLine($"{rec.Username,-15} | {rec.Score,-5} | {rec.Difficulty,-10} | {rec.Date,-20}");

                Console.WriteLine("==============================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading history: {ex.Message}");
            }
        }

        // პატარა კლასია, CSV-ს სტრუქტურის შესანახად
        class PlayerRecord
        {
            public string Username { get; set; } = "";
            public int Score { get; set; }
            public string Difficulty { get; set; } = "";
            public string Date { get; set; } = "";
        }

        // იუზერები + პაროლები ინახება  Dictionary-ში
        private static Dictionary<string, string> Users = new();
        private const string UserFile = "../../../users.csv";

        // ყველა იუზერის ჩატვირთვა
        private static void LoadUsers()
        {
            if (!File.Exists(UserFile))
            {
                File.WriteAllText(UserFile, "Username,Password\n");
                return;
            }

            // CSV ფაილიდან Users Dictionary შევსება
            foreach (var line in File.ReadLines(UserFile).Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                    Users[parts[0]] = parts[1];
            }
        }

        // ლოგინი / რეგისტრაცია
        private static void HandleUserLogin()
        {
            while (true)
            {
                Console.Write("Enter your username: ");
                string? inputName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(inputName))
                {
                    Console.WriteLine("Username cannot be empty.");
                    continue;
                }

                // არსებულ იუზერს ვამოწმებთ
                if (Users.ContainsKey(inputName))
                {
                    Console.Write("Enter your password: ");
                    string? pass = Console.ReadLine();

                    if (pass == Users[inputName])
                    {
                        Console.WriteLine("Login successful.");
                        _currentUser = inputName;
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Wrong password. Try again.");
                        continue;
                    }
                }
                else
                {
                    // ახალი იუზერის შექმნა
                    Console.WriteLine("Username not found. Creating new user...");
                    Console.Write("Create a password: ");
                    string? newPass = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(newPass))
                    {
                        Console.WriteLine("Password cannot be empty.");
                        continue;
                    }

                    Users[inputName] = newPass;
                    File.AppendAllText(UserFile, $"{inputName},{newPass}\n");

                    Console.WriteLine("Registration complete.");
                    _currentUser = inputName;
                    return;
                }
            }
        }
    }
}
