using System.Collections;
using UnityEngine;

//行動の順次実行と遅延制御
public class ActionExecutor
{
    private BattleActionQueue actionQueue;
    private BattleUIManager uiManager;
    private CharacterInstance player1;
    private CharacterInstance player2;
    private CharacterInstance enemy;

    public float ActionDelay { get; set; } = 1f;

    public ActionExecutor(BattleActionQueue queue, BattleUIManager ui, CharacterInstance p1, CharacterInstance p2, CharacterInstance en)
    {
        actionQueue = queue;
        uiManager = ui;
        player1 = p1;
        player2 = p2;
        enemy = en;
    }

    public IEnumerator ExecuteQueueWithDelay()
    {
        while (actionQueue.HasActions())
        {
            actionQueue.ExecuteAllActions();
            uiManager.UpdateHP(player1, player2, enemy);
            yield return new WaitForSeconds(ActionDelay);
        }

        actionQueue.RaiseQueueFinished();
    }
}
