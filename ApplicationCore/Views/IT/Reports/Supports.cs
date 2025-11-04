using ApplicationCore.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.IT;

public class SupportRecordReportModel
{
   public SupportRecordReportModel(string title, List<ITSupportGroup> groups)
   {
      ReportTitle = title;
      Groups = groups;
   }
   public string ReportTitle { get; set; } = String.Empty;

   public List<ITSupportGroup> Groups = new List<ITSupportGroup>();

   public int TotalCount => Groups.Sum(g => g.TotalCount);

   public string Comments => $"總計： {TotalCount} 人次";
}
public class SupportRecordReportDocument : IDocument
{
   static IContainer CellStyleCenter(IContainer container)
   {
      return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
   }
   public SupportRecordReportModel Model { get; }
   public SupportRecordReportDocument(SupportRecordReportModel model)
   {
      Model = model;
   }
   public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
   public DocumentSettings GetSettings() => DocumentSettings.Default;
   public void Compose(IDocumentContainer container)
   {
      container
        .Page(page =>
        {
           page.Margin(50);

           page.Header().Height(100).Background(Colors.Grey.Lighten1);
           page.Header().Element(ComposeHeader);

           page.Content().Element(ComposeContent);
        });
   }
   void ComposeHeader(IContainer container)
   {
      var titleStyle = TextStyle.Default.FontSize(20).SemiBold();//.FontColor(Colors.Blue.Medium);

      container.Row(row =>
      {
         row.RelativeItem().Column(column =>
         {
            column.Item().Text(Model.ReportTitle).Style(titleStyle).AlignCenter();
         });

      });
   }
   void ComposeContent(IContainer container)
   {
      container.PaddingVertical(20).Column(column =>
      {
         column.Spacing(5);

         column.Item().Element(ComposeTable);

         if (!string.IsNullOrWhiteSpace(Model.Comments))
            column.Item().PaddingTop(25).Element(ComposeComments);
      });
   }
   void ComposeTable(IContainer container)
   {
      container.Table(table =>
      {
         // step 1
         table.ColumnsDefinition(columns =>
         {
            columns.ConstantColumn(240);
            columns.ConstantColumn(120);
         });

         // step 2
         table.Header(header =>
         {
            header.Cell().Element(CellStyleCenter).Text("問題類別");
            header.Cell().Element(CellStyleCenter).Text("處理人次");
         });

         // step 3
         foreach (var item in Model.Groups)
         {
            var style = CellStyleCenter;
            table.Cell().Element(style).Text(item.Title);
            table.Cell().Element(style).Text(item.Records.Count().ToString());

            static IContainer CellStyleHLH(IContainer container)
            {

               return container
                       .Border(0)
                       .Background(Colors.Grey.Lighten1)
                       .PaddingVertical(5)
                       .PaddingHorizontal(10)
                       .AlignCenter()
                       .AlignMiddle();
            }

         }
      });
   }
   void ComposeComments(IContainer container)
   {
      container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
      {
         column.Spacing(5);
         column.Item().Text(Model.Comments);
      });
   }
}
public class SupportRecordDetailsDocument : IDocument
{
   static IContainer CellStyleCenter(IContainer container)
   {
      return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
   }
   public SupportRecordReportModel Model { get; }
   public SupportRecordDetailsDocument(SupportRecordReportModel model)
   {
      Model = model;
   }
   public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
   public DocumentSettings GetSettings() => DocumentSettings.Default;
   public void Compose(IDocumentContainer container)
   {
      container
        .Page(page =>
        {
           page.Margin(50);

           page.Content().Column(column =>
           {
              // Header — 只出現在第一頁
              column.Item().Element(ComposeHeader);

              //內容 — 自動分頁
              column.Item().Element(ComposeContent);
           });
        });
   }
   void ComposeHeader(IContainer container)
   {
      var titleStyle = TextStyle.Default.FontSize(20).SemiBold();//.FontColor(Colors.Blue.Medium);

      container.Row(row =>
      {
         row.RelativeItem().Column(column =>
         {
            column.Item().Text(Model.ReportTitle).Style(titleStyle).AlignCenter();
         });

      });
   }
   void ComposeContent(IContainer container)
   {
      container.PaddingVertical(20).Column(column =>
      {
         column.Spacing(5);

         column.Item().Element(ComposeTable);

         if (!string.IsNullOrWhiteSpace(Model.Comments))
            column.Item().PaddingTop(25).Element(ComposeComments);
      });
   }
   void ComposeTable(IContainer container)
   {
      var label = new SupportLabel();
      container.Table(table =>
      {
         // step 1
         table.ColumnsDefinition(columns =>
         {
            columns.ConstantColumn(60);
            columns.ConstantColumn(40);
            columns.ConstantColumn(80);
            columns.ConstantColumn(80);
            columns.ConstantColumn(80);
            columns.ConstantColumn(40);
            columns.ConstantColumn(40);
            columns.ConstantColumn(40);
         });

         // step 2
         table.Header(header =>
         {
            header.Cell().Element(CellStyleCenter).Text(label.Date).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.User).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Name).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Kind).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Content).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Result).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Person).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.PersonCount).FontSize(8);
         });

         // step 3
         foreach (var group in Model.Groups)
         {
            var style = CellStyleHLH;
            //var style = CellStyleCenter;

            table.Cell().ColumnSpan(2).Element(style).Text(group.Title).FontSize(10);
            //table.Cell().Element(style).Text("");
            table.Cell().ColumnSpan(2).Element(style).Text($"小計： {group.TotalCount.ToString()} 人次").FontSize(10);
            table.Cell().Element(style).Text("");
            table.Cell().Element(style).Text("");
            table.Cell().Element(style).Text("");
            table.Cell().Element(style).Text("");
            // 明細列
            foreach (var record in group.Records)
            {
               table.Cell().Element(CellStyleCenter).Text(record.Date).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.User).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.Name).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.Kind).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.Content).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.Result).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.Person).FontSize(8);
               table.Cell().Element(CellStyleCenter).Text(record.PersonCount.ToString()).FontSize(8);
            }



            static IContainer CellStyleHLH(IContainer container)
            {

               return container
                       .Border(0)
                       .Background(Colors.Grey.Lighten1)
                       .PaddingVertical(5)
                       .PaddingHorizontal(10)
                       .AlignCenter()
                       .AlignMiddle();
            }

         }
      });
   }
   void ComposeComments(IContainer container)
   {
      container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
      {
         column.Spacing(5);
         column.Item().Text(Model.Comments);
      });
   }
}