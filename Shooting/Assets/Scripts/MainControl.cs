using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour
{
    static public int score = 0;
    static public int life = 3;
    public GUISkin myskin;

    private void OnGUI()
    {
        GUI.skin = myskin;
        Rect labelRect1 = new Rect(10.0f, 10.0f, 400.0f, 100.0f); // 위치 x, 위치 y, 폭, 높이
        GUI.Label(labelRect1, "SCORE :" + MainControl.score);
        Rect labelRect2 = new Rect(10.0f, 110.0f, 400.0f, 100.0f); // 위치 x, 위치 y, 폭, 높이
        GUI.Label(labelRect2, "LIFE :" + MainControl.life);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
