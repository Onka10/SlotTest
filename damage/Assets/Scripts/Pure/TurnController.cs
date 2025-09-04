using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TurnController
{
    private CharacterInstance player1;
    private CharacterInstance player2;
    private CharacterInstance enemy;

    // ReactiveCollection 
    public ReactiveCollection<string> CurrentQueue { get; private set; } = new ReactiveCollection<string>();

    public TurnController(CharacterInstance p1, CharacterInstance p2, CharacterInstance en)
    {
        player1 = p1;
        player2 = p2;
        enemy = en;
    }

    public void StartTurn(SkillSlot player1Slot, SkillSlot player2Slot, BattleUIManager uiManager)
    {
        CurrentQueue.Clear();

        if (player1.IsAlive) player1Slot.ResetSelection();
        if (player2.IsAlive) player2Slot.ResetSelection();

        // 未確定キュー作成
        if (player1.IsAlive) CurrentQueue.Add("?");
        if (enemy.IsAlive) CurrentQueue.Add(enemy.Name);
        if (player2.IsAlive) CurrentQueue.Add("?");
        if (enemy.IsAlive) CurrentQueue.Add(enemy.Name);

        // ReactiveCollection の変更を UI にバインド
        CurrentQueue.ObserveCountChanged().Subscribe(_ => uiManager.ShowQueue(new List<string>(CurrentQueue))).AddTo(uiManager);
        CurrentQueue.ObserveReplace().Subscribe(_ => uiManager.ShowQueue(new List<string>(CurrentQueue))).AddTo(uiManager);

        // プレイヤーのスロットイベント設定
        if (player1.IsAlive)
        {
            player1Slot.OnStopSpin.Subscribe(skill =>
            {
                ReplaceFirstQuestionMark(player1, skill);
            }).AddTo(uiManager);

            player1Slot.StartSpin();
        }

        if (player2.IsAlive)
        {
            player2Slot.OnStopSpin.Subscribe(skill =>
            {
                ReplaceFirstQuestionMark(player2, skill);
            }).AddTo(uiManager);

            player2Slot.StartSpin();
        }
    }

    private void ReplaceFirstQuestionMark(CharacterInstance player, SkillData skill)
    {
        int index = CurrentQueue.IndexOf("?");
        if (index >= 0)
        {
            CurrentQueue[index] = $"{player.Name} → {skill.skillName}";
        }
    }
}
