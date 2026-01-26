using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// 资源批量重命名工具
/// </summary>
public class AssetRenamerWindow : EditorWindow
{
    private Object m_targetFolder;
    private string m_prefix = "Asset";
    private int m_startNumber = 1;
    private bool m_includeSubfolders = false;
    private string m_filterExtension = "";
    private Vector2 m_scrollPosition;
    private string[] m_previewPaths;
    
    [MenuItem("Tools/资源批量重命名")]
    public static void ShowWindow()
    {
        GetWindow<AssetRenamerWindow>("资源批量重命名");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量重命名设置", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        m_targetFolder = EditorGUILayout.ObjectField("目标文件夹", m_targetFolder, typeof(DefaultAsset), false);
        
        EditorGUILayout.Space();
        
        m_prefix = EditorGUILayout.TextField("前缀名称 (XXX)", m_prefix);
        m_startNumber = EditorGUILayout.IntField("起始序号", m_startNumber);
        m_includeSubfolders = EditorGUILayout.Toggle("包含子文件夹", m_includeSubfolders);
        m_filterExtension = EditorGUILayout.TextField("文件类型过滤 (如: .png)", m_filterExtension);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("重命名格式: XXX_N\nXXX = 前缀名称, N = 序号数字", MessageType.Info);
        
        EditorGUILayout.Space();
        
        using (new EditorGUI.DisabledScope(m_targetFolder == null))
        {
            if (GUILayout.Button("预览", GUILayout.Height(30)))
            {
                PreviewRename();
            }
        }
        
        if (m_previewPaths != null && m_previewPaths.Length > 0)
        {
            EditorGUILayout.Space();
            GUILayout.Label($"预览结果 (共 {m_previewPaths.Length} 个文件):", EditorStyles.boldLabel);
            
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, GUILayout.Height(200));
            
            for (int i = 0; i < m_previewPaths.Length; i++)
            {
                string oldName = Path.GetFileName(m_previewPaths[i]);
                string newName = $"{m_prefix}_{m_startNumber + i}{Path.GetExtension(m_previewPaths[i])}";
                EditorGUILayout.LabelField($"{oldName}  →  {newName}", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("执行重命名", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("确认重命名", 
                    $"确定要重命名 {m_previewPaths.Length} 个文件吗？\n此操作可以通过Ctrl+Z撤销。", 
                    "确定", "取消"))
                {
                    ExecuteRename();
                }
            }
            
            if (GUILayout.Button("清除", GUILayout.Height(40), GUILayout.Width(80)))
            {
                m_previewPaths = null;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }

    private void PreviewRename()
    {
        if (m_targetFolder == null) return;
        
        string folderPath = AssetDatabase.GetAssetPath(m_targetFolder);
        
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("错误", "请选择一个有效的文件夹", "确定");
            return;
        }
        
        SearchOption searchOption = m_includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string fullPath = Path.Combine(Application.dataPath.Replace("Assets", ""), folderPath);
        
        string[] allFiles = Directory.GetFiles(fullPath, "*.*", searchOption)
            .Where(file => !file.EndsWith(".meta"))
            .OrderBy(file => file)
            .ToArray();
        
        if (!string.IsNullOrEmpty(m_filterExtension))
        {
            allFiles = allFiles.Where(file => Path.GetExtension(file).ToLower() == m_filterExtension.ToLower()).ToArray();
        }
        
        m_previewPaths = allFiles.Select(file => file.Replace(Application.dataPath.Replace("Assets", ""), "").Replace("\\", "/")).ToArray();
        
        if (m_previewPaths.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到符合条件的文件", "确定");
        }
    }

    private void ExecuteRename()
    {
        if (m_previewPaths == null || m_previewPaths.Length == 0) return;
        
        AssetDatabase.StartAssetEditing();
        
        try
        {
            for (int i = m_previewPaths.Length - 1; i >= 0; i--)
            {
                string oldPath = m_previewPaths[i];
                string directory = Path.GetDirectoryName(oldPath);
                string extension = Path.GetExtension(oldPath);
                string newName = $"{m_prefix}_{m_startNumber + i}{extension}";
                string newPath = Path.Combine(directory, newName).Replace("\\", "/");
                
                string error = AssetDatabase.RenameAsset(oldPath, newName);
                
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"重命名失败: {oldPath} -> {newName}\n错误: {error}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
        
        EditorUtility.DisplayDialog("完成", $"已成功重命名 {m_previewPaths.Length} 个文件", "确定");
        m_previewPaths = null;
    }
}

