using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AchievementItemView : MonoBehaviour
{
    [Header("State buttons")]
    [SerializeField] private GameObject _activeBtn;
    [SerializeField] private GameObject _lockedBtn;

    [Header("Optional UI")]
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _earnedText;

    public void Set(AchievementData data)
    {
        Debug.Log(
            $"[AIV][SET] go={name} id={data.Id} unlocked={data.IsUnlocked} completed={data.IsCompleted} progress={data.Progress}/{data.Target}"
        );

        bool isCompleted = data.IsCompleted;
        bool isActive = data.IsUnlocked && !data.IsCompleted;
        bool isLocked = !data.IsUnlocked;

        _lockedBtn?.SetActive(isLocked);
        _activeBtn?.SetActive(isCompleted);

        if (_progressSlider != null)
        {
            _progressSlider.gameObject.SetActive(isActive);
            _progressSlider.value = data.Target > 0
                ? Mathf.Clamp01(data.Progress / (float)data.Target)
                : 0f;
        }

        if (_progressText != null)
        {
            _progressText.gameObject.SetActive(isActive);
            _progressText.text = $"{data.Progress}/{data.Target}";
        }

        if (_earnedText != null)
        {
            _earnedText.gameObject.SetActive(isCompleted);
            _earnedText.text =
                data.DaysAgo == 0
                    ? "Earned today"
                    : $"Earned {data.DaysAgo}d ago";
        }
    }



}
