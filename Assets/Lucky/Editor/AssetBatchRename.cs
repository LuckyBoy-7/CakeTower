using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Lucky.Editor
{
    /// <summary>
    /// Asset 批量改名工具 - 提供多种资源重命名功能
    /// </summary>
    public class AssetBatchRename : EditorWindow
    {
        // 基础改名参数
        private string baseName = "Asset"; // 基础名称，用于批量重命名
        private string delimiter = "_"; // 基础名称，用于批量重命名
        private int startNumber = 0; // 起始数字，用于生成序号

        /// <summary>
        /// 在 Assets 菜单中添加工具入口
        /// </summary>
        [MenuItem("Assets/批量改名工具", false, 20)]
        static void ShowWindow()
        {
            // 显示编辑器窗口
            GetWindow<AssetBatchRename>("Asset 批量改名");
        }

        /// <summary>
        /// 窗口启用时订阅选择改变事件
        /// </summary>
        void OnEnable()
        {
            Selection.selectionChanged += Repaint;
        }

        /// <summary>
        /// 窗口禁用时取消订阅事件
        /// </summary>
        void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
        }

        /// <summary>
        /// 绘制编辑器窗口 UI
        /// </summary>
        void OnGUI()
        {
            // 标题
            GUILayout.Label("Asset 批量改名工具", EditorStyles.boldLabel);
            // 显示当前选中的资源数量
            EditorGUILayout.HelpBox($"当前选中: {Selection.objects.Length} 个资源", MessageType.Info);

            GUILayout.Space(10);

            // ========== 基础改名区域 ==========
            GUILayout.Label("基础改名", EditorStyles.boldLabel);
            baseName = EditorGUILayout.TextField("基础名称", baseName);
            delimiter = EditorGUILayout.TextField("分隔符", delimiter);
            startNumber = EditorGUILayout.IntField("起始数字", startNumber);
            // 执行基础改名按钮
            if (GUILayout.Button($"重命名为: 名称{delimiter}数字"))
                BasicRename();

            GUILayout.Space(10);

            // ========== 实用工具区域 ==========
            GUILayout.Label("实用工具", EditorStyles.boldLabel);

            // 第一行：移除数字和空格
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("移除数字"))
                RemoveNumbers();

            if (GUILayout.Button("移除空格"))
                RemoveSpaces();

            EditorGUILayout.EndHorizontal();

            // 第二行：大小写转换
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("转小写"))
                ToLowerCase();

            if (GUILayout.Button("转大写"))
                ToUpperCase();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 基础改名：将选中的资源重命名为 "基础名称 + 分隔符 + 序号" 格式
        /// </summary>
        void BasicRename()
        {
            if (!ValidateSelection()) return;

            var selected = Selection.objects;
            for (int i = 0; i < selected.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(selected[i]);
                string newName = $"{baseName}{delimiter}{startNumber + i}";
                AssetDatabase.RenameAsset(path, newName);
            }

            SaveAndRefresh($"已重命名 {selected.Length} 个资源");
        }

        /// <summary>
        /// 移除资源名称中的数字、下划线、破折号和括号
        /// </summary>
        void RemoveNumbers()
        {
            RenameWithTransform(oldName => Regex.Replace(oldName, @"\d", ""));
        }

        /// <summary>
        /// 移除资源名称中的所有空格
        /// </summary>
        void RemoveSpaces()
        {
            RenameWithTransform(oldName => oldName.Replace(" ", ""));
        }

        /// <summary>
        /// 将资源名称转换为小写
        /// </summary>
        void ToLowerCase()
        {
            RenameWithTransform(oldName => oldName.ToLower());
        }

        /// <summary>
        /// 将资源名称转换为大写
        /// </summary>
        void ToUpperCase()
        {
            RenameWithTransform(oldName => oldName.ToUpper());
        }

// ========== 辅助方法 ==========

        /// <summary>
        /// 验证是否有选中资源
        /// </summary>
        bool ValidateSelection()
        {
            if (Selection.objects.Length == 0)
            {
                ShowError("请先在 Project 窗口选择要改名的资源");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 对选中的资源应用名称转换函数
        /// </summary>
        /// <param name="transform">名称转换函数</param>
        void RenameWithTransform(System.Func<string, string> transform)
        {
            if (!ValidateSelection()) return;

            int count = 0;
            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string oldName = Path.GetFileNameWithoutExtension(path);
                string newName = transform(oldName);

                if (oldName != newName)
                {
                    AssetDatabase.RenameAsset(path, newName);
                    count++;
                }
            }

            SaveAndRefresh(count > 0 ? $"已处理 {count} 个资源" : "没有资源需要修改");
        }

        /// <summary>
        /// 保存资源并刷新，同时输出日志
        /// </summary>
        void SaveAndRefresh(string message = null)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="message">错误信息</param>
        void ShowError(string message)
        {
            EditorUtility.DisplayDialog("错误", message, "确定");
        }
    }
}