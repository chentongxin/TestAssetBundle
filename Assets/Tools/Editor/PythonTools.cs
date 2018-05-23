using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class PythonTools {
    [MenuItem("Itools/python")]
    public static void testPython()
    {
        string frontPath = Application.dataPath;

        int tmpPos = frontPath.IndexOf("Assets");

        string rootPath = frontPath.Substring(0, tmpPos);
        Process proc = Process.Start(rootPath + "Itools");
    }
}
