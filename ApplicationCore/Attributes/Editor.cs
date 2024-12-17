using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EditorAttribute : Attribute
{
   public string Label { get; }
   public bool Enable { get; set; }

   public EditorAttribute(string label)
   {
      Label = label;
   }
}

