namespace MyCommLib.Classes;

using System;

public class clsTime
{
    public int Mins { get; set; }
    public int Hour => Mins / 60;
    public int Min => Mins % 60;
    public bool IsValid { get; set; } = true;
    public clsTime()
    {
        Mins = 0;
    }
    public clsTime(int hour, int min)
    {
        Mins = hour * 60 + min;
    }
    public clsTime(int min)
    {
        Mins = min;
    }
    public clsTime(string time)
    {
        IsValid = true;
        if (string.IsNullOrEmpty(time)) return;
        Mins = GetMins(time);
        if (Mins < 0) IsValid = false;
    }
    public static int GetMins(string time)
    {
        if (string.IsNullOrEmpty(time)) return -1;
        int hour;
        int min = 0;
        string[] hm = time.Split(':');
        if (hm.Length > 2) return -1;
        if (!int.TryParse(hm[0], out hour)) return -1;
        if (hm.Length == 2)
        {
            if (!int.TryParse(hm[1], out min)) return -1;
        }
        if (min < 0 || min > 59) return -1;
        return hour * 60 + min;
    }
    public static string GetHM(int mins)
    {
        var amins = Math.Abs(mins);
        return @$"{(mins < 0 ? "-" : "")}{amins / 60:00}:{amins % 60:00}";
    }
    public override string ToString()
    {
        return GetHM(Mins);
    }
    public void Add(int hour, int min)
    {
        Mins += hour * 60 + min;
    }
    public int MinsFrom(clsTime from)
    {
        return Mins - from.Mins;
    }
}
