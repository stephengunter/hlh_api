using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System;
using System.Text;

namespace ApplicationCore.Services;

public interface IDocModelsService
{
   IEnumerable<DocModel> GetRecords(int flag, string unit, string person);
   IEnumerable<DocModel> GetKeepRecords(List<int> flags);
   IEnumerable<DocModel> GetHideRecords();
   IEnumerable<DocModel> GetChangePersonRecords(string name);
   IEnumerable<DocModel> GetChangePersonRecords();
   void SetJudDoc(IEnumerable<DocModel> records);
   void SetNewPerson(IEnumerable<DocModel> records, string newPersonName, string newPersonId);
   void ExportKeepRecordsToCsv(IEnumerable<DocModel> records, string filePath);
}

public class DocModelsService : IDocModelsService
{
   private readonly DefaultContext _defaultContext;

   public DocModelsService(DefaultContext defaultContext)
   {
      _defaultContext = defaultContext;
   }

   public IEnumerable<DocModel> GetRecords(int flag, string unit, string person)
   {
      return _defaultContext.DocModels.Where(x => x.Flag == flag && x.Unit == unit && x.Person == person);
   }
   public IEnumerable<DocModel> GetChangePersonRecords(string name)
   {
      return _defaultContext.DocModels.Where(x => x.Person == name && x.Keep != 0 && x.Keep != 2);
   }
   public IEnumerable<DocModel> GetChangePersonRecords()
   {
      return _defaultContext.DocModels.Where(x => !string.IsNullOrEmpty(x.NewPersonId) && x.Keep != 0 && x.Keep != 2);
   }
   public IEnumerable<DocModel> GetHideRecords()
   {
      var keeps = new List<int> { 0, 2 };
      return _defaultContext.DocModels.Where(x => keeps.Contains(x.Keep));
   }
   public string StatusText(int status)
   {
      if (status == 1) return "待歸檔";
      if (status == 3) return "待結案";
      if (status == 4) return "已歸檔";
      return "";
   }
   public IEnumerable<DocModel> GetKeepRecords(List<int> flags)
   {
      List<int> keeps = new List<int>() { 1, 3, 4 };
      if(flags.IsNullOrEmpty()) return _defaultContext.DocModels.Where(x => keeps.Contains(x.Keep));
      return _defaultContext.DocModels.Where(x => flags.Contains(x.Flag) &&  keeps.Contains(x.Keep));
   }
   public IEnumerable<DocModel> GetHideRecords(List<int> flags)
   {
      List<int> keeps = new List<int>() { 0, 2 };
      return _defaultContext.DocModels.Where(x => flags.Contains(x.Flag) && keeps.Contains(x.Keep));
   }
   public void SetJudDoc(IEnumerable<DocModel> records)
   {
      string ps = $"審判類公文，已附卷(免歸檔)";
      foreach (var record in records)
      {
         record.Ps = ps;
         _defaultContext.DocModels.Update(record);
      }
      _defaultContext.SaveChanges();
   }
   public void SetNewPerson(IEnumerable<DocModel> records, string newPersonName, string newPersonId)
   {
      string ps = $"此案請轉新承辦人：{newPersonName} (代號{newPersonId})";
      foreach (var record in records)
      {
         record.NewPersonId = newPersonId;
         record.NewPersonName = newPersonName;
         record.Ps = ps;
         _defaultContext.DocModels.Update(record);
      }
      _defaultContext.SaveChanges();
   }

   public void ExportHideRecordsToCsv(IEnumerable<DocModel> records, string filePath)
   {
      if (records.HasItems())
      {
         using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
         {
            // Write CSV header
            writer.WriteLine("舊系統公文文號,新系統公文文號,收創日期,主旨,承辦人姓名,處理方式");

            foreach (var model in records)
            {
               string statusText = StatusText(model.Keep);
               writer.WriteLine($"{model.Old_Num},{model.Num},{model.Date},{model.Title},{model.Person},請予隱藏");
            }
         }
      }

   }

   public void ExportKeepRecordsToCsv(IEnumerable<DocModel> records, string filePath)
   {
      if (records.HasItems())
      {
         using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
         {
            // Write CSV header
            writer.WriteLine("舊系統公文文號,新系統公文文號,收創日期,主旨,承辦人姓名,狀態Id,狀態名稱");

            foreach (var model in records)
            {
               string statusText = StatusText(model.Keep);
               writer.WriteLine($"{model.Old_Num},{model.Num},{model.Date},{model.Title},{model.Person},{model.Keep},{statusText}");
            }
         }
      }
   }

}
