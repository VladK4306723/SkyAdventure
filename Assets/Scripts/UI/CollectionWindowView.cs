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


    private readonly string _instanceTag = $"CWV#{System.Guid.NewGuid().ToString("N")[..6]}";

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

    private void OnSessionFinished(SessionData s, GameFinishReason r)
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
        Debug.Log($"[CWV][SHOW_ACH] {_instanceTag} items={_achievementItems?.Length ?? -1}");

        _collectionBlock.SetActive(false);
        _achievementsBlock.SetActive(true);

        Refresh();
    }


    private void Refresh()
    {
        Debug.Log($"[CWV][REFRESH] {_instanceTag} achievementsBlockActive={_achievementsBlock.activeSelf}");

        var list = _dataManager?.Achievements;

        Debug.Log($"[CWV][REFRESH] {_instanceTag} dataManagerNull={_dataManager == null} listNull={list == null} listCount={(list?.Count ?? -1)}");

        if (list == null || list.Count == 0)
            return;

        int count = Mathf.Min(_achievementItems.Length, list.Count);
        Debug.Log($"[CWV][REFRESH] {_instanceTag} willSet count={count}");

        for (int i = 0; i < count; i++)
        {
            var d = list[i];
            Debug.Log($"[CWV][SET] {_instanceTag} i={i} id={d.Id} unlocked={d.IsUnlocked} completed={d.IsCompleted} progress={d.Progress}/{d.Target}");
            _achievementItems[i].Set(d);
        }
    }



}
