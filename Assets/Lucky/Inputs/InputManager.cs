using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Inputs.VirtualInputs;
using Lucky.Inputs.VirtualInputSet;
using Unity.VisualScripting;
using UnityEngine;

namespace Lucky.Inputs
{
    /// <summary>
    /// 对 VirtualInput 的管理
    /// </summary>
    public class InputManager
    {
        public static InputManager Instance;

        private HashSet<VirtualInput> VirtualInputs => VirtualInput.VirtualInputs;

        public BindingSet bindingSet;

        public BasicInputSet basic;
        public PlayerInputSet player;


        private InputManager()
        {
            Binding.Bindings.Clear();
            VirtualInput.VirtualInputs.Clear();
            bindingSet = new BindingSet();

            basic = new(bindingSet);
            player = new(bindingSet);
            foreach (var binding in Binding.Bindings)
            {
                trackedKeys.AddRange(binding.keys);
                binding.GetKeyFunc = GetKeyFunc;
                binding.GetKeyDownFunc = GetKeyDownFunc;
                binding.GetKeyUpFunc = GetKeyUpFunc;
            }
        }

        public static void Initialize()
        {
            Instance = new InputManager();
        }

        public void Update(float deltaTime)
        {
            if (Engine.Instance.UseCustomTick)
            {
                foreach (var key in trackedKeys)
                {

                    if (Input.GetKey(key))
                    {
                        tmpPressingKeys.Add(key);
                    }
                }
            }
            else
            {
                foreach (var button in VirtualInputs)
                {
                    button.Update(deltaTime);
                }
            }
        }

        public void Tick()
        {
            (curPressingKeys, prePressingKeys) = (prePressingKeys, curPressingKeys);
            (curPressingKeys, tmpPressingKeys) = (tmpPressingKeys, curPressingKeys);
            tmpPressingKeys.Clear();

            foreach (var button in VirtualInputs)
            {
                button.Update(Engine.RawFixedDeltaTime);
            }
        }


        private HashSet<KeyCode> trackedKeys = new();
        private HashSet<KeyCode> curPressingKeys = new();
        private HashSet<KeyCode> prePressingKeys = new();
        private HashSet<KeyCode> tmpPressingKeys = new();

        public Func<KeyCode, bool> GetKeyFunc => Engine.Instance.UseCustomTick ? FixedUpdateGetKey : Input.GetKey;
        private bool FixedUpdateGetKey(KeyCode keyCode) => curPressingKeys.Contains(keyCode);
        public Func<KeyCode, bool> GetKeyDownFunc => Engine.Instance.UseCustomTick ? FixedUpdateGetKeyDown : Input.GetKeyDown;
        private bool FixedUpdateGetKeyDown(KeyCode keyCode) => curPressingKeys.Contains(keyCode) && !prePressingKeys.Contains(keyCode);

        public Func<KeyCode, bool> GetKeyUpFunc => Engine.Instance.UseCustomTick ? FixedUpdateGetKeyUp : Input.GetKeyUp;
        private bool FixedUpdateGetKeyUp(KeyCode keyCode) => !curPressingKeys.Contains(keyCode) && prePressingKeys.Contains(keyCode);
    }
}