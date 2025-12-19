using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HomeWindowView : UIWindowBase
{

    [SerializeField] private Button _startButton;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        _gameFlow.StartGame(PlayerType.Default, 10);
        _uiManager.Show(UIWindowId.Game);
    }
}
