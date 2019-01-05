using FrameWork.Event;

namespace FrameWork
{
    public interface IView
    {
        void OnMessage(IEvent evt);
    }
}
