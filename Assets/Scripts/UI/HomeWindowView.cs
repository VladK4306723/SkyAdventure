using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HomeWindowView : UIWindowBase
{
    [SerializeField] private Transform _recentFlightsRoot;
    [SerializeField] private RecentFlightItemView _recentFlightItemPrefab;
    [SerializeField] private RectTransform _scrollViewContentRect;

    [SerializeField] private GameObject _separatorPrefab;

    [SerializeField] private Button _playButton;

    [SerializeField] private TextMeshProUGUI _flightCostText;

    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;

    [SerializeField] private Button _cost5Button;
    [SerializeField] private Button _cost10Button;
    [SerializeField] private Button _cost25Button;
    [SerializeField] private Button _cost50Button;


    [SerializeField] private TextMeshProUGUI _totalStarsText;
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    [SerializeField] private TextMeshProUGUI _bestMultiText;

    private int _selectedCost = 10;

    private const int MinCost = 10;
    private const int MaxCost = 500;
    private const int Step = 5;



    private void Awake()
    {
        _minusButton.onClick.AddListener(() => ChangeCost(-Step));
        _plusButton.onClick.AddListener(() => ChangeCost(+Step));

        _cost5Button.onClick.AddListener(() => SetCost(5));
        _cost10Button.onClick.AddListener(() => SetCost(10));
        _cost25Button.onClick.AddListener(() => SetCost(25));
        _cost50Button.onClick.AddListener(() => SetCost(50));

        _playButton.onClick.AddListener(OnStartButtonClicked);
    }

    public override void Show()
    {
        base.Show();

        _selectedCost = 10;

        if (_dataManager.Meta.Coins == 0)
            _dataManager.Meta.Coins = 10;

        _totalStarsText.text = _dataManager.Meta.TotalStars.ToString();
        _totalCoinsText.text = _dataManager.Meta.Coins.ToString();
        _bestMultiText.text = _dataManager.Meta.BestMultiplier.ToString("0.0") + "x";

        UpdateRecentFlights();
        RefreshUI();
    }


    private void ChangeCost(int delta)
    {
        SetCost(_selectedCost + delta);
    }

    private void SetCost(int value)
    {
        _selectedCost = Mathf.Clamp(value, MinCost, MaxCost);
        RefreshUI();
    }

    private void RefreshUI()
    {
        _flightCostText.text = _selectedCost.ToString();

        int coins = _dataManager.Meta.Coins;
        bool canPlay = _selectedCost <= coins;

        _playButton.interactable = canPlay;

        _minusButton.interactable = _selectedCost > MinCost;
        _plusButton.interactable = _selectedCost < MaxCost;
    }


    private void OnStartButtonClicked()
    {
        if (_selectedCost > _dataManager.Meta.Coins)
            return;

        _gameFlow.StartGame(PlayerType.Default, _selectedCost);
        _uiManager.Show(UIWindowId.Game);
    }


    private void UpdateRecentFlights()
    {
        foreach (Transform child in _recentFlightsRoot)
            Destroy(child.gameObject);

        var sessions = _dataManager.RecentSessions.Take(10).ToList();

        for (int i = 0; i < sessions.Count; i++)
        {
            var item = Instantiate(
                _recentFlightItemPrefab,
                _recentFlightsRoot
            );

            item.Set(sessions[i]);

            if (i < sessions.Count - 1)
            {
                Instantiate(
                    _separatorPrefab,
                    _recentFlightsRoot
                );
            }
        }

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            _recentFlightsRoot as RectTransform
        );

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            _scrollViewContentRect
        );
    }



}
