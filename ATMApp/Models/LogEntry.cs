using System;

namespace ATM_Project.Models
{
    public class LogEntry
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
