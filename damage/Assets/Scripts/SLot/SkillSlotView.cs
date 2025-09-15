using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : MonoBehaviour
{
    [Header("リール UI")]
    public RectTransform reelContent;
    public SkillSlotCell symbolPrefab;

    [Header("操作ボタン")]
    public Button stopButton;

    private List<SkillSlotCell> rowCells = new List<SkillSlotCell>();
    private float totalHeight = 0f;

    // ボタン押下を発火するイベント
    public event Action OnStopButtonPressed;

    private void Awake()
    {
        if (stopButton != null)
        {
            stopButton.onClick.AddListener(() =>
            {
                OnStopButtonPressed?.Invoke();
            });
        }
    }

    public void InitializeCells(SkillData[] skillList, int loopCount = 5)
    {
        for (int i = reelContent.childCount - 1; i >= 0; i--)
            Destroy(reelContent.GetChild(i).gameObject);
        rowCells.Clear();

        float[] rowHeights = new float[skillList.Length];
        totalHeight = 0f;
        for (int i = 0; i < skillList.Length; i++)
        {
            rowHeights[i] = skillList[i].reelHeight;
            totalHeight += rowHeights[i];
        }

        float posY = 0f;
        for (int i = 0; i < skillList.Length * loopCount; i++)
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

    public void Spin(float speed, bool isSpinning)
    {
        if (!isSpinning) return;

        foreach (var cell in rowCells)
        {
            var rt = cell.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0f, -speed * Time.deltaTime);

            if (rt.anchoredPosition.y < -totalHeight)
                rt.anchoredPosition += new Vector2(0f, totalHeight);
        }
    }

    public List<SkillSlotCell> GetCells() => rowCells;
}
