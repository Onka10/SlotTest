using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;

public class SkillSlot : MonoBehaviour
{
    [Header("リール UI")]
    public RectTransform reelContent;
    public SkillSlotCell symbolPrefab;

    [Header("回転設定")]
    public float startSpeed = 1500f;

    [Header("停止範囲設定")]
    public float stopRange = 60f; // 中央 ± stopRange 内のセルが対象

    private SkillData selectedSkill;
    private bool spinning = false;
    private float currentSpeed = 0f;

    private List<SkillSlotCell> rowCells = new List<SkillSlotCell>();
    private float[] rowHeights;
    private float totalHeight = 0f;

    // StopSpin 時の候補
    public List<SkillData> candidateSkills = new List<SkillData>();

    // UniRx Subject
    public Subject<SkillData> OnStopSpin = new Subject<SkillData>();

    #region 公開メソッド
    public void Initialize(SkillData[] skillList)
    {
        selectedSkill = null;

        for (int i = reelContent.childCount - 1; i >= 0; i--)
            Destroy(reelContent.GetChild(i).gameObject);
        rowCells.Clear();

        rowHeights = new float[skillList.Length];
        totalHeight = 0f;
        for (int i = 0; i < skillList.Length; i++)
        {
            rowHeights[i] = skillList[i].reelHeight;
            totalHeight += rowHeights[i];
        }

        float posY = 0f;
        for (int i = 0; i < skillList.Length * 5; i++)
        {
            int idx = i % skillList.Length;
            var cell = Instantiate(symbolPrefab, reelContent);
            cell.SetSkill(skillList[idx]);

            var rt = cell.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, -posY);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, skillList[idx].reelHeight);

            rowCells.Add(cell);
            posY += skillList[idx].reelHeight;
        }
    }

    public void ResetSelection() => selectedSkill = null;

    public void StartSpin()
    {
        if (rowCells.Count == 0) return;
        spinning = true;
        currentSpeed = startSpeed;
        selectedSkill = null;
    }

public void StopSpin()
{
    spinning = false;
    candidateSkills.Clear();

    float center = 0f; // リール中央位置

    foreach (var cell in rowCells)
    {
        RectTransform rt = cell.GetComponent<RectTransform>();
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
    #endregion

    private void Update()
    {
        if (!spinning) return;

        foreach (var cell in rowCells)
        {
            var rt = cell.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0f, -currentSpeed * Time.deltaTime);

            if (rt.anchoredPosition.y < -totalHeight)
                rt.anchoredPosition += new Vector2(0f, totalHeight);
        }
    }
}
