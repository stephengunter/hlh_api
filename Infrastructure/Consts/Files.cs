namespace Infrastructure.Consts;

public enum FileTypes
{
   UnKnown = -1,
   Image,
   Pdf,
   Word,
   Excel

}


public class FileContentType
{
   public static string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}