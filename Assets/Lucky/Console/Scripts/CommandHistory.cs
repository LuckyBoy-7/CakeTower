using Lucky.Inputs;
using Lucky.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Console.Scripts
{
    public class CommandHistroy : MonoBehaviour
    {
        [SerializeField] private CustomInputField inputField;
        [FormerlySerializedAs("commanAutoComplete")] [SerializeField] private CommandAutoComplete commandAutoComplete;
        [SerializeField] private ConsoleInput consoleInput;
        [SerializeField] private TMP_Text historyCompleteText;

        private HistoryTraceback<string> commandHistory = new();

        private void OnEnable()
        {
            inputField.OnSubmit += OnSubmit;
        }

        private void OnDisable()
        {
            inputField.OnSubmit -= OnSubmit;
        }

        private void Update()
        {
            if (consoleInput.consoleInputType == ConsoleInput.ConsoleInputTypes.HistoryTraceback)
            {
                if (InputManager.Instance.basic.Tab.Pressed)
                {
                    InputManager.Instance.basic.Tab.ConsumePress();
                    if (commandHistory.TryGetItem(out string content))
                    {
                        inputField.SetContent(content);
                        Hide();
                    }
                }
                
                // 也就是如果输入了那就不提示了
                if (inputField.Content != "")
                {
                    Hide();
                    commandHistory.ResetIndex();
                }
            }


            if (consoleInput.consoleInputType != ConsoleInput.ConsoleInputTypes.AutoCompleting)
            {
                bool success = false;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    commandHistory.Backward();
                    success = TryShowHistory();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    commandHistory.Forward();
                    success = TryShowHistory();
                }

                if (success)
                    inputField.SetContent("");
            }
        }

        private void Hide()
        {
            historyCompleteText.text = "";
            consoleInput.consoleInputType = ConsoleInput.ConsoleInputTypes.Typing;
        }

        private bool TryShowHistory()
        {
            if (commandHistory.TryGetItem(out string content))
            {
                historyCompleteText.text = content;
                consoleInput.consoleInputType = ConsoleInput.ConsoleInputTypes.HistoryTraceback;
                return true;
            }

            Hide();
            return false;
        }

        private void OnSubmit(string content)
        {
            commandHistory.Add(content);
            commandHistory.ResetIndex();
        }
    }
}