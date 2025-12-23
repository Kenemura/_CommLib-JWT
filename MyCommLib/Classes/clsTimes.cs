namespace MyCommLib.Classes;

using System.Collections.Generic;

public class clsTimes
{
    public clsTime TimeFrom { get; set; }
    public clsTime TimeTo { get; set; }
    public clsTimes(string from, string to)
    {
        TimeFrom = new clsTime(from);
        TimeTo = new clsTime(to);
    }
    public clsTimes(int from, int to)
    {
        TimeFrom = new clsTime(from);
        TimeTo = new clsTime(to);
    }
    public bool IsValid(List<string> errorMessages)
    {
        bool result = true;
        if (!TimeFrom.IsValid)
        {
            errorMessages.Add($"Time From invalid format!");
            result = false;
        }
        if (!TimeTo.IsValid)
        {
            errorMessages.Add($"Time To invalid format!");
            result = false;
        }
        if (TimeTo.MinsFrom(TimeFrom) < 0)
        {
            errorMessages.Add($"Time To have to be later than Time From!");
            result = false;
        }
        return result;
    }
    public int Mins()
    {
        return TimeTo.MinsFrom(TimeFrom);
    }
}
