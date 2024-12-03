using ApplicationCore.Consts;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Models.IT;
using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Views.Fetches;
using ApplicationCore.Views.IT;
using Infrastructure.Views;
using Microsoft.AspNetCore.Hosting.Server;
using System.Drawing;
using System.Numerics;

namespace Web.Models.IT;

public class DbBackupPlanLabels
{
   public string Title => "���D";
   public string Ps => "�Ƶ�";
   public string Type => "����";
   public string StartTime => "�}�l�ɶ�";
   public string MinutesInterval => "�ɶ����j(��)";
   public string Database => "��Ʈw";
   public string TypeFull => "����ƥ�";
   public string TypeDiff => "�t���ƥ�";
   public string TargetServerId => "Ftp Server";
   public string TargetPath => "�ؼи��|";
}
public class DbBackupPlansIndexModel
{
   public DbBackupPlansIndexModel(DbBackupPlansFetchRequest request, ICollection<DatabaseViewModel> servers)
   {
      Request = request;
      Databases = servers;
   }
   public DbBackupPlanLabels Labels => new DbBackupPlanLabels();
   public DbBackupPlansFetchRequest Request { get; set; }
   public ICollection<DatabaseViewModel> Databases { get; set; }
}

public class DbBackupPlansFetchRequest
{
   public DbBackupPlansFetchRequest()
   {
      
   }
   public int DatabaseId { get; set; }
}
public abstract class DbBackupPlanBaseForm
{
   public string Title { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;
   public int StartTime { get; set; }
   public int MinutesInterval { get; set; }
   public int DatabaseId { get; set; }
   public int? TargetServerId { get; set; }
   public string TargetPath { get; set; } = string.Empty;
   public bool Active { get; set; }

   public bool CanRemove { get; set; }

}
public class DbBackupPlanAddForm : DbBackupPlanBaseForm
{
   public DbBackupPlanAddForm()
   {
      StartTime = -1;
   }
}
public class DbBackupPlanEditForm : DbBackupPlanBaseForm
{

}
public abstract class DbBackupPlanBaseRequest
{
   public DbBackupPlanBaseRequest()
   {
      var label = new DbBackupPlanLabels();
      TypeOptions = new List<BaseOption<string>>();
      TypeOptions.Add(new BaseOption<string>(DbBackupType.Full, label.TypeFull));
      TypeOptions.Add(new BaseOption<string>(DbBackupType.Diff, label.TypeDiff));
   }
   public ICollection<BaseOption<string>> TypeOptions { get; set; }
}

public class DbBackupPlanAddRequest : DbBackupPlanBaseRequest
{
   public DbBackupPlanAddRequest(DbBackupPlanAddForm form, ICollection<ServerViewModel> servers)
   {
      Form = form;
      Servers = servers;
   }
   public ICollection<ServerViewModel> Servers { get; set; }
   public DbBackupPlanAddForm Form { get; set; }
}
public class DbBackupPlanEditRequest : DbBackupPlanBaseRequest
{
   public DbBackupPlanEditRequest(DbBackupPlanEditForm form, ICollection<ServerViewModel> servers)
   {
      Form = form;
      Servers = servers;
   }
   public ICollection<ServerViewModel> Servers { get; set; }
   public DbBackupPlanEditForm Form { get; set; }
}