using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class ResumeDatabaseConst
{
    public BaseDatabaseModel<ResumeDataDatabaseModel> RESUME_0 = new()
    {
        Data = new ResumeDataDatabaseModel
        {
            Name = "Jakub Olejarczyk - Resume English",
            JobTitle = "Mid Software Engineer",
            Summary = "Mid Software Engineer with 6+ years of experience in full-stack web development, specializing in Angular on the frontend and ASP.NET on the backend. Experienced in designing and developing scalable web applications, RESTful APIs, and backend services, with a strong focus on clean architecture, performance, maintainability, and code quality. Proven ability to contribute across the full software development lifecycle and collaborate effectively with cross-functional teams to deliver scalable and maintainable solutions."
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 0,
            ParentId = 0,
            Order = 0,
            IsSelected = false
        }
    };
}
