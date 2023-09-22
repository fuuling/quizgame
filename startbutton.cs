using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class startbutton : MonoBehaviour
{
    public static int count;
    public static int clearQ;
    public static double respoint;
    public static float qvalue;
    public static int levelpoint;
    public static int voicecheck;
    public static Toggle easy;
    public static Toggle normal;
    public static Toggle hard;
    public static Toggle all;
    public static Toggle onsei;

    private bool firstPush;
    public static int currentSentenceNum;

    Slider slider;

    void Start()
    {
        count = -1;
        clearQ = 0;
        respoint = 0;
        firstPush = false;
        currentSentenceNum = 0;
        voicecheck = 0;
    }

    public void PressStart()
    {
        slider = GameObject.Find("Canvas/Slider").GetComponent<Slider>();
        qvalue = slider.value;
        onsei = GameObject.Find("Canvas/onsei").GetComponentInChildren<Toggle>();

        //１度もゲームスタートボタンが押されていない場合、画面遷移
        if (!firstPush)
        {
            easy = GameObject.Find("Canvas/easy").GetComponentInChildren<Toggle>();
            normal = GameObject.Find("Canvas/normal").GetComponentInChildren<Toggle>();
            hard = GameObject.Find("Canvas/hard").GetComponentInChildren<Toggle>();
            all = GameObject.Find("Canvas/all").GetComponentInChildren<Toggle>();

            if (easy.isOn == true)
            {
                levelpoint = 0;
            }
            if (normal.isOn == true)
            {
                levelpoint = 1;
            }
            if (hard.isOn == true)
            {
                levelpoint = 2;
            }
            if (all.isOn == true)
            {
                levelpoint = 3;
            }

            if (onsei.isOn == true)
            {
                voicecheck = 1;
            }

            SceneManager.LoadScene("GameScene");
            firstPush = true;

        }
    }

    public static int getqCount()
    {
        count += 1;

        return count;
    }
}
