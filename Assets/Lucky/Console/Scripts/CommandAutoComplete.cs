using System;
using System.Collections.Generic;
using Lucky.Collections;
using Lucky.Framework;
using Lucky.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Console.Scripts
{
    public class CommandAutoComplete : UIMonoBehaviour
    {
        public class CommandTrie
        {
            public DefaultDict<string, CommandTrie> children = new(() => new());
        }

        [SerializeField] private MessagePanel messagePanel;
        [SerializeField] private CustomInputField inputField;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private ConsoleInput consoleInput;

        private CommandTrie root;

        private string lastContent = "";
        private int lastCareIndex = -1;
        private bool dirty = false;

        private void OnEnable()
        {
            inputField.OnCaretChanged += OnCaretChanged;
        }


        private void OnDisable()
        {
            inputField.OnCaretChanged -= OnCaretChanged;
        }

        private void OnCaretChanged(string content, int caretIndex)
        {
            if (lastContent != content || lastCareIndex != caretIndex)
            {
                lastContent = content;
                lastCareIndex = caretIndex;
                if (lastContent == "" || caretIndex == 0)
                {
                    Hide();
                }
                else // 只有在非空情况下才 build panel, 反之用 tab 生成 panel
                    dirty = true;
            }
        }

        private void Start()
        {
            BuildCommandTrie();
        }

        private void BuildCommandTrie()
        {
            root = new CommandTrie();
            List<string> commands = consoleInput.GetCommandNames();

            foreach (string command in commands)
            {
                string[] splitedString = command.Split(".");
                CommandTrie currentNode = root;
                foreach (string part in splitedString)
                {
                    if (!currentNode.children.ContainsKey(part))
                        currentNode.children[part] = new CommandTrie();
                    currentNode = currentNode.children[part];
                }
            }
        }

        private void LateUpdate()
        {
            if (consoleInput.consoleInputType != ConsoleInput.ConsoleInputTypes.HistoryTraceback)
            {
                if (dirty)
                {
                    RebuildPanel(lastContent.Substring(0, lastCareIndex));
                    dirty = false;
                }


                if (!TryShow())
                {
                    Hide();
                    consoleInput.consoleInputType = ConsoleInput.ConsoleInputTypes.Typing;
                }
            }

            if (consoleInput.consoleInputType == ConsoleInput.ConsoleInputTypes.AutoCompleting)
            {
                if (InputManager.Instance.basic.Tab.Pressed)
                {
                    InputManager.Instance.basic.Tab.ConsumePress();
                    if (messagePanel.MessageCount > 0)
                        inputField.AutoComplete(messagePanel.GetSelectedMessage());
                    else
                        RebuildPanel("");
                }
            }

            if (consoleInput.consoleInputType == ConsoleInput.ConsoleInputTypes.Typing)
            {
                if (InputManager.Instance.basic.Tab.Pressed && inputField.CaretIndex == 0)
                {
                    InputManager.Instance.basic.Tab.ConsumePress();
                    RebuildPanel("");
                    TryShow();
                }
            }
        }

        private bool TryShow()
        {
            bool anyItme = messagePanel.MessageCount > 0;
            if (anyItme)
                Show();
            return anyItme;
        }

        public void RebuildPanel(string value)
        {
            messagePanel.Clear();
            string[] splitedString = value.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
            CommandTrie node = root;
            if (splitedString.Length == 0)
            {
                AddItems(node, "");
                return;
            }

            string[] commandParts = splitedString.Length == 0 ? new string[] { "" } : splitedString[0].Split(".");

            string lastPart = "";
            int lastMatchIndex = -1;
            for (var i = 0; i < commandParts.Length; i++)
            {
                string part = commandParts[i];
                lastPart = part;
                if (part == "" && i == 0)
                    break;
                if (node.children.ContainsKey(part))
                {
                    node = node.children[part];
                    lastMatchIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (lastMatchIndex == commandParts.Length - 2)
                AddItems(node, lastPart);
        }

        private void AddItems(CommandTrie node, string prefix)
        {
            foreach (var child in node.children)
            {
                if (child.Key.StartsWith(prefix))
                {
                    messagePanel.AppendMessage(child.Key);
                }
            }
        }

        private void Hide()
        {
            messagePanel.Clear();
            backgroundImage.enabled = false;
        }

        private void Show()
        {
            consoleInput.consoleInputType = ConsoleInput.ConsoleInputTypes.AutoCompleting;
            backgroundImage.enabled = true;
            gameObject.SetActive(true);
        }
    }
}