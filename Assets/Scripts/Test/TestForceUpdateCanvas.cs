using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestForceUpdateCanvas : MonoBehaviour {

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private Image m_Image;

	// Use this for initialization
	void Start () {

        m_Text.text = "34234234234234";
        m_Text.RegisterDirtyVerticesCallback(() => { UnityEngine.Debug.Log("  Text Dirty Vertices Callback"); });

        m_Image.RegisterDirtyLayoutCallback(() => { UnityEngine.Debug.Log("  Image Dirty Layout Callback"); });
        m_Image.RegisterDirtyMaterialCallback(() => { UnityEngine.Debug.Log("  Image Dirty Material Callback"); });
        m_Image.RegisterDirtyVerticesCallback(() => { UnityEngine.Debug.Log("  Image Dirty Vertices Callback"); });

    }
	
	// Update is called once per frame
	void Update () {
        RectTransform rectTransform = m_Text.GetComponent<RectTransform>();
        if (null != rectTransform)
        {
            //UnityEngine.Debug.Log(rectTransform.rect);
        }
	}
}
