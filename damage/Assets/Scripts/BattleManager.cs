using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BattleManager : MonoBehaviour
{
    public CharacterData player1Data;
    public CharacterData player2Data;
    public CharacterData enemyData;

    public SkillSlot player1Slot;
    public SkillSlot player2Slot;
    public BattleUIManager uiManager;

    private CharacterInstance player1;
    private CharacterInstance player2;
    private CharacterInstance enemy;

    private BattleActionQueue actionQueue;
    private TurnController turnController;
    private ActionExecutor actionExecutor;

    public float actionDelay = 1f;

    private void Start()
    {
        player1 = new CharacterInstance(player1Data);
        player2 = new CharacterInstance(player2Data);
        enemy = new CharacterInstance(enemyData);

        uiManager.UpdateHP(player1, player2, enemy);

        player1Slot.Initialize(player1.data.skills);
        player2Slot.Initialize(player2.data.skills);

        actionQueue = new BattleActionQueue();
        turnController = new TurnController(player1, player2, enemy);
        actionExecutor = new ActionExecutor(actionQueue, uiManager, player1, player2, enemy)
        {
            ActionDelay = actionDelay
        };

        // UniRxでスロット確定監視
        Observable.CombineLatest(
            player1Slot.OnStopSpin,
            player2Slot.OnStopSpin,
            (skill1, skill2) => new { Skill1 = skill1, Skill2 = skill2 }
        ).Subscribe(result =>
        {
            Debug.Log("[BattleManager] 両方のスロットが確定しました。Confirmボタンを表示するタイミングです。");
            OnBothSlotsConfirmed(result.Skill1, result.Skill2);
        }).AddTo(this);

        // BattleActionQueue の Subject を購読
        actionQueue.OnActionExecuted.Subscribe(action =>
        {
            Debug.Log($"{action.user.Name} は {action.skill.skillName} を使った！ {action.target.Name} に {action.skill.power} ダメージ！");
            uiManager.UpdateHP(player1, player2, enemy);
        }).AddTo(this);

        actionQueue.OnQueueFinished.Subscribe(_ =>
        {
            if (!enemy.IsAlive)
                uiManager.ShowMessage("勝利！敵を倒した！");
            else if (!player1.IsAlive && !player2.IsAlive)
                uiManager.ShowMessage("敗北…全滅した！");
            else
                StartTurn();
        }).AddTo(this);

        StartTurn();
    }

    private void StartTurn()
    {
        turnController.StartTurn(player1Slot, player2Slot, uiManager);
    }

    public void OnStartQueueButtonPressed()
    {
        foreach (var entry in turnController.CurrentQueue)
        {
            if (entry.StartsWith("?")) continue;

            if (entry.Contains(player1.Name))
                actionQueue.EnqueueAction(player1, player1Slot.GetSelectedSkill(), enemy);
            else if (entry.Contains(player2.Name))
                actionQueue.EnqueueAction(player2, player2Slot.GetSelectedSkill(), enemy);
            else if (entry == enemy.Name)
            {
                var alivePlayers = new List<CharacterInstance>();
                if (player1.IsAlive) alivePlayers.Add(player1);
                if (player2.IsAlive) alivePlayers.Add(player2);

                if (alivePlayers.Count > 0)
                {
                    CharacterInstance target = alivePlayers[Random.Range(0, alivePlayers.Count)];
                    actionQueue.EnqueueAction(enemy, enemy.data.skills[0], target);
                }
            }
        }

        StartCoroutine(actionExecutor.ExecuteQueueWithDelay());
    }

    private void OnBothSlotsConfirmed(SkillData skill1, SkillData skill2)
    {
        // Confirmボタン表示やUI処理に置き換え可能
        Debug.Log($"[BattleManager] Player1: {skill1.skillName}, Player2: {skill2.skillName}");
    }
}
