using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableAndStackController : MonoBehaviour
{
    [Space(20)]
    [SerializeField]
    public GameObject[] stacks;

    Vector3 rotationFocus = Vector3.zero;

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
                gameObject.transform.RotateAround(rotationFocus, Vector3.up, -0.25f);
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                gameObject.transform.RotateAround(rotationFocus, Vector3.up, 0.25f);
            }
        }
    }

    public void SwitchRotationFocus(int grade)
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = Vector3.zero;

        if (grade == 6)
        {
            transform.position = new Vector3(3f, 0, 0);
            rotationFocus = stacks[0].transform.position;
        }
        else if (grade == 7)
        {
            rotationFocus = stacks[1].transform.position;
        }
        else if (grade == 8)
        {
            transform.position = new Vector3(-3f, 0, 0);
            rotationFocus = stacks[2].transform.position;
        }
    }
}
