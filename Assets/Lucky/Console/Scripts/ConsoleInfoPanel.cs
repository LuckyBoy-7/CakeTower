using Lucky.Framework;
using UnityEngine;

namespace Lucky.Console.Scripts
{
    public class ConsoleInfoPanel : UIMonoBehaviour
    {
        [SerializeField] private MessagePanel messagePanel;

        private void OnEnable()
        {
            // 获取 Debug 输出
            Application.logMessageReceived += ApplicationOnlogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= ApplicationOnlogMessageReceived;
        }

        private void ApplicationOnlogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Log(condition, LogTypeToMessageLevel(type));
        }

        public void Log(string message, MessageLevel messageLevel = MessageLevel.Info)
        {
            messagePanel.AppendMessageWithTimeStamp(message, messageLevel);
        }

        public MessageLevel LogTypeToMessageLevel(LogType logType)
        {
            return logType switch
            {
                LogType.Error => MessageLevel.Error,
                LogType.Assert => MessageLevel.Error,
                LogType.Warning => MessageLevel.Warning,
                LogType.Log => MessageLevel.Info,
                LogType.Exception => MessageLevel.Error,
                _ => MessageLevel.Info
            };
        }

        [Command("cls")]
        public void ClearConsolePanel()
        {
            messagePanel.Clear();
        }

        public void ScrollToBottom()
        {
            messagePanel.ScrollToBottom();
        }
    }
}