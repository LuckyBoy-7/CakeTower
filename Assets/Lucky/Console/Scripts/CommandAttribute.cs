using System;

namespace Lucky.Console.Scripts
{
    // 可以添加命令描述和帮助信息
    public class CommandAttribute : Attribute
    {
        public string name;
        public string description; // 添加描述


        public CommandAttribute(string name, string description = "")
        {
            this.name = name;
            this.description = description;
        }
    }
}