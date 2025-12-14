using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Console.Scripts
{
    [Serializable]
    public class MessageData
    {
        public string message;
        public MessageLevel messageLevel;
        public float height;
        public bool selected;

        public MessageData(string message, MessageLevel messageLevel, float height)
        {
            this.message = message;
            this.messageLevel = messageLevel;
            this.height = height;
        }
    }

    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public class MessagePanel : UIMonoBehaviour
    {
        [Header("Basic Settings")] [SerializeField]
        private TMP_Text messageTextPrefab;

        [SerializeField] private float space;
        [SerializeField] private bool fitContentSizeY;


        [Header("Scroll")] [SerializeField] private bool canScroll = true;
        [SerializeField] private float scrollSpeed = 160;

        [Header("Selection")] [SerializeField] private bool enableSelection;
        [SerializeField] private int selectIndex = 0;
        [SerializeField] private Color selectionColor = new Color(1, 1, 0);

        private List<TMP_Text> messageTexts = new();
        [SerializeField] private List<MessageData> messageDatas = new();
        private TMP_Text heightCheckMessageText;

        private Vector2 messageStartPositionDelta;
        private Vector2 origMessageStartPositionDelta;
        private float textSumHeight;

        // 所有消息内容的总高度 (包括间隔)
        private float TotalTextContentHeight => messageDatas.Count == 0 ? 0 : textSumHeight + space * (messageDatas.Count - 1);

        private float StartPositionDeltaClampMinY => PanelHeight - space - TotalTextContentHeight;

        // private float StartPositionDeltaClampMinY => PanelHeight - origMessageStartPositionDelta.y - TotalTextContentHeight;
        private float StartPositionDeltaClampMaxY => space;

        private Rect panelRect;
        private Vector2 bottomLeft;
        private Vector2 topLeft;
        private float PanelHeight => topLeft.y - bottomLeft.y;
        public int MessageCount => messageDatas.Count;


        private void Awake()
        {
            origMessageStartPositionDelta = messageStartPositionDelta = Vector2.one * space;
            // 用于测量高度的 TMP_Text
            heightCheckMessageText = Instantiate(messageTextPrefab, transform);
            heightCheckMessageText.enabled = false;
            heightCheckMessageText.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private void Start()
        {
            UpdatePanelSizeInfo();
        }

        private void UpdatePanelSizeInfo()
        {
            panelRect = RectTransform.GetScreenSpaceRect();
            bottomLeft = panelRect.min;
            topLeft = panelRect.max.WithX(bottomLeft.x);
        }

        private void Update()
        {
            TryUpdateScroll();
            TryUpdateSelection();
        }

        private void TryUpdateSelection()
        {
            if (!enableSelection)
                return;
            if (messageDatas.Count == 0)
                return;
            selectIndex = MathUtils.Clamp(selectIndex, 0, messageDatas.Count - 1);
            int moveDir = Input.GetKeyDown(KeyCode.DownArrow) ? 1 : Input.GetKeyDown(KeyCode.UpArrow) ? -1 : 0;
            if (moveDir != 0)
            {
                messageDatas[selectIndex].selected = false;
                selectIndex = (selectIndex + moveDir + messageDatas.Count) % messageDatas.Count;
                messageDatas[selectIndex].selected = true;
                UpdateLayout();
            }
            else if (!messageDatas[selectIndex].selected)
            {
                messageDatas[selectIndex].selected = true;
                UpdateLayout();
            }
        }

        private void TryUpdateScroll()
        {
            if (!canScroll)
                return;
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                messageStartPositionDelta += -Input.mouseScrollDelta * scrollSpeed;
                messageStartPositionDelta = messageStartPositionDelta.WithY(MathUtils.Clamp(messageStartPositionDelta.y, StartPositionDeltaClampMinY, StartPositionDeltaClampMaxY));
                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            TryFitContentY();


            Vector2 currentPosition = bottomLeft + messageStartPositionDelta;

            ClearTexts();
            int textIndex = 0;
            bool visibleItemHasAppeared = false; // 小优化, 因为需要显示的 message 只会出现在中间一段, 所以如果已经出现过一个可见的 item, 那么后面不可见的的就不需要再检查了
            for (var i = messageDatas.Count - 1; i >= 0; i--)
            {
                MessageData messageData = messageDatas[i];

                float bottomHeight = currentPosition.y;
                float topHeight = currentPosition.y + messageData.height;
                bool textItemVisible = topHeight > bottomLeft.y && bottomHeight < topLeft.y;
                if (textItemVisible)
                {
                    PlaceTextItem(textIndex++, messageData, currentPosition);
                    visibleItemHasAppeared = true;
                }
                else if (visibleItemHasAppeared)
                    break;

                currentPosition += Vector2.up * (space + messageData.height);
            }
        }

        private void TryFitContentY()
        {
            if (!fitContentSizeY)
                return;
            float targetY = TotalTextContentHeight + space * 2;
            RectTransform.sizeDelta = RectTransform.sizeDelta.WithY(targetY);
            UpdatePanelSizeInfo();
        }

        private void ClearTexts()
        {
            foreach (var tmpText in messageTexts)
            {
                tmpText.text = "";
            }
        }

        private void PlaceTextItem(int i, MessageData messageData, Vector2 currentPosition)
        {
            MakeSureTextCapacity(i + 1);

            TMP_Text messageText = messageTexts[i];
            messageText.transform.position = currentPosition;
            messageText.rectTransform.sizeDelta = messageText.rectTransform.sizeDelta.WithX(RectTransform.rect.width - space * 2);
            messageText.text = messageData.message;
            // 根据消息等级设置颜色
            messageText.color = messageData.messageLevel switch
            {
                MessageLevel.Info => Color.white,
                MessageLevel.Warning => Color.yellow,
                MessageLevel.Error => Color.red,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (messageData.selected)
                messageText.color = selectionColor;
        }

        private void MakeSureTextCapacity(int capacity)
        {
            while (messageTexts.Count < capacity)
            {
                var tmpText = Instantiate(messageTextPrefab, transform);
                messageTexts.Add(tmpText);
                tmpText.rectTransform.SetAsFirstSibling();
            }
        }

        /// <summary>
        /// 在信息数据中加入一个条目
        /// </summary>
        /// <param name="info"></param>
        /// <param name="messageLevel"></param>
        public void AppendMessage(string info, MessageLevel messageLevel = MessageLevel.Info)
        {
            var messageHeight = GetMessageHeight(info);
            textSumHeight += messageHeight;
            bool hasScrolled = messageStartPositionDelta != origMessageStartPositionDelta;
            if (hasScrolled)
                messageStartPositionDelta += Vector2.down * (messageHeight + space);
            messageDatas.Add(new MessageData(info, messageLevel, messageHeight));
            UpdateLayout();
        }

        public void AppendMessageWithTimeStamp(string info, MessageLevel messageLevel = MessageLevel.Info) => AppendMessage(AddFormattedTimeBeforeMessage(info), messageLevel);

        private float GetMessageHeight(string info)
        {
            heightCheckMessageText.rectTransform.sizeDelta = heightCheckMessageText.rectTransform.sizeDelta.WithX(RectTransform.rect.width - space * 2);
            heightCheckMessageText.enabled = true;
            heightCheckMessageText.text = info;
            // 获取 content size fitter 更新后的值
            LayoutRebuilder.ForceRebuildLayoutImmediate(heightCheckMessageText.rectTransform);
            heightCheckMessageText.enabled = false;

            float messageHeight = heightCheckMessageText.rectTransform.rect.height;
            // 补偿新内容的误差, 也就是不 scroll, 不然得重新写布局逻辑弄成从上往下排布
            return messageHeight;
        }

        private static string AddFormattedTimeBeforeMessage(string info)
        {
            float seconds = Time.time;
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            string time = span.ToString(@"mm\:ss");
            info = $"[{time}] {info}";
            return info;
        }

        public void Clear()
        {
            messageDatas.Clear();
            textSumHeight = 0;
            UpdatePanelSizeInfo();
            UpdateLayout();
        }

        public void ScrollToBottom()
        {
            messageStartPositionDelta = messageStartPositionDelta.WithY(StartPositionDeltaClampMaxY);
            UpdateLayout();
        }

        public string GetSelectedMessage()
        {
            if (messageDatas.Count == 0)
            {
                Debug.LogWarning("No message in the panel");
                return "";
            }
            return messageDatas[selectIndex].message;
        }
    }
}