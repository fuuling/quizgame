using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class inputquestion : MonoBehaviour
{
    TextAsset csvFile; // CSVファイル
    TextAsset csvFile1; // CSVファイル1
    TextAsset csvFile2; // CSVファイル2
    public static List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    public static List<int> qnumber = new List<int>(); // 出題番号リスト    
    public int csvrow; // CSVファイルの行数
    public static string answer1;// 音声認識用クイズの答え
    public static string answer2;// クイズの答え1
    public static string answer3;// クイズの答え2
    public static string answer4;// クイズの答え3
    public static int qnum = 0;
    public static bool timer;//　制限時間起動
    public static bool timeout;//　解答時間終了
    public static bool clearcheck;//　問題の正解確認
    public static bool startsig;//　問題開始の合図
    public static bool voicestart;// 音声認識開始
    public static string currentSentence;
    private int k = 0; //配列の変数
    private int qransu = 0; // 出題する問題の行
    private string displayText = "";
    private int textCharNumber = 0;
    private int displayTextSpeed = 0;

    //音声認識
    private DictationRecognizer m_DictationRecognizer;


    [SerializeField] GameObject hayaoshibutton;
    [SerializeField] GameObject kaitoupanel;

    void Start()
    {

        hayaoshibutton.SetActive(true);
        kaitoupanel.SetActive(false);
        timeout = false;
        timer = false;
        clearcheck = false;
        startsig = false;
        voicestart = true;

        int qcount = startbutton.getqCount();
        if (qcount == 0) // １問目の時のみ以下処理を行う
        {
            clearlist();

            if (startbutton.levelpoint == 0)
            {
                csvFile = Resources.Load("easyquiz") as TextAsset; // Resouces下のCSV読み込み
            }
            if (startbutton.levelpoint == 1)
            {
                csvFile = Resources.Load("normalquiz") as TextAsset;
            }
            if (startbutton.levelpoint == 2)
            {
                csvFile = Resources.Load("hardquiz") as TextAsset;
            }
            if (startbutton.levelpoint == 3)
            {
                csvFile = Resources.Load("easyquiz") as TextAsset;
                csvFile1 = Resources.Load("normalquiz") as TextAsset;
                csvFile2 = Resources.Load("hardquiz") as TextAsset;
            }
            StringReader reader = new StringReader(csvFile.text);
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み
                csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
                csvrow++; // CSVファイルの行数カウント
            }

            if (startbutton.levelpoint == 3)
            {
                StringReader reader1 = new StringReader(csvFile1.text);
                StringReader reader2 = new StringReader(csvFile2.text);
                while (reader1.Peek() != -1) // reader.Peaekが-1になるまで
                {
                    string line = reader1.ReadLine(); // 一行ずつ読み込み
                    csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
                    csvrow++; // CSVファイルの行数カウント
                }
                while (reader2.Peek() != -1) // reader.Peaekが-1になるまで
                {
                    string line = reader2.ReadLine(); // 一行ずつ読み込み
                    csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
                    csvrow++; // CSVファイルの行数カウント
                }
            }
                for (int j = 1; j <= csvrow - 1; j++)
            {
                qnumber.Add(j); // CSVファイルの行をリストに追加（1行目は除外）
            }
            qnumber = qnumber.OrderBy(a => Guid.NewGuid()).ToList(); // 読み込んだリストをシャッフル 
            csvrow = 0; //変数初期化
        }
        qransu = qnumber[qcount];// シャッフルされたリストを問題数順に取得
        qnum = qcount;

        print(string.Join(",", qnumber));
        print(qransu.ToString());
        print(startbutton.voicecheck);

        csvDatas[k] = csvDatas[qransu]; // CSVの"qransu"行目の問題を取得   
        AnswerLabelSet();
        QuestionNumSet();


        m_DictationRecognizer = new DictationRecognizer();
        InputField inputField = GameObject.Find("Canvas/Panel/InputField").GetComponent<InputField>();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            inputField.text += text;
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

    }

    void Update()
    {
        Text timerText = GameObject.Find("Canvas/Timer").GetComponentInChildren<Text>();

        QuestionLabelSet();
        // エンターキー押下の認識
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (timeout == false)
            {
                hayaoshi();
            }
            else
            {
                buttonpush();
            }
        }

        if (timeout == true)
        {

            InputField inputField = GameObject.Find("Canvas/Panel/InputField").GetComponent<InputField>();
            inputField.ActivateInputField();

            if (startbutton.voicecheck == 1 && voicestart == true)
            {
                voicestart = false;
                Debug.Log("Start recognizing.");
                m_DictationRecognizer.Start();
            }
        }

        if (timerText.text == "0")
        {
            SceneManager.LoadScene("BadScene");
        }
    }

    private async void QuestionLabelSet()
    {
   
        Text qLabel = GameObject.Find("Canvas/Question").GetComponent<Text>();
        Text timerText = GameObject.Find("Canvas/Timer").GetComponentInChildren<Text>();
        Button hayaoshib = GameObject.Find("Canvas/hayaoshi").GetComponentInChildren<Button>();
        if (startsig == false)
        {
            timerText.text = "問題";

            await UniTask.Delay(1000);

            startsig = true;
            hayaoshib.interactable = true;
        }


        currentSentence = csvDatas[k][0];


        displayTextSpeed++;
        if (displayTextSpeed % 30 == 0)
        {
            if (textCharNumber != currentSentence.Length && timeout == false)
            {
                displayText = displayText + currentSentence[textCharNumber];
                textCharNumber = textCharNumber + 1;
            }

            else
            {
                timer = true;
            }
        }
        qLabel.text = displayText;

    }

    private void AnswerLabelSet()
    {
        answer2 = csvDatas[k][2];

        if(csvDatas[k][3] != null)
        {
            answer3 = csvDatas[k][3];

            if (csvDatas[k][4] != null)
            {
                answer4 = csvDatas[k][4];
            }
        }
    }

    private void QuestionNumSet()
    {
        int qCount = qnum + 1;
        Text qnumLabel = GameObject.Find("Canvas/QuestionNum").GetComponentInChildren<Text>();
        qnumLabel.text = "問題 " + qCount.ToString() + "/" + startbutton.qvalue.ToString();
    }

    private void clearlist()
    {
        csvDatas.Clear();
        qnumber.Clear();
    }

    public static void buttonpush()
    {
        // UIボタンの取得
        Button pushbutton = GameObject.Find("Canvas/Panel/AnsButton").GetComponentInChildren<Button>();
        // UIボタンの押下
        pushbutton.onClick.Invoke();
    }

    public void ClearOrBad()
    {
        InputField inputField = GameObject.Find("Canvas/Panel/InputField").GetComponent<InputField>();

        string anstext = inputField.text;
        if (startbutton.onsei.isOn == true)
        {
            m_DictationRecognizer.Stop();

            if (answer1 == anstext)
            {
                startbutton.clearQ += 1;
                startbutton.respoint = startbutton.respoint + (100.0 * (1.1 - ((float)textCharNumber / (float)currentSentence.Length)));
                clearcheck = true;
                SceneManager.LoadScene("OKScene");
            }
            else
            {
                SceneManager.LoadScene("BadScene");
            }
        }

        if (answer2 == anstext || answer3 == anstext || answer4 == anstext)
        {
            startbutton.clearQ += 1;
            startbutton.respoint = startbutton.respoint + (100.0 * (1.1 - ((float)textCharNumber / (float)currentSentence.Length)));
            clearcheck = true;
            SceneManager.LoadScene("OKScene");
        }
        else
        {
            SceneManager.LoadScene("BadScene");
        }
    }

    public static void hayaoshi()
    {
        Button hayaoshibutton = GameObject.Find("Canvas/hayaoshi").GetComponentInChildren<Button>();
        hayaoshibutton.interactable = false;
        hayaoshibutton.onClick.Invoke();
    }

    public void kaitou()
    {
        hayaoshibutton.SetActive(false);
        kaitoupanel.SetActive(true);
        timeout = true;
    }

}