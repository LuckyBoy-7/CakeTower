using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucky.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Console.Scripts
{
    public class ConsoleInput : MonoBehaviour
    {
        [SerializeField] private CustomInputField inputField;

        [FormerlySerializedAs("infoPanel")] [SerializeField]
        private ConsoleInfoPanel consoleInfoPanel;

        [FormerlySerializedAs("commanAutoComplete")] [SerializeField]
        private CommandAutoComplete commandAutoComplete;

        private readonly DefaultDict<string, List<MethodInfo>> commandNameToMethodInfos = new(() => new());

        public enum ConsoleInputTypes
        {
            Typing,
            AutoCompleting,
            HistoryTraceback
        }

        public ConsoleInputTypes consoleInputType = ConsoleInputTypes.Typing;

        private void Awake()
        {
            InitCommandInformation();
            inputField.AddForbiddenKeys(ConsoleManager.ConsoleSwitchKey);
        }

        private void OnEnable()
        {
            inputField.OnSubmit += TrySubmit;
            inputField.SetContent("");
        }

        private void OnDisable()
        {
            inputField.OnSubmit -= TrySubmit;
        }


        private void InitCommandInformation()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    var attributes = method.GetCustomAttributes(typeof(CommandAttribute), false);
                    foreach (var attribute in attributes)
                    {
                        if (attribute is CommandAttribute commandAttribute)
                        {
                            string commandName = commandAttribute.name;
                            commandNameToMethodInfos[commandName].Add(method);
                        }
                    }
                }
            }
        }

        private void TrySubmit(string content)
        {
            consoleInfoPanel.Log(">>> " + content);
            string[] splitedCommandText = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (splitedCommandText.Length == 0)
                return;

            string commandName = splitedCommandText[0];
            if (!commandNameToMethodInfos.ContainsKey(commandName))
            {
                Debug.LogWarning($"Unknown command name: {commandName}");
                consoleInfoPanel.ScrollToBottom();
                return;
            }

            List<MethodInfo> methods = commandNameToMethodInfos[commandName];

            string[] args = splitedCommandText.Skip(1).ToArray();

            bool findMatchedArguments = false;
            foreach (var methodInfo in methods.Where(m => m.GetParameters().Length == args.Length))
            {
                if (TryConvertArguments(methodInfo.GetParameters(), args, out object[] result))
                {
                    findMatchedArguments = true;
                    if (methodInfo.IsStatic)
                    {
                        methodInfo.Invoke(null, result); // 假设是静态方法
                    }
                    else
                    {
                        object ins = FindAnyObjectByType(methodInfo.DeclaringType);
                        if (ins != null)
                        {
                            methodInfo.Invoke(ins, result); // 假设是静态方法
                            break;
                        }

                        Debug.LogWarning($"Cannot find instance of type `{methodInfo.DeclaringType.Name}` to invoke method called `{commandName}`");
                    }
                }
            }

            if (!findMatchedArguments)
            {
                Debug.LogWarning($"{commandName}: No matched command with offered arguments");
            }

            consoleInfoPanel.ScrollToBottom();
        }

        private bool TryConvertArguments(ParameterInfo[] parameters, string[] args, out object[] result)
        {
            result = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                // 有一个参数对不上就无法调用此函数
                if (!CheckConvertArgumentValid(args[i], parameters[i].ParameterType))
                    return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                // 有一个参数对不上就无法调用此函数
                result[i] = ConvertArgument(args[i], parameters[i].ParameterType);
            }

            return true;
        }

        private object ConvertArgument(string value, Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.Parse(value);
            if (targetType == typeof(float)) return float.Parse(value);
            if (targetType == typeof(bool)) return bool.Parse(value);
            if (targetType.IsEnum) return Enum.Parse(targetType, value.Split(".", 2)[1], true);

            return Convert.ChangeType(value, targetType);
        }

        private bool CheckConvertArgumentValid(string value, Type targetType)
        {
            try
            {
                if (targetType == typeof(string))
                    return true;

                if (targetType == typeof(int))
                    return int.TryParse(value, out _);

                if (targetType == typeof(float))
                    return float.TryParse(value, out _);

                if (targetType == typeof(bool))
                    return bool.TryParse(value, out _);

                if (targetType.IsEnum)
                {
                    string[] splitedValue = value.Split(".", 2);
                    if (splitedValue.Length == 2)
                    {
                        string enumValue = splitedValue[1];
                        return Enum.TryParse(targetType, enumValue, true, out _);
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Cannot convert '{value}' to {targetType.Name}: {e.Message}");
            }
        }

        public List<string> GetCommandNames() => commandNameToMethodInfos.Keys.ToList();


        [Command("help")]
        public void ShowHelp(string commandName = "")
        {
            if (commandNameToMethodInfos.TryGetValue(commandName, out var methods))
            {
                foreach (var methodInfo in methods)
                {
                    CommandAttribute commandAttribute = (CommandAttribute)methodInfo.GetCustomAttributes(typeof(CommandAttribute), false)[0];
                    string signature = GetSignatureString(methodInfo);

                    consoleInfoPanel.Log($"{commandName}{GetSignatureString(methodInfo)}");
                    if (commandAttribute.description != "")
                        consoleInfoPanel.Log($"Description: {commandAttribute.description}");
                }
            }
            else
            {
                consoleInfoPanel.Log("No matched command found!", MessageLevel.Warning);
            }
        }

        private string GetSignatureString(MethodInfo methodInfo)
        {
            var paramTypes = methodInfo.GetParameters().Select(param => GetSimplifiedTypeName(param.ParameterType) + " " + param.Name);
            return $"({string.Join(", ", paramTypes)})";
        }

        private string GetSimplifiedTypeName(Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(string))
                return "string";
            if (type == typeof(bool))
                return "bool";
            return type.Name;
        }
    }
}