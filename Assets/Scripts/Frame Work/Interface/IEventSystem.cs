using System;

namespace FrameWork
{
    public interface IEventSystem
    {
        void RegisterListener(string eventName, Type eventType);

        void RegisterViewListener(IView view, string[] eventNames);

        void DispatchListener(IEvent evt);

        void UnRegisterListener(string eventName);

        void UnRegisterViewListener(IView view, string[] eventNames);

        bool HasListener(string eventName);
    }
}
