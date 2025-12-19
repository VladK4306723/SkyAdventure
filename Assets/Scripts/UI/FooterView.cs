using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

public class FooterView : UIWindowBase
{
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
    }

    private void OnDisable()
    {
        if (_uiManager is IUIManager ui)
            ui.WindowShown -= OnWindowShown;
    }

    private void OnWindowShown(UIWindowId id)
    {
        SetActiveWindow(id);
    }

    private void SetActiveWindow(UIWindowId id)
    {
        foreach (var pair in _buttonsByWindow)
        {
            bool isActive = pair.Key == id;
            SetButtonAlpha(pair.Value, isActive ? 1f : inactiveAlpha);
        }
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
}
