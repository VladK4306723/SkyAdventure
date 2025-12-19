using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindowView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _currentStarsText;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _finishBtn;

    private void Awake()
    {
        _continueBtn.onClick.AddListener(OnContinueClicked);
        _finishBtn.onClick.AddListener(OnFinishClicked);
    }

    public override void Show()
    {
        base.Show();
        UpdateStarsText();
    }

    private void UpdateStarsText()
    {
        int currentStars = _progress.CurrentSession.StarsCollected;
        _currentStarsText.text = currentStars.ToString();
    }


    private void OnContinueClicked()
    {
        _gameFlow.ResumeGame();
        _uiManager.Show(UIWindowId.Game);
    }

    private void OnFinishClicked()
    {
        _gameFlow.AbortGame();
    }
}
