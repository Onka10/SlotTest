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

    [Header("停止範囲表示")]
    public RectTransform stopRangeIndicator; // ← EditorでアタッチするImageのRectTransform

    private List<SkillSlotCell> rowCells = new List<SkillSlotCell>();
    private float totalHeight = 0f;

    public event Action OnStopButtonPressed;

    private void Awake()
    {
        if (stopButton != null)
            stopButton.onClick.AddListener(() => OnStopButtonPressed?.Invoke());
    }

    /// <summary>
    /// 停止範囲の表示サイズを更新
    /// </summary>
    public void UpdateStopRangeIndicator(float stopRange)
    {
        if (stopRangeIndicator == null) return;

        // 親スロットのスケールやアンカーを考慮して高さを設定
        RectTransform slotRect = GetComponent<RectTransform>();
        float slotScaleY = transform.lossyScale.y;

        // stopRangeは世界座標系の距離なので、ローカル座標系に変換
        float localHeight = stopRange * 2 / slotScaleY;

        // Y軸方向に stopRange*2 の高さで中央に配置
        stopRangeIndicator.sizeDelta = new Vector2(stopRangeIndicator.sizeDelta.x, localHeight);
        stopRangeIndicator.anchoredPosition = Vector2.zero;

        // Debug.Log($"[SkillSlotView] StopRangeIndicator 高さ: {localHeight}, 親スケールY: {slotScaleY}");
    }


    public void InitializeCells(SkillData[] skillList)
    {
        // 既存セルを破棄
        for (int i = reelContent.childCount - 1; i >= 0; i--)
            Destroy(reelContent.GetChild(i).gameObject);
        rowCells.Clear();

        float posY = 0f;
        for (int i = 0; i < skillList.Length; i++)
        {
            var cell = Instantiate(symbolPrefab, reelContent);
            cell.SetSkill(skillList[i]);

            var rt = cell.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, -posY);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, skillList[i].reelHeight);

            rowCells.Add(cell);
            posY += skillList[i].reelHeight;
        }

        totalHeight = posY; // リール全体の高さ
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
