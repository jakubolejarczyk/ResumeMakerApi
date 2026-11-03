using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ResumeMakerApi.Services.Logic;

public class PdfService
{
    public byte[] GeneratePdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header()
                    .Text("My First PDF Document")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Item().Text("Hello from ASP.NET Core!");

                        column.Item().PaddingTop(10)
                            .Text("This PDF was generated dynamically.");

                        column.Item().PaddingTop(20)
                            .Text("Generated on: " + DateTime.Now);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Generated using QuestPDF");
            });
        });

        return document.GeneratePdf();
    }
}
