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
    private bool turnConfirmed = false;
    private CompositeDisposable turnDisposable;

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

        actionQueue.OnActionExecuted.Subscribe(action =>
        {
            uiManager.UpdateHP(player1, player2, enemy);
            Debug.Log($"{action.user.Name} は {action.skill.skillName} を使った！ {action.target.Name} に {action.skill.power} ダメージ！");
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
        Debug.Log("[BattleManager] 新しいターン開始");
        turnConfirmed = false;
        turnDisposable?.Dispose();
        turnDisposable = new CompositeDisposable();

        turnController.StartTurn(player1Slot, player2Slot, uiManager);

        Observable.CombineLatest(
            player1Slot.OnStopSpin,
            player2Slot.OnStopSpin,
            (s1, s2) => new { Slot1 = s1, Slot2 = s2 }
        ).Subscribe(_ =>
        {
            if (turnConfirmed) return;
            turnConfirmed = true;

            // Player1 候補リストを渡して Confirm 表示
            uiManager.ShowSkillConfirmOverlay(player1Slot.candidateSkills, selectedSkill =>
            {
                player1Slot.SetSelectedSkill(selectedSkill);
                player1Slot.OnStopSpin.OnNext(selectedSkill);
            });

            // Player2 候補リストを渡して Confirm 表示
            uiManager.ShowSkillConfirmOverlay(player2Slot.candidateSkills, selectedSkill =>
            {
                player2Slot.SetSelectedSkill(selectedSkill);
                player2Slot.OnStopSpin.OnNext(selectedSkill);
            });
        }).AddTo(turnDisposable);
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
}
