using UnityEngine;
using UnityEngine.UI;

public class SkillSlotCell : MonoBehaviour
{
    [Header("UI参照")]
    public Text skillNameText;    
    public Image backgroundImage; 

    private SkillData currentSkill;
    private int playerNumber;

    // このGameObjectにButtonコンポーネントがあるかチェック
    public Button button;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning($"{name} に Button コンポーネントがアタッチされていません");
        }
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    private void OnButtonClicked()
    {
        if (currentSkill != null) {
            Debug.Log($"ボタンが押されました: {currentSkill.skillName}");
            BattleManager.I.SetSelectedSkill(currentSkill,playerNumber);
        }
        else
            Debug.Log("ボタンが押されましたが、スキルは未設定です");
    }

    /// <summary>
    /// スキル名と色を設定 オーバーレイ用
    /// </summary>
    public void SetSkill(SkillData skill, int player)
    {
        currentSkill = skill;
        playerNumber = player;

        if (skillNameText != null) { }
            skillNameText.text = skill.skillName;

        if (backgroundImage != null)
            backgroundImage.color = skill.skillColor;
    }

    /// <summary>
    /// スキル名と色を設定 スロット用
    /// </summary>
    public void SetSkill(SkillData skill)
    {
        currentSkill = skill;

        if (skillNameText != null) { }
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
