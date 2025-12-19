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
    [SerializeField] private GameObject _progressSliderObj;
    [SerializeField] private Image _dangerFill;

    [SerializeField] private Button _pauseButton;

    [SerializeField] private Button _completeFlightBTN;


    private void Awake()
    {
        _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        _completeFlightBTN.onClick.AddListener(OnCompleteFlightButtonClicked);

        
    }

    public override void Show()
    {
        base.Show();

        if (_progressSliderObj != null)
            _progressSliderObj.SetActive(GameSettings.ShowDangerLevel);
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

        float danger = Mathf.Clamp01(session.Danger);
        _progressSlider.value = danger;

        if (_dangerFill != null)
        {
            if (danger < 0.33f)
                _dangerFill.color = new Color32(0x4C, 0xAF, 0x50, 0xFF);
            else if (danger < 0.66f)
                _dangerFill.color = new Color32(0xFF, 0xD7, 0x00, 0xFF); 
            else
                _dangerFill.color = new Color32(0xFF, 0x6B, 0x35, 0xFF);
        }
    }


    private void OnCompleteFlightButtonClicked()
    {
        _gameFlow.FinishGame();
        _uiManager.Show(UIWindowId.Victory);
    }
}
