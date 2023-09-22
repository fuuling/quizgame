using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class result : MonoBehaviour
{

    private bool firstPush = false;
    // Start is called before the first frame update
    void Start()
    {
        Text seikaiText = GameObject.Find("Canvas/seikai").GetComponentInChildren<Text>();
        Text pointText = GameObject.Find("Canvas/point").GetComponentInChildren<Text>();

        seikaiText.text = "正解数：" + startbutton.clearQ.ToString();
        pointText.text = "総ポイント：" + Math.Floor(startbutton.respoint).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            buttonpush();
        }
    }

    public void ReStart()
    {
        if (!firstPush)
        {
            SceneManager.LoadScene("titleScene");
            firstPush = true;
        }
    }

    public static void buttonpush()
    {
        // UIボタンの取得
        Button pushbutton = GameObject.Find("Canvas/retitle").GetComponentInChildren<Button>();
        // UIボタンの押下
        pushbutton.onClick.Invoke();
    }

}
