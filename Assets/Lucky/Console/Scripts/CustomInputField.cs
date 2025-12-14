using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Inputs;
using Lucky.Inputs.VirtualInputSet;
using TMPro;
using UnityEngine;

namespace Lucky.Console.Scripts
{
    public class CustomInputField : MonoBehaviour
    {
        [SerializeField] private CaretPositioner caretPositioner;
        [SerializeField] private TMP_Text realText;


        public string Content
        {
            get => realText.text;
            private set => realText.text = value;
        }


        public int CaretIndex
        {
            get => caretPositioner.caretPosition;
            private set
            {
                caretPositioner.caretPosition = value;
                OnCaretChanged?.Invoke(Content, CaretIndex);
            }
        }

        public Action<string> OnSubmit;
        public Action<string, int> OnCaretChanged;

        private BasicInputSet Basic => InputManager.Instance.basic;

        private HashSet<KeyCode> forbiddenKeys = new HashSet<KeyCode>();

        
        public void AddForbiddenKeys(params KeyCode[] keys)
        {
            foreach (var key in keys)
            {
                forbiddenKeys.Add(key);
            }
        }

        private void Update()
        {
            foreach (var c in Input.inputString)
            {
                if (c.IsAsciiPrintable())
                {
                    AddChar(c);
                }
            }

            // 组合键
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Basic.Backspace.Pressed)
                    DoFastBackspace();
                else if (Basic.MenuRight.Pressed)
                    TryFastMoveCursorRight();
                else if (Basic.MenuLeft.Pressed)
                    TryFastMoveCursorLeft();
            }
            else if (Input.GetKeyDown(KeyCode.Home))
                TryMoveCursorToHome();
            else if (Input.GetKeyDown(KeyCode.End))
                TryMoveCursorToEnd();
            else if (Basic.MenuLeft.Pressed)
                TryMoveCursorLeft();
            else if (Basic.MenuRight.Pressed)
                TryMoveCursorRight();
            else if (Basic.Backspace.Pressed)
                TryBackspace();
            else if (Input.GetKeyDown(KeyCode.Return))
                TrySubmit();


            caretPositioner.UpdateCaretVisual();
        }

        private void TryMoveCursorToEnd()
        {
            CaretIndex = Content.Length;
        }

        private void TryMoveCursorToHome()
        {
            CaretIndex = 0;
        }

        private void TryFastMoveCursorRight()
        {
            if (CaretIndex == Content.Length)
                return;
            string origText = Content;
            int i = CaretIndex;
            while (i < Content.Length && !char.IsLetterOrDigit(origText[i]))
                i += 1;
            while (i < Content.Length && char.IsLetterOrDigit(origText[i]))
                i += 1;
            CaretIndex = i;
        }

        private void TryFastMoveCursorLeft()
        {
            if (CaretIndex == 0)
                return;
            string origText = Content;
            int i = CaretIndex - 1;
            while (i >= 0 && !char.IsLetterOrDigit(origText[i]))
                i -= 1;
            while (i >= 0 && char.IsLetterOrDigit(origText[i]))
                i -= 1;
            CaretIndex = i + 1;
        }

        private void TryBackspace()
        {
            if (CaretIndex == 0)
                return;
            Content = Content.Remove(CaretIndex - 1, 1);
            CaretIndex -= 1;
        }

        private void AddChar(char c)
        {
            Content = Content.Insert(CaretIndex, c.ToString());
            CaretIndex += 1;
        }

        private void TrySubmit()
        {
            var content = Content;
            if (content == "")
                return;

            OnSubmit?.Invoke(content);
            Content = "";
            CaretIndex = 0;
        }

        private void TryMoveCursorLeft() => TryMoveCursor(-1);
        private void TryMoveCursorRight() => TryMoveCursor(1);

        private void TryMoveCursor(int i)
        {
            int nextPosition = CaretIndex + i;
            if (nextPosition >= 0 && nextPosition <= Content.Length)
            {
                CaretIndex += i;
            }
        }

        private void DoFastBackspace()
        {
            if (CaretIndex == 0)
                return;
            string origText = Content;
            int i = CaretIndex - 1;
            while (i >= 0 && !char.IsLetterOrDigit(origText[i]))
                i -= 1;
            while (i >= 0 && char.IsLetterOrDigit(origText[i]))
                i -= 1;
            Content = origText.Substring(0, i + 1) + origText.Substring(CaretIndex);
            CaretIndex = i + 1;
        }

        public void AutoComplete(string item, char delimiter = '.')
        {
            if (Content == "")
            {
                Content = item;
                CaretIndex = item.Length;
                return;
            }

            int left = CaretIndex - 1;
            while (left >= 0 && Content[left] != delimiter)
            {
                left -= 1;
            }

            int right = CaretIndex;
            while (right < Content.Length && Content[right] != delimiter)
            {
                right += 1;
            }

            Content = Content.Substring(0, left + 1) + item + Content.Substring(right);
            CaretIndex = left + 1 + item.Length;
        }

        public void SetContent(string content)
        {
            Content = content;
            CaretIndex = content.Length;
        }
    }
}