using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public interface IBaseFile
{
   public string FileName { get; set; }
   public byte[] FileBytes { get; set; }
}

public class BaseFile : IBaseFile
{
   public string FileName { get; set; } = string .Empty;
   public byte[] FileBytes { get; set; }
}


