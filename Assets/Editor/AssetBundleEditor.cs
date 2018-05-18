using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditor : MonoBehaviour {

    [MenuItem("Itools/BuildAssetBundle")]
    public static  void BuildAssetBundle()
    {
        //streamingassetpath /windows
        string outPath = IPathTools.GetAssetBundlePath();//Application.streamingAssetsPath + "/AssetBundle";

        BuildPipeline.BuildAssetBundles(outPath,0,EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    [MenuItem("Itools/MarkAssetBundle")]
    public static void MarkAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames(); 

        string path = Application.dataPath + "/Art/Scenes";
        DirectoryInfo dir = new DirectoryInfo(path);

        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();

        for(int i=0; i<fileInfo.Length; i++)
        {
            FileSystemInfo tmpFile = fileInfo[i];
            if(tmpFile is DirectoryInfo)
            {
                string tmpPath = Path.Combine(path, tmpFile.Name);
                tmpPath = FixedPath(tmpPath);
                SceneOverView(tmpPath);
            }
        }
        string outPath = IPathTools.GetAssetBundlePath();
        CopyRecord(path, outPath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 将搜集到的assetBundl信息记录到文件 
    /// </summary>
    /// <param name="filePath">要写入的文件</param>
    /// <param name="content">要写入的内容</param>
    public static void WriteToFile(string filePath, Dictionary<string, string> content)
    {
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);
        bw.WriteLine(content.Count);
        foreach (var key in content.Keys)
        {
            bw.Write(key);
            bw.Write(" ");
            bw.Write(content[key]);
            bw.Write("\n");
        }
        bw.Close();
        fs.Close();
    }

    /// <summary>
    /// 处理每个场景文件
    /// </summary>
    /// <param name="scenePath">Application.dataPath + "/Art/AssetBundle/Scenes/SceneOne"</param>
    public static void SceneOverView(string scenePath)
    {
        //存储对应关系
        Dictionary<string, string> readDict = new Dictionary<string, string>();

        ChangerHead(scenePath, readDict);

        string textFileName = "Record.txt";
        string tmpPath = scenePath + textFileName;
        WriteToFile(tmpPath, readDict);
    }

    //截取相对路径
    public static void ChangerHead(string fullPath, Dictionary<string, string> theWriter)
    {
        int tmpCount = fullPath.IndexOf("Assets");
        int tmpLenth = fullPath.Length;
        string replacePath = fullPath.Substring(tmpCount, tmpLenth - tmpCount);

        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if(dir != null)
        {
            ListFiles(dir, replacePath, theWriter);
        }
        else
        {
            Debug.Log("this path is not exist");
        }
    }

    public static void ListFiles(FileSystemInfo info,string replacePath, Dictionary<string, string> theWriter)
    {
        if(!info.Exists)
        {
            Debug.Log("is not exist");
            return;
        }
        DirectoryInfo dir = info as DirectoryInfo;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for(int i=0; i<files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if(file != null )
            {
                ChangeMark(file, replacePath, theWriter);
            }
            else
            {
                ListFiles(files[i], replacePath, theWriter);
            }
        }
    }

    public static string FixedPath(string path)
    {
        path = path.Replace("\\", "/");
        return path;
    }

    //计算mark标记值
    public static string GetBundlePath(FileInfo file, string replacePath)
    {
        string tmpPath = file.FullName;
        tmpPath = FixedPath(tmpPath);

        int assetCount = tmpPath.IndexOf(replacePath);
        assetCount += replacePath.Length + 1;
        int nameCount = tmpPath.LastIndexOf(file.Name);
        int tmpCount = replacePath.LastIndexOf("/");

        string sceneHead = replacePath.Substring(tmpCount + 1, replacePath.Length - tmpCount - 1);

        int tmpLength = nameCount - assetCount;

        if(tmpLength>0)
        {
            string subString = tmpPath.Substring(assetCount, tmpPath.Length - assetCount);
            string[] result = subString.Split("/".ToCharArray());
            return sceneHead + "/" + result[0];
        }
        else
        {
            return sceneHead;
        }
    }

    public static void  ChangeMark(FileInfo tmpFile, string replacePath, Dictionary<string, string> theWriter)
    {
        if(tmpFile.Extension == ".meta")
        {
            return;
        }

        string markStr = GetBundlePath(tmpFile, replacePath);
        Debug.Log("ChangeMark markStr:" + markStr);
        ChangeAssetMark(tmpFile, markStr, theWriter);
    }

    public static void ChangeAssetMark(FileInfo tmpFile, string markStr, Dictionary<string, string> theWriter)
    {
        string fullPath = tmpFile.FullName;
        int assetCount = fullPath.IndexOf("Assets");
        string assetPath = fullPath.Substring(assetCount, fullPath.Length - assetCount);
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        importer.assetBundleName = markStr;

        if(tmpFile.Extension == ".unity")
        {
            importer.assetBundleVariant = "u3d";
        }
        else
        {
            importer.assetBundleVariant = "ld";
        }

        string modleName = "";
        string[] subMark = markStr.Split("/".ToCharArray());

        if(subMark.Length > 1)
        {
            modleName = subMark[1];
        }
        else
        {
            modleName = markStr;
        }
        string modlePath = markStr.ToLower() + "." + importer.assetBundleVariant;
        if(!theWriter.ContainsKey(modleName))
        {
            theWriter.Add(modleName, modlePath);
        }
    }


    public static void CopyRecord(string srcPath, string disPath)
    {
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        if(!dir.Exists)
        {
            Debug.Log("is not exist");
            return;
        }

        if(!Directory.Exists(disPath))
        {
            Directory.CreateDirectory(disPath);
        }

        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for(int i=0; i<files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;

            if(file !=null && file.Extension == ".txt")
            {
                string srcFile = srcPath + "/" + file.Name;
                string disFile = disPath + "/" + file.Name;
                File.Copy(srcFile, disFile, true);
            }
        }
    }
}
