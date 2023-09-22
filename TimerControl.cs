using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerControl : MonoBehaviour
{
    public Text Timer;

    public float totalTime;
    int seconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inputquestion.timeout == true || inputquestion.timer == true)
        {
            Timer.enabled = true;
            totalTime -= Time.deltaTime;
            seconds = (int)totalTime;
            Timer.text = seconds.ToString();

            if (seconds == 0)
            {
                totalTime = 0;
            }
        }
    }
}
