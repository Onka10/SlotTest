using UnityEngine;
using UnityEngine.UI;

public class SkillSlotCell : MonoBehaviour
{
    [Header("UI参照")]
    public Text skillNameText;    // スキル名表示用
    public Image backgroundImage; // 色表示用

    /// <summary>
    /// スキル名と色を設定
    /// </summary>
    public void SetSkill(SkillData skill)
    {
        if (skillNameText != null)
            skillNameText.text = skill.skillName;

        if (backgroundImage != null)
            backgroundImage.color = skill.skillColor;
    }
}
