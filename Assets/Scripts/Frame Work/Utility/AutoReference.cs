using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FrameWork.Utility
{
    public class AutoReference<T> : MonoBehaviour where T : AutoReference<T>
    {
#if UNITY_EDITOR
        
        protected virtual void Reset()
        {
            foreach(var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).
                Where(field => field.GetValue(this) == null))
            {
                Transform obj;
                if(transform.name.Equals(field.Name))
                {
                    obj = transform;
                }
                else
                {
                    obj = transform.Find(field.Name);
                }

                if(null == obj)
                {
                    UnityEngine.Debug.Log(string.Format("[AutoReference.Reset] : {0} Can not Find Field : {1}",
                        typeof(T).Name, field.Name));
                }
                else
                {
                    field.SetValue(this, obj.GetComponent(field.FieldType));
                }
            }
        }

        private void OnValidate()
        {
            Reset();
        }
#endif //UNITY_EDITOR
    }
}

