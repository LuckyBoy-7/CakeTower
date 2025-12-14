using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Lucky.Collections;
using UnityEngine;

namespace Lucky.Framework.CustomTick
{
    public static class Tracker
    {
        private static DefaultDict<Type, HashSet<Type>> typeToParentTypes = new(() => new());

        private static DefaultDict<Type, UniqueList<TickBehaviour>> typeToEntities = new(() => new());
        public static UniqueList<TickBehaviour> Entities { get; } = new();


        public static HashSet<Type> GetParentTypes(Type type)
        {
            if (typeToParentTypes.TryGetValue(type, out var parentTypes))
            {
                return parentTypes;
            }

            return new HashSet<Type>();
        }


        public static bool Contains(TickBehaviour tickBehaviour) => typeToParentTypes.ContainsKey(tickBehaviour.GetType());

        public static void Initialize()
        {
            typeToParentTypes.Clear();
            typeToEntities.Clear();
            Entities.Clear();

            Assembly assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes();
            foreach (Type type in allTypes)
            {
                var attributes = type.GetCustomAttributes(typeof(TrackedAttribute), false);
                foreach (var attribute in attributes)
                {
                    if (attribute is TrackedAttribute trackedAttribute)
                    {
                        RegisterTrackedType(trackedAttribute, allTypes, type);
                    }
                }
            }
        }

        private static void RegisterTrackedType(TrackedAttribute trackedAttribute, Type[] allTypes, Type type)
        {
            if (trackedAttribute.AlsoInherited)
            {
                foreach (var otherType in allTypes)
                {
                    if (type.IsAssignableFrom(otherType))
                    {
                        typeToParentTypes[otherType].Add(type);
                    }
                }
            }
            else
            {
                typeToParentTypes[type].Add(type);
            }
        }


        /// <summary>
        /// 注册 entity
        /// </summary>
        public static bool Add(TickBehaviour entity)
        {
            if (entity == null)
                return false;

            if (!Entities.Add(entity))
                return false;

            // 按类型存储
            Type type = entity.GetType();
            foreach (var parentType in GetParentTypes(type))
            {
                typeToEntities[parentType].Add(entity);
            }

            return true;
        }

        /// <summary>
        /// 注销 entity
        /// </summary>
        public static bool Remove(TickBehaviour entity)
        {
            if (entity == null)
                return false;

            if (!Entities.Remove(entity))
                return false;

            // 从类型列表移除
            Type type = entity.GetType();
            foreach (var parentType in GetParentTypes(type))
            {
                typeToEntities[parentType].Remove(entity);
            }

            return true;
        }


        /// <summary>
        /// 获取所有某类型的 entity
        /// </summary>
        public static IEnumerable<T> GetEntities<T>() where T : TickBehaviour
        {
            Type type = typeof(T);
            if (!typeToEntities.TryGetValue(type, out var entities))
            {
                Debug.LogWarning("You are trying to get entities of a type that is not tracked: " + type.Name);
                yield break;
            }

            foreach (var entity in entities)
            {
                yield return (T)entity;
            }
        }

        /// <summary>
        /// 获取第一个某类型的 entity
        /// </summary>
        public static T GetFirst<T>() where T : TickBehaviour
        {
            foreach (var entity in Entities)
            {
                if (entity is T typedEntity)
                    return typedEntity;
            }

            return null;
        }
    }
}