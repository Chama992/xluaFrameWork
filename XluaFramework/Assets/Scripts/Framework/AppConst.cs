
using System.IO;

public enum GameMode
{
    EditorMode,
    PackBundle,
    UpdateMode
}

public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";
    public static GameMode GameMode = GameMode.EditorMode;
    public const string ResourceUrl = "http://127.0.0.1/AssetBundles";
}
