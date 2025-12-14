using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lucky.Inputs
{
    /// <summary>
    /// 对Key的封装, 一对多
    /// </summary>
    public class Binding
    {
        public List<KeyCode> keys;

        /// <summary>
        /// 其他和自己不能共存的binding, 简单来说就是不能绑一样的键, 常用于一些跟UI有关的操作, 比如移动菜单, 确认取消什么的
        /// </summary>
        private readonly List<Binding> ExclusiveFrom = new List<Binding>();

        public bool HasInput => keys.Count > 0;

        public static HashSet<Binding> Bindings { get; private set; } = new();

        public Func<KeyCode, bool> GetKeyFunc;
        public Func<KeyCode, bool> GetKeyDownFunc;
        public Func<KeyCode, bool> GetKeyUpFunc;

        public Binding(params KeyCode[] keys)
        {
            this.keys = keys.ToList();
            Bindings.Add(this);
        }

        public bool Add(params KeyCode[] keys)
        {
            bool anySuccess = false;
            foreach (KeyCode key in keys)
            {
                if (this.keys.Contains(key))
                    continue;

                bool success = true;
                foreach (var other in ExclusiveFrom)
                {
                    if (other.ContainsKey(key)) // 但凡其他的binding需要这个键位, 我们就不跟他抢
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    this.keys.Add(key);
                    anySuccess = true;
                }
            }

            return anySuccess;
        }
        
        public bool ContainsKey(KeyCode key) => keys.Contains(key);

        /// <summary>
        /// 清空键盘binding, 尽量留一个没被其他 binding 绑过的 key
        /// </summary>
        public bool ClearKeyboard()
        {
            // 如果 ExclusiveFrom.Counter > 0, 意味着这个 Binding 还算重要, 得至少留一个键, 因为总不能让菜单中跳转和移动的键被扣完了吧
            if (ExclusiveFrom.Count > 0)
            {
                // 只剩一个key就不需要清空了
                if (this.keys.Count <= 1)
                    return false;

                KeyCode keys = this.keys[0];
                this.keys.Clear();
                this.keys.Add(keys);
            }
            else
            {
                keys.Clear();
            }

            return true;
        }

        public bool Check() => keys.Any(GetKeyFunc);

        public bool Pressed() => keys.Any(GetKeyDownFunc);

        public bool Released() => keys.Any(GetKeyUpFunc);

        public static void SetExclusive(params Binding[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].ExclusiveFrom.Clear();
            }

            for (var i = 0; i < list.Length; i++)
            {
                for (var j = i + 1; j < list.Length; j++)
                {
                    list[i].ExclusiveFrom.Add(list[j]);
                    list[j].ExclusiveFrom.Add(list[i]);
                }
            }
        }
    }
}