using FrameWork.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IGGApplication : MonoBehaviour
{
    private void Awake()
    {
    }

    // Use this for initialization
    void Start ()
    {
        SceneManager.instance.Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
        SceneManager.instance.OnUpdate();
    }
}
