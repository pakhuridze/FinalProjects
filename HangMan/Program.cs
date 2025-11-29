using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HangMan
{
    public class PlayerStat
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }

    public static class ScoreManager
    {
        private static string filePath = "../../../leaderboard.xml";

        public static void SaveScore(string name, int score)
        {
            List<PlayerStat> stats = LoadScores();
            stats.Add(new PlayerStat { Name = name, Score = score, Date = DateTime.Now });

            XmlSerializer serializer = new XmlSerializer(typeof(List<PlayerStat>));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, stats);
            }
        }

        public static List<PlayerStat> LoadScores()
        {
            if (!File.Exists(filePath)) return new List<PlayerStat>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<PlayerStat>));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (List<PlayerStat>)serializer.Deserialize(stream);
            }
        }

        public static void ShowTop10()
        {
            var stats = LoadScores();
            var top10 = stats.OrderByDescending(s => s.Score).Take(10).ToList();

            Console.Clear();
            Console.WriteLine("\n--- TOP 10 PLAYERS ---");
            Console.WriteLine("{0,-15} | {1,-10} | {2}", "Name", "Score", "Date");
            Console.WriteLine(new string('-', 50));

            foreach (var stat in top10)
            {
                Console.WriteLine($"{stat.Name,-15} | {stat.Score,-10} | {stat.Date.ToShortDateString()}");
            }
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
    }

    // 
    internal class Program
    {
        private static List<string> words = new List<string>
        {
            "apple", "banana", "orange", "grape", "kiwi",
            "strawberry", "pineapple", "blueberry", "peach", "watermelon"
        };

        // ვიმახსოვრებთ  რომ იგივე სიტყვა არ გავიმეოროთ
        private static string lastPickedWord = "";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- HANGMAN GAME ---");
                Console.WriteLine("0. Play Game");
                Console.WriteLine("1. Top 10 Stats");
                Console.WriteLine("Other. Exit");
                Console.Write("Select Mode: ");

                string input = Console.ReadLine();

                if (input == "0")
                {
                    GamePlay();
                }
                else if (input == "1")
                {
                    ScoreManager.ShowTop10();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void GamePlay()
        {
            Console.Clear();
            Console.Write("\nEnter your Player Name: ");
            string playerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";

            // განახლებული ლოგიკა სიტყვის არჩევისთვის 
            string targetWord;

            // თუ ლისტში 1 სიტყვაა, ლუპში არ ჩავაგდოთ, თორემ გაიჭედება
            if (words.Count > 1)
            {
                do
                {
                    targetWord = words[Random.Shared.Next(words.Count)];
                }
                while (targetWord == lastPickedWord); // ვირჩევთ მანამ, სანამ ძველს არ დაემთხვევა
            }
            else
            {
                targetWord = words[0];
            }

            lastPickedWord = targetWord; // ვიმახსოვრებთ ახალს შემდეგი ხელისთვის
            

            char[] guessedWord = new string('*', targetWord.Length).ToCharArray();
            List<char> usedLetters = new List<char>();

            int lives = 6;
            bool isWinner = false;

            while (lives > 0)
            {
                Console.Clear();
                Console.WriteLine($"Player: {playerName} | Lives: {lives}");
                Console.WriteLine($"Word: {new string(guessedWord)}");
                Console.WriteLine($"Used letters: {string.Join(", ", usedLetters)}");

                Console.Write("\nGuess a letter OR type the full word to win: ");
                string input = Console.ReadLine().ToLower();

                if (string.IsNullOrWhiteSpace(input)) continue;

                //ერთი ასოს შეყვანა
                if (input.Length == 1)
                {
                    char letter = input[0];

                    if (usedLetters.Contains(letter))
                    {
                        Console.WriteLine("You already used this letter! Press key...");
                        Console.ReadKey();
                        continue;
                    }

                    usedLetters.Add(letter);

                    if (targetWord.Contains(letter))
                    {
                        for (int i = 0; i < targetWord.Length; i++)
                        {
                            if (targetWord[i] == letter)
                                guessedWord[i] = letter;
                        }

                        if (!new string(guessedWord).Contains('*'))
                        {
                            isWinner = true;
                            break;
                        }
                    }
                    else
                    {
                        lives--;
                    }
                }
                // 2. მთლიანი სიტყვის შეყვანა
                else
                {
                    if (input == targetWord)
                    {
                        isWinner = true;
                        guessedWord = targetWord.ToCharArray();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Wrong word! The word was NOT '{input}'.");
                        lives = 0; // ან lives -= 2; როგორც გინდა
                    }
                }
            }

            Console.Clear();
            if (isWinner)
            {
                int score = lives * 100 * targetWord.Length;
                Console.WriteLine("CONGRATULATIONS! YOU WON!");
                Console.WriteLine($"The word was: {targetWord}");
                Console.WriteLine($"Score: {score}");
                ScoreManager.SaveScore(playerName, score);
            }
            else
            {
                Console.WriteLine("GAME OVER! YOU LOST!");
                Console.WriteLine($"The correct word was: {targetWord}");
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}