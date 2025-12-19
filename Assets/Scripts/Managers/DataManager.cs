using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

public interface IDataManager
{
    PlayerMetaData Meta { get; }
    IReadOnlyList<SessionSummary> RecentSessions { get; }
    IReadOnlyList<DailyFlightStats> WeeklyFlights { get; }

    DailyStats Today { get; }
    WeeklyStats Week { get; }
    Averages Averages { get; }

    void ApplySession(SessionData session, GameFinishReason reason);
    void Save();
}


public sealed class DataManager : IDataManager
{
    private const string SaveKey = "GAME_SAVE_V1";

    public PlayerMetaData Meta { get; private set; }
    public IReadOnlyList<SessionSummary> RecentSessions => _recentSessions;
    public IReadOnlyList<DailyFlightStats> WeeklyFlights => _weeklyFlights;

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

    public void ApplySession(SessionData session, GameFinishReason reason)
    {
        UpdateMeta(session, reason);
        AddSessionSummary(session, reason);
        UpdateDaily(session, reason);
        UpdateWeekly(session, reason);
        UpdateWeeklyFlights();
        UpdateAverages(session);

        Save();
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
            LastSaveDate = Today.Date
        };

        string json = JsonUtility.ToJson(data, true);
        string path = GetSavePath();

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

        HandleDateRollover(data.LastSaveDate);
    }


    private void CreateNew()
    {
        Meta = new PlayerMetaData();
        _recentSessions = new List<SessionSummary>();
        _weeklyFlights = new List<DailyFlightStats>();

        Today = CreateToday();
        Week = new WeeklyStats();
        Averages = new Averages();
    }

    // ───────────────────────── META ─────────────────────────

    private void UpdateMeta(SessionData s, GameFinishReason reason)
    {
        Meta.TotalStars += s.StarsCollected;
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
}
