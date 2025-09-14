using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("HP表示")]
    public Text player1HPText;
    public Text player2HPText;
    public Text enemyHPText;

    [Header("行動キュー")]
    public Text actionQueueText;

    [Header("Confirmオーバーレイ")]
    public GameObject confirmOverlay;       // オーバーレイルート
    public GameObject skillSlotCellPrefab;  // SkillSlotCell プレハブ
    public Button confirmButton;

    [Header("Startボタン")]
    public Button startButton;

    [Header("Confirm 親オブジェクト (インスペクターで指定)")]
    public Transform confirmParent1;
    public Transform confirmParent2;

    public ReactiveProperty<int> Player1HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> Player2HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> EnemyHP = new ReactiveProperty<int>();
    public ReactiveCollection<string> ActionQueue = new ReactiveCollection<string>();

    private Action confirmCallback;

    private void Awake()
    {
        Player1HP.Subscribe(value => player1HPText.text = $"Player1 HP: {value}").AddTo(this);
        Player2HP.Subscribe(value => player2HPText.text = $"Player2 HP: {value}").AddTo(this);
        EnemyHP.Subscribe(value => enemyHPText.text = $"Enemy HP: {value}").AddTo(this);

        ActionQueue.ObserveCountChanged().Subscribe(_ => RefreshQueueText()).AddTo(this);
        ActionQueue.ObserveReplace().Subscribe(_ => RefreshQueueText()).AddTo(this);

        if (startButton != null)
            startButton.interactable = true;

        if (confirmOverlay != null)
            confirmOverlay.SetActive(false);
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
    /// 確定スキルオーバーレイを表示（複数候補対応）
    /// </summary>
    public void ShowSkillConfirmOverlay(List<SkillData> candidateSkills, Action<SkillData> onConfirm)
    {
        if (confirmOverlay == null || skillSlotCellPrefab == null || candidateSkills.Count == 0) return;

        // 既存の子をクリア
        foreach (Transform child in confirmParent1) Destroy(child.gameObject);
        foreach (Transform child in confirmParent2) Destroy(child.gameObject);

        // 候補を左右に交互に配置
        for (int i = 0; i < candidateSkills.Count; i++)
        {
            Transform parent = (i % 2 == 0) ? confirmParent1 : confirmParent2;
            GameObject cellObj = Instantiate(skillSlotCellPrefab, parent, false);
            SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
            cell.SetSkill(candidateSkills[i]);
        }

        confirmOverlay.SetActive(true);

        // Confirm ボタン押下時にランダム選択
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            confirmOverlay.SetActive(false);
            int index = UnityEngine.Random.Range(0, candidateSkills.Count);
            SkillData selected = candidateSkills[index];
            onConfirm?.Invoke(selected);
        });
    }
}
