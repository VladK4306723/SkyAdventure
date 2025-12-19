using UnityEngine;
using UnityEngine.UI;

public sealed class ToggleView : MonoBehaviour
{
    [SerializeField] private GameObject _activeState;
    [SerializeField] private GameObject _inactiveState;
    [SerializeField] private Button _button;

    private bool _value;
    private System.Action<bool> _onChanged;

    public void Init(bool startValue, System.Action<bool> onChanged)
    {
        _value = startValue;
        _onChanged = onChanged;
        Apply();

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        _value = !_value;
        Apply();
        _onChanged?.Invoke(_value);
    }

    private void Apply()
    {
        if (_activeState != null)
            _activeState.SetActive(_value);

        if (_inactiveState != null)
            _inactiveState.SetActive(!_value);
    }
}
