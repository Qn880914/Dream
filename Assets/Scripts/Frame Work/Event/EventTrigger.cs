using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Event
{
    public class EventTrigger : IEventTrigger
    {
        public bool log { get; set; }

        protected EventTrigger()
        {
            log = false;
        }

        public virtual void Execute(IEvent evt)
        {
        }

        public virtual void LogEvent()
        {
            if(log)
            {
                UnityEngine.Debug.Log(string.Format("[EventTrigger.LogEvent] Event Dispatched : " + this.GetType().Name));
            }
        }
    }
}
