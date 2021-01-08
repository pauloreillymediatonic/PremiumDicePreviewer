using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
    public RectTransform rectTransform;

    public void Toggle()
    {
        rectTransform.gameObject.SetActive(!rectTransform.gameObject.activeSelf);
    }
}
