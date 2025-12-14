using System.Collections;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text
{
    public class TypewriterEffect : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private TMP_Text tmpText;
    
        [Header("设置")]
        [SerializeField] private float charsPerSecond = 20f; // 每秒显示多少个字符
    
        private Coroutine typingCoroutine;
        private string fullText;
        private bool isTyping;
    
        void Awake()
        {
            if (tmpText == null)
                tmpText = GetComponent<TMP_Text>();
        }
    
        /// <summary>
        /// 逐字符显示文本
        /// </summary>
        public void ShowText(string text)
        {
            // 如果正在显示，先停止
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
        
            fullText = text;
            typingCoroutine = StartCoroutine(TypeText());
        }
    
        /// <summary>
        /// 立即显示完整文本（跳过动画）
        /// </summary>
        public void ShowTextImmediate()
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
        
            tmpText.text = fullText;
            isTyping = false;
        }
    
        /// <summary>
        /// 停止显示并清空文本
        /// </summary>
        public void StopAndClear()
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
        
            tmpText.text = "";
            isTyping = false;
        }
    
        /// <summary>
        /// 设置显示速度
        /// </summary>
        public void SetSpeed(float speed)
        {
            charsPerSecond = speed;
        }
    
        /// <summary>
        /// 是否正在显示中
        /// </summary>
        public bool IsTyping => isTyping;
    
        private IEnumerator TypeText()
        {
            isTyping = true;
            tmpText.text = "";
        
            float delay = 1f / charsPerSecond;
        
            for (int i = 0; i <= fullText.Length; i++)
            {
                tmpText.text = fullText.Substring(0, i);
                yield return new WaitForSeconds(delay);
            }
        
            isTyping = false;
        }
    }
}