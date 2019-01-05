using FrameWork.Utility;
using UnityEngine.Events;

namespace FrameWork.Manager
{
    public class ResourceManager : Singleton<ResourceManager>, IManage
    {
        public void LoadScene(string name, UnityAction<float> progressCallback, UnityAction<object> completeCallback, bool async)
        {
            LoaderManager.instance.LoadScene(name, progressCallback, completeCallback, async);
        }
    }
}
