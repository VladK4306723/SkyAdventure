using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompleteFlightWindowView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _finalMultiplierText;
    [SerializeField] private TextMeshProUGUI _flightCostText;
    [SerializeField] private TextMeshProUGUI _durationText;
    [SerializeField] private TextMeshProUGUI _starsColectedText;

    [SerializeField] private Button _tryAgainBTN;
    [SerializeField] private Button _homeBTN;

    private void Awake()
    {
        _tryAgainBTN.onClick.AddListener(OnTryAgainClicked);
        _homeBTN.onClick.AddListener(OnHomeClicked);
    }

    public override void Show()
    {
        base.Show();
        UpdateFlightStats();
    }

    private void UpdateFlightStats()
    {
        var session = _progress.CurrentSession;
        _finalMultiplierText.text = session.Multiplier.ToString("0.0");
        _flightCostText.text = session.FlightCost.ToString();
        _durationText.text = $"{session.FlightTime:0.0} s";
        _starsColectedText.text = session.StarsCollected.ToString();
    }

    private void OnTryAgainClicked()
    {
        _gameFlow.RestartGame();
        _uiManager.Show(UIWindowId.Game);
    }

    private void OnHomeClicked()
    {
       _uiManager.Show(UIWindowId.Home);
    }
}
