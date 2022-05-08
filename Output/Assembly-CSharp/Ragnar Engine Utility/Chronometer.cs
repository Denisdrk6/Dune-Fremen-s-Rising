using System;
using RagnarEngine;

public class Chronometer
{
    public float timer = 0;
    private float sec = 0;
    private float min = 0;
    private string text = "";

    public void Update()
    {
        timer += Time.deltaTime;
    }
    public string GetTimeToString()
    {
        if (timer < 60)
        {
            sec = timer;
        }
        else if (timer >= 60)
        {
            min = timer / 60;
            sec = timer % 60;
        }

        if (timer < 60)
            text = "00:";
        else if (min < 10)
            text = min.ToString().Insert(0, "0").Substring(0, 2) + ":";
        else
            text = min.ToString().Substring(0, 2) + ":";

        if (sec < 10)
            text += sec.ToString().Insert(0, "0").Substring(0, 2);
        else
            text += sec.ToString().Substring(0, 2);

        text += ":" + timer.ToString().Remove(0, timer.ToString().IndexOf(",") + 1).ToString().Substring(0,2);

        return text;
    }
}
