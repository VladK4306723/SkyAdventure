using System;
using UnityEngine;

[Serializable]
public class WeeklyStats
{
    public int Sessions;
    public int SuccessfulSessions;
    public int StarsCollected;
    public int CoinsSpent;

    public int SuccessRatePercent =>
        Sessions == 0
            ? 0
            : Mathf.RoundToInt((float)SuccessfulSessions / Sessions * 100f);
}
