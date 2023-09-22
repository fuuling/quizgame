using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextQuestion : MonoBehaviour
{
    private bool firstPush = false;
    // Start is called before the first frame update
    void Start()
    {
        if (inputquestion.clearcheck == false)
        {
            Text AnsDisp = GameObject.Find("Canvas/AnsDisplay").GetComponentInChildren<Text>();
            AnsDisp.text = "正解:" + inputquestion.answer2;
        }

        if(inputquestion.qnum == (startbutton.qvalue - 1))
        {
            Button risbutton = GameObject.Find("Canvas/nextButton").GetComponentInChildren<Button>();
            Text result = risbutton.GetComponentInChildren<Text>();
            result.text = "結果へ";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            buttonpush();
        }
    }

    public void pressnext()
    {
        if (inputquestion.qnum == (startbutton.qvalue - 1))
        {
            SceneManager.LoadScene("resultScene");
        }
        else if (!firstPush)
        {
            SceneManager.LoadScene("GameScene");
            firstPush = true;
        }
    }

    private void buttonpush()
    {
        Button pushbutton = GameObject.Find("Canvas/nextButton").GetComponentInChildren<Button>();
        // UIボタンの押下
        pushbutton.onClick.Invoke();
    }
}
