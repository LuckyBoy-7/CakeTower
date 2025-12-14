using UnityEngine;

namespace Lucky.Console.Scripts
{
    public class ConsoleManager
    {
        public const KeyCode ConsoleSwitchKey = KeyCode.BackQuote;
        public static ConsoleManager Instance;
        private bool inited = false;
        private bool on = false;
        private GameObject canvas;

        public void Update()
        {
            if (Input.GetKeyDown(ConsoleSwitchKey))
            {
                TrySwitchConsole();
            }
        }

        private void TrySwitchConsole()
        {
            if (!inited)
            {
                inited = true;
                canvas = Object.Instantiate(Resources.Load<GameObject>("Console/Canvas"));
                Object.DontDestroyOnLoad(canvas);
            }

            if (on)
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
            }

            on = !on;
        }
    }
}