using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public interface IDataManager
{
    PlayerMetaData Meta { get; }
    IReadOnlyList<SessionSummary> RecentSessions { get; }
    IReadOnlyList<DailyFlightStats> WeeklyFlights { get; }
    IReadOnlyList<AchievementData> Achievements { get; }

    DailyStats Today { get; }
    WeeklyStats Week { get; }
    Averages Averages { get; }

    SessionResult ApplySession(SessionData session, GameFinishReason reason);
    void Save();
}


public sealed class DataManager : IDataManager
{
    private const int c_StartCoins = 10;

    public PlayerMetaData Meta { get; private set; }
    public IReadOnlyList<SessionSummary> RecentSessions => _recentSessions;
    public IReadOnlyList<DailyFlightStats> WeeklyFlights => _weeklyFlights;

    private List<AchievementData> _achievements;
    public IReadOnlyList<AchievementData> Achievements => _achievements;


    public DailyStats Today { get; private set; }
    public WeeklyStats Week { get; private set; }
    public Averages Averages { get; private set; }

    private List<SessionSummary> _recentSessions;
    private List<DailyFlightStats> _weeklyFlights;

    private int[] _hourHistogram = new int[24];

    private int _totalFlights;
    private float _totalFlightTime;
    private float _totalMultiplier;
    private int _totalCost;


    public DataManager()
    {
        Load();
    }

    private string GetSavePath()
    {
        return Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );
    }


    // ───────────────────────── PUBLIC API ─────────────────────────

    public SessionResult ApplySession(SessionData session, GameFinishReason reason)
    {
        int completedBefore = 0;
        for (int i = 0; i < _achievements.Count; i++)
            if (_achievements[i].IsCompleted)
                completedBefore++;

        float prevLongestFlight = Meta.LongestFlight;

        UpdateAchievements(session, reason);
        UpdateMeta(session, reason);

        int completedAfter = 0;
        for (int i = 0; i < _achievements.Count; i++)
            if (_achievements[i].IsCompleted)
                completedAfter++;

        bool hasNewAchievement = completedAfter > completedBefore;
        bool hasNewFlightRecord = session.FlightTime > prevLongestFlight;

        AddSessionSummary(session, reason);
        UpdateDaily(session, reason);
        UpdateWeekly(session, reason);
        UpdateWeeklyFlights();
        UpdateAverages(session);

        Save();

        return new SessionResult(hasNewAchievement, hasNewFlightRecord);
    }


    public void Save()
    {
        var data = new SaveData
        {
            Meta = Meta,
            RecentSessions = _recentSessions,
            WeeklyFlights = _weeklyFlights,
            Today = Today,
            Week = Week,
            Averages = Averages,
            Achievements = _achievements,
            LastSaveDate = Today.Date
        };



        string json = JsonUtility.ToJson(data, true);
        string path = GetSavePath();

        if (_achievements == null || _achievements.Count == 0)
        {
            _achievements = CreateDefaultAchievements();
        }



        File.WriteAllText(path, json);
    }


    // ───────────────────────── LOAD ─────────────────────────

    private void Load()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            CreateNew();
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);

        Meta = data.Meta ?? new PlayerMetaData();
        _recentSessions = data.RecentSessions ?? new List<SessionSummary>();
        _weeklyFlights = data.WeeklyFlights ?? new List<DailyFlightStats>();
        Today = data.Today ?? CreateToday();
        Week = data.Week ?? new WeeklyStats();
        Averages = data.Averages ?? new Averages();

        if (data.Achievements == null || data.Achievements.Count == 0)
        {
            _achievements = CreateDefaultAchievements();
        }
        else
        {
            _achievements = data.Achievements;
        }


        HandleDateRollover(data.LastSaveDate);

        if (_achievements != null)
        {
            for (int i = 0; i < _achievements.Count; i++)
            {
                var a = _achievements[i];
            }
        }

    }



    private void CreateNew()
    {
        Meta = new PlayerMetaData();
        Meta.Coins = c_StartCoins;
        Meta.TotalStars = 0;

        _recentSessions = new List<SessionSummary>();
        _weeklyFlights = new List<DailyFlightStats>();
        _achievements = CreateDefaultAchievements();

        Today = CreateToday();
        Week = new WeeklyStats();
        Averages = new Averages();

        Save();
    }



    // ───────────────────────── META ─────────────────────────

    private void UpdateMeta(SessionData s, GameFinishReason reason)
    {
        Meta.TotalStars += s.StarsCollected;

        if (reason == GameFinishReason.Completed)
        {
            int earnedCoins = Mathf.RoundToInt(s.StarsCollected * s.MaxMultiplier);
            Meta.Coins += earnedCoins;
        }

        Meta.Coins -= s.FlightCost;

        if (s.FlightTime > Meta.LongestFlight)
            Meta.LongestFlight = s.FlightTime;

        if (s.MaxMultiplier > Meta.BestMultiplier)
            Meta.BestMultiplier = s.MaxMultiplier;

        if (s.StarsCollected > Meta.BestStarsInSession)
            Meta.BestStarsInSession = s.StarsCollected;

        if (reason == GameFinishReason.Completed)
        {
            Meta.CurrentWinStreak++;
            Meta.BestWinStreak = Mathf.Max(
                Meta.BestWinStreak,
                Meta.CurrentWinStreak
            );
        }
        else
        {
            Meta.CurrentWinStreak = 0;
        }
    }


    // ───────────────────────── SESSION HISTORY ─────────────────────────

    private void AddSessionSummary(SessionData s, GameFinishReason reason)
    {
        _recentSessions.Insert(0, new SessionSummary
        {
            DateTime = DateTime.UtcNow.ToString("o"),
            Distance = s.Distance,
            Stars = s.StarsCollected,
            Multiplier = s.MaxMultiplier,
            Success = reason == GameFinishReason.Completed
        });

        if (_recentSessions.Count > 20)
            _recentSessions.RemoveAt(_recentSessions.Count - 1);
    }

    // ───────────────────────── DAILY / WEEKLY ─────────────────────────

    private void UpdateDaily(SessionData s, GameFinishReason reason)
    {
        EnsureToday();

        Today.Sessions++;
        Today.StarsCollected += s.StarsCollected;
        Today.CoinsSpent += s.FlightCost;

        if (reason == GameFinishReason.Completed)
            Today.SuccessfulSessions++;
    }

    private void UpdateWeekly(SessionData s, GameFinishReason reason)
    {
        Week.Sessions++;
        Week.StarsCollected += s.StarsCollected;
        Week.CoinsSpent += s.FlightCost;

        if (reason == GameFinishReason.Completed)
            Week.SuccessfulSessions++;
    }

    private void UpdateWeeklyFlights()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var day = _weeklyFlights.FirstOrDefault(d => d.Date == today);
        if (day == null)
        {
            day = new DailyFlightStats { Date = today };
            _weeklyFlights.Add(day);
        }

        day.Sessions++;

        _weeklyFlights = _weeklyFlights
            .OrderByDescending(d => d.Date)
            .Take(7)
            .ToList();
    }

    // ───────────────────────── AVERAGES ─────────────────────────

    private void UpdateAverages(SessionData s)
    {
        _totalFlights++;
        _totalFlightTime += s.FlightTime;
        _totalMultiplier += s.MaxMultiplier;
        _totalCost += s.FlightCost;

        Averages.AvgFlightTime = _totalFlightTime / _totalFlights;
        Averages.AvgMultiplier = _totalMultiplier / _totalFlights;
        Averages.AvgFlightCost = (float)_totalCost / _totalFlights;

        int hour = DateTime.Now.Hour;
        _hourHistogram[hour]++;
        Averages.MostActiveHour =
            Array.IndexOf(_hourHistogram, _hourHistogram.Max());
    }

    // ───────────────────────── DATE LOGIC ─────────────────────────

    private void HandleDateRollover(string lastDate)
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        if (lastDate != today)
        {
            Today = CreateToday();
        }
    }

    private void EnsureToday()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        if (Today.Date != today)
            Today = CreateToday();
    }

    private DailyStats CreateToday()
    {
        return new DailyStats
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };
    }

    private void UpdateAchievements(SessionData session, GameFinishReason reason)
    {
        Debug.Log("=== UpdateAchievements ===");

        for (int i = 0; i < _achievements.Count; i++)
        {
            var a = _achievements[i];

            Debug.Log(
                $"[ACH] {a.Id} | unlocked={a.IsUnlocked} completed={a.IsCompleted} progress={a.Progress}/{a.Target}"
            );

            if (!a.IsUnlocked || a.IsCompleted)
                continue;

            bool completedNow = false;

            switch (a.Id)
            {
                case "first_flight":
                    if (reason == GameFinishReason.Completed)
                    {
                        Debug.Log("[ACH] first_flight completed");
                        a.Complete();
                        completedNow = true;
                    }
                    break;

                case "stars_1000":
                    a.Progress += session.StarsCollected;
                    Debug.Log($"[ACH] stars_1000 progress += {session.StarsCollected}");

                    if (a.Progress >= a.Target)
                    {
                        Debug.Log("[ACH] stars_1000 completed");
                        a.Complete();
                        completedNow = true;
                    }
                    break;

                case "flight_60s":
                    if (session.FlightTime >= a.Target)
                    {
                        Debug.Log("[ACH] flight_60s completed");
                        a.Complete();
                        completedNow = true;
                    }
                    break;
            }

            _achievements[i] = a;

            if (completedNow || a.IsCompleted)
            {
                Debug.Log($"[ACH] Unlock next after {a.Id}");
                UnlockNextAchievement(i);
                break;
            }
        }
    }



    private void UnlockNextAchievement(int index)
    {
        int next = index + 1;
        if (next >= _achievements.Count)
            return;

        var nextAchievement = _achievements[next];

        if (!nextAchievement.IsUnlocked)
            nextAchievement.Unlock();

        _achievements[next] = nextAchievement;
    }


    private List<AchievementData> CreateDefaultAchievements()
    {
        var list = new List<AchievementData>
    {
        new AchievementData
        {
            Id = "first_flight",
            Target = 1,
            IsUnlocked = true
        },
        new AchievementData
        {
            Id = "stars_1000",
            Target = 1000,
            IsUnlocked = false
        },
        new AchievementData
        {
            Id = "flight_60s",
            Target = 60,
            IsUnlocked = false
        }
    };

        return list;
    }


}

public readonly struct SessionResult
{
    public readonly bool HasNewAchievement;
    public readonly bool HasNewFlightRecord;

    public SessionResult(bool achievement, bool record)
    {
        HasNewAchievement = achievement;
        HasNewFlightRecord = record;
    }
}

