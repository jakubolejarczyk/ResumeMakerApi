
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Services.Logic;

namespace ResumeMakerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController(
    PdfService pdfService,
    DatabaseContext context
) : ControllerBase
{
    [HttpGet("download")]
    public async Task<IActionResult> DownloadResume()
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.IsSelected)
            .OrderBy(u => u.Order)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new
            {
                message = "No selected user was found."
            });
        }

        try
        {
            var result = await pdfService.GeneratePdf(user.Id);
            return File(
                result.Pdf,
                "application/pdf",
                result.FileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}
