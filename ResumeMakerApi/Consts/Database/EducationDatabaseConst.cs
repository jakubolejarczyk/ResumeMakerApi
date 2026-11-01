using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class EducationDatabaseConst
{
    public BaseDatabaseModel<EducationDatabaseModel> EDUCATION_0 = new()
    {
        Data = new EducationDatabaseModel
        {
            InstitutionName = "Jan Kochanowski University of Kielce",
            FieldOfStudy = "Computer Science",
            Degree = "Bachelor of Engineering",
            StartYear = 2017,
            EndYear = 2021,
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 0,
            ParentId = 0,
            Order = 0,
            IsSelected = false
        }
    };

    public BaseDatabaseModel<EducationDatabaseModel> EDUCATION_1 = new()
    {
        Data = new EducationDatabaseModel
        {
            InstitutionName = "Kielce University of Technology",
            FieldOfStudy = "Computer Science",
            Degree = "Master of Science",
            StartYear = 2024,
            EndYear = 2026,
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
