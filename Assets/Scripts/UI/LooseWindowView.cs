using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LooseWindowView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _currentMultiplier;
    [SerializeField] private TextMeshProUGUI _coinsLost;
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
        UpdateLooseStats();

        int cost = _progress.CurrentSession.FlightCost;

        if (_dataManager.Meta.Coins < cost)
        {
            _tryAgainBTN.interactable = false;
            return;
        }
    }

    private void UpdateLooseStats()
    {
        _currentMultiplier.text = _progress.CurrentSession.Multiplier.ToString("0.0");
        _coinsLost.text = _progress.CurrentSession.FlightCost.ToString();
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

