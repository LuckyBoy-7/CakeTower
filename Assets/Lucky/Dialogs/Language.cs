using System.Collections.Generic;
using Lucky.Collections;
using Lucky.Extensions;
using Lucky.Utilities;

namespace Lucky.Dialogs
{
    public class Language
    {
        public string name;
        private DefaultDict<string, List<string>> _keyToRawTexts;

        public Language(string name, string fileContent)
        {
            this.name = name;
            _keyToRawTexts = ParseAllText(fileContent);
        }

        public List<string> this[string key] => _keyToRawTexts[key];


        public static DefaultDict<string, List<string>> ParseAllText(string fileContent)
        {
            DefaultDict<string, List<string>> res = new(() => new());
            string key = "";
            // 两个有内容的行之间有多少空行(在 split 之后)
            int emptyLineCount = 0;
            foreach (string rawLine in fileContent.Split("\n"))
            {
                string line = rawLine.Trim();
                // 跳过空行
                if (line == "")
                {
                    emptyLineCount += 1;
                    continue;
                }

                // 跳过注释
                if (line.StartsWith("//"))
                    continue;


                int eqIndex = line.FindABeforeB('=', new HashSet<char>('<'));
                bool hasNewKeyValuePair = eqIndex != -1;


                if (hasNewKeyValuePair)
                {
                    string[] splitedString = line.Split('=', 2);
                    key = splitedString[0].Trim();
                    string possibleValue = splitedString[1].Trim();
                    if (possibleValue == "")
                        continue;
                    res[key].Add(possibleValue);
                }
                else
                {
                    // 说明是连着的一句话
                    if (emptyLineCount == 0)
                    {
                        // 如果为空则直接加入, 反之则拼接
                        if (res[key].Count == 0)
                            res[key].Add(line);
                        else
                            res[key].Add(res[key].Pop() + line);
                    }
                    else // 新的一句话
                        res[key].Add(line);
                }


                emptyLineCount = 0;
            }

            foreach ((string _, List<string> contents) in res)
            {
                for (var i = 0; i < contents.Count; i++)
                {
                    contents[i] = contents[i].Replace("\\n", "\n");
                }
            }

            return res;
        }

        public bool ContainsKey(string localizationKey) => _keyToRawTexts.ContainsKey(localizationKey);
    }
}