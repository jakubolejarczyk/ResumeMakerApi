
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeMakerApi.Data;
using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Models;
using System.Globalization;

public class PdfService
{
    private readonly DatabaseContext context;

    private static readonly CultureInfo PolishCulture =
        CultureInfo.GetCultureInfo("pl-PL");

    // Kolory
    private const string PrimaryColor = "#171717";
    private const string AccentColor = "#2563EB";
    private const string TextColor = "#404040";
    private const string SecondaryColor = "#737373";
    private const string DividerColor = "#D4D4D4";

    // Spójne odstępy
    private const float BodyLineHeight = 1.2f;
    private const float BulletLineHeight = 1.3f;
    private const float SectionSpacing = 12f;
    private const float ItemSpacing = 6f;
    private const float ExperienceSpacing = 11f;
    private const float BulletSpacing = 3f;

    public PdfService(DatabaseContext context)
    {
        this.context = context;
    }

    public async Task<byte[]> GeneratePdf(int userId)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Pobranie wybranego CV użytkownika
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

        // Doświadczenie
        var experiences = await context.Experiences
            .AsNoTracking()
            .Where(e => e.ParentId == resume.Id)
            .OrderBy(e => e.Order)
            .ToListAsync();

        // Edukacja
        var educations = await context.Educations
            .AsNoTracking()
            .Where(e => e.ParentId == resume.Id)
            .OrderBy(e => e.Order)
            .ToListAsync();

        // Umiejętności
        var skills = await context.Skills
            .AsNoTracking()
            .Where(s => s.ParentId == resume.Id)
            .OrderBy(s => s.Order)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);

                page.MarginHorizontal(23);
                page.MarginVertical(19);

                page.PageColor(Colors.White);

                page.DefaultTextStyle(style => style
                    .FontFamily("Lato")
                    .FontSize(8.7f)
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
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(PrimaryColor);

                                if (!string.IsNullOrWhiteSpace(resume.JobTitle))
                                {
                                    left.Item()
                                        .PaddingTop(3)
                                        .Text(resume.JobTitle)
                                        .FontSize(11.5f)
                                        .SemiBold()
                                        .FontColor(AccentColor);
                                }
                            });

                        header.RelativeItem(1)
                            .AlignRight()
                            .Column(right =>
                            {
                                // E-mail
                                AddHeaderContact(
                                    right,
                                    resume.User?.Email,
                                    CreateEmailLink(resume.User?.Email));

                                // Telefon
                                AddHeaderContact(
                                    right,
                                    resume.User?.PhoneNumber,
                                    CreatePhoneLink(resume.User?.PhoneNumber));

                                // Lokalizacja - bez linku
                                var location = string.Join(", ",
                                    new[] { resume.City, resume.Country }
                                        .Where(x => !string.IsNullOrWhiteSpace(x)));

                                AddHeaderContact(right, location);
                            });
                    });

                    // Linia pod nagłówkiem
                    layout.Item()
                        .PaddingTop(11)
                        .PaddingBottom(12)
                        .Height(2)
                        .Background(AccentColor);

                    // =====================================
                    // GŁÓWNA CZĘŚĆ - DWIE KOLUMNY
                    // =====================================

                    layout.Item().Row(columns =>
                    {
                        // =====================================
                        // LEWA KOLUMNA
                        // =====================================

                        columns.ConstantItem(175)
                            .PaddingRight(18)
                            .Column(left =>
                            {
                                // TECHNOLOGIE
                                if (skills.Any())
                                {
                                    AddSectionTitle(left, "TECHNOLOGIE");

                                    foreach (var skill in skills)
                                    {
                                        if (!string.IsNullOrWhiteSpace(skill.Category))
                                        {
                                            left.Item()
                                                .PaddingTop(5)
                                                .Text(skill.Category)
                                                .FontSize(8.7f)
                                                .Bold()
                                                .FontColor(PrimaryColor);
                                        }

                                        if (skill.Skills != null)
                                        {
                                            foreach (var item in skill.Skills)
                                            {
                                                if (!string.IsNullOrWhiteSpace(item))
                                                {
                                                    AddBullet(left, item, 8);
                                                }
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
                                                item.Item()
                                                    .Text($"{education.StartYear} – {education.EndYear}")
                                                    .FontSize(8)
                                                    .FontColor(AccentColor)
                                                    .SemiBold();

                                                if (!string.IsNullOrWhiteSpace(education.Degree))
                                                {
                                                    item.Item()
                                                        .Text(education.Degree)
                                                        .FontSize(8.7f)
                                                        .Bold()
                                                        .FontColor(PrimaryColor);
                                                }

                                                if (!string.IsNullOrWhiteSpace(education.FieldOfStudy))
                                                {
                                                    item.Item()
                                                        .Text(education.FieldOfStudy)
                                                        .FontSize(8)
                                                        .FontColor(TextColor)
                                                        .LineHeight(BodyLineHeight);
                                                }

                                                if (!string.IsNullOrWhiteSpace(education.InstitutionName))
                                                {
                                                    item.Item()
                                                        .Text(education.InstitutionName)
                                                        .FontSize(8)
                                                        .FontColor(SecondaryColor)
                                                        .LineHeight(BodyLineHeight);
                                                }
                                            });
                                    }
                                }

                                // LINKI
                                if (resume.User?.Links != null &&
                                    resume.User.Links.Length > 0)
                                {
                                    left.Item().PaddingTop(SectionSpacing);

                                    AddSectionTitle(left, "LINKI");

                                    foreach (var link in resume.User.Links)
                                    {
                                        if (!string.IsNullOrWhiteSpace(link))
                                        {
                                            AddWebLink(left, link);
                                        }
                                    }
                                }
                            });

                        // =====================================
                        // PRAWA KOLUMNA
                        // =====================================

                        columns.RelativeItem()
                            .PaddingLeft(18)
                            .Column(right =>
                            {
                                // PODSUMOWANIE
                                if (!string.IsNullOrWhiteSpace(resume.Summary))
                                {
                                    AddSectionTitle(right, "PODSUMOWANIE");

                                    right.Item()
                                        .PaddingBottom(SectionSpacing)
                                        .Text(resume.Summary)
                                        .FontSize(8.7f)
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
                                                                    .Text(experience.JobTitle)
                                                                    .FontSize(10.5f)
                                                                    .Bold()
                                                                    .FontColor(PrimaryColor);
                                                            }

                                                            if (!string.IsNullOrWhiteSpace(experience.Company))
                                                            {
                                                                details.Item()
                                                                    .PaddingTop(2)
                                                                    .Text(experience.Company)
                                                                    .FontSize(8.7f)
                                                                    .SemiBold()
                                                                    .FontColor(SecondaryColor);
                                                            }
                                                        });

                                                    header.ConstantItem(125)
                                                        .AlignRight()
                                                        .AlignTop()
                                                        .Text(
                                                            $"{FormatDate(experience.StartDate)} – {FormatDate(experience.EndDate, true)}")
                                                        .FontSize(7.5f)
                                                        .FontColor(AccentColor)
                                                        .SemiBold();
                                                });

                                                if (experience.Description != null)
                                                {
                                                    foreach (var description in experience.Description)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(description))
                                                        {
                                                            AddBullet(item, description, 8.7f);
                                                        }
                                                    }
                                                }
                                            });
                                    }
                                }

                                // RODO
                                if (!string.IsNullOrWhiteSpace(resume.Rodo))
                                {
                                    right.Item()
                                        .PaddingTop(7)
                                        .Text(resume.Rodo)
                                        .FontSize(7)
                                        .FontColor(SecondaryColor)
                                        .LineHeight(BodyLineHeight);
                                }
                            });
                    });
                });
            });
        });

        return document.GeneratePdf();
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

        return date;
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
            .PaddingBottom(3)
            .AlignRight();

        if (!string.IsNullOrWhiteSpace(hyperlink))
        {
            item.DefaultTextStyle(style => style
                .FontFamily("Lato")
                .FontSize(7.5f)
                .FontColor(SecondaryColor)
                .LineHeight(BodyLineHeight))
                .Text(text =>
                {
                    text.Hyperlink(value.Trim(), hyperlink);
                });
        }
        else
        {
            item.Text(value)
                .FontFamily("Lato")
                .FontSize(7.5f)
                .FontColor(SecondaryColor)
                .LineHeight(BodyLineHeight);
        }
    }

    // =====================================
    // LINKI WWW W SEKCJI LINKI
    // =====================================

    private static void AddWebLink(
        ColumnDescriptor column,
        string link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return;

        var normalizedLink = NormalizeWebUrl(link);

        var item = column.Item().PaddingTop(3);

        if (!string.IsNullOrWhiteSpace(normalizedLink))
        {
            item.DefaultTextStyle(style => style
                .FontFamily("Lato")
                .FontSize(8)
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
                .FontSize(8)
                .FontColor(AccentColor)
                .LineHeight(BodyLineHeight);
        }
    }

    // =====================================
    // TYTUŁ SEKCJI
    // =====================================

    private static void AddSectionTitle(
        ColumnDescriptor column,
        string title)
    {
        column.Item()
            .PaddingBottom(5)
            .Text(title)
            .FontFamily("Lato")
            .FontSize(9.7f)
            .Bold()
            .FontColor(PrimaryColor);

        column.Item()
            .PaddingBottom(3)
            .Height(1)
            .Background(DividerColor);
    }

    // =====================================
    // PUNKT LISTY Z WCIĘCIEM WISZĄCYM
    // =====================================

    private static void AddBullet(
        ColumnDescriptor column,
        string text,
        float fontSize = 8.5f)
    {
        column.Item()
            .PaddingTop(BulletSpacing)
            .PaddingLeft(5)
            .Row(row =>
            {
                row.ConstantItem(11)
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
