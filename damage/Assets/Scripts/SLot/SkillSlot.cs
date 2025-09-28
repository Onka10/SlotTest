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

    public List<SkillData> candidateSkills = new List<SkillData>();

    public Subject<List<SkillData>> OnStopSpin = new Subject<List<SkillData>>();

    private void Awake()
    {
        // Viewのボタンイベントを購読してStopSpinを呼ぶ
        if (view != null)
            view.OnStopButtonPressed += StopSpin;
    }

    public void Initialize(SkillData[] skillList)
    {
        candidateSkills.Clear();
        view.InitializeCells(skillList);
    }

    public void StartSpin()
    {
        if (view.GetCells().Count == 0) return;
        spinning = true;
        currentSpeed = startSpeed;

        // 停止範囲表示を更新
        view.UpdateStopRangeIndicator(stopRange);
    }

public void StopSpin()
{
    if (!spinning) return; // 二重呼び出し防止
    spinning = false;
    candidateSkills.Clear();

    // 中央の判定用のTransform（リールの中心）
    Vector3 centerWorldPos = view.transform.position;
    float rangeTop = centerWorldPos.y + stopRange;
    float rangeBottom = centerWorldPos.y - stopRange;

    // Debug.Log($"[SkillSlot] stopRange座標範囲: {rangeBottom} ～ {rangeTop}");

    foreach (var cell in view.GetCells())
    {
        RectTransform rt = cell.GetComponent<RectTransform>();
        float cellHeight = rt.rect.height;

        Vector3 cellWorldPos = cell.transform.position;
        float top = cellWorldPos.y + cellHeight / 2f;
        float bottom = cellWorldPos.y - cellHeight / 2f;

        if ((top >= rangeBottom && top <= rangeTop) ||
            (bottom >= rangeBottom && bottom <= rangeTop))
        {
            candidateSkills.Add(cell.GetSkill());
            Debug.Log($"[SkillSlot] stopRange内のスキル: {cell.GetSkill().skillName} (top={top}, bottom={bottom}, siblingIndex={cell.transform.GetSiblingIndex()})");
        }

        // 全セルログ
        // Debug.Log($"[SkillSlot] セル: {cell.GetSkill().skillName}, top={top}, bottom={bottom}, siblingIndex={cell.transform.GetSiblingIndex()}");
    }


    if (candidateSkills.Count == 0)
        throw new Exception("StopSpin: 範囲内にセルが存在しません");

    // ランダム選択はやめて範囲内のリストをそのまま通知
    OnStopSpin.OnNext(new List<SkillData>(candidateSkills));

    // Debug.Log($"[SkillSlot] StopSpin 候補数: {candidateSkills.Count}, 選択スキル: {selectedSkill.skillName}");
}

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
