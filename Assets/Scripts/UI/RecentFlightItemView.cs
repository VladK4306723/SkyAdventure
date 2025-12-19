using TMPro;
using UnityEngine;

public sealed class RecentFlightItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _starsText;
    [SerializeField] private TextMeshProUGUI _multiplierText;

    public void Set(SessionSummary data)
    {
        _distanceText.text = data.Distance.ToString("00.000");
        _starsText.text = data.Stars.ToString();
        _multiplierText.text = data.Multiplier.ToString("0.0") + "x";
    }
}
