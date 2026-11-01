using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class SkillDatabaseData
{
    private readonly List<BaseDatabaseModel<SkillDatabaseModel>> Skills = [];

    public SkillDatabaseData(SkillDatabaseConst skillDatabaseConst)
    {
        Skills.Clear();
        Skills.Add(skillDatabaseConst.SKILL_0);
        Skills.Add(skillDatabaseConst.SKILL_1);
        Skills.Add(skillDatabaseConst.SKILL_2);
        Skills.Add(skillDatabaseConst.SKILL_3);
    }

    public void SetSkills(List<BaseDatabaseModel<SkillDatabaseModel>> skills)
    {
        Skills.Clear();
        Skills.AddRange(skills);
    }

    public List<BaseDatabaseModel<SkillDatabaseModel>> GetSkills()
    {
        return Skills;
    }
}
