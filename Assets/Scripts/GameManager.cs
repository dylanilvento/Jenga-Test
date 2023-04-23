using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    List<BrickManager> brickManagers = new List<BrickManager>();

    [SerializeField]
    public ApiStackData[] apiStackDataList;

    public List<ApiStackData> sixthGrade = new List<ApiStackData>();
    public List<ApiStackData> seventhGrade = new List<ApiStackData>();
    public List<ApiStackData> eighthGrade = new List<ApiStackData>();

    [SerializeField]
    GameObject stackRowGroupPrefab;

    string url = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

    public static GameManager Instance;

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

    public IEnumerator GetDataAndSpawnStacks()
    {
        yield return StartCoroutine(FetchData());

        SortGrades();

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
            for (int ii = 0; ii < stack.Count; ii += 3)
            {
                ApiStackData leftBrick = stack[ii];
                ApiStackData middleBrick = ii + 1 < stack.Count ? stack[ii + 1] : null;
                ApiStackData rightBrick = ii + 2 < stack.Count ? stack[ii + 2] : null;
                yield return new WaitForSeconds(0.25f);
                SpawnStackRowGroup(leftBrick, middleBrick, rightBrick);
            }
        }
    }

    void SpawnStackRowGroup(
        ApiStackData leftBrick,
        ApiStackData middleBrick,
        ApiStackData rightBrick
    )
    {
        StackController currentStack = GetAssignedGradeStack(leftBrick.grade);

        Vector3 spawnPosition = new Vector3(
            currentStack.transform.position.x,
            currentStack.transform.position.y + 10f,
            currentStack.transform.position.z
        );

        GameObject spawnedStackRow = Instantiate(
            stackRowGroupPrefab,
            spawnPosition,
            Quaternion.identity,
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
}
