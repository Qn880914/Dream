using FrameWork.StateMachine;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineTest : MonoBehaviour {

    private Player m_Player;

    [SerializeField]
    private Button m_ButtonIdle;

    [SerializeField]
    private Button m_ButtonMove;

	// Use this for initialization
	void Start () {
        m_Player = new Player();
        m_Player.Init();
        m_Player.OnIdle();
        m_ButtonIdle.onClick.AddListener(OnClickButtonIdle);
        m_ButtonMove.onClick.AddListener(OnClickButtonMove);
    }
	
	// Update is called once per frame
	void Update () {
        m_Player.OnUpdate(Time.deltaTime);
    }

    private void OnClickButtonIdle()
    {
        m_Player.OnIdle();
    }

    private void OnClickButtonMove()
    {
        m_Player.OnMove();
    }
}
