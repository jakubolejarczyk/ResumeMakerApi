using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class SkillDatabaseData(SkillDatabaseConst skillDatabaseConst)
{
    private readonly List<BaseDatabaseModel<SkillDataDatabaseModel>> Skills = [];

    public void Init()
    {
        Skills.Clear();
        Skills.Add(skillDatabaseConst.SKILL_0);
        Skills.Add(skillDatabaseConst.SKILL_1);
        Skills.Add(skillDatabaseConst.SKILL_2);
        Skills.Add(skillDatabaseConst.SKILL_3);
    }

    public void Set(List<BaseDatabaseModel<SkillDataDatabaseModel>> skills)
    {
        Skills.Clear();
        Skills.AddRange(skills);
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> Get()
    {
        return Skills;
    }
}
