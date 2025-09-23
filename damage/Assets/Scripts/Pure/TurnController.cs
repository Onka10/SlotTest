using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TurnController
{
    private CharacterInstance player1;
    private CharacterInstance player2;
    private CharacterInstance enemy;

    // ReactiveCollection でキュー管理
    public ReactiveCollection<string> CurrentQueue { get; private set; } = new ReactiveCollection<string>();

    // ターンごとの購読管理
    private CompositeDisposable turnDisposable;

    public TurnController(CharacterInstance p1, CharacterInstance p2, CharacterInstance en)
    {
        player1 = p1;
        player2 = p2;
        enemy = en;
    }

    public void StartTurn(SkillSlot player1Slot, SkillSlot player2Slot, BattleUIManager uiManager)
    {
        // 古い購読を破棄
        turnDisposable?.Dispose();
        turnDisposable = new CompositeDisposable();

        CurrentQueue.Clear();

        // スロットリセット
        if (player1.IsAlive) player1Slot.ResetSelection();
        if (player2.IsAlive) player2Slot.ResetSelection();

        // 未確定キュー作成
        if (player1.IsAlive) CurrentQueue.Add("?");
        if (enemy.IsAlive) CurrentQueue.Add(enemy.Name);
        if (player2.IsAlive) CurrentQueue.Add("?");
        if (enemy.IsAlive) CurrentQueue.Add(enemy.Name);

        // UI にキューを反映
        CurrentQueue.ObserveCountChanged()
            .Subscribe(_ => uiManager.ShowQueue(new List<string>(CurrentQueue)))
            .AddTo(turnDisposable);
        CurrentQueue.ObserveReplace()
            .Subscribe(_ => uiManager.ShowQueue(new List<string>(CurrentQueue)))
            .AddTo(turnDisposable);

        // プレイヤー1のスロット購読
        if (player1.IsAlive)
        {
            player1Slot.OnStopSpin
                .Subscribe(skills => {
                    ReplaceFirstQuestionMark(player1, skills);
                })
                .AddTo(turnDisposable);
            player1Slot.StartSpin();
        }

        // プレイヤー2のスロット購読
        if (player2.IsAlive)
        {
            player2Slot.OnStopSpin
                .Subscribe(skills => {
                    ReplaceFirstQuestionMark(player2, skills);
                })
                .AddTo(turnDisposable);
            player2Slot.StartSpin();
        }
    }

    private void ReplaceFirstQuestionMark(CharacterInstance player, List<SkillData> skills)
    {
        int index = CurrentQueue.IndexOf("?");
        if (index >= 0 && skills.Count > 0)
        {
            // 候補リストの先頭スキル名を表示
            CurrentQueue[index] = $"{player.Name} → {skills[0].skillName}";
        }

        // デバッグ用: 候補スキルの中身をログ出力
        string skillNames = string.Join(", ", skills.ConvertAll(s => s.skillName));
        // Debug.Log($"[TurnController] {player.Name} の候補スキル: {skillNames}");
    }

    // ターン終了時に購読を破棄
    public void EndTurn()
    {
        turnDisposable?.Dispose();
        turnDisposable = null;
    }
}
