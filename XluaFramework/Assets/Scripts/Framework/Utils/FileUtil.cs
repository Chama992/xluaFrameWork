using System;
using System.IO;
using UnityEngine;

public class FileUtil : MonoBehaviour
{

    public static bool FileExists(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        return fileInfo.Exists;
    }

    public static void WriteFile(string path,byte[] bytes)
    {
        path = PathUtil.GetStandardPath(path);
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes,0,bytes.Length);
            }
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
    }
}
