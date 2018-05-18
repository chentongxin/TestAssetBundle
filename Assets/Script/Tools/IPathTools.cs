using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IPathTools
{
    public static string GetPlatFormFolderName(RuntimePlatform platform)
    {
        switch(platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
    }

    static string GetAppFilePath()
    {
        string tmpPath = "";
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            tmpPath = Application.streamingAssetsPath;
        }
        else
        {
            tmpPath = Application.persistentDataPath;
        }
        return tmpPath;
    }

    public static string GetAssetBundlePath()
    {
        string platFolder = GetPlatFormFolderName(Application.platform);
        string allPath = GetAppFilePath() + "/" + platFolder; //Path.Combine(GetAppFilePath(), platFolder);
        return allPath;
    }

    public static string GetWWWAssetBundlePath()
    {
        string tmpStr = "";
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            tmpStr = "file:///" + GetAssetBundlePath();
        }
        else
        {
            string tmpPath = GetAssetBundlePath();
#if UNITY_ANDROID
            tmpStr = "jar:file://" + tmpPath;
#elif UNITY_STANDALONE_WIN
            tmpStr = "file:///" + tmpPath;
#else
            tmpStr = "file://" + tmpPath;
#endif
        }

        return tmpStr;
    }
}
