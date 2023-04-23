using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickInfoBoxManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI infoText;

    public static BrickInfoBoxManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(
            transform.position.x,
            Screen.height - (GetComponent<RectTransform>().sizeDelta.y)
        );
    }

    public void SetInfoText(ApiStackData data)
    {
        string text =
            data.grade
            + ": "
            + data.domain
            + "\n\n"
            + data.cluster
            + "\n\n"
            + data.standardid
            + ": "
            + data.standarddescription;

        infoText.text = text;
    }
}
