using System;
using UnityEngine;

[Serializable]
public sealed class AchievementData
{
    public string Id;

    public bool IsUnlocked;
    public bool IsCompleted;

    public int Progress;
    public int Target;

    public long CompletedUnixTime;

    public int DaysAgo
    {
        get
        {
            if (!IsCompleted) return 0;
            var date = DateTimeOffset.FromUnixTimeSeconds(CompletedUnixTime).UtcDateTime;
            return Mathf.Max(0, (int)(DateTime.UtcNow - date).TotalDays);
        }
    }

    public void Unlock()
    {
        IsUnlocked = true;
        Progress = 0;
    }

    public void Complete()
    {
        IsCompleted = true;
        CompletedUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
