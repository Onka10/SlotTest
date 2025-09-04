using UnityEngine;

//CharacterData のインスタンス化、現在HPなどの状態管理
public class CharacterInstance
{
    public CharacterData data;
    public int currentHP;

    public bool IsAlive => currentHP > 0;
    public string Name => data.characterName;
    public int MaxHP => data.maxHP;

    public CharacterInstance(CharacterData data)
    {
        this.data = data;
        this.currentHP = data.maxHP;
    }

    public void Attack(CharacterInstance target)
    {
        // 今は1つ目の技を使う（複数対応も可能）
        SkillData skill = data.skills[0];
        int damage = skill.power;

        target.currentHP -= damage;
        if (target.currentHP < 0) target.currentHP = 0;

        Debug.Log($"{Name} は「{skill.skillName}」を使った！ {target.Name} に {damage} ダメージ！");
    }
}
