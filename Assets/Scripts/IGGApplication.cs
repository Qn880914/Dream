using FrameWork.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IGGApplication : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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
        LoaderManager.instance.OnUpdate();
    }
}
