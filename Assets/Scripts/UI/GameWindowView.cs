using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameWindowView : UIWindowBase
{

    
    [SerializeField] private TextMeshProUGUI _starsText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _multiplierText;
    [SerializeField] private Slider _progressSlider;

    [SerializeField] private Button _pauseButton;

    [SerializeField] private Button _completeFlightBTN;


    private void Awake()
    {
        _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        _completeFlightBTN.onClick.AddListener(OnCompleteFlightButtonClicked);
    }

    private void OnPauseButtonClicked()
    {
        _gameFlow.PauseGame();
        _uiManager.Show(UIWindowId.Pause);
    }

    private void Update()
    {
        var session = _progress.CurrentSession;

        _starsText.text = session.StarsCollected.ToString();
        _timeText.text = session.FlightTime.ToString("0.0") + " s";
        _multiplierText.text = session.Multiplier.ToString("0.0");
        _progressSlider.value = session.Danger;
    }

    private void OnCompleteFlightButtonClicked()
    {
        _gameFlow.FinishGame();
        _uiManager.Show(UIWindowId.Victory);
    }
}
