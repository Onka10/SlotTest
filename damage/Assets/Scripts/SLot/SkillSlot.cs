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

    private SkillData[] skills;
    private SkillData selectedSkill;
    private bool spinning = false;
    private float currentSpeed = 0f;

    private List<SkillSlotCell> rowCells = new List<SkillSlotCell>();
    private float[] rowHeights;
    private float totalHeight = 0f;

    // UniRx Subject に変更
    public Subject<SkillData> OnStopSpin = new Subject<SkillData>();

    #region 公開メソッド
    public void Initialize(SkillData[] skillList)
    {
        skills = skillList;
        selectedSkill = null;

        for (int i = reelContent.childCount - 1; i >= 0; i--)
            Destroy(reelContent.GetChild(i).gameObject);
        rowCells.Clear();

        rowHeights = new float[skills.Length];
        totalHeight = 0f;
        for (int i = 0; i < skills.Length; i++)
        {
            rowHeights[i] = skills[i].reelHeight;
            totalHeight += rowHeights[i];
        }

        float posY = 0f;
        for (int i = 0; i < skills.Length * 5; i++)
        {
            int idx = i % skills.Length;
            var cell = Instantiate(symbolPrefab, reelContent);
            cell.SetSkill(skills[idx]);

            var rt = cell.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, -posY);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, skills[idx].reelHeight);

            rowCells.Add(cell);
            posY += skills[idx].reelHeight;
        }
    }

    public void ResetSelection() => selectedSkill = null;

    public void StartSpin()
    {
        if (skills == null || skills.Length == 0) return;
        spinning = true;
        currentSpeed = startSpeed;
        selectedSkill = null;
    }

    public void StopSpin()
    {
        spinning = false;

        float offset = UnityEngine.Random.Range(0f, totalHeight);
        float accum = 0f;
        int idx = 0;
        for (int i = 0; i < skills.Length; i++)
        {
            accum += skills[i].reelHeight;
            if (accum >= offset)
            {
                idx = i;
                break;
            }
        }

        selectedSkill = skills[idx];
        Debug.Log($"[SkillSlot] スロット停止: {selectedSkill.skillName}");

        // Subject に通知
        OnStopSpin.OnNext(selectedSkill);
    }

    public SkillData GetSelectedSkill() => selectedSkill;
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
