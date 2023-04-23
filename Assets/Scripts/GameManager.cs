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
    GameObject glassBrickPrefab,
        woodBrickPrefab,
        stoneBrickPrefab;

    string url = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;

        StartCoroutine(GetData());
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

    public IEnumerator GetData()
    {
        yield return StartCoroutine(FetchData());

        SortGrades();
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
                Debug.Log(requestText);

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
}
