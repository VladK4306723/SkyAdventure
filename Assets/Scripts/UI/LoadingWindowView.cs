using UnityEngine;
using Zenject;

public class LoadingWindowView : UIWindowBase
{
    private void Start()
    {
        Invoke(nameof(OnLoadingComplete), 2f);
    }

    private void OnLoadingComplete()
    {
        _uiManager.Show(UIWindowId.Home);
    }
}
