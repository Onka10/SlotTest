using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BattleActionQueue
{
    private Queue<(CharacterInstance user, SkillData skill, CharacterInstance target)> actionQueue
        = new Queue<(CharacterInstance, SkillData, CharacterInstance)>();

    // UniRx Subject に変更
    public Subject<(CharacterInstance user, SkillData skill, CharacterInstance target)> OnActionExecuted = new Subject<(CharacterInstance, SkillData, CharacterInstance)>();
    public Subject<Unit> OnQueueFinished = new Subject<Unit>();

    public void EnqueueAction(CharacterInstance user, SkillData skill, CharacterInstance target)
    {
        actionQueue.Enqueue((user, skill, target));
    }

    public bool HasActions() => actionQueue.Count > 0;

    public void ExecuteAllActions()
    {
        while (actionQueue.Count > 0)
        {
            var action = actionQueue.Dequeue();

            // ダメージ適用
            action.target.currentHP -= action.skill.power;
            if (action.target.currentHP < 0) action.target.currentHP = 0;

            // Subject に通知
            OnActionExecuted.OnNext(action);
        }
    }

    public void RaiseQueueFinished()
    {
        OnQueueFinished.OnNext(Unit.Default);
    }
}
