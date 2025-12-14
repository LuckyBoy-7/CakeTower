using System.Collections.Generic;
using Lucky.Extensions;
using UnityEngine;


namespace Lucky.Collections
{
    public interface ICommand
    {
        public ICommand Do();
        public ICommand Undo();
    }

    public class CommandManager
    {
        private List<List<ICommand>> commands = new();
        private int idx = -1; // 最后一个可撤销的操作序列的索引
        public bool log;


        public void CreateNewCommandSequence() // 表示一个操作下的首个command
        {
            while (commands.Count > idx + 1)
                commands.RemoveAt(commands.Count - 1);

            commands.Add(new());
            idx += 1;
        }

        public void AddCommand(ICommand command) => commands[idx].Add(command);


        public void Pop()
        {
            idx -= 1;
            if (commands.Count > 0)
                commands.Pop();
        }

        public void Do()
        {
            if (log)
                Debug.Log("Do");
            if (idx == commands.Count - 1)
            {
                if (log)
                    Debug.Log("已经回溯到底啦！");
                return;
            }

            commands[++idx].ForEach(command => command.Do());
        }

        public void Undo()
        {
            if (log)
                Debug.Log("Undo");
            if (idx == -1)
            {
                if (log)
                    Debug.Log("已经撤销到底啦！");
                return;
            }

            commands[idx--].ForEach(command => command.Undo());
        }
    }
}