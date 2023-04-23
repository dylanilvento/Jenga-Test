using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<BrickManager> brickManagers = new List<BrickManager>();

    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update() { }

    public void AddToBrickManagerList(BrickManager brickManager)
    {
        brickManagers.Add(brickManager);
    }

    public void DestroyAllGlassBricks()
    {
        foreach (BrickManager brick in brickManagers)
        {
            if (brick.brickType == BrickType.Glass)
            {
                Destroy(brick.gameObject);
            }
        }
    }
}
