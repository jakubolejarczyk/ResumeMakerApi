using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class SkillDatabaseConst
{
    public BaseDatabaseModel<SkillDataDatabaseModel> SKILL_0 = new()
    {
        Data = new SkillDataDatabaseModel
        {
            Category = "Programming Language",
            Skills = ["C#", "JavaScript", "TypeScript"]
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 0,
            ParentId = 0,
            Order = 0,
            IsSelected = false
        }
    };

    public BaseDatabaseModel<SkillDataDatabaseModel> SKILL_1 = new()
    {
        Data = new SkillDataDatabaseModel
        {
            Category = "Frontend",
            Skills = ["HTML", "CSS", "SASS", "Angular", "RxJS", "NGXS", "PrimeNG", "Storybook"]
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 1,
            ParentId = 0,
            Order = 1,
            IsSelected = false
        }
    };

    public BaseDatabaseModel<SkillDataDatabaseModel> SKILL_2 = new()
    {
        Data = new SkillDataDatabaseModel
        {
            Category = "Developer Tools",
            Skills = ["Git", "Node.js", "npm", "Vitest", "ESLint", "typescript-eslint", "Visual Studio", "Microsoft Visual Studio Code"]
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 2,
            ParentId = 0,
            Order = 2,
            IsSelected = false
        }
    };

    public BaseDatabaseModel<SkillDataDatabaseModel> SKILL_3 = new()
    {
        Data = new SkillDataDatabaseModel
        {
            Category = "Languages",
            Skills = ["English", "Polish"]
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 3,
            ParentId = 0,
            Order = 3,
            IsSelected = false
        }
    };
}
