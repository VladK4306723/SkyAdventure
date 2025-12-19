using UnityEngine;

public static class GameSettings
{
    private const string MusicKey = "settings_music";
    private const string DangerKey = "settings_danger";
    private const string BonusKey = "settings_bonus";

    public static bool MusicEnabled
    {
        get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
        set => PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
    }

    public static bool ShowDangerLevel
    {
        get => PlayerPrefs.GetInt(DangerKey, 1) == 1;
        set => PlayerPrefs.SetInt(DangerKey, value ? 1 : 0);
    }

    public static bool DailyBonusReminder
    {
        get => PlayerPrefs.GetInt(BonusKey, 1) == 1;
        set => PlayerPrefs.SetInt(BonusKey, value ? 1 : 0);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
