using Infrastructure.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.Keyin
{
   public class PersonPassesReportDocument : IDocument
   {
      static IContainer CellStyleCenter(IContainer container)
      {
         return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
      }
      public PersonPassesReportModel Model { get; }
      public PersonPassesReportDocument(PersonPassesReportModel model) 
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
               columns.ConstantColumn(120);
               columns.ConstantColumn(120);
               columns.ConstantColumn(120);
            });

            // step 2
            table.Header(header =>
            {
               header.Cell().Element(CellStyleCenter).Text(KeyinPersonLabels.Account);
               header.Cell().Element(CellStyleCenter).Text(KeyinPersonLabels.Name);
               header.Cell().Element(CellStyleCenter).Text(KeyinPersonLabels.HighRun);
            });

            // step 3
            foreach (var item in Model.Items)
            {
               var style = CellStyleCenter;
               table.Cell().Element(style).Text(item.Person.Account);
               table.Cell().Element(style).Text(item.Person.Name);
               table.Cell().Element(style).Text(item.Person.HighRun.ToString());
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

   public class PersonPassesReportItem
   {
      public PersonPassesReportItem(KeyinPersonView person) 
      {
         Person = person;
      }
      public KeyinPersonView Person { get; set; }

   }
   public class PersonPassesReportModel
   {
      public PersonPassesReportModel(string title, PersonPassesReportRequest request, List<PersonPassesReportItem> items) 
      {
         ReportTitle = title;
         Request = request;
         Items = items;
      }
      public string ReportTitle { get; set; } = String.Empty;

      public PersonPassesReportRequest Request { get; set; }



      public List<PersonPassesReportItem> Items = new List<PersonPassesReportItem>();

      public string Comments => "";
   }


   public class PersonPassesReportRequest
   {
      public PersonPassesReportRequest()
      {
      }
   }

}
