using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrickType
{
    Glass,
    Wood,
    Stone
}

public class BrickManager : MonoBehaviour
{
    public BrickType brickType;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddToBrickManagerList(this);
    }

    // Update is called once per frame
    void Update() { }
}
