using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    [Header("Confirmオーバーレイ")]
    public GameObject confirmOverlay;       // オーバーレイルート
    public GameObject skillSlotCellPrefab;  // SkillSlotCell プレハブ
    public Button confirmButton;

    public Transform confirmParent1;  // 左側配置
    public Transform confirmParent2;  // 右側配置

    /// <summary>
    /// 確定スキルオーバーレイを表示（複数候補対応）
    /// </summary>
    public void ShowSkillConfirmOverlay(List<SkillData> candidateSkills, Action<SkillData> onConfirm)
    {
        if (confirmOverlay == null || skillSlotCellPrefab == null || candidateSkills.Count == 0) return;

        // 既存の子をクリア
        foreach (Transform child in confirmParent1) Destroy(child.gameObject);
        foreach (Transform child in confirmParent2) Destroy(child.gameObject);

        // 候補を左右に交互に配置
        for (int i = 0; i < candidateSkills.Count; i++)
        {
            Transform parent = (i % 2 == 0) ? confirmParent1 : confirmParent2;
            GameObject cellObj = Instantiate(skillSlotCellPrefab, parent, false);
            SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
            cell.SetSkill(candidateSkills[i]);
        }

        confirmOverlay.SetActive(true);

        // Confirm ボタン押下時にランダム選択
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            confirmOverlay.SetActive(false);
            int index = UnityEngine.Random.Range(0, candidateSkills.Count);
            SkillData selected = candidateSkills[index];
            onConfirm?.Invoke(selected);
        });
    }

    public void HideOverlay()
    {
        if (confirmOverlay != null)
            confirmOverlay.SetActive(false);
    }
}
