using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableAndStackController : MonoBehaviour
{
    [Space(20)]
    [SerializeField]
    public GameObject[] stacks;

    public static TableAndStackController Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") > 0)
            {
                gameObject.transform.RotateAround(Vector3.zero, Vector3.up, -0.25f);
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                gameObject.transform.RotateAround(Vector3.zero, Vector3.up, 0.25f);
            }
        }
    }
}
