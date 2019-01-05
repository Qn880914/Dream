namespace FrameWork
{
    public interface IEventTrigger
    {
        void Execute(IEvent evt);

        void LogEvent();
    }
}
