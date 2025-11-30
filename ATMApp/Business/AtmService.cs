using ATM_Project.Data;
using ATM_Project.Models;
using System;

namespace ATM_Project.Business
{
    public class AtmService
    {
        private readonly LogRepository _logRepo;

        public AtmService()
        {
            _logRepo = new LogRepository();
        }

        public void CheckBalance(User user)
        {
            string msg =
                $"User {user.FirstName} {user.LastName} checked balance. Current balance: {user.Balance} GEL";

            _logRepo.WriteLog(new LogEntry
            {
                UserId = user.Id,
                Message = msg,
                Date = DateTime.Now
            });

            Console.WriteLine($"\nYour balance: {user.Balance} GEL");
        }

        public void Deposit(User user, decimal amount)
        {
            user.Balance += amount;

            _logRepo.WriteLog(new LogEntry
            {
                UserId = user.Id,
                Message = $"User {user.FirstName} {user.LastName} deposited {amount} GEL. New balance: {user.Balance}",
                Date = DateTime.Now
            });
        }

        public bool Withdraw(User user, decimal amount)
        {
            if (amount > user.Balance)
                return false;

            user.Balance -= amount;

            _logRepo.WriteLog(new LogEntry
            {
                UserId = user.Id,
                Message = $"User {user.FirstName} {user.LastName} withdrew {amount} GEL. New balance: {user.Balance}",
                Date = DateTime.Now
            });

            return true;
        }
    }
}
