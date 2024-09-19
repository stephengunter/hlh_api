using Infrastructure.Entities;

namespace ApplicationCore.Views.Keyin;
public class BranchRecordView : EntityBaseView
{
   public int Rank { get; set; }
   public int Year { get; set; }
   public int Month { get; set; }
   public int BranchId { get; set; }
   public int Score { get; set; }

   public string AbsentRate { get; set; } = string.Empty;
   public string Diff { get; set; } = string.Empty;
   public BranchViewModel? Branch { get; set; }
}


public class BranchRecordLabels
{
   public static string Rank = "�W��";
   public static string Year = "�~��";
   public static string Month = "���";
   public static string BranchId = "�k�|";
   public static string Score = "���Z(�r/��)";

   public static string AbsentRate = "�ʦҲv";
   public static string Diff = "�i�B�v(%)";
}

public class PersonRecordView : EntityBaseView
{
   public int Year { get; set; }
   public int Month { get; set; }
   public int PersonId { get; set; }
   public int Score { get; set; }

   public string Unit { get; set; } = string.Empty;
   public string CorrectRate { get; set; } = string.Empty;
   public string Diff { get; set; } = string.Empty;
   public KeyinPersonView? Person { get; set; }
}

public class PersonRecordLabels
{
   
   public static string Year = "�~��";
   public static string Month = "���";
   public static string Unit = "�ѧO";
   public static string PersonId = "�m�W";
   public static string Score = "���Z(�r/��)";

   public static string CorrectRate = "���T�v(%)";
   public static string Diff = "�i�B�v(%)";
}

