using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewRecordWindowView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _multiplierText;
    [SerializeField] private TextMeshProUGUI _achivementText;
    [SerializeField] private Button _exitBTN;

    private void Awake()
    {
        _exitBTN.onClick.AddListener(OnContinueClicked);
    }

    public override void Show()
    {
        base.Show();
        UpdateRecordStats();
    }

    private void UpdateRecordStats()
    {
        _multiplierText.text = _progress.CurrentSession.Multiplier.ToString("0.0");
        _achivementText.text = $"Risk Taker";
    }

    private void OnContinueClicked()
    {
        _uiManager.Show(UIWindowId.Victory);
    }
}
