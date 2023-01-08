using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayNight : MonoBehaviour
{
    public float currentTime;
    public float dayLengthMin;
    public TextMeshProUGUI timeText;

    float rotationSpeed;
    float midday;
    public float translateTime;
    string AMPM = "PM";

    private float fixedTime;
    private bool timeFixed;

    // public ParticleSystem stars;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 360 / dayLengthMin / 60;
        midday = dayLengthMin * 60 / 2;
        fixTime(60);
    }

    public void fixTime(float time)
    {
        timeFixed = true;
        fixedTime = time;
        transform.Rotate(new Vector3(1, 0, 0) * rotationSpeed * (time-currentTime));
    }
    public void unfixTime()
    {
        timeFixed = false;
    }

    public void fixNight() { fixTime(dayLengthMin*30); }

    // Update is called once per frame
    void Update()
    {

        currentTime += 1 * Time.deltaTime;
        if (timeFixed) currentTime = fixedTime;
        translateTime = (currentTime / (midday * 2));

        float t = translateTime * 24f;

        float hours = Mathf.Floor(t);

        string displayHours = hours.ToString();
        
        if (hours == 0)
        {
            displayHours = "12";
        }
        if (hours > 12)
        {
            displayHours = (hours - 12).ToString();
        }
        if (currentTime >= midday)
        {
            if (AMPM != "AM")
            {
                AMPM = "AM";
            }
        }
        if (currentTime >= midday * 2)
        {
            if (AMPM != "PM")
            {
                AMPM = "PM";
            }
        }

        t *= 60;
        float minutes = Mathf.Floor(t);

        string displayMinutes = minutes.ToString();
        if (minutes < 10)
        {
            displayMinutes = "0" + minutes.ToString();
        }

        string displayTime = displayHours + ":" + displayMinutes + " " + AMPM;
        timeText.text = displayTime;

        if (!timeFixed) { transform.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime); }
        
    }
}
