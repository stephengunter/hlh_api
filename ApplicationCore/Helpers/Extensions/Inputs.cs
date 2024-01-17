namespace ApplicationCore.Helpers;
public static class InputHelpers
{
   public static IList<string> GetKeywords(this string input)
	{
		if (String.IsNullOrWhiteSpace(input) || String.IsNullOrEmpty(input)) return new List<string>();

		return input.Split(new string[] { "-", " ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
	}
}