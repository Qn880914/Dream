namespace FrameWork.Event
{
    public class Event : IEvent
    {
        private string m_Name;
        public string name { get { return m_Name; } }

        private string m_Type;
        public string type { get { return m_Type; } set { m_Type = value; } }

        private object m_Data;
        public object data { get { return m_Data; } set { m_Data = value; } }

        public Event(string name)
            : this(name, null)
        { }

        public Event(string name, object data)
        {
            m_Name = name;
            m_Data = data;
        }

        public override string ToString()
        {
            string msg = "Notification Name: " + name;
            msg += "\nData:" + ((data == null) ? "null" : data.ToString());
            msg += "\nType:" + ((type == null) ? "null" : type);
            return msg;
        }
    }
}
