namespace MyCommLib.Classes;

using System;

public class clsMoney
{
    public static string Disp(decimal amount) => $"{amount.ToString("#,0.00")}";
    public static string Disp(string? cur, Decimal amount) => $"{cur} {Disp(amount)}";
}
