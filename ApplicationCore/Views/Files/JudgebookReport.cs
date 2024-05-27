using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers.Files;
using Infrastructure.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.Files
{
   public class JudgebookFileReportDocument : IDocument
   {
      public JudgebookFileReportModel Model { get; }
      public JudgebookFileReportDocument(JudgebookFileReportModel model) 
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

               column.Item().PaddingTop(15).Text(text =>
               {
                  text.Span("建檔日期： ").SemiBold();
                  text.Span(Model.Request.ReviewedAt).SemiBold();

                  text.Span("         ");
                  text.Span("裁判日期： ").SemiBold();
                  text.Span(Model.Request.JudgeDate).SemiBold();
                  //text.Span(DateTime.Now.ToDateTimeString());
                  //text.AlignRight();
               });
            });
            
         });
      }
      void ComposeContent(IContainer container)
      {
         container.PaddingVertical(40).Column(column =>
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
               columns.ConstantColumn(60);
               columns.ConstantColumn(60);
               columns.ConstantColumn(120);
               columns.ConstantColumn(80);
               columns.RelativeColumn();
            });

            // step 2
            table.Header(header =>
            {
               header.Cell().Element(CellStyle).Text("案類");
               header.Cell().Element(CellStyle).Text("書類");
               header.Cell().Element(CellStyle).Text("案號");
               header.Cell().Element(CellStyle).Text("裁判日期");
               header.Cell().Element(CellStyle).Text("檔號");

               static IContainer CellStyle(IContainer container)
               {
                  return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
               }
            });

            // step 3
            foreach (var item in Model.Items)
            {
               table.Cell().Element(CellStyle).Text(item.CourtTypeTitle);
               table.Cell().Element(CellStyle).Text(item.TypeTitle);
               table.Cell().Element(CellStyle).Text(item.CaseInfo);
               table.Cell().Element(CellStyle).Text(item.JudgeDateText);
               table.Cell().Element(CellStyle).Text(item.FileNumber);

               //table.Cell().Element(CellStyle).AlignRight().Text(item.JudgeDateText);

               static IContainer CellStyle(IContainer container)
               {
                  return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
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
            column.Item().PaddingTop(15).Text(text => {
               text.Span("列印日期： ").SemiBold();
               text.Span(DateTime.Now.ToDateTimeString());
            });
         });
      }
   }

   public class JudgebookFileReportItem
   {
      public int Id { get; set; }
      public string TypeTitle { get; set; } = String.Empty;
      public string CourtType { get; set; } = String.Empty;
      public string Year { get; set; } = String.Empty;
      public string Category { get; set; } = String.Empty;
      public string Num { get; set; } = String.Empty;
      public string FileNumber { get; set; } = String.Empty;
      public string OriginType { get; set; } = String.Empty;
      public int JudgeDate { get; set; }

      public string ReviewdAtText = String.Empty;


      public string CourtTypeTitle => CourtType.CourtTypeTitle();
      public string OriginTypeTitle => OriginType.CourtTypeTitle();
      public string CaseInfo => $"{Year} {Category} {Num}";
      public string JudgeDateText => JudgeDate.ToRocDateText();
   }
   public class JudgebookFileReportModel
   {
      public JudgebookFileReportModel(string title, JudgebookReportsRequest request, List<JudgebookFileReportItem> items) 
      {
         ReportTitle = title;
         Request = request;
         Items = items;
      }
      public string ReportTitle { get; set; } = String.Empty;

      public JudgebookReportsRequest Request { get; set; }



      public List<JudgebookFileReportItem> Items = new List<JudgebookFileReportItem>();

      public string Comments => $"合計： {Items.Count} 件";
   }


   public class JudgebookReportsRequest
   {
      public ICollection<int> Ids { get; set; } = new List<int>();
      public string JudgeDate { get; set; } = String.Empty;
      public string ReviewedAt { get; set; } = String.Empty;
   }

}
