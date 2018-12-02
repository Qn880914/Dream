using FrameWork.Resource;
using FrameWork.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork.Manager
{
    public class LoaderManager : Singleton<LoaderManager>
    {
        public delegate void LoadProgressCallback(float rage);

        private LoadTask m_LoadTask;

        private AssetBundleManifest m_AssetBundleManifest;

        private AssetBundleMapping m_AssetBundleMapping;

        /// 安装包附带的资源
        private Dictionary<string, string> m_DicAssetBundlePathMapMD5Origin = new Dictionary<string, string>();

        /// 下载的资源
        private Dictionary<string, string> m_DicAssetBundlePathMapMD5Download = new Dictionary<string, string>();

        /// 最后清除缓存时间
        private float m_LastClearCacheTime;

        /// 加载资源 搜寻路径
        private List<string> m_PathSearches = new List<string>();

        /// 所有的 Assetbundle 名字列表         AssetBundleManifest.GetAllAssetBundles();
        private List<string> m_AssetBundleNames = new List<string>();

        /// AssetBundle 缓存队列
        private Dictionary<string, AssetBundleCache> m_AssetBundleCaches = new Dictionary<string, AssetBundleCache>();

        public LoaderManager()
        {
            if (ConstantData.enableDecompressAsset)
                AddSearchPath(ConstantData.downloadResourceSavePath);

            AddSearchPath(ConstantData.decompressPath);

            m_LoadTask = new LoadTask();

            Clear();

            if (ConstantData.enableAssetBundle)
                LoadVersion();
        }

        public void Update()
        {
            m_LoadTask.OnUpdate();

            UpdateAssetBundleCache();
        }

        private void UpdateAssetBundleCache()
        {
            if (0 == ConstantData.assetBundleCacheTime || Time.realtimeSinceStartup - m_LastClearCacheTime < ConstantData.assetBundleCacheTime)
                return;

            m_LastClearCacheTime = Time.realtimeSinceStartup;

            ClearAssetBundleCache(true, true, false);
        }

        private void LoadVersion()
        {
            if (ConstantData.enableMD5Name)
            {
                string pathPatch = string.Format("{0}/version_patch", Application.persistentDataPath);

                bool hasPatch = false;
                if (ConstantData.enableCheckUpdate)
                    hasPatch = File.Exists(pathPatch);

                LoadStream("version", (data) =>
                {
                    byte[] bytes = data as byte[];
                    if (null == bytes)
                    {
                        UnityEngine.Debug.Log("[LoaderManager.LoadVersion] Load version Failed!");
                        return;
                    }

                    string text = System.Text.Encoding.UTF8.GetString(bytes);
                    JSONClass json = JSONNode.Parse(text) as JSONClass;

                    SetVersionData(json["list"] as JSONClass, !hasPatch);
                }, false);

                if(hasPatch)
                {
                    LoadStream(pathPatch, (data)=>
                    {
                        byte[] bytes = data as byte[];
                        if(null == bytes)
                        {
                            UnityEngine.Debug.Log("[LoaderManager.LoadVersion] Load Patch version Failed!");
                            return;
                        }

                        string text = System.Text.Encoding.UTF8.GetString(bytes);
                        JSONClass json = JSONNode.Parse(text) as JSONClass;

                        if (string.Equals(json["version"], ConstantData.version))
                            json = json["list"] as JSONClass;

                        SetPatchData(json);
                    }, false, false, true);
                }
            }
            else
                LoadAssetBundleManifest();
        }

        private void SetVersionData(JSONClass json, bool start = true)
        {
            Clear();

            m_DicAssetBundlePathMapMD5Origin.Clear();

            if(null != json)
            {
                foreach (KeyValuePair<string, JSONNode> item in json)
                    m_DicAssetBundlePathMapMD5Origin.Add(item.Key, item.Value["md5"]);
            }

            if (start)
                LoadAssetBundleManifest();
        }

        private void SetPatchData(JSONClass json, bool clear = false)
        {
            if (clear)
                Clear();

            m_DicAssetBundlePathMapMD5Download.Clear();

            if(null != json)
            {
                foreach (KeyValuePair<string, JSONNode> item in json)
                    m_DicAssetBundlePathMapMD5Download.Add(item.Key, item.Value["md5"]);
            }

            LoadAssetBundleManifest();
        }

        private void LoadAssetBundleManifest()
        {
            if(null != m_AssetBundleManifest)
            {
                Object.DestroyImmediate(m_AssetBundleManifest, true);
                m_AssetBundleManifest = null;
            }

            m_AssetBundleNames.Clear();
            m_AssetBundleMapping = null;

            LoadAssetBundle(ConstantData.assetBundleManifestName, (data)=> 
            {
                AssetBundleCache cache = data as AssetBundleCache;
                if (null != cache)
                    m_AssetBundleManifest = cache.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                UnloadAssetBundle(ConstantData.assetBundleManifestName, true);

                if(null != m_AssetBundleManifest)
                {
                    string[] bundleNames = m_AssetBundleManifest.GetAllAssetBundles();
                    int count = bundleNames.Length;
                    for(int i = 0; i< count; ++ i)
                    {
                        string name = bundleNames[i];
                        if (m_AssetBundleNames.Contains(name))
                        {
                            UnityEngine.Debug.Log(string.Format("[LoaderManager.LoadAssetBundleManifest] Error With Repeat Name : {0}", name));
                        }
                        m_AssetBundleNames.Add(name);
                    }
                }
            }, false, false, false);

            LoadAssetFromAssetBundle(ConstantData.assetBundleMappingName, ConstantData.assetBundleMappingName, typeof(AssetBundleMapping), (data)=>
            {
                m_AssetBundleMapping = data as AssetBundleMapping;
                if (null != m_AssetBundleMapping)
                    m_AssetBundleMapping.Init();
            }, false, false, true);

            LoadAssetBundle("shader", null, false, true);

            WarmupShader();
        }

        private void WarmupShader()
        {
            if (!ConstantData.enableAssetBundle)
                return;

            LoadAssetBundle("shader", (data)=>
            {
                AssetBundle assetbundle = data as AssetBundle;
                if (null == assetbundle)
                    return;

                ShaderVariantCollection variant = assetbundle.LoadAsset<ShaderVariantCollection>("warmshader");
                if (null != variant)
                    variant.WarmUp();
            });
        }

        private void RefreshShader(AssetBundle assetbundle)
        {
            if (assetbundle.isStreamedSceneAssetBundle)
                return;

            Material[] materials = assetbundle.LoadAllAssets<Material>();

            int count = materials.Length;
            for(int i = 0; i < count; ++ i)
            {
                Material material = materials[i];
                string shaderName = material.shader.name;
                Shader newShader = Shader.Find(shaderName);

                if (null != newShader)
                    material.shader = newShader;
            }
        }

        private string GetResourceName(string name)
        {
            string path = name;
            if(ConstantData.enableMD5Name)
            {
                string md5 = string.Empty;
                if(m_DicAssetBundlePathMapMD5Download.TryGetValue(name, out md5))
                {
                    path = SearchPath(md5, true, true, true);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }

                if (m_DicAssetBundlePathMapMD5Origin.TryGetValue(name, out md5))
                    path = md5;
                else
                    UnityEngine.Debug.Log(string.Format("[LoaderManager.GetResourceName] Get Md5 Failed : {0}", name));
            }

            return SearchPath(path, false, true, true);
        }

        private void AddSearchPath(string path)
        {
            if (m_PathSearches.Contains(path))
                m_PathSearches.Add(path);
        }

        private string SearchPath(string subPath, bool noStreamAssetPath = false, bool needSuffix = false, bool isAssetBundle = true)
        {
            if (needSuffix)
                subPath = string.Format("{0}{1}", subPath, ConstantData.abExtend);

            string fullPath = string.Empty;
            for(int i = 0; i < m_PathSearches.Count; ++ i)
            {
                fullPath = string.Format("{0}/{1}", m_PathSearches[i], subPath);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            if (noStreamAssetPath)
                return string.Empty;

            if (isAssetBundle)
                return string.Format("{0}/{1}", ConstantData.assetBundleAbsolutePath, subPath);

            return string.Format("{0}/{1}", Application.streamingAssetsPath, subPath);
        }

        /// <summary>
        /// 加载 assetbundle
        ///     先从persistentData 读取
        ///     没有找到则从 streamingAssets 读取
        ///     Note ： 
        ///         带后缀
        /// </summary>
        /// <param name="path"></param>
        /// <param name="completeCallback"></param>
        /// <param name="async"></param>
        /// <param name="persistent"></param>
        /// <param name="manifest"></param>
        private void LoadAssetBundle(string path, LoadAction<object> completeCallback, bool async = true, bool persistent = false, bool manifest = true)
        {
            path = path.ToLower();
            if(manifest)
            {
                if(!CheckAssetBundle(path))
                {
                    UnityEngine.Debug.Log(string.Format("[LoaderManager.LoadAssetBundle] assetbundle not exist : {0}", path));
                    if(null != completeCallback)
                    {
                        if (!async)
                            completeCallback(null);
                        else
                            m_LoadTask.AddAsyncCallback(completeCallback, null);
                    }

                    return;
                }

                bool asyncInfact;
                LoadAssetBundleDependencies(path, async, persistent, out asyncInfact);
                async = asyncInfact;
            }

            if (CheckAssetBundleCache(path, completeCallback, persistent, async))
                return;

            if (m_LoadTask.CheckImmediateLoad(path, completeCallback))
                return;

            m_LoadTask.AddLoadTask(LoaderType.BundleAsset, path, null, (data)=>
            {
                AssetBundle assetbundle = data as AssetBundle;
                AssetBundleCache cache = null;

                if(null != assetbundle)
                {
                    RefreshShader(assetbundle);

                    int refCount = m_LoadTask.GetWaitLoadingCount(path) + 1;
                    cache = AddAssetBundleCache(path, assetbundle, persistent, refCount);
                }

                if (null != completeCallback)
                    completeCallback(cache);

                m_LoadTask.WaitLoadingFinish(path, cache);
            }, async);
        }

        /*private void UnloadAssetBundle(string path, bool immediate = false)
        {
            path = path.ToLower();

            if (RemoveAssetBundleCache(path, immediate))
                UnloadDependencies(path, immediate);
        }*/

        /// <summary>
        /// Load all dependent AssetBundles for the given AssetBundle
        /// </summary>
        /// <param name="name"> given AssetBundle </param>
        /// <param name="async"></param>
        /// <param name="persistent"></param>
        /// <param name="asyncInFact"></param>
        private void LoadAssetBundleDependencies(string name, bool async, bool persistent, out bool asyncInFact)
        {
            asyncInFact = async;
            if (null == m_AssetBundleManifest)
                return;

            string[] dependencies = m_AssetBundleManifest.GetDirectDependencies(name);
            int count = dependencies.Length;
            for(int i = 0; i < count; ++ i)
            {
                LoadAssetBundle(dependencies[i], null, async, persistent);
                if (async)
                    continue;

                //同步加载，只要有一个被依赖ab没有加载完成，说明它正在加载，那么依赖方自己也变成异步加载，这种情况应该及其少见
                if (m_AssetBundleCaches.ContainsKey(dependencies[i]))
                    asyncInFact = false;
            }
        }

        /// <summary>
        ///  从 assetbundle 中加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="completeCallback"></param>
        /// <param name="async"></param>
        /// <param name="persistent"></param>
        /// <param name="unload"></param>
        private void LoadAssetFromAssetBundle(string path, string name, System.Type type, LoadAction<object> completeCallback, bool async, bool persistent, bool unload = false)
        {
            string fullpath = path;
            if (!path.EndsWith(ConstantData.abExtend))
                fullpath = string.Format("{0}{1}", path, ConstantData.abExtend);

            fullpath = fullpath.ToLower();

            LoadAssetBundle(fullpath, (data)=>
            {
                AssetBundleCache cache = data as AssetBundleCache;
                Object asset = null;
                if(null != cache)
                {
                    if (!string.IsNullOrEmpty(name))
                        asset = cache.LoadAsset(name, type);
                }

                if (null == asset)
                    UnityEngine.Debug.Log(string.Format("[LoaderManager.LoadAssetFromAssetBundle] Not Found Asset is null with -> Path : {0}, Name : {1}", path, name));

                if (null != completeCallback)
                    completeCallback(asset);

                if (unload)
                    UnloadAssetBundle(path);
            }, async, persistent);
        }

        private void LoadStream(string path, LoadAction<object> completeCallback, bool async = true, bool remote = false, bool isFullPath = false)
        {
            string fullpath = path;
            if (!remote && !isFullPath)
                fullpath = SearchPath(path, false, false, false);
            else
                async = true;

            m_LoadTask.AddLoadTask(LoaderType.Stream, fullpath, remote, completeCallback, async);
        }

        private AssetBundle GetAssetBundle(string path)
        {
            string fullpath = path;
            if (!path.EndsWith(ConstantData.abExtend))
                fullpath = string.Format("{0}{1}", path, ConstantData.abExtend);

            fullpath = fullpath.ToLower();

            AssetBundleCache cache = null;
            if (!m_AssetBundleCaches.TryGetValue(fullpath, out cache))
                return null;

            return cache.assetBundle;
        }

        /// <summary>
        /// 卸载 assetbundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="immediate"></param>
        private void UnloadAssetBundle(string path, bool immediate = false)
        {
            path = path.ToLower();

            if(RemoveAssetBundleCache(path, immediate))
                UnloadDependencies(path, immediate);
        }

        private AssetBundleCache AddAssetBundleCache(string name, AssetBundle assetbundle, bool persistent, int refCount)
        {
            AssetBundleCache cache;
            if(!m_AssetBundleCaches.TryGetValue(name, out cache))
            {
                cache = new AssetBundleCache(name, assetbundle, persistent, refCount);
                m_AssetBundleCaches.Add(name, cache);
            }

            return cache;
        }

        private bool RemoveAssetBundleCache(string name, bool immediate = false)
        {
            AssetBundleCache cache;
            if (!m_AssetBundleCaches.TryGetValue(name, out cache))
                return false;

            --cache.refCount;

            if ((0 != ConstantData.assetBundleCacheTime || immediate) && cache.canRemove)
                UnloadAssetBundleCache(name);

            return true;
        }

        /// <summary>
        /// 检查 是否已在缓存
        ///     是：引用计数 +1 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compeleteCallback"></param>
        /// <param name="persistent"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        private bool CheckAssetBundleCache(string name, LoadAction<object> compeleteCallback, bool persistent, bool async)
        {
            AssetBundleCache cache = GetAssetBundleCache(name, persistent);
            if (null == cache)
                return false;

            if(null != compeleteCallback)
            {
                if (!async)
                    compeleteCallback(cache);
                else
                    m_LoadTask.AddAsyncCallback(compeleteCallback, cache);
            }

            return true;
        }

        /*private bool RemoveAssetBundleCache(string name, bool immediate = false)
        {
            AssetBundleCache cache = null;
            if (!m_AssetBundleCaches.TryGetValue(name, out cache))
                return false;

            --cache.refCount;

            if ((0 != ConstantData.assetBundleCacheTime || immediate) && cache.canRemove)
                UnloadAssetBundleCache(name);

            return true;
        }*/

        private bool CheckAssetBundle(string path)
        {
            path = path.ToLower();
            return (0 == m_AssetBundleNames.Count) || m_AssetBundleNames.Contains(path) || string.Equals(path, ConstantData.assetBundleManifestName);
        }

        private AssetBundleCache GetAssetBundleCache(string name, bool persistent)
        {
            AssetBundleCache cache;
            if (!m_AssetBundleCaches.TryGetValue(name, out cache))
                return null;

            ++cache.refCount;

            if (persistent)
                cache.persistent = true;

            return cache;
        }

        private void UnloadAssetBundleCache(string key, bool unloadUnusedAsset = false)
        {
            AssetBundleCache cache;
            if (!m_AssetBundleCaches.TryGetValue(key, out cache))
                return;

            cache.UnLoad(unloadUnusedAsset);
            m_AssetBundleCaches.Remove(key);
        }

        private void UnloadDependencies(string name, bool immediate = false)
        {
            if (null == m_AssetBundleManifest)
                return;

            string[] dependencies = m_AssetBundleManifest.GetDirectDependencies(name);

            int count = dependencies.Length;
            for (int i = 0; i < count; ++i)
                UnloadAssetBundle(dependencies[i], immediate);
        }

        private void ClearAssetBundleCache(bool onlyRefCountIsZero = true, bool timeOut = true, bool includePersistent = false)
        {
            AssetBundleCache abCache = null;
            int count = m_AssetBundleCaches.Count;
            string[] keys = new string[count];
            m_AssetBundleCaches.Keys.CopyTo(keys, 0);
            string key;
            for(int i = 0; i < count; ++ i)
            {
                key = keys[i];
                abCache = m_AssetBundleCaches[key];

                if(onlyRefCountIsZero)
                {
                    if (abCache.canRemove && (!timeOut || abCache.isTimeOut))
                        UnloadAssetBundleCache(key);
                }
                else if(includePersistent || abCache.canRemove)
                {
                    UnloadAssetBundleCache(key);
                }
            }

        }

        private void Clear()
        {
            m_LoadTask.Clear();

            ClearAssetBundleCache(true, false, false);

            Resources.UnloadUnusedAssets();

            System.GC.Collect();
        }

    }
}

