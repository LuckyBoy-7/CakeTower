using TMPro;
using UnityEngine;

namespace Lucky.Dialogs
{
    /// <summary>
    /// 适用于主界面切换语言, 而不是游戏运行中, 不然在对话的时候改文本会比较麻烦(换句话就是说在主界面都是静态文本的时候直接替换方便一点)
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string localizationKey; // 在 Inspector 里填 key

        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            UpdateText();
            Dialog.AfterLanguageChanged += UpdateText;
        }

        private void OnDestroy()
        {
            Dialog.AfterLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(localizationKey))
                return;
            text.text = Dialog.Clean(localizationKey)[0];
        }
    }
}