using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    public BoidsGroup boidsGroup;

    private Toggle toggle;
    private Text text;

    private float lastUpdateTime;
    private float freshFreq = 0.2f;
    private int frameCnt;
    private float fps;

    private void Start()
    {
        lastUpdateTime = Time.realtimeSinceStartup;
        toggle = transform.Find("Toggle").GetComponent<Toggle>();
        text = transform.Find("FPS").GetComponent<Text>();

        toggle.onValueChanged.AddListener(OnToggleChange);
    }

    private void Update()
    {
        frameCnt++;
        float deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
        if (deltaTime > freshFreq)
        {
            fps = frameCnt / deltaTime;
            frameCnt = 0;
            lastUpdateTime = Time.realtimeSinceStartup;
            text.text = "FPS:" + fps.ToString("F1");
        }
    }

    private void OnToggleChange(bool isOn)
    {
        boidsGroup.setting.useComputerShader = isOn;
    }
}
