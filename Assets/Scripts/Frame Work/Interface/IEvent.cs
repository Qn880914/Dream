namespace FrameWork
{
    public interface IEvent
    {
        string name { get; }

        object data { get; set;}

        string type { get; set; }

        string ToString();
    }
}
