using Infrastructure.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.Keyin
{
   public class PersonRecordReportDocument : IDocument
   {
      static IContainer CellStyleCenter(IContainer container)
      {
         return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
      }
      public PersonRecordReportModel Model { get; }
      public PersonRecordReportDocument(PersonRecordReportModel model) 
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
               columns.ConstantColumn(50);
               columns.ConstantColumn(100);
               columns.ConstantColumn(80);
               columns.ConstantColumn(80);
               columns.ConstantColumn(80);
               columns.ConstantColumn(80);
            });

            // step 2
            table.Header(header =>
            {
               header.Cell().Element(CellStyleCenter).Text(PersonRecordLabels.Unit);
               header.Cell().Element(CellStyleCenter).Text(PersonRecordLabels.PersonId);
               header.Cell().Element(CellStyleCenter).Text(PersonRecordLabels.Score);
               header.Cell().Element(CellStyleCenter).Text(PersonRecordLabels.CorrectRate);
               header.Cell().Element(CellStyleCenter).Text(PersonRecordLabels.Diff);
               header.Cell().Element(CellStyleCenter).Text("備註");
            });

            // step 3
            foreach (var item in Model.Items)
            {
               var style = CellStyleCenter;
               if (item.ScortText == "缺考") style = CellStyleHighlight;
               table.Cell().Element(style).Text(item.Record.Unit);
               table.Cell().Element(style).Text(item.Record.Person!.Name);
               table.Cell().Element(style).Text(item.ScortText);
               table.Cell().Element(style).Text(item.CorrectRateText);
               table.Cell().Element(style).Text(item.IncreaseRateText);
               table.Cell().Element(style).Text(item.Record.Person!.AllPass ? $"({item.Record.Person!.AllPassText})" : "");

               static IContainer CellStyleHighlight(IContainer container)
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

   public class PersonRecordReportItem
   {
      public PersonRecordReportItem(PersonRecordView record) 
      {
         Record = record;
      }
      public PersonRecordView Record { get; set; }

      public string ScortText => Record.Score == 0 ? "缺考" : Record.Score.ToString();
      public string CorrectRateText => Record.CorrectRate == "0" ? "" : Record.CorrectRate;
      public string IncreaseRateText => Record.Diff == "0" ? "" : Record.Diff;
      public int PersonId { get; set; }
      public int Score { get; set; }
      public string Diff { get; set; } = string.Empty;

   }
   public class PersonRecordReportModel
   {
      public PersonRecordReportModel(string title, PersonRecordReportRequest request, List<PersonRecordReportItem> items) 
      {
         ReportTitle = title;
         Request = request;
         Items = items;
      }
      public string ReportTitle { get; set; } = String.Empty;

      public PersonRecordReportRequest Request { get; set; }



      public List<PersonRecordReportItem> Items = new List<PersonRecordReportItem>();

      public string Comments => "";
   }


   public class PersonRecordReportRequest
   {
      public PersonRecordReportRequest(int year, int month)
      {
         Year = year;
         Month = month;
      }
      public int Year { get; set; }
      public int Month { get; set; }
   }

}
