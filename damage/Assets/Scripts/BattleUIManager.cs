using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public Text player1HPText;
    public Text player2HPText;
    public Text enemyHPText;
    public Text actionQueueText;

    // ReactiveProperty を用意
    public ReactiveProperty<int> Player1HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> Player2HP = new ReactiveProperty<int>();
    public ReactiveProperty<int> EnemyHP = new ReactiveProperty<int>();

    // ReactiveCollection でキューを管理
    public ReactiveCollection<string> ActionQueue = new ReactiveCollection<string>();

    private void Awake()
    {
        // HP変更時に自動更新
        Player1HP.Subscribe(value => player1HPText.text = $"Player1 HP: {value}").AddTo(this);
        Player2HP.Subscribe(value => player2HPText.text = $"Player2 HP: {value}").AddTo(this);
        EnemyHP.Subscribe(value => enemyHPText.text = $"Enemy HP: {value}").AddTo(this);

        // キュー変更時に自動更新
        ActionQueue.ObserveCountChanged().Subscribe(_ => RefreshQueueText()).AddTo(this);
        ActionQueue.ObserveReplace().Subscribe(_ => RefreshQueueText()).AddTo(this);
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

    // 外部から HP を更新
    public void UpdateHP(CharacterInstance player1, CharacterInstance player2, CharacterInstance enemy)
    {
        if (player1 != null) Player1HP.Value = player1.currentHP;
        if (player2 != null) Player2HP.Value = player2.currentHP;
        if (enemy != null) EnemyHP.Value = enemy.currentHP;
    }

    // 外部からキューを設定
    public void ShowQueue(List<string> queue)
    {
        ActionQueue.Clear();
        foreach (var s in queue)
        {
            ActionQueue.Add(s);
        }
    }

    // メッセージ表示
    public void ShowMessage(string message)
    {
        Debug.Log(message);
        actionQueueText.text = message;
    }
}
