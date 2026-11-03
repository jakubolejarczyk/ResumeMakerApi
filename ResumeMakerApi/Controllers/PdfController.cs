using Microsoft.AspNetCore.Mvc;
using ResumeMakerApi.Services.Logic;

namespace ResumeMakerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly PdfService _pdfService;

    public PdfController(PdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet("download")]
    public IActionResult DownloadPdf()
    {
        var pdfBytes = _pdfService.GeneratePdf();

        return File(
            pdfBytes,
            "application/pdf",
            "my-document.pdf"
        );
    }
}