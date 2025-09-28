using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public BattleManager battleManager;
    [Header("Confirmオーバーレイ")]
    public GameObject confirmOverlay;       // オーバーレイルート
    public GameObject skillSlotCellPrefab;  // SkillSlotCell プレハブ
    public Button confirmButton;

    public Transform confirmParent1;  // 左側配置
    public Transform confirmParent2;  // 右側配置

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning($"{name} に Button コンポーネントがアタッチされていません");
        }
    }

    /// <summary>
    /// 確定スキルオーバーレイを表示（複数候補対応）
    /// </summary>
    // public void ShowSkillConfirmOverlay(List<SkillData> candidateSkills, Action<SkillData> onConfirm)
    // {
    //     if (confirmOverlay == null || skillSlotCellPrefab == null || candidateSkills.Count == 0) return;

    //     // 既存の子をクリア
    //     foreach (Transform child in confirmParent1) Destroy(child.gameObject);
    //     foreach (Transform child in confirmParent2) Destroy(child.gameObject);

    //     // 候補を左右に交互に配置
    //     for (int i = 0; i < candidateSkills.Count; i++)
    //     {
    //         Transform parent = (i % 2 == 0) ? confirmParent1 : confirmParent2;
    //         GameObject cellObj = Instantiate(skillSlotCellPrefab, parent, false);
    //         SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
    //         cell.SetSkill(candidateSkills[i]);
    //     }

    //     confirmOverlay.SetActive(true);

    //     // Confirm ボタン押下時にランダム選択
    //     confirmButton.onClick.RemoveAllListeners();
    //     confirmButton.onClick.AddListener(() =>
    //     {
    //         confirmOverlay.SetActive(false);
    //         int index = UnityEngine.Random.Range(0, candidateSkills.Count);
    //         SkillData selected = candidateSkills[index];
    //         onConfirm?.Invoke(selected);
    //     });
    // }

        /// <summary>
    /// 確定スキルオーバーレイを表示（複数候補対応）
    /// </summary>
    public void ShowSkillConfirmOverlay(List<SkillData> candidateSkills, int player)
    {
        if (confirmOverlay == null || skillSlotCellPrefab == null || candidateSkills.Count == 0) return;

        if(player==1){
            for (int i = 0; i < candidateSkills.Count; i++)
            {
                GameObject cellObj = Instantiate(skillSlotCellPrefab, confirmParent1, false);
                SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
                cell.SetSkill(candidateSkills[i],1);
            }
        }else{
            for (int i = 0; i < candidateSkills.Count; i++)
            {
                GameObject cellObj = Instantiate(skillSlotCellPrefab, confirmParent2, false);
                SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
                cell.SetSkill(candidateSkills[i],2);
            }
        }
        // 候補を左右に交互に配置
        // Transform parent = (i % 2 == 0) ? confirmParent1 : confirmParent2;


        // for (int i = 0; i < candidateSkills.Count; i++)
        // {
        //     GameObject cellObj = Instantiate(skillSlotCellPrefab, parent, false);
        //     SkillSlotCell cell = cellObj.GetComponent<SkillSlotCell>();
        //     cell.SetSkill(candidateSkills[i]);
        // }

        confirmOverlay.SetActive(true);
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    private void OnButtonClicked()
    {
        //ダメージ処理開始
        battleManager.OnStartQueueButtonPressed();

        //オーバーレイ閉じる
        HideOverlay();
    }

    private void HideOverlay()
    {
        if (confirmOverlay != null)
            confirmOverlay.SetActive(false);

        // 既存の子をクリア
        foreach (Transform child in confirmParent1) Destroy(child.gameObject);
        foreach (Transform child in confirmParent2) Destroy(child.gameObject);
    }
}
