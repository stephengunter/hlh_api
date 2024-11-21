using System.Net;

namespace Infrastructure.Helpers;

public static class IpHelpers
{
   public static bool IsValidIpAddress(this string ipAddress)
   {
      if (string.IsNullOrWhiteSpace(ipAddress))
         return false;

      return IPAddress.TryParse(ipAddress, out var address) &&
             address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
   }
   public static string GetLastTwoSegments(this string ipAddress)
   {
      if (!IsValidIpAddress(ipAddress))
         throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));

      var segments = ipAddress.Split('.');
      if (segments.Length != 4)
         throw new FormatException("Invalid IP address format.");

      // Return the last two segments
      return $"{segments[2]}.{segments[3]}";
   }
   public static string GetLastThreeSegments(this string ipAddress)
   {
      if (!IsValidIpAddress(ipAddress))
         throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));

      var segments = ipAddress.Split('.');
      if (segments.Length != 4)
         throw new FormatException("Invalid IP address format.");

      // Return the last 3 segments
      return $"{segments[1]}.{segments[2]}.{segments[3]}";
   }
}