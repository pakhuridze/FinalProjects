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
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("\nPersonal ID: ");
                    string pid = Console.ReadLine();

                    Console.Write("Password: ");
                    string pass = Console.ReadLine();

                    loggedUser = userService.Login(pid, pass);

                    if (loggedUser == null)
                        Console.WriteLine("Invalid credentials.");
                }
                else if (choice == "2")
                {
                    Console.Write("\nFirst name: ");
                    string f = Console.ReadLine();

                    Console.Write("Last name: ");
                    string l = Console.ReadLine();

                    Console.Write("Personal ID: ");
                    string pid = Console.ReadLine();

                    var newUser = userService.Register(f, l, pid);

                    if (newUser == null)
                    {
                        Console.WriteLine("User with this ID already exists.");
                    }
                    else
                    {
                        Console.WriteLine($"Registered successfully! Your password is: {newUser.Password}");
                    }
                }
            }

            while (true)
            {
                Console.WriteLine("\n--- ATM MENU ---");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Exit");

                Console.Write("Choose: ");
                string mode = Console.ReadLine();

                if (mode == "1")
                {
                    atm.CheckBalance(loggedUser);
                }
                else if (mode == "2")
                {
                    Console.Write("Amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    atm.Deposit(loggedUser, amount);
                    userService.Update(loggedUser);
                }
                else if (mode == "3")
                {
                    Console.Write("Amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    if (atm.Withdraw(loggedUser, amount))
                        userService.Update(loggedUser);
                    else
                        Console.WriteLine("Not enough funds.");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
