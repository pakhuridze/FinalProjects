using System;

namespace ConsoleCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Console Calculator";
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Enter operator (+, -, *, /) or \"exit\" ': ");
                Console.ResetColor();

                string? operation = Console.ReadLine();

                if (operation == "exit")
                    break;

                if (operation != "+" && operation != "-" && operation != "*" && operation != "/")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid operator. Please try again.\n");
                    Console.ResetColor();
                    continue;
                }

                // Input numbers and perform calculation with error handling
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Enter first number: ");
                    Console.ResetColor();
                    double firstNumber = double.Parse(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Enter second number: ");
                    Console.ResetColor();
                    double secondNumber = double.Parse(Console.ReadLine());

                    double result = 0;

                    switch (operation)
                    {
                        case "+":
                            result = firstNumber + secondNumber;
                            break;
                        case "-":
                            result = firstNumber - secondNumber;
                            break;
                        case "*":
                            result = firstNumber * secondNumber;
                            break;
                        case "/":
                            if (secondNumber == 0)
                                throw new DivideByZeroException("Cannot divide by zero.");
                            result = firstNumber / secondNumber;
                            break;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Result: {result}\n");
                    Console.ResetColor();
                    Console.Beep();
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid number format. Please enter valid numeric values.\n");
                    Console.ResetColor();
                }
                catch (DivideByZeroException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message + "\n");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Unexpected error: {ex.Message}\n");
                    Console.ResetColor();
                }
            }
        }
    }
}
