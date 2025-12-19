using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticWindowView : UIWindowBase
{
    [Header("Tabs")]
    [SerializeField] private GameObject _todayBlock;
    [SerializeField] private GameObject _weekBlock;

    [SerializeField] private Button _todayButton;
    [SerializeField] private Button _weekButton;

    [Header("Daily stats")]
    [SerializeField] private TextMeshProUGUI _totalStarsText;
    [SerializeField] private TextMeshProUGUI _dailyTotalStarsText;
    [SerializeField] private TextMeshProUGUI _dailyCoinsSpentText;
    [SerializeField] private TextMeshProUGUI _dailySuccessRateText;

    [Header("Weekly stats")]
    [SerializeField] private TextMeshProUGUI _totalStarsText1;
    [SerializeField] private TextMeshProUGUI _weeklyTotalStarsText;
    [SerializeField] private TextMeshProUGUI _weeklyCoinsSpentText;
    [SerializeField] private TextMeshProUGUI _weeklySuccessRateText;

    [Header("Personal records")]
    [SerializeField] private TextMeshProUGUI _bestMultiplierText;
    [SerializeField] private TextMeshProUGUI _longestFlightText;
    [SerializeField] private TextMeshProUGUI _bestStarsText;
    [SerializeField] private TextMeshProUGUI _bestStreakText;

    [Header("Weekly flights list")]
    [SerializeField] private Transform _weekListRoot;
    [SerializeField] private StatisticElementView _dayItemPrefab;
    [SerializeField] private GameObject _separatorPrefab;

    [Header("Averages")]
    [SerializeField] private TextMeshProUGUI _avgCostText;
    [SerializeField] private TextMeshProUGUI _avgTimeText;
    [SerializeField] private TextMeshProUGUI _avgMultiplierText;
    [SerializeField] private TextMeshProUGUI _mostActiveHourText;

    private void Awake()
    {
        _todayButton.onClick.AddListener(ShowToday);
        _weekButton.onClick.AddListener(ShowWeek);
    }

    public override void Show()
    {
        base.Show();

        ShowToday();
        FillPersonalRecords();
        FillWeeklyFlights();
        FillAverages();
    }

    private void ShowToday()
    {
        _todayBlock.SetActive(true);
        _weekBlock.SetActive(false);

        FillDailyStats();
    }

    private void FillDailyStats()
    {
        var today = _dataManager.Today;

        _totalStarsText.text = _dataManager.Meta.TotalStars.ToString();
        _dailyTotalStarsText.text = today.StarsCollected.ToString();
        _dailyCoinsSpentText.text = today.CoinsSpent.ToString();
        _dailySuccessRateText.text = today.SuccessRatePercent + "%";
    }

    private void ShowWeek()
    {
        _todayBlock.SetActive(false);
        _weekBlock.SetActive(true);

        FillWeeklyStats();
    }

    private void FillWeeklyStats()
    {
        var week = _dataManager.Week;

        _totalStarsText1.text = _dataManager.Meta.TotalStars.ToString();
        _weeklyTotalStarsText.text = week.StarsCollected.ToString();
        _weeklyCoinsSpentText.text = week.CoinsSpent.ToString();
        _weeklySuccessRateText.text = week.SuccessRatePercent + "%";
    }


    private void FillPersonalRecords()
    {
        var meta = _dataManager.Meta;

        _bestMultiplierText.text = meta.BestMultiplier.ToString("0.0") + "x";
        _longestFlightText.text = meta.LongestFlight.ToString("0.0") + " s";
        _bestStarsText.text = meta.BestStarsInSession.ToString();
        _bestStreakText.text = meta.BestWinStreak + " wins";
    }

    private void FillWeeklyFlights()
    {
        foreach (Transform child in _weekListRoot)
            Destroy(child.gameObject);

        var days = _dataManager.WeeklyFlights;

        for (int i = 0; i < days.Count; i++)
        {
            var item = Instantiate(_dayItemPrefab, _weekListRoot);
            item.Set(days[i]);

            if (i < days.Count - 1)
            {
                Instantiate(_separatorPrefab, _weekListRoot);
            }
        }
    }

    private void FillAverages()
    {
        var avg = _dataManager.Averages;

        _avgCostText.text = avg.AvgFlightCost.ToString("0");
        _avgTimeText.text = avg.AvgFlightTime.ToString("0.0") + " s";
        _avgMultiplierText.text = avg.AvgMultiplier.ToString("0.0");
        _mostActiveHourText.text = avg.MostActiveHour + ":00";
    }

}
