using FrameWork.Event;
using FrameWork.Manager;
using FrameWork.StateMachine;

namespace FrameWork.Scene
{
    public class LoginScene : SceneBase
    {
        private StateMachine<LoginSceneType, LoginScene> m_StateMachine = new StateMachine<LoginSceneType, LoginScene>();

        public LoginScene()
        {
            RegisterState();
        }

        private void RegisterState()
        {
            m_StateMachine.AddState(LoginSceneType.Loading, new LoginSceneLoading());
            m_StateMachine.AddState(LoginSceneType.Update, new LoginSceneUpdate());
            m_StateMachine.AddState(LoginSceneType.Decompress, new LoginSceneDecompress());
            m_StateMachine.AddState(LoginSceneType.Normal, new LoginSceneNormal());
        }

        private void UnRegisterState()
        {
            m_StateMachine.RemoveState(LoginSceneType.Loading);
            m_StateMachine.RemoveState(LoginSceneType.Update);
            m_StateMachine.RemoveState(LoginSceneType.Decompress);
            m_StateMachine.RemoveState(LoginSceneType.Normal);
        }

        public override void OnEnter()
        {
            ResourceManager.instance.LoadScene("Login", null, (data)=> 
            {
                m_StateMachine.SetState(LoginSceneType.Normal);
            }, false);
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            //throw new System.NotImplementedException();
        }
    }

    public abstract class LoginSceneState : State<LoginSceneType, LoginScene>
    { }
}

