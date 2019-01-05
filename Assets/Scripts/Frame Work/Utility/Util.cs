using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace FrameWork.Utility
{
    public class Util
    {
        /// <summary>
        /// check runtime evironment
        /// </summary>
        /// <returns></returns>
        public static bool CheckEnvironment()
        {
#if UNITY_EDITOR
            int resultID = CheckRuntimeFile();
            if(-1 == resultID)
            {
                Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            else if(-2 == resultID)
            {
                Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
                EditorApplication.isPlaying = false;
                return false;
            }

            if("Test" == Application.loadedLevelName && !ConstantData.debugMode)
            {
                Debug.LogError("测试场景，必须打开调试模式，AppConst.DebugMode = true！！");
                EditorApplication.isPlaying = false;
                return false;
            }
#endif

            return true;
        }

        /// <summary>
        /// check runtime file path
        /// </summary>
        /// <returns></returns>
        public static int CheckRuntimeFile()
        {
            if (!Application.isEditor)
                return 0;

            string streamDir = Application.dataPath + "/StreamingAssets";
            if (!Directory.Exists(streamDir))
                return -1;
            else
            {
                string[] files = Directory.GetFiles(streamDir);
                if (0 == files.Length)
                    return -1;

                if (!File.Exists(streamDir + "files.txt"))
                    return -1;
            }

            string sourceDir = ConstantData.rootPath + "/ToLua/Source/Generate/";
            if (!Directory.Exists(sourceDir))
                return -2;
            else
            {
                string[] files = Directory.GetFiles(sourceDir);
                if (0 == files.Length)
                    return -2;
            }

            return 0;
        }
    }
}
