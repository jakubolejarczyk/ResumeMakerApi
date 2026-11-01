using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class ExperienceDatabaseConst
{
    public BaseDatabaseModel<ExperienceDataDatabaseModel> EXPERIENCE_0 = new()
    {
        Data = new ExperienceDataDatabaseModel
        {
            Company = "APR System",
            JobTitle = "Software Engineer",
            Description = [
              "Developed and maintained full-stack applications using Angular and ASP.NET, working across frontend components, business logic, backend services, REST APIs, and data integration.",
              "Designed and implemented a model-driven code generation platform that automatically generated frontend and backend structures from UML/domain models, reducing development time and allowing developers to focus on business logic.",
              "Developed internal npm tooling to standardize and streamline development workflows, improving team productivity and ensuring a consistent, well-structured process across the software delivery lifecycle.",
              "Created and maintained custom ESLint rules to automate adherence to company coding standards, reducing manual formatting effort and improving code consistency.",
              "Developed and maintained unit tests to verify application behavior, ensure code correctness, and improve overall software reliability.",
            ],
            StartDate = "01/07/2020",
            EndDate = "01/02/2025",
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 0,
            ParentId = 0,
            Order = 0,
            IsSelected = false
        }
    };

    public BaseDatabaseModel<ExperienceDataDatabaseModel> EXPERIENCE_1 = new()
    {
        Data = new ExperienceDataDatabaseModel
        {
            Company = "Primaris",
            JobTitle = "Software Engineer",
            Description = [
                "Designed, developed, deployed, and maintained UiPath RPA solutions using .NET/C# to automate repetitive, time-consuming manual processes, reducing manual effort for non-technical employees and streamlining business workflows.",
                "Developed an internal C#/.NET library with custom UiPath activities that enabled seamless integration with KSeF 2.0, providing reusable components for implementing KSeF functionality across RPA workflows.",
                "Developed and maintained a UiPath automation for KSeF 2.0 that automated key KSeF operations, reducing time-consuming manual work and streamlining the company's invoicing processes.",
                "Integrated UiPath RPA solutions with external services through REST APIs and implemented data operations using Microsoft SQL Server, enabling automated communication and data processing across business systems.",
                "Collaborated directly with clients to analyze business processes, gather and clarify requirements, and identify automation opportunities, using the collected information to design effective RPA solutions.",
            ],
            StartDate = "01/06/2025",
            EndDate = null,
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 1,
            ParentId = 0,
            Order = 1,
            IsSelected = false
        }
    };
}
