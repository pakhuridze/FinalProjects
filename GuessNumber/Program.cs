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
        private static string _currentUser = "Guest"; // მიმდინარე მომხმარებელი
        private const string FilePath = "game_history.csv";
       
        public enum DifficultyLevel
        {
            Easy = 1,   // 1-15
            Medium = 2, // 1-25
            Hard = 3    // 1-50
        }

        static void Main(string[] args)
        {
            // ქართული უნიკოდის მხარდაჭერა კონსოლში
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(" გამარჯობა! კეთილი იყოს თქვენი მობრძანება თამაშში 'გამოიცანი რიცხვი'");

            // მომხმარებლის სახელის მოთხოვნა
            Console.Write("შეიყვანეთ თქვენი სახელი: ");
            string? inputName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputName))
            {
                _currentUser = inputName;
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n===== მთავარი მენიუ ({_currentUser}) =====");
                Console.ResetColor();

                Console.WriteLine("1) თამაშის დაწყება");
                Console.WriteLine("2) TOP 10 რეიტინგი");
                Console.WriteLine("3) გასვლა");

                Console.Write("აირჩიეთ მოქმედება: ");
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
                        Console.WriteLine("ნახვამდის!");
                        return;
                    default:
                        Console.WriteLine("❌ არასწორი არჩევანი! სცადეთ თავიდან.");
                        break;
                }
            }
        }

        private static void DisplayDifficultySelection()
        {
            while (true)
            {
                Console.WriteLine("\nაირჩიეთ სირთულე: 1-Easy (მარტივი) | 2-Medium (საშუალო) | 3-Hard (რთული)");
                string? input = Console.ReadLine();

                if (Enum.TryParse(input, out DifficultyLevel level) && Enum.IsDefined(typeof(DifficultyLevel), level))
                {
                    PlayGame(level);
                    break;
                }
                else
                {
                    Console.WriteLine("❌ არასწორი სირთულე. სცადეთ თავიდან.");
                }
            }
        }

        private static int GetRandomNumber(DifficultyLevel level)
        {
            return level switch
            {
                DifficultyLevel.Easy => _random.Next(1, 16),   // 1-15
                DifficultyLevel.Medium => _random.Next(1, 26), // 1-25
                DifficultyLevel.Hard => _random.Next(1, 51),   // 1-50
                _ => 1
            };
        }

        private static void PlayGame(DifficultyLevel level)
        {
            int randomNumber = GetRandomNumber(level);
            int maxAttempts = 10;
            int attemptsUsed = 0;
            bool isWin = false;

            Console.WriteLine($"\n🎮 დაიწყო! გამოიცანი რიცხვი. სირთულე: {level}");
            Console.WriteLine($"გაქვს {maxAttempts} მცდელობა.");

            while (attemptsUsed < maxAttempts)
            {
                Console.Write($"\nმცდელობა {attemptsUsed + 1}/{maxAttempts}. შეიყვანეთ რიცხვი: ");

                if (!int.TryParse(Console.ReadLine(), out int guess))
                {
                    Console.WriteLine("❌ გთხოვთ, შეიყვანოთ მხოლოდ ციფრები!");
                    continue;
                }

                attemptsUsed++;

                if (guess == randomNumber)
                {
                    isWin = true;
                    break; // მოგება
                }

                if (guess < randomNumber)
                {
                    Console.WriteLine("🔼 ჩაფიქრებული რიცხვი მეტია.");
                }
                else
                {
                    Console.WriteLine("🔽 ჩაფიქრებული რიცხვი ნაკლებია.");
                }
            }

            if (isWin)
            {
                // ქულების დათვლა: რაც უფრო მალე გამოიცნობს და რაც უფრო რთულია დონე, მეტი ქულაა
                int score = CalculateScore(maxAttempts, attemptsUsed, level);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n🎉 გილოცავთ! თქვენ გამოიცანით რიცხვი {attemptsUsed} მცდელობაში!");
                Console.WriteLine($"თქვენი ქულა: {score}");
                Console.ResetColor();

                SaveToCsv(_currentUser, score, level);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ სამწუხაროდ მცდელობები ამოიწურა! სწორი რიცხვი იყო: {randomNumber}");
                Console.ResetColor();
            }
        }

        // ქულების გამოთვლა
        private static int CalculateScore(int maxAttempts, int usedAttempts, DifficultyLevel level)
        {
            // მაგალითად: (10 - გამოყენებული + 1) * სირთულის კოეფიციენტი
            // Easy = x1, Medium = x2, Hard = x3
            int baseScore = (maxAttempts - usedAttempts) + 1;
            int multiplier = (int)level;
            return baseScore * multiplier * 10;
        }

        // CSV-ში შენახვა
        private static void SaveToCsv(string username, int score, DifficultyLevel level)
        {
            try
            {
                // თუ ფაილი არ არსებობს, ვქმნით ჰედერს
                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "Username,Score,Difficulty,Date\n");
                }

                string record = $"{username},{score},{level},{DateTime.Now:yyyy-MM-dd HH:mm}\n";
                File.AppendAllText(FilePath, record);
                Console.WriteLine("✅ შედეგი შენახულია ისტორიაში.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ფაილში ჩაწერისას: {ex.Message}");
            }
        }

        // TOP 10-ის გამოტანა
        private static void ShowTop10()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("\n ისტორია ცარიელია.");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(FilePath).Skip(1); // პირველი ხაზი (სათაური) გამოტოვება

                var records = new List<PlayerRecord>();

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

                // სორტირება ქულების მიხედვით (კლებადობით) და ტოპ 10-ის აღება
                var top10 = records.OrderByDescending(r => r.Score).Take(10).ToList();

                Console.WriteLine("\n🏆 ===== TOP 10 მოთამაშე =====");
                Console.WriteLine("{0,-15} | {1,-5} | {2,-10} | {3,-20}", "სახელი", "ქულა", "სირთულე", "თარიღი");
                Console.WriteLine(new string('-', 60));

                foreach (var rec in top10)
                {
                    Console.WriteLine($"{rec.Username,-15} | {rec.Score,-5} | {rec.Difficulty,-10} | {rec.Date,-20}");
                }
                Console.WriteLine("==============================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ისტორიის წაკითხვისას: {ex.Message}");
            }
        }

        // დამხმარე კლასი ჩანაწერებისთვის
        class PlayerRecord
        {
            public string Username { get; set; } = "";
            public int Score { get; set; }
            public string Difficulty { get; set; } = "";
            public string Date { get; set; } = "";
        }
    }
}