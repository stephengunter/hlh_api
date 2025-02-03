using System;
using System.Collections.Generic;
using System.IO;

namespace ApplicationCore.Helpers;
public class IISLogEntry
{
   public DateTime Timestamp { get; set; }
   public string ServerIP { get; set; }
   public string HttpMethod { get; set; }
   public string UrlPath { get; set; }
   public string QueryString { get; set; }
   public int Port { get; set; }
   public string ClientIP { get; set; }
   public string UserAgent { get; set; }
   public string Referrer { get; set; }
   public int StatusCode { get; set; }
   public int SubStatusCode { get; set; }
   public int Win32StatusCode { get; set; }
   public int TimeTaken { get; set; }
}

public static class IISLogReader
{
   public static List<IISLogEntry> ReadLogEntries(string filePath)
   {
      var logEntries = new List<IISLogEntry>();

      foreach (var line in File.ReadLines(filePath))
      {
         // Skip empty or invalid lines
         if (string.IsNullOrWhiteSpace(line)) continue;

         var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

         if (parts.Length < 14) continue; // Ensure sufficient parts for parsing

         try
         {
            var logEntry = new IISLogEntry
            {
               Timestamp = DateTime.Parse($"{parts[0]} {parts[1]}"),
               ServerIP = parts[2],
               HttpMethod = parts[3],
               UrlPath = parts[4],
               QueryString = parts[5] == "-" ? null : parts[5],
               Port = int.Parse(parts[6]),
               ClientIP = parts[8],
               UserAgent = line.Substring(line.IndexOf('"') + 1).Split('"')[0],
               Referrer = parts[10] == "-" ? null : parts[10],
               StatusCode = int.Parse(parts[11]),
               SubStatusCode = int.Parse(parts[12]),
               Win32StatusCode = int.Parse(parts[13]),
               TimeTaken = int.Parse(parts[14])
            };

            logEntries.Add(logEntry);
         }
         catch
         {
            // Optionally log or handle parse errors
            Console.WriteLine($"Error parsing line: {line}");
         }
      }

      return logEntries;
   }

   public static List<string> GetDistinctClientIPs(this List<IISLogEntry> logEntries)
   {
      if (logEntries == null || !logEntries.Any())
      {
         return new List<string>();
      }

      return logEntries
          .Select(entry => entry.ClientIP)
          .Where(clientIP => !string.IsNullOrEmpty(clientIP))
          .Distinct()
          .ToList();
   }

}
