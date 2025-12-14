using UnityEditor;
using UnityEngine;

namespace Lucky.Editor
{
    public static class AlignToSceneCameraPosition
    {
        [MenuItem("Tools/Align To Scene Camera Position %#&f")] // Ctrl + Shift + Alt F
        private static void AlignPositionOnly()
        {
            if (Selection.activeTransform == null)
            {
                Debug.LogWarning("No object selected.");
                return;
            }

            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                Debug.LogWarning("No active Scene view found.");
                return;
            }

            // 获取 Scene 视图相机的位置
            Vector3 camPos = sceneView.camera.transform.position;

            // 记录撤销操作（方便 Ctrl+Z）
            Undo.RecordObject(Selection.activeTransform, "Align To Scene Camera Position");

            // 只更新位置
            Selection.activeTransform.position = camPos;
        }
    }
}