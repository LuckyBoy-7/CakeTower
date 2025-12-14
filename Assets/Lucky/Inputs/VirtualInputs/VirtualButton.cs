using System;
using UnityEngine;

namespace Lucky.Inputs.VirtualInputs
{
    /// <summary>
    /// 对binding的管理, 主要提供buffer, 和重复输入的功能
    /// </summary>
    public class VirtualButton : VirtualInput
    {
        public bool Repeating { get; private set; }
        public Binding Binding;
        public float BufferTime;

        private float startRepeatTime; // 按下按键的那一刻经过多少时间被视作开始重复输入(常用于菜单中焦点的移动, 一开始顿一下, 然后自动移动)
        private float repeatDuration; // 当button被视作重复输入时, 多久trigger一次
        private float repeatTimer;
        private bool canRepeat;

        public float BufferTimer { get; private set; }
        public bool PressedConsumed { get; private set; } // 约等于禁用一帧 pressed 检查(相当于把 pressed 按键事件给吃掉了)

        public VirtualButton(Binding binding, float bufferTime)
        {
            Binding = binding;
            BufferTime = bufferTime;
        }

        public VirtualButton SetRepeat(float repeatTime) => SetRepeat(repeatTime, repeatTime);

        public VirtualButton SetRepeat(float startRepeatTime, float repeatDuration)
        {
            this.startRepeatTime = startRepeatTime;
            this.repeatDuration = repeatDuration;
            canRepeat = this.startRepeatTime > 0f;
            if (!canRepeat)
            {
                Repeating = false;
            }

            return this;
        }

        public override void Update(float deltaTime)
        {
            PressedConsumed = false;
            BufferTimer -= deltaTime;
            // 是否看作按下按键, 按下或者buffer还在且按着的时候
            if (Binding.Check())
            {
                if (Binding.Pressed())
                    BufferTimer = BufferTime;
            }
            else // 松了
            {
                Repeating = false;
                repeatTimer = Single.NegativeInfinity;
                BufferTimer = 0f;
                return;
            }

            if (canRepeat)
            {
                Repeating = false;
                if (Single.IsNegativeInfinity(repeatTimer))
                {
                    repeatTimer = startRepeatTime;
                    return;
                }

                repeatTimer -= Time.deltaTime;
                if (repeatTimer <= 0f)
                {
                    Repeating = true; // 重复模拟输入的那一帧会变成true, 后续又变成false
                    repeatTimer = repeatDuration;
                }
            }
        }

        public bool Check => Binding.Check();

        public bool Pressed => !PressedConsumed && (Binding.Pressed() || BufferTimer > 0f || Repeating);

        public bool Released => Binding.Released();

        public void ConsumeBuffer()
        {
            BufferTimer = 0f;
        }

        /// <summary>
        /// 约等于把按键吃了, 当前帧其他函数检测不到该按键
        /// </summary>
        public void ConsumePress()
        {
            BufferTimer = 0f;
            PressedConsumed = true;
        }

        public static implicit operator bool(VirtualButton button)
        {
            return button.Check;
        }
    }
}