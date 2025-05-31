using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HealthStatus : MonoBehaviour
{
    public Transform anchor;

    RectTransform _rectTransform;
    TextMeshProUGUI _text;
    Canvas _canvas;

    public void Init()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();

        _rectTransform.SetParent(_canvas.transform);

        _text = GetComponent<TextMeshProUGUI>();

        //FixPositionAtStart();
    }

    //async void FixPositionAtStart()
    //{
    //    await UniTask.Delay(10);
    //    UpdatePosition();
    //}

    private void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        _rectTransform.position = Camera.main.WorldToScreenPoint(anchor.position + Vector3.up);

        float f = 7.0f / Vector3.Distance(anchor.position, Camera.main.transform.position);
        _rectTransform.localScale = new Vector3(f, f, f);
    }
    public void SetHealthText(float health, float maxHealth)
    {
        _text.text = health.ToString("0") + "/" + maxHealth.ToString("0");
    }
}
