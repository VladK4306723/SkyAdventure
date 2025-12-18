using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface IUIManager
{
    void Show(UIWindowId id);
}

public interface IUIWindow
{
    void Show();
    void HideWindow();
}


public abstract class UIWindowBase : MonoBehaviour, IUIWindow
{
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

    [SerializeField] private Transform windowsRoot;
    [SerializeField] private WindowPrefab[] prefabs;

    [Inject] private DiContainer _container;

    private readonly Dictionary<UIWindowId, UIWindowBase> _instances = new();
    private UIWindowBase _current;

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
        _current.Show();
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
