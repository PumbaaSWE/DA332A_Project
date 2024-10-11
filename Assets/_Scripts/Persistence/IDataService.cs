using System.Collections.Generic;

public interface IDataService
{
    public void Save<T>(string fileName, T data, bool overwrite = true);
    public T Load<T>(string fileName);
    public void Delete(string fileName);
    public bool Exists(string fileName);
    IEnumerable<string> ListAllSaveFiles();
}

