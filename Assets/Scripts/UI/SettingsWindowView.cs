using TMPro;
using UnityEngine;

public sealed class SettingsWindowView : UIWindowBase
{
    [Header("Toggles")]
    [SerializeField] private ToggleView _musicToggle;
    [SerializeField] private ToggleView _dangerToggle;
    [SerializeField] private ToggleView _bonusToggle;

    [Header("Footer")]
    [SerializeField] private TextMeshProUGUI _versionText;
    [SerializeField] private TextMeshProUGUI _copyrightText;

    public override void Show()
    {
        base.Show();

        _musicToggle.Init(
            GameSettings.MusicEnabled,
            v =>
            {
                GameSettings.MusicEnabled = v;
                GameSettings.Save();
            });

        _dangerToggle.Init(
            GameSettings.ShowDangerLevel,
            v =>
            {
                GameSettings.ShowDangerLevel = v;
                GameSettings.Save();
            });

        _bonusToggle.Init(
            GameSettings.DailyBonusReminder,
            v =>
            {
                GameSettings.DailyBonusReminder = v;
                GameSettings.Save();
            });

        FillFooter();
    }

    private void FillFooter()
    {
        _versionText.text = $"Version {Application.version}";
        _copyrightText.text =
            $"© {System.DateTime.Now.Year} Sky Adventure";
    }
}
