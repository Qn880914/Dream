using FrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Event
{
    public class StartUpEventTrigger : EventTrigger
    {
        public override void Execute(IEvent evt)
        {
            if (!Util.CheckEnvironment())
                return;

            GameObject gameMgr = GameObject.Find("GlobalGenerator");
            if (null != gameMgr)
                gameMgr.AddComponent<AppView>();
        }
    }
}
