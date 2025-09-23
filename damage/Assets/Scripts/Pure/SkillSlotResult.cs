using System.Collections.Generic;


public class SkillSlotResult
{
    public CharacterInstance Player { get; private set; }
    public List<SkillData> CandidateSkills { get; private set; }

    public SkillSlotResult(CharacterInstance player, List<SkillData> candidateSkills)
    {
        Player = player;
        CandidateSkills = candidateSkills;
    }
}
