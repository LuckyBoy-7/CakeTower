using System.Linq;
using Lucky.Console.Scripts;
using Lucky.Dialogs;
using Lucky.Framework.CustomTick;
using Lucky.Inputs;
using Lucky.Interactive;
using Lucky.IO;
using Lucky.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Lucky.Framework
{
    /// <summary>
    /// 在设置界面保证Engine最先调用, 然后Engine去初始化各种Manager, 以保证更新顺序正确
    /// </summary>
    public class Engine : Singleton<Engine>
    {
        [Title("CustomTick")] public const float RawFixedDeltaTime = 1 / 60f;

        [SerializeField] private bool _useCustomTick;
        public bool UseCustomTick => _useCustomTick;
        [SerializeField] private bool _useCustomPhysics;
        public bool UseCustomPhysics => _useCustomPhysics;

        // 如果执行顺序是 update -> fixed update, 就不需要这个 bool 来操控 input 的更新了 
        private bool fixedUpdateHasUpdatedThisFrame = false;

        private int freezeCount = 0;

        public bool FreezeFixedUpdate { get; private set; }


        [Title("Dialog")] [SerializeField] private string startLanguage = "Simplified Chinese";

        protected override void SingletonAwake()
        {
            base.SingletonAwake();

            Time.fixedDeltaTime = RawFixedDeltaTime;

            InputManager.Initialize();
            GameCursor.Instance = new GameCursor();
            SaveLoadManager.Instance = new SaveLoadManager();
            ConsoleManager.Instance = new ConsoleManager();
            if (UseCustomTick)
                Tracker.Initialize();

            if (UseCustomPhysics)
            {
                // 保证更改 transform 后 Collider 能及时更新
                Physics.autoSyncTransforms = true;
                Physics.simulationMode = SimulationMode.Script;
            }

            Dialog.Initialize();
            Dialog.TrySetLanguage(startLanguage);

            LogUtils.Initialize();

            DontDestroyOnLoad(gameObject);
        }

        protected void Update()
        {
            if (!fixedUpdateHasUpdatedThisFrame)
                InputManager.Instance.Update(Time.deltaTime);

            fixedUpdateHasUpdatedThisFrame = false;

            GameCursor.Instance.Update();
            ConsoleManager.Instance.Update();
        }


        public void FixedUpdate()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    Time.timeScale /= 2;
                else if (Input.GetKeyDown(KeyCode.E))
                    Time.timeScale *= 2;
            }
#endif

            if (UseCustomTick)
            {
                fixedUpdateHasUpdatedThisFrame = true;
                InputManager.Instance.Update(Time.deltaTime);
                InputManager.Instance.Tick();

                FreezeFixedUpdate = freezeCount-- > 0;
            }
        }

        public void Freeze(int count) => freezeCount = Mathf.Max(freezeCount, count);
    }
}