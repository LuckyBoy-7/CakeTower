using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Utilities.Groups
{
    public class ComponentGroup<T> : MonoBehaviour where T : Component
    {
        public List<T> components;

        public void DoAction(Action<T> action)
        {
            foreach (var component in components)
            {
                action(component);
            }
        }
    }
}