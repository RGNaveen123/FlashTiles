using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float scaleAmount = 0.9f;
    public float scaleSpeed = 8f;

    private Vector3 originalScale;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        transform.localScale = originalScale;
    }

    void Update()
    {
        if (isPressed)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleAmount, Time.deltaTime * scaleSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed);
        }
    }
}
