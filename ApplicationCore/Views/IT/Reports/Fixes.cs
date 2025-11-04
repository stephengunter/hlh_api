using ApplicationCore.Services;
using Infrastructure.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplicationCore.Views.IT;

public class FixRecordReportModel
{
   public FixRecordReportModel(string title, List<FixViewModel> records)
   {
      ReportTitle = title;
      Records = records;
   }
   public string ReportTitle { get; set; } = String.Empty;

   public List<FixViewModel> Records = new List<FixViewModel>();

   public int TotalCount => Records.Count;

   public string Comments => $"總計： {TotalCount} 件";
}
public class FixRecordDetailsDocument : IDocument
{
   static IContainer CellStyleCenter(IContainer container)
   {
      return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter();
   }
   public FixRecordReportModel Model { get; }
   public FixRecordDetailsDocument(FixRecordReportModel model)
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
      var label = new FixRecordLabel();
      container.Table(table =>
      {
         // step 1
         table.ColumnsDefinition(columns =>
         {
            columns.ConstantColumn(60);
            columns.ConstantColumn(60);
            columns.ConstantColumn(60);
            columns.ConstantColumn(80);
            columns.ConstantColumn(30);
            columns.ConstantColumn(100);
            columns.ConstantColumn(100);
         });

         // step 2
         table.Header(header =>
         {
            header.Cell().Element(CellStyleCenter).Text(label.Date).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Number).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.User).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Content).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Count).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Result).FontSize(8);
            header.Cell().Element(CellStyleCenter).Text(label.Ps).FontSize(8);
         });

         // step 3
         foreach (var record in Model.Records)
         {
            table.Cell().Element(CellStyleCenter).Text(record.Date).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.Number).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.User).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.Content).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.Count.ToString()).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.Result).FontSize(8);
            table.Cell().Element(CellStyleCenter).Text(record.Ps.ToString()).FontSize(8);


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