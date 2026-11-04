
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;

namespace ResumeMakerApi.Services.Logic;

public class PdfService(DatabaseContext context)
{
    public async Task<byte[]> GeneratePdf(int userId)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Find the first selected resume belonging to the user.
        var resume = await context.Resumes
            .AsNoTracking()
            .Where(r => r.User.Id == userId && r.IsSelected)
            .OrderBy(r => r.Order)
            .Include(r => r.User)
            .Include(r => r.Skills)
            .Include(r => r.Experiences)
            .Include(r => r.Educations)
            .FirstOrDefaultAsync();

        if (resume == null)
        {
            throw new KeyNotFoundException(
                "No selected resume was found for this user.");
        }

        var user = resume.User;

        // Fetch selected skills.
        var skills = resume.Skills
            .Where(s => s.IsSelected)
            .OrderBy(s => s.Order)
            .ToList();

        // Fetch selected experiences.
        var experiences = resume.Experiences
            .Where(e => e.IsSelected)
            .OrderBy(e => e.Order)
            .ToList();

        // Fetch selected educations.
        var educations = resume.Educations
            .Where(e => e.IsSelected)
            .OrderBy(e => e.Order)
            .ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.DefaultTextStyle(style =>
                    style.FontSize(10)
                        .FontColor(Colors.Grey.Darken3));

                page.PageColor(Colors.White);

                // HEADER
                page.Header().Column(column =>
                {
                    column.Spacing(4);

                    column.Item()
                        .Text($"{user.FirstName} {user.LastName}")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);

                    column.Item()
                        .Text(resume.JobTitle)
                        .FontSize(14)
                        .FontColor(Colors.Grey.Darken1);

                    column.Item()
                        .PaddingTop(8)
                        .Text($"{user.Email} | {user.PhoneNumber}")
                        .FontSize(9);

                    column.Item()
                        .Text($"{resume.City}, {resume.Country}")
                        .FontSize(9);

                    foreach (var link in user.Links ?? [])
                    {
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            column.Item()
                                .Text(link)
                                .FontSize(9)
                                .FontColor(Colors.Blue.Darken2);
                        }
                    }

                    column.Item()
                        .PaddingTop(10)
                        .LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten1);
                });

                // MAIN CONTENT
                page.Content()
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        // PROFILE / SUMMARY
                        if (!string.IsNullOrWhiteSpace(resume.Summary))
                        {
                            column.Item().Column(section =>
                            {
                                AddSectionTitle(section, "PROFILE");

                                section.Item()
                                    .Text(resume.Summary)
                                    .LineHeight(1.4f);
                            });
                        }

                        // WORK EXPERIENCE
                        if (experiences.Any())
                        {
                            column.Item().Column(section =>
                            {
                                AddSectionTitle(section, "WORK EXPERIENCE");

                                foreach (var experience in experiences)
                                {
                                    section.Item()
                                        .PaddingBottom(8)
                                        .Column(item =>
                                        {
                                            item.Spacing(3);

                                            item.Item()
                                                .Text(experience.JobTitle)
                                                .Bold()
                                                .FontSize(12);

                                            item.Item()
                                                .Text(experience.Company)
                                                .FontColor(Colors.Blue.Darken2);

                                            item.Item()
                                                .Text(
                                                    $"{experience.StartDate} - {experience.EndDate ?? "Present"}")
                                                .FontSize(9)
                                                .FontColor(Colors.Grey.Darken1);

                                            foreach (var description in experience.Description ?? [])
                                            {
                                                if (!string.IsNullOrWhiteSpace(description))
                                                {
                                                    item.Item()
                                                        .PaddingLeft(10)
                                                        .Text($"• {description}")
                                                        .LineHeight(1.3f);
                                                }
                                            }
                                        });
                                }
                            });
                        }

                        // EDUCATION
                        if (educations.Any())
                        {
                            column.Item().Column(section =>
                            {
                                AddSectionTitle(section, "EDUCATION");

                                foreach (var education in educations)
                                {
                                    section.Item()
                                        .PaddingBottom(8)
                                        .Column(item =>
                                        {
                                            item.Spacing(3);

                                            item.Item()
                                                .Text(education.InstitutionName)
                                                .Bold()
                                                .FontSize(11);

                                            item.Item()
                                                .Text(
                                                    $"{education.Degree} - {education.FieldOfStudy}");

                                            item.Item()
                                                .Text(
                                                    $"{education.StartYear} - {education.EndYear}")
                                                .FontSize(9)
                                                .FontColor(Colors.Grey.Darken1);
                                        });
                                }
                            });
                        }

                        // SKILLS
                        if (skills.Any())
                        {
                            column.Item().Column(section =>
                            {
                                AddSectionTitle(section, "SKILLS");

                                foreach (var skill in skills)
                                {
                                    section.Item()
                                        .PaddingBottom(6)
                                        .Column(item =>
                                        {
                                            item.Item()
                                                .Text(skill.Category)
                                                .Bold();

                                            item.Item()
                                                .Text(string.Join(
                                                    " • ",
                                                    skill.Skills ?? []));
                                        });
                                }
                            });
                        }

                        // GDPR / RODO CLAUSE
                        if (!string.IsNullOrWhiteSpace(resume.Rodo))
                        {
                            column.Item()
                                .PaddingTop(15)
                                .Text(resume.Rodo)
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);
                        }
                    });

                // FOOTER
                page.Footer()
                    .PaddingTop(10)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("ResumeMaker")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static void AddSectionTitle(
        ColumnDescriptor column,
        string title)
    {
        column.Item()
            .PaddingBottom(5)
            .Text(title)
            .FontSize(13)
            .Bold()
            .FontColor(Colors.Blue.Darken3);

        column.Item()
            .PaddingBottom(5)
            .LineHorizontal(1)
            .LineColor(Colors.Grey.Lighten1);
    }
}
