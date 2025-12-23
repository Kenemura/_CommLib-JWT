namespace MyCommLib.Classes;

using System;
public class clsDate
{
    public static string Disp(DateTime date) => (date == DateTime.MinValue) ? "" : date.ToString("dd MMM yyyy", (new clsCultureInfo()).ci);
    public static bool IsDate(DateTime date) => (date == DateTime.MinValue) ? false : true;
    public static DateTime NoDate => DateTime.MinValue;
}
