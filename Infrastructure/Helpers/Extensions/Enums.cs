using Infrastructure.Views;

namespace Infrastructure.Helpers;
public static class EnumHelpers
{
   public static List<BaseOption<int>> EnumToBaseOptions<TEnum>() where TEnum : Enum
   {
      var options = new List<BaseOption<int>>();

      foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
      {
         var intValue = (int)enumValue;
         var name = enumValue.ToString();

         options.Add(new BaseOption<int>(intValue, name));
      }

      return options;
   }
   public static List<T> ToList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();

	public static T ToEnum<T>(this string inString, bool ignoreCase = true) where T : struct
		=> (T)ParseEnum<T>(inString, ignoreCase);

	public static T ToEnum<T>(this string inString, T defaultValue, bool ignoreCase = true) where T : struct
		=> (T)ParseEnum<T>(inString, defaultValue, ignoreCase);


	public static T ParseEnum<T>(string inString, bool ignoreCase = true) where T : struct
		=> (T)ParseEnum<T>(inString, default(T), ignoreCase);


   public static T ParseEnum<T>(string inString, T defaultValue, bool ignoreCase = true) where T : struct
   {
      T returnEnum = defaultValue;

      if (!typeof(T).IsEnum)
      {
         throw new InvalidOperationException("Invalid Enum Type" + typeof(T).ToString() + "  must be an Enum");
      }

      try
      {
         if (Enum.TryParse<T>(inString, ignoreCase, out returnEnum))
         {
            if (Enum.IsDefined(typeof(T), returnEnum) || returnEnum.ToString()!.Contains(","))
            {
               return returnEnum;
            }
         }
      }
      catch (Exception ex)
      {
         // Handle any other exceptions here if needed
         Console.WriteLine($"Exception occurred: {ex.Message}");
      }

      return defaultValue;
   }

}
