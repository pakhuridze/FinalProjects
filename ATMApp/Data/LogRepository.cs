using ATM_Project.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ATM_Project.Data
{
    public class LogRepository
    {
        private readonly string _filePath = "../../../DataStorage/logs.json";

        public LogRepository()
        {
            // Ensure folder exists
            var folder = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Ensure file exists
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public void WriteLog(LogEntry entry)
        {
            var logs = LoadAll();
            logs.Add(entry);

            var json = JsonSerializer.Serialize(
                logs,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_filePath, json);
        }

        public List<LogEntry> LoadAll()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<LogEntry>();

                var json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<LogEntry>();

                return JsonSerializer.Deserialize<List<LogEntry>>(json)
                       ?? new List<LogEntry>();
            }
            catch
            {
                // If corrupted, return empty list instead of crashing
                return new List<LogEntry>();
            }
        }
    }
}
