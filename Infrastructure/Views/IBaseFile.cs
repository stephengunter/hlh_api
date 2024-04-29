using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Views;

public interface IBaseFileView
{
   public string FileName { get; set; }
   public byte[] FileBytes { get; set; }
}


public class BaseFileView : IBaseFileView
{
   public string FileName { get; set; } = string.Empty;
   public byte[] FileBytes { get; set; }
}

