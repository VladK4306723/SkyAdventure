using TMPro;
using UnityEngine;
using Zenject;

public sealed class PickupEffectView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TMP_Text text;

    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float moveUpSpeed = 0.6f;
    [SerializeField] private float startScale = 1.2f;
    [SerializeField] private float endScale = 1f;

    private float _time;
    private Color _iconColor;
    private Color _textColor;

    private Pool _pool;

    public void Init(Sprite sprite, float value)
    {
        icon.sprite = sprite;
        text.text = $"+{Mathf.RoundToInt(value)}";

        _iconColor = icon.color;
        _textColor = text.color;

        _time = 0f;
        transform.localScale = Vector3.one * startScale;
    }

    public void SetPool(Pool pool)
    {
        _pool = pool;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float t = _time / lifetime;

        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * scale;

        float alpha = Mathf.Lerp(1f, 0f, t);
        SetAlpha(alpha);

        if (t >= 1f)
        {
            _pool.Despawn(this);
        }
    }

    private void SetAlpha(float a)
    {
        _iconColor.a = a;
        icon.color = _iconColor;

        _textColor.a = a;
        text.color = _textColor;
    }

    // ───────────────────────── POOL ─────────────────────────

    public sealed class Pool : MonoMemoryPool<PickupEffectView>
    {
        protected override void OnSpawned(PickupEffectView item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(PickupEffectView item)
        {
            item.gameObject.SetActive(false);
        }
    }
}
