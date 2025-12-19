using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int Version = 1;

    public PlayerMetaData Meta;
    public List<SessionSummary> RecentSessions;
    public List<DailyFlightStats> WeeklyFlights;
    public DailyStats Today;
    public WeeklyStats Week;
    public Averages Averages;

    public List<AchievementData> Achievements;

    public string LastSaveDate;
}
