using System;
using System.Globalization;

[System.Serializable]
public sealed class DailyFlightStats
{
    public string Date;
    public int Sessions;

    public string DayShortName
    {
        get
        {
            if (DateTime.TryParse(Date, out var date))
                return date.ToString("ddd", CultureInfo.InvariantCulture);

            return "?";
        }
    }
}
