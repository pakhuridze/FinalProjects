using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ATM_Project.Business
{
    public static class Validators
    {
        public static bool IsValidPersonalId(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                return false;

            return pid.Length == 11 && pid.All(char.IsDigit);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length == 4 && password.All(char.IsDigit);
        }

        public static bool IsValidAmount(decimal amount)
        {
            return amount > 0;
        }
    }
}
