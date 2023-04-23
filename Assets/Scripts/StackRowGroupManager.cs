using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackRowGroupManager : MonoBehaviour
{
    [SerializeField]
    Transform leftBrickSpawn,
        middleBrickSpawn,
        rightBrickSpawn;

    [SerializeField]
    GameObject glassBrickPrefab,
        woodBrickPrefab,
        stoneBrickPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddToStackRowGroupManagersList(this);
    }

    // Update is called once per frame
    void Update() { }

    public void SpawnBricks(
        ApiStackData leftBrick,
        ApiStackData middleBrick,
        ApiStackData rightBrick
    )
    {
        SpawnBrick(leftBrickSpawn, leftBrick);
        SpawnBrick(middleBrickSpawn, middleBrick);
        SpawnBrick(rightBrickSpawn, rightBrick);
    }

    void SpawnBrick(Transform spawn, ApiStackData data)
    {
        if (data == null)
            return;

        GameObject brickPrefab = null;

        if (data.mastery == 0)
        {
            brickPrefab = glassBrickPrefab;
        }
        else if (data.mastery == 1)
        {
            brickPrefab = woodBrickPrefab;
        }
        else if (data.mastery == 2)
        {
            brickPrefab = stoneBrickPrefab;
        }

        if (brickPrefab != null)
        {
            GameObject spawnedBrick = Instantiate(
                brickPrefab,
                spawn.position,
                transform.rotation,
                this.transform
            );

            spawnedBrick.GetComponent<BrickManager>().AssignBrickData(data);
        }
    }
}
