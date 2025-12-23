namespace MyCommLib.Classes;

using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class clsDates
{
    public DateTime DateFrom { get; set; } = default!;
    public DateTime DateTo { get; set; } = default!;
    public string City { get; set; } = "SYD";
    private string _Type = default!;
    private int _Count;

    public clsDates()
    {
        Today();
    }
    public clsDates(string sel)
    {
        if (sel == "W") Weeks(2);
        else if (sel == "M") Months(1);
        else if (sel == "Y") Years(1);
        else Today();
    }
    public clsDates(int yearFrom, int monthFrom, int yearTo, int monthTo)
    {
        DateFrom = DateTime.MinValue;
        DateFrom = DateFrom.AddYears(yearFrom);
        DateFrom = DateFrom.AddMonths(monthFrom);
        DateTo = DateTime.MinValue;
        DateTo = DateTo.AddYears(yearTo);
        DateTo = DateTo.AddMonths(monthTo);
        DateTo = clsDates.MonthLast(DateTo);
    }
    public clsDates(DateTime dateFrom, DateTime dateTo)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
    }
    public void Weeks(int cnt) // cnt Weeks
    {
        _Type = "Week";
        _Count = cnt;
        Today();
        var n = (int)DateFrom.DayOfWeek; // Sun = 0 ... Sat = 6
        n = n == 0 ? 6 : --n; // Mon = 0 ... Sun = 6
        DateFrom = DateFrom.AddDays(n * -1); // Last Mon
        DateTo = DateFrom.AddDays(7 * cnt - 1);
    }
    public void Months(int cnt) // cnt Months
    {
        Months(cnt, clsLocalTime.Today(City));
    }
    public void Months(int cnt, DateTime date) // cnt Months
    {
        _Type = "Month";
        _Count = cnt;
        DateFrom = clsDates.MonthFirst(date);
        DateTo = clsDates.NextMonthFirstN(DateFrom, cnt).AddDays(-1);
    }
    public void Years(int cnt)
    {
        Years(cnt, clsLocalTime.Today(City));
    }
    public void Years(int cnt, DateTime date) // cc Years
    {
        _Type = "Year";
        _Count = cnt;
        DateFrom = Convert.ToDateTime(date.ToString("yyyy") + " Jan 01");
        DateTo = Convert.ToDateTime(DateFrom.ToString("yyyy") + " Dec 31");
        DateTo = DateTo.AddYears(cnt-1);
    }
    public void Today()
    {
        Days(1);
    }
    public void Days(int cnt) // cnt Days
    {
        _Type = "Day";
        _Count = cnt;
        DateFrom = clsLocalTime.Today(City);
        DateTo = DateFrom.AddDays(cnt - 1);
    }
    public void All()
    {
        _Type = "All";
        DateFrom = DateTime.MinValue;
        DateTo = DateTime.MaxValue;
    }
    public void Next(int n)
    {
        if (_Type == "All") { }
        else if (_Type == "Month")
        {
            DateFrom = n > 0 ? clsDates.NextMonthFirstN(DateFrom, _Count * n) : clsDates.PrevMonthFirstN(DateFrom, _Count * n * -1);
            DateTo = clsDates.NextMonthFirstN(DateFrom, _Count).AddDays(-1);
        }
        else if (_Type == "Year")
        {
            DateFrom = DateFrom.AddYears(n);
            DateTo = DateTo.AddYears(n);
        }
        else // Week, Day
        {
            var days = NoOfDays;
            DateFrom = DateFrom.AddDays(days * n);
            DateTo = DateTo.AddDays(days * n);
        }
    }
    public int NoOfDays => (int)(DateTo - DateFrom).TotalDays + 1;
    public static DateTime NextMonthFirstN(DateTime date, int n)
    {
        var first = date;
        for (int i = 0; i < n; i++)
        {
            first = clsDates.NextMonthFirst(first);
        }
        return first;
    }
    public static DateTime PrevMonthFirstN(DateTime date, int n)
    {
        var first = date;
        for (int i = 0; i < n; i++)
        {
            first = clsDates.PrevMonthFirst(first);
        }
        return first;
    }
    public static DateTime MonthFirst(DateTime date) => date.AddDays(date.Day * -1 + 1);
    public static DateTime MonthLast(DateTime date) => clsDates.MonthFirst(date.AddMonths(1)).AddDays(-1);
    public static DateTime NextMonthFirst(DateTime date) => clsDates.MonthLast(date).AddDays(1);
    public static DateTime NextMonthLast(DateTime date) => clsDates.MonthLast(clsDates.NextMonthFirst(date));
    public static DateTime PrevMonthFirst(DateTime date) => clsDates.MonthFirst(clsDates.PrevMonthLast(date));
    public static DateTime PrevMonthLast(DateTime date) => clsDates.MonthFirst(date).AddDays(-1);
    public static DateTime GetDateTime(DateTime date, string time) => DateTime.Parse($"{date.ToString("dd MMM yyyy", (new clsCultureInfo()).ci)} {time}");
}
