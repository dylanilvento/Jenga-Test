using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    List<BrickManager> brickManagers = new List<BrickManager>();

    [SerializeField]
    public ApiStackData[] apiStackDataList;

    string url = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;

        GetData();
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

    public void GetData()
    {
        StartCoroutine(FetchData());
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
}
