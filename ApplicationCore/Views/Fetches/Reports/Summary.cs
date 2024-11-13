using ApplicationCore.Views.Keyin;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.Fetches
{
   public class FetchesRecordReportDocument : IDocument
   {
      public FetchesRecordReportDocument(FetchRecordsReportModel model)
      {
         Model = model;
      }      
      public FetchRecordsReportModel Model { get; }
      
      public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
      public DocumentSettings GetSettings() => DocumentSettings.Default;
      static IContainer CellStyleCenter(IContainer container)
      {
         return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
      }
      static IContainer CellStyleLeft(IContainer container)
      {
         return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignLeft();
      }

      public void Compose(IDocumentContainer container)
      {
         container
           .Page(page =>
           {
              page.Margin(50);

              page.Header().Height(100).Background(Colors.Grey.Lighten1);
              page.Header().Element(ComposeHeader);


              // page.Content().Background(Colors.Grey.Lighten3);
              page.Content().Element(ComposeContent);
              //page.Footer().Height(50).Background(Colors.Grey.Lighten1);
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
               columns.ConstantColumn(240);
            });

            // step 2
            table.Header(header =>
            {
               header.Cell().Element(CellStyleCenter).Text("系統名稱");
               header.Cell().Element(CellStyleCenter).Text("查詢紀錄筆數"); ;
            });

            // step 3
            foreach (var summary in Model.Summaries)
            {
               table.Cell().Element(CellStyleLeft).Text(summary.System);
               table.Cell().Element(CellStyleCenter).Text(summary.Count);               
            }
         });
      }
      void ComposeComments(IContainer container)
      {
         container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
         {
            column.Spacing(5);
            column.Item().Text(Model.Comments);
            //column.Item().PaddingTop(15).Text(text => {
            //   text.Span("列印日期： ").SemiBold();
            //   text.Span(DateTime.Now.ToDateTimeString());
            //});
         });
      }
   }

   public class FetchRecordsSummary
   {
      public FetchRecordsSummary(string system, int count)
      {
         System = system;
         Count = count;
      }
      public string System { get; set; }
      public ICollection<FetchesRecordView> Records { get; set; } = new List<FetchesRecordView>();
      public int Count { get; set; }
   }
   
   public class FetchRecordsReportModel
   {
      public FetchRecordsReportModel(string title, List<FetchRecordsSummary> summaries) 
      {
         ReportTitle = title;
         Summaries = summaries;
      }
      public string ReportTitle { get; set; } = String.Empty;



      public List<FetchRecordsSummary> Summaries = new List<FetchRecordsSummary>();

      public string Comments => $"合計： {Summaries.Sum(x => x.Count)} 筆";
   }


}
