using FrameWork.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Event
{
    public class LoadSceneEvent : GameEvent
    {
        public SceneType type;

        public UnityAction<object> callback;
    }

}