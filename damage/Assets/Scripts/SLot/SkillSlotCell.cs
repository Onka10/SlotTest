using UnityEngine;
using UnityEngine.UI;

public class SkillSlotCell : MonoBehaviour
{
    [Header("UI参照")]
    public Text skillNameText;    
    public Image backgroundImage; 

    private SkillData currentSkill;

    /// <summary>
    /// スキル名と色を設定
    /// </summary>
    public void SetSkill(SkillData skill)
    {
        currentSkill = skill;

        if (skillNameText != null)
            skillNameText.text = skill.skillName;

        if (backgroundImage != null)
            backgroundImage.color = skill.skillColor;
    }

    /// <summary>
    /// 現在セットされているスキルを取得
    /// </summary>
    public SkillData GetSkill()
    {
        return currentSkill;
    }
}
