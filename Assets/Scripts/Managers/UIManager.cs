using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface IUIManager
{
    void Show(UIWindowId id);
    UIWindowId CurrentWindowId { get; }
    event System.Action<UIWindowId> WindowShown;
}

public interface IUIWindow
{
    void Show();
    void HideWindow();
    
}


public abstract class UIWindowBase : MonoBehaviour, IUIWindow
{
    [Inject] protected IUIManager _uiManager;
    [Inject] protected IGameFlow _gameFlow;
    [Inject] protected IGameProgressService _progress;
    [Inject] protected IDataManager _dataManager;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideWindow()
    {
        gameObject.SetActive(false);
    }
}


public sealed class UIManager : MonoBehaviour, IUIManager
{
    [System.Serializable]
    private struct WindowPrefab
    {
        public UIWindowId Id;
        public UIWindowBase Prefab;
    }

    [SerializeField] private Transform _footer;
    [SerializeField] private Transform windowsRoot;
    [SerializeField] private WindowPrefab[] prefabs;
    

    [Inject] private DiContainer _container;

    private readonly Dictionary<UIWindowId, UIWindowBase> _instances = new();
    private UIWindowBase _current;

    private readonly HashSet<UIWindowId> WindowsWithoutFooter = new()
    {
        UIWindowId.Loading,
        UIWindowId.Pause,
        UIWindowId.Victory,
        UIWindowId.GameOver,
        UIWindowId.Record
    };


    public UIWindowId CurrentWindowId { get; private set; }

    public event System.Action<UIWindowId> WindowShown;

    public void Show(UIWindowId id)
    {
        if (_current != null)
            _current.HideWindow();

        if (!_instances.TryGetValue(id, out var window))
        {
            window = CreateWindow(id);
            _instances[id] = window;
        }

        _current = window;
        CurrentWindowId = id;

        _current.Show();
        WindowShown?.Invoke(id);

        UpdateFooterVisibility(id);
    }

    private void UpdateFooterVisibility(UIWindowId id)
    {
        bool showFooter = !WindowsWithoutFooter.Contains(id);

        if (_footer.gameObject.activeSelf != showFooter)
            _footer.gameObject.SetActive(showFooter);

        if (showFooter)
            _footer.SetAsLastSibling();
    }


    private UIWindowBase CreateWindow(UIWindowId id)
    {
        foreach (var p in prefabs)
        {
            if (p.Id == id)
            {
                var instance = Instantiate(p.Prefab, windowsRoot);
                _container.Inject(instance);
                instance.HideWindow();
                return instance;
            }
        }

        Debug.LogError($"UI prefab for {id} not found");
        return null;
    }
}

