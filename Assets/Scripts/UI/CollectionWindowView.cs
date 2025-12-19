using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class CollectionWindowView : UIWindowBase
{
    [Header("Tabs")]
    [SerializeField] private GameObject _collectionBlock;
    [SerializeField] private GameObject _achievementsBlock;

    [SerializeField] private Button _collectionButton;
    [SerializeField] private Button _achievementsButton;

    [Header("Achievements")]
    [SerializeField] private AchievementItemView[] _achievementItems;

    private void Awake()
    {
        _collectionButton.onClick.AddListener(ShowCollection);
        _achievementsButton.onClick.AddListener(ShowAchievements);
    }

    public override void Show()
    {
        base.Show();
        _progress.SessionFinished += OnSessionFinished;
        ShowCollection();
    }


    private void OnDisable()
    {
        _progress.SessionFinished -= OnSessionFinished;
    }

    private void OnSessionFinished(
    SessionData s,
    GameFinishReason r,
    SessionResult result)
    {
        if (_achievementsBlock.activeSelf)
            Refresh();
    }


    private void ShowCollection()
    {
        _collectionBlock.SetActive(true);
        _achievementsBlock.SetActive(false);
    }

    private void ShowAchievements()
    {

        _collectionBlock.SetActive(false);
        _achievementsBlock.SetActive(true);

        Refresh();
    }


    private void Refresh()
    {
        var list = _dataManager?.Achievements;

        if (list == null || list.Count == 0)
            return;

        int count = Mathf.Min(_achievementItems.Length, list.Count);

        for (int i = 0; i < count; i++)
        {
            var d = list[i];
            _achievementItems[i].Set(d);
        }
    }



}
