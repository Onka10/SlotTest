using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [Header("リール設定")]
    public SkillSlotView view;
    public float startSpeed = 1500f;
    public float stopRange = 60f;

    private bool spinning = false;
    private float currentSpeed = 0f;

    private SkillData selectedSkill;
    public List<SkillData> candidateSkills = new List<SkillData>();

    public Subject<SkillData> OnStopSpin = new Subject<SkillData>();

    private void Awake()
    {
        // Viewのボタンイベントを購読してStopSpinを呼ぶ
        if (view != null)
            view.OnStopButtonPressed += StopSpin;
    }

    public void Initialize(SkillData[] skillList)
    {
        selectedSkill = null;
        candidateSkills.Clear();
        view.InitializeCells(skillList);
    }

    public void ResetSelection() => selectedSkill = null;

    public void StartSpin()
    {
        if (view.GetCells().Count == 0) return;
        spinning = true;
        currentSpeed = startSpeed;
        selectedSkill = null;

        // 停止範囲表示を更新
        view.UpdateStopRangeIndicator(stopRange);
    }

    public void StopSpin()
    {
        if (!spinning) return; // 二重呼び出し防止
        spinning = false;
        candidateSkills.Clear();

        foreach (var cell in view.GetCells())
        {
            var rt = cell.GetComponent<RectTransform>();
            float top = rt.anchoredPosition.y;
            float bottom = rt.anchoredPosition.y - rt.sizeDelta.y;

            // 中央 ± stopRange にかかっていれば候補に追加
            if ((top >= -stopRange && top <= stopRange) ||
                (bottom >= -stopRange && bottom <= stopRange))
            {
                candidateSkills.Add(cell.GetSkill());
            }
        }

        if (candidateSkills.Count == 0)
            throw new Exception("StopSpin: 範囲内にセルが存在しません");

        // 候補からランダムで1つを選択
        int randomIndex = UnityEngine.Random.Range(0, candidateSkills.Count);
        selectedSkill = candidateSkills[randomIndex];

        // OnStopSpin を発火して BattleManager 側に通知
        OnStopSpin.OnNext(selectedSkill);

        Debug.Log($"[SkillSlot] StopSpin 候補数: {candidateSkills.Count}, 選択スキル: {selectedSkill.skillName}");
    }

    public SkillData GetSelectedSkill() => selectedSkill;
    public void SetSelectedSkill(SkillData skill) => selectedSkill = skill;

    private void Update()
    {
        view.Spin(currentSpeed, spinning);
    }

    private void OnDestroy()
    {
        if (view != null)
            view.OnStopButtonPressed -= StopSpin;
    }
}
