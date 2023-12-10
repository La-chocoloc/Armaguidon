using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowTimer : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;
        int minutes = (int)(time / 60);
        int seconds = (int)time % 60;
        text.text = $"{minutes}:{seconds.ToString("00")}";
    }
}
