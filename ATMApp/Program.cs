using ATM_Project.Business;
using ATM_Project.Models;
using System;

namespace ATM_Project.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var userService = new UserService();
            var atm = new AtmService();

            Console.WriteLine("--- ATM SYSTEM ---");

            User loggedUser = null;

            
            while (loggedUser == null)
            {
                Console.WriteLine("\n1. Login");
                Console.WriteLine("2. Register");
                Console.Write("Choose: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        loggedUser = HandleLogin(userService);
                        break;

                    case "2":
                        HandleRegister(userService);
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            // ATM MENU
            while (true)
            {
                Console.WriteLine("\n--- ATM MENU ---");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Exit");

                Console.Write("Choose: ");
                string mode = Console.ReadLine()?.Trim();

                switch (mode)
                {
                    case "1":
                        atm.CheckBalance(loggedUser);
                        break;

                    case "2":
                        decimal? dep = ReadAmount();
                        if (dep != null)
                        {
                            atm.Deposit(loggedUser, dep.Value);
                            userService.Update(loggedUser);
                        }
                        break;

                    case "3":
                        decimal? wd = ReadAmount();
                        if (wd != null)
                        {
                            if (atm.Withdraw(loggedUser, wd.Value))
                                userService.Update(loggedUser);
                            else
                                Console.WriteLine("Not enough funds.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // --- HELPERS ---

        private static User HandleLogin(UserService userService)
        {
            Console.Write("\nPersonal ID: ");
            string pid = Console.ReadLine();

            Console.Write("Password: ");
            string pass = Console.ReadLine();

            var user = userService.Login(pid, pass);

            if (user == null)
                Console.WriteLine("Invalid credentials.");

            return user;
        }

        private static void HandleRegister(UserService userService)
        {
            Console.Write("\nFirst name: ");
            string f = Console.ReadLine();

            Console.Write("Last name: ");
            string l = Console.ReadLine();

            Console.Write("Personal ID: ");
            string pid = Console.ReadLine();

            var newUser = userService.Register(f, l, pid);

            if (newUser == null)
                Console.WriteLine("User with this ID already exists.");
            else
                Console.WriteLine($"Registered successfully! Your password is: {newUser.Password}");
        }

        private static decimal? ReadAmount()
        {
            Console.Write("Amount: ");
            string input = Console.ReadLine();

            if (decimal.TryParse(input, out decimal amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Amount must be greater than 0.");
                    return null;
                }

                return amount;
            }

            Console.WriteLine("Invalid amount.");
            return null;
        }
    }
}
