
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataService : IDataService
{
    
    private readonly ISerializer serializer;
    private readonly string dataPath;
    private readonly string fileExtention;

    public FileDataService(ISerializer serializer, string fileExtention)
    {
        this.serializer = serializer;
        dataPath = Application.persistentDataPath;
        this.fileExtention = fileExtention;

    }

    private string GetPath(string fileName)
    {
        return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtention));
    }
    public void Save<T>(string name, T data, bool overwrite = true)
    {
        string filePath = GetPath(name);
        
        if(!overwrite && File.Exists(filePath))
        {
            throw new IOException($"File {filePath} exists and cannot be overwritten");
        }

        File.WriteAllText(filePath, serializer.Serialize(data));
    }

    public T Load<T>(string name)
    {
        string filePath = GetPath(name);
        if (!File.Exists(filePath))
        {
            throw new IOException($"File {filePath} does not exists");
        }
        return serializer.Deserialize<T>(File.ReadAllText(filePath));
    }

    
    public void Delete(string name)
    {
        string filePath = GetPath(name);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

    }

    public IEnumerable<string> ListAllSaveFiles()
    {
        foreach (string path in Directory.EnumerateFiles(dataPath))
        {
            if (Path.GetExtension(path) == fileExtention)
            {
                yield return Path.GetFileNameWithoutExtension(path);
            }
        }
    }

    public bool Exists(string name)
    {
        return File.Exists(GetPath(name));
    }
}

