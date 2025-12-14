using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Dialogs;
using Lucky.UI.Text.TextEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lucky.UI.Text
{
    [RequireComponent(typeof(TextEffectController))]
    public class TextController : MonoBehaviour
    {
        public bool isDebug;
        public string debugDialogKey;

        private TextEffectController textEffectController;

        public bool UseMouseButtonToInteract = false;

        [ShowIf("@ CanSkip && !UseMouseButtonToInteract")]
        public KeyCode nextKey = KeyCode.Return;

        public bool CanSkip = true;

        public bool InteractTrigger => UseMouseButtonToInteract && Input.GetMouseButtonDown(0) || !UseMouseButtonToInteract && Input.GetKeyDown(nextKey);
        public bool SkipTrigger => CanSkip && InteractTrigger;

        public float dialogueSpeed = 10;
        public bool isTalking;
        private bool isSkipping;
        public Action OnDialogueOver;
        public Coroutine dialogCoroutine = null;

        private void Awake()
        {
            textEffectController = GetComponent<TextEffectController>();
        }

        private void Start()
        {
            if (isDebug)
                Say(Dialog.Clean(debugDialogKey));
        }

        public void Update()
        {
            // 跳过
            if (SkipTrigger)
                isSkipping = true;
        }

        public void Say(List<string> dialogues)
        {
            if (dialogCoroutine != null)
                StopCoroutine(dialogCoroutine);
            dialogCoroutine = StartCoroutine(_ShowDialogues(dialogues));
        }

        private IEnumerator _ShowDialogues(List<string> dialogues)
        {
            foreach (string dialogue in dialogues)
            {
                yield return StartCoroutine(ShowDialogue(dialogue));
            }

            OnDialogueOver?.Invoke();
        }

        private IEnumerator ShowDialogue(string dialog)
        {
            isTalking = true;
            yield return ShowCharOneByOne(dialog);
            isTalking = false;
            yield return new WaitUntil(() => InteractTrigger);
        }

        private IEnumerator ShowCharOneByOne(string content)
        {
            var talkSpeed = dialogueSpeed;
            textEffectController.showCharNum = 0;
            textEffectController.RawContent = content;
            isSkipping = false;

            for (int i = 0; i <= textEffectController.finalParsedContent.Length; i += 1)
            {
                textEffectController.showCharNum = i;
                if (textEffectController.charPosToEventInfo.TryGetValue(i, out var eventArgs))
                {
                    if (eventArgs.TryGetValue("speed", out var speed))
                    {
                        talkSpeed = float.Parse(speed);
                    }

                    if (eventArgs.TryGetValue("delay", out var delay))
                    {
                        yield return new WaitForSeconds(float.Parse(delay));
                    }
                }

                if (isSkipping)
                    break;

                yield return new WaitForSeconds(1 / talkSpeed);
            }

            textEffectController.showCharNum = textEffectController.finalParsedContent.Length;
        }
    }
}