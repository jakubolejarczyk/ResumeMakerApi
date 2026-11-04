
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeMakerApi.Database.Context;
using System.Globalization;

public class PdfService
{
    private readonly DatabaseContext context;

    private static readonly CultureInfo PolishCulture =
        CultureInfo.GetCultureInfo("pl-PL");

    // Paleta kolorów
    private const string PrimaryColor = "#172033";
    private const string AccentColor = "#2563EB";
    private const string TextColor = "#3F4754";
    private const string SecondaryColor = "#727C8C";
    private const string DividerColor = "#E4E8EF";
    private const string HeaderDividerColor = "#2563EB";

    // Typografia i odstępy
    private const float BodyFontSize = 8.8f;
    private const float BodyLineHeight = 1.25f;
    private const float BulletLineHeight = 1.3f;

    private const float SectionSpacing = 13f;
    private const float ItemSpacing = 7f;
    private const float ExperienceSpacing = 12f;
    private const float BulletSpacing = 3f;

    public PdfService(DatabaseContext context)
    {
        this.context = context;
    }

    public async Task<(byte[] Pdf, string FileName)> GeneratePdf(int userId)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // =====================================
        // POBIERANIE DANYCH
        // =====================================

        var resume = await context.Resumes
            .AsNoTracking()
            .Where(r => r.User.Id == userId && r.IsSelected)
            .OrderBy(r => r.Order)
            .Include(r => r.User)
            .FirstOrDefaultAsync();

        if (resume == null)
        {
            throw new Exception("Nie znaleziono wybranego CV użytkownika.");
        }

        var experiences = await context.Experiences
            .AsNoTracking()
            .Where(e => e.ParentId == resume.Id)
            .OrderBy(e => e.Order)
            .ToListAsync();

        var educations = await context.Educations
            .AsNoTracking()
            .Where(e => e.ParentId == resume.Id)
            .OrderBy(e => e.Order)
            .ToListAsync();

        var skills = await context.Skills
            .AsNoTracking()
            .Where(s => s.ParentId == resume.Id)
            .OrderBy(s => s.Order)
            .ToListAsync();

        // =====================================
        // GENEROWANIE PDF
        // =====================================

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);

                page.MarginHorizontal(25);
                page.MarginVertical(22);

                page.PageColor(Colors.White);

                page.DefaultTextStyle(style => style
                    .FontFamily("Lato")
                    .FontSize(BodyFontSize)
                    .FontColor(TextColor)
                    .LineHeight(BodyLineHeight));

                page.Content().Column(layout =>
                {
                    // =====================================
                    // NAGŁÓWEK
                    // =====================================

                    layout.Item().Row(header =>
                    {
                        header.RelativeItem(1.5f)
                            .Column(left =>
                            {
                                var fullName =
                                    $"{resume.User?.FirstName} {resume.User?.LastName}".Trim();

                                left.Item()
                                    .Text(string.IsNullOrWhiteSpace(fullName)
                                        ? "Twoje CV"
                                        : fullName)
                                    .FontFamily("Lato")
                                    .FontSize(25)
                                    .Bold()
                                    .FontColor(PrimaryColor);

                                if (!string.IsNullOrWhiteSpace(resume.JobTitle))
                                {
                                    left.Item()
                                        .PaddingTop(4)
                                        .Text(resume.JobTitle.Trim())
                                        .FontSize(11)
                                        .SemiBold()
                                        .FontColor(AccentColor);
                                }
                            });

                        header.RelativeItem(1)
                            .AlignRight()
                            .Column(right =>
                            {
                                AddHeaderContact(
                                    right,
                                    resume.User?.Email,
                                    CreateEmailLink(resume.User?.Email));

                                AddHeaderContact(
                                    right,
                                    resume.User?.PhoneNumber,
                                    CreatePhoneLink(resume.User?.PhoneNumber));

                                var location = string.Join(", ",
                                    new[] { resume.City, resume.Country }
                                        .Where(x => !string.IsNullOrWhiteSpace(x)));

                                AddHeaderContact(right, location);
                            });
                    });

                    // Mocniejsza linia pod nagłówkiem
                    layout.Item()
                        .PaddingTop(12)
                        .PaddingBottom(15)
                        .Height(1.2f)
                        .Background(HeaderDividerColor);

                    // =====================================
                    // DWIE KOLUMNY
                    // =====================================

                    layout.Item().Row(columns =>
                    {
                        // =====================================
                        // LEWA KOLUMNA
                        // =====================================

                        columns.ConstantItem(170)
                            .PaddingRight(17)
                            .Column(left =>
                            {
                                // TECHNOLOGIE I UMIEJĘTNOŚCI
                                if (skills.Any())
                                {
                                    AddSectionTitle(left, "UMIEJĘTNOŚCI");

                                    foreach (var skill in skills)
                                    {
                                        var validSkills = skill.Skills?
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .ToList();

                                        if (!string.IsNullOrWhiteSpace(skill.Category))
                                        {
                                            left.Item()
                                                .PaddingTop(6)
                                                .PaddingBottom(2)
                                                .Text(skill.Category.Trim())
                                                .FontSize(8.7f)
                                                .Bold()
                                                .FontColor(PrimaryColor);
                                        }

                                        if (validSkills != null)
                                        {
                                            foreach (var item in validSkills)
                                            {
                                                AddBullet(left, item, 8.3f);
                                            }
                                        }
                                    }
                                }

                                // EDUKACJA
                                if (educations.Any())
                                {
                                    left.Item().PaddingTop(SectionSpacing);

                                    AddSectionTitle(left, "EDUKACJA");

                                    foreach (var education in educations)
                                    {
                                        left.Item()
                                            .PaddingTop(ItemSpacing)
                                            .Column(item =>
                                            {
                                                var dateRange = FormatYearRange(
                                                    education.StartYear,
                                                    education.EndYear);

                                                if (!string.IsNullOrWhiteSpace(dateRange))
                                                {
                                                    item.Item()
                                                        .Text(dateRange)
                                                        .FontSize(7.8f)
                                                        .SemiBold()
                                                        .FontColor(AccentColor);
                                                }

                                                if (!string.IsNullOrWhiteSpace(education.Degree))
                                                {
                                                    item.Item()
                                                        .PaddingTop(2)
                                                        .Text(education.Degree.Trim())
                                                        .FontSize(8.8f)
                                                        .Bold()
                                                        .FontColor(PrimaryColor);
                                                }

                                                if (!string.IsNullOrWhiteSpace(education.FieldOfStudy))
                                                {
                                                    item.Item()
                                                        .PaddingTop(2)
                                                        .Text(education.FieldOfStudy.Trim())
                                                        .FontSize(8.2f)
                                                        .FontColor(TextColor);
                                                }

                                                if (!string.IsNullOrWhiteSpace(education.InstitutionName))
                                                {
                                                    item.Item()
                                                        .PaddingTop(2)
                                                        .Text(education.InstitutionName.Trim())
                                                        .FontSize(8.1f)
                                                        .FontColor(SecondaryColor);
                                                }
                                            });
                                    }
                                }

                                // LINKI
                                var userLinks = resume.User?.Links?
                                    .Where(link => !string.IsNullOrWhiteSpace(link))
                                    .ToList();

                                if (userLinks != null && userLinks.Any())
                                {
                                    left.Item().PaddingTop(SectionSpacing);

                                    AddSectionTitle(left, "LINKI");

                                    foreach (var link in userLinks)
                                    {
                                        AddWebLink(left, link);
                                    }
                                }
                            });

                        // =====================================
                        // PRAWA KOLUMNA
                        // =====================================

                        columns.RelativeItem()
                            .PaddingLeft(17)
                            .Column(right =>
                            {
                                // PODSUMOWANIE
                                if (!string.IsNullOrWhiteSpace(resume.Summary))
                                {
                                    AddSectionTitle(right, "PROFIL ZAWODOWY");

                                    right.Item()
                                        .PaddingBottom(SectionSpacing)
                                        .Text(resume.Summary.Trim())
                                        .FontSize(BodyFontSize)
                                        .FontColor(TextColor)
                                        .LineHeight(BodyLineHeight);
                                }

                                // DOŚWIADCZENIE
                                if (experiences.Any())
                                {
                                    AddSectionTitle(right, "DOŚWIADCZENIE");

                                    foreach (var experience in experiences)
                                    {
                                        right.Item()
                                            .PaddingBottom(ExperienceSpacing)
                                            .Column(item =>
                                            {
                                                item.Item().Row(header =>
                                                {
                                                    header.RelativeItem()
                                                        .Column(details =>
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(experience.JobTitle))
                                                            {
                                                                details.Item()
                                                                    .Text(experience.JobTitle.Trim())
                                                                    .FontSize(10.2f)
                                                                    .Bold()
                                                                    .FontColor(PrimaryColor);
                                                            }

                                                            if (!string.IsNullOrWhiteSpace(experience.Company))
                                                            {
                                                                details.Item()
                                                                    .PaddingTop(2)
                                                                    .Text(experience.Company.Trim())
                                                                    .FontSize(8.7f)
                                                                    .SemiBold()
                                                                    .FontColor(SecondaryColor);
                                                            }
                                                        });

                                                    var dateRange = FormatDateRange(
                                                        experience.StartDate,
                                                        experience.EndDate);

                                                    if (!string.IsNullOrWhiteSpace(dateRange))
                                                    {
                                                        header.ConstantItem(112)
                                                            .AlignRight()
                                                            .AlignTop()
                                                            .Text(dateRange)
                                                            .FontSize(7.3f)
                                                            .SemiBold()
                                                            .FontColor(AccentColor);
                                                    }
                                                });

                                                if (experience.Description != null)
                                                {
                                                    foreach (var description in experience.Description)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(description))
                                                        {
                                                            AddBullet(item, description, 8.5f);
                                                        }
                                                    }
                                                }
                                            });
                                    }
                                }

                                // KLAUZULA RODO
                                if (!string.IsNullOrWhiteSpace(resume.Rodo))
                                {
                                    right.Item()
                                        .PaddingTop(5)
                                        .Text(resume.Rodo.Trim())
                                        .FontSize(6.8f)
                                        .FontColor(SecondaryColor)
                                        .LineHeight(1.2f);
                                }
                            });
                    });
                });
            });
        });

        var pdf = document.GeneratePdf();

        // Pobranie nazwy CV z bazy danych
        var resumeName = string.IsNullOrWhiteSpace(resume.Name)
            ? "CV"
            : resume.Name.Trim();

        // Usunięcie niedozwolonych znaków z nazwy pliku
        var invalidChars = Path.GetInvalidFileNameChars();

        var safeFileName = new string(
            resumeName
                .Select(c => invalidChars.Contains(c) ? '_' : c)
                .ToArray());

        // Ostateczna nazwa pliku
        var fileName = $"{safeFileName}.pdf";

        return (pdf, fileName);
    }

    // =====================================
    // FORMATOWANIE DAT
    // =====================================

    private static string FormatDate(string? date, bool isEndDate = false)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            return isEndDate ? "obecnie" : "";
        }

        string[] formats =
        {
            "yyyy-MM-dd",
            "yyyy-MM",
            "yyyy",
            "dd.MM.yyyy",
            "d.M.yyyy",
            "MM/yyyy",
            "M/yyyy",
            "dd/MM/yyyy",
            "d/M/yyyy"
        };

        if (DateTime.TryParseExact(
            date.Trim(),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate))
        {
            return parsedDate.ToString("MMMM yyyy", PolishCulture);
        }

        if (DateTime.TryParse(
            date,
            PolishCulture,
            DateTimeStyles.None,
            out parsedDate))
        {
            return parsedDate.ToString("MMMM yyyy", PolishCulture);
        }

        if (DateTime.TryParse(
            date,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate))
        {
            return parsedDate.ToString("MMMM yyyy", PolishCulture);
        }

        return date.Trim();
    }

    private static string FormatDateRange(string? startDate, string? endDate)
    {
        var start = FormatDate(startDate);
        var end = FormatDate(endDate, true);

        if (string.IsNullOrWhiteSpace(start))
            return end;

        if (string.IsNullOrWhiteSpace(end))
            return start;

        return $"{start} – {end}";
    }

    private static string FormatYearRange(int startYear, int endYear)
    {
        if (startYear <= 0)
            return endYear > 0 ? endYear.ToString() : "";

        if (endYear <= 0)
            return $"{startYear} – obecnie";

        return $"{startYear} – {endYear}";
    }

    // =====================================
    // LINK E-MAIL
    // =====================================

    private static string? CreateEmailLink(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return $"mailto:{email.Trim()}";
    }

    // =====================================
    // LINK TELEFON
    // =====================================

    private static string? CreatePhoneLink(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var trimmedPhone = phone.Trim();

        var digits = new string(
            trimmedPhone.Where(char.IsDigit).ToArray());

        if (digits.Length == 0)
            return null;

        var prefix = trimmedPhone.StartsWith("+") ? "+" : "";

        return $"tel:{prefix}{digits}";
    }

    // =====================================
    // NORMALIZACJA LINKÓW WWW
    // =====================================

    private static string? NormalizeWebUrl(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return null;

        var url = link.Trim();

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "https://" + url;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps))
        {
            return uri.AbsoluteUri;
        }

        return null;
    }

    // =====================================
    // KONTAKT W NAGŁÓWKU
    // =====================================

    private static void AddHeaderContact(
        ColumnDescriptor column,
        string? value,
        string? hyperlink = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        var item = column.Item()
            .PaddingBottom(4)
            .AlignRight();

        if (!string.IsNullOrWhiteSpace(hyperlink))
        {
            item.DefaultTextStyle(style => style
                .FontFamily("Lato")
                .FontSize(7.8f)
                .FontColor(SecondaryColor)
                .LineHeight(BodyLineHeight))
                .Text(text =>
                {
                    text.Hyperlink(value.Trim(), hyperlink);
                });
        }
        else
        {
            item.Text(value.Trim())
                .FontFamily("Lato")
                .FontSize(7.8f)
                .FontColor(SecondaryColor)
                .LineHeight(BodyLineHeight);
        }
    }

    // =====================================
    // LINKI WWW
    // =====================================

    private static void AddWebLink(
        ColumnDescriptor column,
        string link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return;

        var normalizedLink = NormalizeWebUrl(link);

        var item = column.Item()
            .PaddingTop(4);

        if (!string.IsNullOrWhiteSpace(normalizedLink))
        {
            item.DefaultTextStyle(style => style
                .FontFamily("Lato")
                .FontSize(8.1f)
                .FontColor(AccentColor)
                .LineHeight(BodyLineHeight))
                .Text(text =>
                {
                    text.Hyperlink(link.Trim(), normalizedLink);
                });
        }
        else
        {
            item.Text(link.Trim())
                .FontFamily("Lato")
                .FontSize(8.1f)
                .FontColor(AccentColor)
                .LineHeight(BodyLineHeight);
        }
    }

    // =====================================
    // NAGŁÓWKI SEKCJI
    // =====================================

    private static void AddSectionTitle(
        ColumnDescriptor column,
        string title)
    {
        column.Item()
            .PaddingBottom(5)
            .Row(row =>
            {
                row.ConstantItem(3)
                    .Height(10)
                    .Background(AccentColor);

                row.RelativeItem()
                    .PaddingLeft(7)
                    .AlignMiddle()
                    .Text(title)
                    .FontFamily("Lato")
                    .FontSize(9.2f)
                    .Bold()
                    .FontColor(PrimaryColor);
            });

        column.Item()
            .PaddingBottom(4)
            .Height(0.5f)
            .Background(DividerColor);
    }

    // =====================================
    // PUNKTY LISTY
    // =====================================

    private static void AddBullet(
        ColumnDescriptor column,
        string text,
        float fontSize = 8.5f)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        column.Item()
            .PaddingTop(BulletSpacing)
            .PaddingLeft(2)
            .Row(row =>
            {
                row.ConstantItem(10)
                    .PaddingTop(1)
                    .Text("•")
                    .FontFamily("Lato")
                    .FontSize(fontSize)
                    .FontColor(AccentColor);

                row.RelativeItem()
                    .Text(text.Trim())
                    .FontFamily("Lato")
                    .FontSize(fontSize)
                    .FontColor(TextColor)
                    .LineHeight(BulletLineHeight);
            });
    }
}
