using UnityEngine;

//スキルの基本データ保持（名前、威力、リール高さなど）
[CreateAssetMenu(fileName = "SkillData", menuName = "RPG/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int power;

    [Tooltip("このスキルがリール上で表示される高さ（px）")]
    public float reelHeight = 60f;

    [Tooltip("リール上で表示される際の色")]
    public Color skillColor = Color.white; // 追加
}
