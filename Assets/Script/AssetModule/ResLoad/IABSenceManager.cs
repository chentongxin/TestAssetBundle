using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IABSenceManager
{
    IABManager abManager;

    private Dictionary<string, string> allAsset;

    public IABSenceManager(string scenename)
    {
       
    }

    /// <summary>
    /// 场景名字 SceneOne
    /// </summary>
    /// <param name="fileName"></param>
    public void ReadConfiger(string scenename)
    {
        string path = IPathTools.GetAssetBundlePath() + "/" + scenename + "Record.txt";

        allAsset = new Dictionary<string, string>();
        abManager = new IABManager(scenename);
        ReadConfig(path);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    private void ReadConfig(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        StreamReader br = new StreamReader(fs);

        string line = br.ReadLine();
        int allCount = int.Parse(line);
        for (int i= 0; i<allCount; i++)
        {
            string tmpStr = br.ReadLine();
            string[] tmpArr = tmpStr.Split(" ".ToCharArray());
            allAsset.Add(tmpArr[0], tmpArr[1]);
        }
        br.Close();
        fs.Close();
    }

    public void LoadAsset(string bundleName, LoaderProgress progress, LoadAssetBundleCallBack callBack)
    {
        if(allAsset.ContainsKey(bundleName))
        {
            string tmpValue = allAsset[bundleName];
            abManager.LoadAssetBundle(tmpValue, progress, callBack);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
    }

    #region 由下层提供功能
    public IEnumerator LoadAssetSys(string bundleName)
    {
        yield return abManager.LoadAssetBundles(bundleName);
    }

    public Object GetSingleResources(string bundleName, string resName)
    {
        if(allAsset.ContainsKey(bundleName))
        {
            return abManager.GetSingleResources(allAsset[bundleName], resName);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
        return null;
    }

    public Object[] GetMultiResources(string bundleName, string resName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            return abManager.GetMultiResources(allAsset[bundleName], resName);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
        return null;
    }

    //释放单个资源
    public void DisposeResObj(string bundleName, string resName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            abManager.DisposeResObj(allAsset[bundleName], resName);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
    }

    public void DisposeBundleRes(string bundleName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            abManager.DisposeResObj(allAsset[bundleName]);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
    }

    public void DisposeAllRes()
    {
        abManager.DisposAllObj();
    }

    public void DisposeBundle(string bundleName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            abManager.DisposeBundle(allAsset[bundleName]);
        }
        else
        {
            Debug.Log("Dont contain the bundle == " + bundleName);
        }
    }

    public void DisposeAllBundle()
    {
        abManager.DisposAllBundle();
        allAsset.Clear();
    }

    public void DisposeAllBundleAndRes()
    {
        abManager.DisposeAllBundleAndRes();
        allAsset.Clear();
    }

    public void DebugAllAsset()
    {
        List<string> keys = new List<string>();
        keys.AddRange(allAsset.Keys);

        for(int i=0; i<keys.Count; i++)
        {
            abManager.DebugAssetBundle(allAsset[keys[i]]);
        }
    }

    public bool IsLoadingFinish(string bundleName)
    {
        if(allAsset.ContainsKey(bundleName))
        {
            return abManager.IsLoadingFinish(allAsset[bundleName]);
        }
        return false;
    }

    public bool IsLoadingAssetBundle(string bundleName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            return abManager.IsLoadingAssetBundle(allAsset[bundleName]);
        }
        return false;
    }

    public string GetBundleRelateName(string bundleName)
    {
        if (allAsset.ContainsKey(bundleName))
        {
            return allAsset[bundleName];
        }
        return null;
    }
    #endregion
}
