using UnityEngine;

// キャラクターの基本データ保持（名前、最大HP、スキルリストなど）


[CreateAssetMenu(fileName = "CharacterData", menuName = "RPG/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int maxHP;

    [Header("習得している技")]
    public SkillData[] skills;
}
