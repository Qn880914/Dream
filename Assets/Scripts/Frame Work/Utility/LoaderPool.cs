using FrameWork.Resource;

namespace FrameWork.Utility
{
    internal static class LoaderPool
    {
        private static readonly ObjectPool<StreamLoader> s_StreamLoaderPool = new ObjectPool<StreamLoader>(null, loader => loader.Reset());
        private static readonly ObjectPool<AssetBundleLoader> s_AssetBundleLoaderPool = new ObjectPool<AssetBundleLoader>(null, loader=>loader.Reset());
        private static readonly ObjectPool<AssetLoader> s_AssetLoaderPool = new ObjectPool<AssetLoader>(null, loader=>loader.Reset());
        private static readonly ObjectPool<ResourceLoader> s_ResourceLoaderPool = new ObjectPool<ResourceLoader>(null, loader=>loader.Reset());
        private static readonly ObjectPool<SceneLoader> s_SceneLoaderPool = new ObjectPool<SceneLoader>(null, loader=>loader.Reset());

        public static Loader Get(LoaderType type)
        {
            switch(type)
            {
                case LoaderType.Stream:
                    return s_StreamLoaderPool.Get();
                case LoaderType.Bundle:
                    return s_AssetBundleLoaderPool.Get();
                case LoaderType.Asset:
                    return s_AssetLoaderPool.Get();
                case LoaderType.Resouces:
                    return s_ResourceLoaderPool.Get();
                case LoaderType.Scene:
                    return s_SceneLoaderPool.Get();
            }

            return null;
        }

        public static void Release(Loader element)
        {
            switch(element.type)
            {
                case LoaderType.Stream:
                    s_StreamLoaderPool.Release(element as StreamLoader);
                    break;
                case LoaderType.Bundle:
                    s_AssetBundleLoaderPool.Release(element as AssetBundleLoader);
                    break;
                case LoaderType.Asset:
                    s_AssetLoaderPool.Release(element as AssetLoader);
                    break;
                case LoaderType.Resouces:
                    s_ResourceLoaderPool.Release(element as ResourceLoader);
                    break;
                case LoaderType.Scene:
                    s_SceneLoaderPool.Release(element as SceneLoader);
                    break;
            }
        }
    }
}

