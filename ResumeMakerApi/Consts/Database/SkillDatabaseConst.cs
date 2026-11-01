using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class SkillDatabaseConst
{
    public BaseDatabaseModel<SkillDatabaseModel> SKILL_0 = new()
    {
        Data = new SkillDatabaseModel
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

    public BaseDatabaseModel<SkillDatabaseModel> SKILL_1 = new()
    {
        Data = new SkillDatabaseModel
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

    public BaseDatabaseModel<SkillDatabaseModel> SKILL_2 = new()
    {
        Data = new SkillDatabaseModel
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

    public BaseDatabaseModel<SkillDatabaseModel> SKILL_3 = new()
    {
        Data = new SkillDatabaseModel
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
