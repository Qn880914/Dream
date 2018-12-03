using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.StateMachine;
using FrameWork.Manager;

namespace FrameWork.Scene
{
    public abstract class SceneBase : State<SceneType, SceneManager>
    {}
}
