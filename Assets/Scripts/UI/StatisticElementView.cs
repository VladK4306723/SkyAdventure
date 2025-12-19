using TMPro;
using UnityEngine;

public sealed class StatisticElementView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _countText;

    [SerializeField] private Transform _iconsRoot;
    [SerializeField] private GameObject _flightIconPrefab;
    [SerializeField] private TextMeshProUGUI _moreText;

    private const int MaxIcons = 6;

    public void Set(DailyFlightStats data)
    {
        _dayText.text = data.DayShortName;
        _countText.text = $"{data.Sessions} flights";

        RebuildIcons(data.Sessions);
    }

    private void RebuildIcons(int count)
    {
        for (int i = _iconsRoot.childCount - 1; i >= 0; i--)
            Destroy(_iconsRoot.GetChild(i).gameObject);

        int iconsToShow = Mathf.Min(count, MaxIcons);

        for (int i = 0; i < iconsToShow; i++)
            Instantiate(_flightIconPrefab, _iconsRoot);

        if (count > MaxIcons)
        {
            _moreText.gameObject.SetActive(true);
            _moreText.text = $"+{count - MaxIcons}";
        }
        else
        {
            _moreText.gameObject.SetActive(false);
        }
    }
}
