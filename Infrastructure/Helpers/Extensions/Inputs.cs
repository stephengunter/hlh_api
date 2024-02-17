using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Infrastructure.Helpers;
public static class InputHelpers
{
   public static IList<string> GetKeywords(this string? input)
	{
		if (String.IsNullOrWhiteSpace(input) || String.IsNullOrEmpty(input)) return new List<string>();

		return input.Split(new string[] { "-", " ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	public static bool IsAlphaNumeric(this string input)
	{
      string pattern = "^[a-zA-Z0-9]*$";
      return Regex.IsMatch(input, pattern);
   }
   public static bool IsValidUserName(this string input)
   {
      string pattern = "^[a-zA-Z0-9_.@]*$";
      return Regex.IsMatch(input, pattern);
   }
   public static bool IsValidEmail(this string input)
   {
      try
      {
         // This will throw an exception if the email is not in a valid format
         var mailAddress = new MailAddress(input);
         return true;
      }
      catch (FormatException)
      {
         // Email is not in a valid format
         return false;
      }
   }
}