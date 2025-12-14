using TMPro;
using UnityEngine;

namespace Lucky.Console.Scripts
{
    public class InputFieldPlaceholder : MonoBehaviour
    {
        [SerializeField] private TMP_Text placeholderText;
        [SerializeField] private TMP_Text inputText;
        [SerializeField] private TMP_Text historyText;

        private void LateUpdate()
        {
            placeholderText.enabled = inputText.text == "" && historyText.text == "";
        }
    }
}