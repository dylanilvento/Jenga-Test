using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextResizer : MonoBehaviour
{
    TextMeshProUGUI tmp;

    RectTransform rectTransform;

    float preferredHeight;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        SetHeight();
    }

    void SetHeight()
    {
        if (tmp == null)
            return;

        preferredHeight = tmp.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    }
}
