using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FooterView : UIWindowBase
{
    [Inject] private IGameStateService _gameStateService;

    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _statsButton;
    [SerializeField] private Button _collectionButton;
    [SerializeField] private Button _settingsButton;

    [SerializeField, Range(0f, 1f)] private float inactiveAlpha = 0.5f;

    private Dictionary<UIWindowId, Button> _buttonsByWindow;

    private void Awake()
    {
        _buttonsByWindow = new Dictionary<UIWindowId, Button>
        {
            { UIWindowId.Home, _homeButton },
            { UIWindowId.Stats, _statsButton },
            { UIWindowId.Collection, _collectionButton },
            { UIWindowId.Settings, _settingsButton }
        };

        _homeButton.onClick.AddListener(() => _uiManager.Show(UIWindowId.Home));
        _statsButton.onClick.AddListener(() => _uiManager.Show(UIWindowId.Stats));
        _collectionButton.onClick.AddListener(() => _uiManager.Show(UIWindowId.Collection));
        _settingsButton.onClick.AddListener(() => _uiManager.Show(UIWindowId.Settings));
    }

    private void OnEnable()
    {
        if (_uiManager is IUIManager ui)
        {
            ui.WindowShown += OnWindowShown;
            SetActiveWindow(ui.CurrentWindowId);
        }

        _gameStateService.StateChanged += OnGameStateChanged;
        ApplyGameState(_gameStateService.State);
    }

    private void OnDisable()
    {
        if (_uiManager is IUIManager ui)
            ui.WindowShown -= OnWindowShown;

        _gameStateService.StateChanged -= OnGameStateChanged;
    }


    private void OnWindowShown(UIWindowId id)
    {
        SetActiveWindow(id);
    }

    private void SetButtonAlpha(Button button, float alpha)
    {
        var graphics = button.GetComponentsInChildren<Graphic>(true);

        foreach (var graphic in graphics)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        ApplyGameState(state);
    }


    private void ApplyGameState(GameState state)
    {
        bool blocked = state == GameState.Playing;

        foreach (var pair in _buttonsByWindow)
        {
            pair.Value.interactable = !blocked;
        }

        RefreshVisualState();
    }

    private void RefreshVisualState()
    {
        var current =
            (_uiManager as IUIManager)?.CurrentWindowId ?? UIWindowId.Home;

        if (current == UIWindowId.Game)
            current = UIWindowId.Home;

        foreach (var pair in _buttonsByWindow)
        {
            bool isActive = pair.Key == current;
            SetButtonAlpha(pair.Value, isActive ? 1f : inactiveAlpha);
        }
    }


    private void SetActiveWindow(UIWindowId id)
        {
            RefreshVisualState();
        }
}
