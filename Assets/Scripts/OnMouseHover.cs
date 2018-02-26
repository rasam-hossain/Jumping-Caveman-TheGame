using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseHover : MonoBehaviour {


    private bool mouseHover = false;
    private GameObject debuggerHelper;

    void Start()
    {
        debuggerHelper = GameObject.Find("DebuggerHelper");
    }
    public void OnMouseEnter()
    {
        mouseHover = true;
        debuggerHelper.GetComponent<Image>().enabled = true;
        Debug.Log(mouseHover);
    }
    public void OnMouseExit()
    {
        mouseHover = false;
        debuggerHelper.GetComponent<Image>().enabled = false;
        Debug.Log(mouseHover);
    }
}
