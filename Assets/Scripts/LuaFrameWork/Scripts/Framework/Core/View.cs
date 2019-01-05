using FrameWork;
using FrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework.Framework
{
    public class View : Base, IView
    {
        public virtual void OnMessage(IEvent evt){ }
    }
}
