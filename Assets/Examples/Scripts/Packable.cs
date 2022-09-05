using System;
using System.IO;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Simple abstraction to read/write json files with ease.
    /// You need to implement your own Sanitize. And you can 
    /// override the Pack/Unpack to allow compression or encryption.
    /// </summary>
    public abstract class Packable
    {
        string filePath;
        public bool LoadedFromFile { get; private set; }
        public string FullPath => GetFullPath(filePath);
        public bool FileExists => File.Exists(FullPath);
        protected Packable(string file_path) => filePath = file_path;
        public virtual string Pack()
        {
            return JsonUtility.ToJson(this);
        }
        public virtual void Unpack(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            Sanitize();
        }
        public abstract void Sanitize();
        public bool Load() => LoadedFromFile = Read(this, filePath);
        public bool Save() => Write(this, filePath);
        public static bool Write(Packable packable, string file_name)
        {
            var path = GetFullPath(file_name);
            try
            {
                File.WriteAllText(path, packable.Pack());
                return true;
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                if (File.Exists(path)) { File.Delete(path); }
            }
            return false;
        }
        public static bool Read(Packable packable, string file_name)
        {
            var path = GetFullPath(file_name);
            if (File.Exists(path))
            {
                try
                {
                    packable.Unpack(File.ReadAllText(path));
                    return true;
                }
                catch (Exception error)
                {
                    Debug.LogError(error);
                    File.Delete(path);  //TODO: should delete only if the file is malformed
                }
            }
            return false;
        }
        public static string GetFullPath(string file_name)
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, file_name);
#else
            return Path.Combine(Application.persistentDataPath, file_name);
#endif
        }
    }
}