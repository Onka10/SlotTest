using UnityEngine;
using UnityEditor;
using System.IO;

public class SkillImporter : EditorWindow
{
    private string csvPath = "Assets/Data/skills.csv";
    private string outputFolder = "Assets/Data/Skills/";

    [MenuItem("Tools/Import Skills from CSV")]
    public static void ShowWindow()
    {
        GetWindow<SkillImporter>("Skill Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("CSVからSkillDataを自動生成", EditorStyles.boldLabel);

        csvPath = EditorGUILayout.TextField("CSV Path", csvPath);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Import"))
        {
            ImportSkills();
        }
    }

    void ImportSkills()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSVファイルが見つかりません: " + csvPath);
            return;
        }

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Debug.Log("出力フォルダを作成します: " + outputFolder);
            Directory.CreateDirectory(outputFolder);
        }

        string[] lines = File.ReadAllLines(csvPath);

        // 1行目はヘッダとしてスキップ
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            if (values.Length < 4) // skillName, power, reelHeight, colorName
            {
                Debug.LogWarning($"スキル情報不足: {lines[i]}");
                continue;
            }

            string skillName = values[0];
            int power = int.Parse(values[1]);
            float reelHeight = float.Parse(values[2]);
            Color skillColor = ParseColorName(values[3].Trim().ToLower());

            string assetPath = $"{outputFolder}{skillName}.asset";
            SkillData skill = AssetDatabase.LoadAssetAtPath<SkillData>(assetPath);

            if (skill == null)
            {
                skill = ScriptableObject.CreateInstance<SkillData>();
                AssetDatabase.CreateAsset(skill, assetPath);
            }

            skill.skillName = skillName;
            skill.power = power;
            skill.reelHeight = reelHeight;
            skill.skillColor = skillColor;

            EditorUtility.SetDirty(skill);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("SkillData のインポート完了！");
    }

    /// <summary>
    /// red, green, blue, white の文字列に変換
    /// </summary>
    private Color ParseColorName(string colorName)
    {
        switch (colorName)
        {
            case "red": return Color.red;
            case "green": return Color.green;
            case "blue": return Color.blue;
            case "white": return Color.white;
            default:
                Debug.LogWarning($"未定義の色名 '{colorName}' が指定されました。白を代入します。");
                return Color.white;
        }
    }
}
