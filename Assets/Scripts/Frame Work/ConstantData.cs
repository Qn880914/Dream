using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    internal static class ConstantData
    {
        public readonly static string abExtend = ".ab";

        public readonly static bool EnableMD5Name = false;

        public static string DownloadResourceSavePath { get { return Application.persistentDataPath + "/"; } }
    }
}
