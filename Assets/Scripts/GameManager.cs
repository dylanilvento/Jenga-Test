using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<BrickManager> brickManagers = new List<BrickManager>();

    List<StackRowGroupManager> rowGroupManagers = new List<StackRowGroupManager>();

    [SerializeField]
    public ApiStackData[] apiStackDataList;

    public List<ApiStackData> sixthGrade = new List<ApiStackData>();
    public List<ApiStackData> seventhGrade = new List<ApiStackData>();
    public List<ApiStackData> eighthGrade = new List<ApiStackData>();

    [SerializeField]
    GameObject stackRowGroupPrefab;

    string url = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

    public static GameManager Instance;

    [SerializeField]
    Button testMyStackButton,
        resetButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;

        StartCoroutine(GetDataAndSpawnStacks());
    }

    // Update is called once per frame
    void Update() { }

    public void AddToBrickManagerList(BrickManager brickManager)
    {
        brickManagers.Add(brickManager);
    }

    public void AddToStackRowGroupManagersList(StackRowGroupManager rowGroupManager)
    {
        rowGroupManagers.Add(rowGroupManager);
    }

    void DestroyAllBricks()
    {
        foreach (BrickManager brick in brickManagers)
        {
            brick.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            Destroy(brick.gameObject);
        }

        brickManagers.Clear();
    }

    public void DestroyAllGlassBricks()
    {
        testMyStackButton.interactable = false;

        List<BrickManager> brickManagersToRemove = new List<BrickManager>();

        foreach (BrickManager brick in brickManagers)
        {
            brick.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if (brick.brickType == BrickType.Glass)
            {
                brickManagersToRemove.Add(brick);
                Destroy(brick.gameObject);
            }
        }

        foreach (BrickManager brick in brickManagersToRemove)
        {
            brickManagers.Remove(brick);
        }

        resetButton.interactable = true;
    }

    public IEnumerator GetDataAndSpawnStacks()
    {
        yield return StartCoroutine(FetchData());

        testMyStackButton.interactable = false;

        if (sixthGrade.Count == 0)
        {
            SortGrades();
        }

        StartCoroutine(SpawnStacks());
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string requestText = request.downloadHandler.text.ToString();

                apiStackDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiStackData[]>(
                    requestText
                );
            }
        }
    }

    void SortGrades()
    {
        foreach (ApiStackData stack in apiStackDataList)
        {
            if (stack.grade.Contains("6th"))
            {
                sixthGrade.Add(stack);
            }
            if (stack.grade.Contains("7th"))
            {
                seventhGrade.Add(stack);
            }
            if (stack.grade.Contains("8th"))
            {
                eighthGrade.Add(stack);
            }
        }

        foreach (
            List<ApiStackData> group in new List<List<ApiStackData>>
            {
                sixthGrade,
                seventhGrade,
                eighthGrade
            }
        )
        {
            //apparently creates an unstable sort?
            group.Sort(
                (x, y) =>
                {
                    var order = x.domain.CompareTo(y.domain);
                    if (order == 0)
                        order = x.cluster.CompareTo(y.cluster);
                    if (order == 0)
                        order = x.standardid.CompareTo(y.standardid);
                    return order;
                }
            );
        }
    }

    IEnumerator SpawnStacks()
    {
        foreach (
            List<ApiStackData> stack in new List<List<ApiStackData>>
            {
                sixthGrade,
                seventhGrade,
                eighthGrade
            }
        )
        {
            int stackCount = 0;
            for (int ii = 0; ii < stack.Count; ii += 3)
            {
                ApiStackData leftBrick = stack[ii];
                ApiStackData middleBrick = ii + 1 < stack.Count ? stack[ii + 1] : null;
                ApiStackData rightBrick = ii + 2 < stack.Count ? stack[ii + 2] : null;

                float rotationZ = (stackCount % 2 == 0) ? 90f : 0;
                stackCount++;

                yield return new WaitForSeconds(0.25f);
                SpawnStackRowGroup(leftBrick, middleBrick, rightBrick, rotationZ);
            }
        }

        testMyStackButton.interactable = true;
    }

    void SpawnStackRowGroup(
        ApiStackData leftBrick,
        ApiStackData middleBrick,
        ApiStackData rightBrick,
        float rotationZ
    )
    {
        StackController currentStack = GetAssignedGradeStack(leftBrick.grade);

        Vector3 spawnRotation = new Vector3(0, rotationZ, 0);

        Vector3 spawnPosition = new Vector3(
            currentStack.transform.position.x,
            currentStack.transform.position.y + 10f,
            currentStack.transform.position.z
        );

        GameObject spawnedStackRow = Instantiate(
            stackRowGroupPrefab,
            spawnPosition,
            Quaternion.Euler(spawnRotation),
            currentStack.transform
        );
        // yield return new WaitForSeconds(0.5f);
        spawnedStackRow
            .GetComponent<StackRowGroupManager>()
            .SpawnBricks(leftBrick, middleBrick, rightBrick);
    }

    StackController GetAssignedGradeStack(string grade)
    {
        if (grade.Contains("6th"))
        {
            return TableAndStackController.Instance.stacks[0].GetComponent<StackController>();
        }
        else if (grade.Contains("7th"))
        {
            return TableAndStackController.Instance.stacks[1].GetComponent<StackController>();
        }
        else if (grade.Contains("8th"))
        {
            return TableAndStackController.Instance.stacks[2].GetComponent<StackController>();
        }

        return null;
    }

    public void ResetStacks()
    {
        testMyStackButton.interactable = false;
        resetButton.interactable = false;

        DestroyAllBricks();

        DestroyAllRowGroupManagers();

        StartCoroutine(SpawnStacks());
    }

    void DestroyAllRowGroupManagers()
    {
        foreach (StackRowGroupManager groupManager in rowGroupManagers)
        {
            Destroy(groupManager.gameObject);
        }
        rowGroupManagers.Clear();
    }
}
