using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Lucky.IO
{
    public class SaveLoadManager
    {
        public static SaveLoadManager Instance;

        private readonly string SavePath = Path.Join(Application.persistentDataPath, "Saves");

        public bool Save<T>(string fileName, T data)
        {
            MakeSureSavePathExists();
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            });

            string filePath = Path.Join(SavePath, $"{fileName}.json");
            File.WriteAllText(filePath, json);

            Debug.Log($"数据保存成功: {fileName}.json");
            return true;
        }

        public T Load<T>(string fileName, T defaultValue)
        {
            MakeSureSavePathExists();
            string filePath = Path.Join(SavePath, $"{fileName}.json");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"⚠️ 存档不存在: {fileName}，返回默认值");
                return defaultValue;
            }

            string json = File.ReadAllText(filePath);
            T data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            Debug.Log($"数据加载成功: {fileName}.json");
            return data;
        }

        private void MakeSureSavePathExists()
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
        }
    }
}