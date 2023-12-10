using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowTimer : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    private ArcadeBicycle player;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ArcadeBicycle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.dead) { return; }
        time = Time.time;
        int minutes = (int)(time / 60);
        int seconds = (int)time % 60;
        text.text = $"{minutes}:{seconds.ToString("00")}";
    }
}
