using Infrastructure.Views;

namespace ApplicationCore.Views.Keyin;
public class KeyinPersonView : EntityBaseView
{
   public string Account { get; set; } = string.Empty;
   public string Unit { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;

   public bool AllPass { get; set; }

   public string LeaveAtText { get; set; } = string.Empty;
   public bool Active { get; set; }
   public int HighRun { get; set; }
}

public class KeyinPersonLabels
{
   public static string Account = "�b��";
   public static string Unit = "�ѧO";
   public static string Name = "�m�W";
   public static string HighRun = "�̨Φ��Z";

}


