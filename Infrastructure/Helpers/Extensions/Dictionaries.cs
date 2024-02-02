using System;
using System.Reflection;

namespace Infrastructure.Helpers;
public static class DictionaryHelpers
{
   public static Dictionary<string, TValue> GetDictionaries<TValue>(this Type type)
   {
      Dictionary<string, TValue> result = new Dictionary<string, TValue>();

      FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

      foreach (FieldInfo field in fields)
      {
         string fieldName = field.Name;
         TValue fieldValue = (TValue)field.GetValue(null)!;
         result[fieldName] = fieldValue;
      }
      return result;
   }
}
