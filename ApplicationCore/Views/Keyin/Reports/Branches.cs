using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.Keyin
{
   public class BranchRecordReportDocument : IDocument
   {
      static IContainer CellStyleCenter(IContainer container)
      {
         return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
      }
      public BranchRecordReportModel Model { get; }
      public BranchRecordReportDocument(BranchRecordReportModel model) 
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
               columns.ConstantColumn(35);
               columns.ConstantColumn(240);
               columns.ConstantColumn(60);
               columns.ConstantColumn(60);
               columns.ConstantColumn(60);
            });

            // step 2
            table.Header(header =>
            {
               header.Cell().Element(CellStyleCenter).Text(BranchRecordLabels.Rank);
               header.Cell().Element(CellStyleCenter).Text(BranchRecordLabels.BranchId);
               header.Cell().Element(CellStyleCenter).Text(BranchRecordLabels.Score);
               header.Cell().Element(CellStyleCenter).Text(BranchRecordLabels.AbsentRate);
               header.Cell().Element(CellStyleCenter).Text(BranchRecordLabels.Diff);
            });

            // step 3
            foreach (var item in Model.Items)
            {
               var style = CellStyleCenter;
               if (item.Record.Branch!.Key == "HLH") style = CellStyleHLH;
               table.Cell().Element(style).Text(item.Record.Rank > 0 ? item.Record.Rank.ToString() : "");
               table.Cell().Element(style).Text(item.Record.Branch!.Title);
               table.Cell().Element(style).Text(item.Record.Score.ToString());
               table.Cell().Element(style).Text(item.Record.AbsentRate);
               table.Cell().Element(style).Text(item.IncreaseRateText);
               //if (item.Record.Branch!.Key == "HLH")
               //{
               //   table.Cell().Element(CellStyleHLH).Text(item.Record.Rank > 0 ? item.Record.Rank.ToString() : "");
               //   table.Cell().Element(CellStyleHLH).Text(item.Record.Branch!.Title);
               //   table.Cell().Element(CellStyleHLH).Text(item.Record.Score.ToString());
               //   table.Cell().Element(CellStyleHLH).Text(item.Record.AbsentRate.ToString());
               //   table.Cell().Element(CellStyleHLH).Text(item.Record.Diff.ToString());
               //}
               //else 
               //{
               //   table.Cell().Element(CellStyleCenter).Text(item.Record.Rank > 0 ? item.Record.Rank.ToString() : "");
               //   table.Cell().Element(CellStyleCenter).Text(item.Record.Branch!.Title);
               //   table.Cell().Element(CellStyleCenter).Text(item.Record.Score.ToString());
               //   table.Cell().Element(CellStyleCenter).Text(item.Record.AbsentRate.ToString());
               //   table.Cell().Element(CellStyleCenter).Text(item.Record.Diff.ToString());
               //}

               

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
            //column.Item().PaddingTop(15).Text(text => {
            //   text.Span("列印日期： ").SemiBold();
            //   text.Span(DateTime.Now.ToDateTimeString());
            //});
         });
      }
   }

   public class BranchRecordReportItem
   {
      public BranchRecordReportItem(BranchRecordView record) 
      {
         Record = record;
      }
      public BranchRecordView Record { get; set; }
      public string IncreaseRateText => Record.Diff == "0" ? "" : Record.Diff;
   }
   public class BranchRecordReportModel
   {
      public BranchRecordReportModel(string title, BranchRecordReportRequest request, List<BranchRecordReportItem> items) 
      {
         ReportTitle = title;
         Request = request;
         Items = items;
      }
      public string ReportTitle { get; set; } = String.Empty;

      public BranchRecordReportRequest Request { get; set; }



      public List<BranchRecordReportItem> Items = new List<BranchRecordReportItem>();

      public string Comments => "註： 缺考率逾百分之5者，不列入排名。";
   }


   public class BranchRecordReportRequest
   {
      public BranchRecordReportRequest(int year, int month)
      {
         Year = year;
         Month = month;
      }
      public int Year { get; set; }
      public int Month { get; set; }
   }

}
