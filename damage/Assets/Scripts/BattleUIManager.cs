using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class BattleUIManager : MonoBehaviour
{
    [Header("HP表示")]
    public Text player1HPText;
    public Text player2HPText;
    public Text enemyHPText;

    [Header("行動キュー")]
    public Text actionQueueText;

    [Header("Startボタン")]
    public Button startButton;

    [Header("Confirmオーバーレイ管理")]
    public OverlayManager overlayManager; // Confirmオーバーレイを担当

    public ReactiveProperty<int> Player1HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> Player2HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> EnemyHP = new ReactiveProperty<int>();
    public ReactiveCollection<string> ActionQueue = new ReactiveCollection<string>();

    private void Awake()
    {
        // HP更新購読
        Player1HP.Subscribe(value => player1HPText.text = $"Player1 HP: {value}").AddTo(this);
        Player2HP.Subscribe(value => player2HPText.text = $"Player2 HP: {value}").AddTo(this);
        EnemyHP.Subscribe(value => enemyHPText.text = $"Enemy HP: {value}").AddTo(this);

        // 行動キュー表示更新
        ActionQueue.ObserveCountChanged().Subscribe(_ => RefreshQueueText()).AddTo(this);
        ActionQueue.ObserveReplace().Subscribe(_ => RefreshQueueText()).AddTo(this);

        if (startButton != null)
            startButton.interactable = true;
    }

    private void RefreshQueueText()
    {
        actionQueueText.text = "行動キュー:\n";
        foreach (var s in ActionQueue)
        {
            if (!string.IsNullOrEmpty(s))
                actionQueueText.text += s + "\n";
        }
    }

    public void UpdateHP(CharacterInstance player1, CharacterInstance player2, CharacterInstance enemy)
    {
        if (player1 != null) Player1HP.Value = player1.currentHP;
        if (player2 != null) Player2HP.Value = player2.currentHP;
        if (enemy != null) EnemyHP.Value = enemy.currentHP;
    }

    public void ShowQueue(List<string> queue)
    {
        ActionQueue.Clear();
        foreach (var s in queue)
            ActionQueue.Add(s);
    }

    public void ShowMessage(string message)
    {
        actionQueueText.text = message;
        Debug.Log("[UI] " + message);
    }

    /// <summary>
    /// Confirmオーバーレイ表示
    /// </summary>
    // public void ShowSkillConfirmOverlay(List<SkillData> candidateSkills, System.Action<SkillData> onConfirm)
    // {
    //     if (overlayManager != null)
    //     {
    //         overlayManager.ShowSkillConfirmOverlay(candidateSkills, onConfirm);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("[BattleUIManager] OverlayManagerがセットされていません");
    //     }
    // }

    public void ShowSkillConfirmOverlay(SkillSlotResult result, int player)
    {
        //  List<SkillData> candidateSkills
        
        overlayManager.ShowSkillConfirmOverlay(result.CandidateSkills, player);
    }

    public void HideSkillConfirmOverlay()
    {
        if (overlayManager != null)
        {
            overlayManager.HideOverlay();
        }
    }
}
