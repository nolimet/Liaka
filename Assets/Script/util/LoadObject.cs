using UnityEngine;
using System.Collections;

public class LoadObject : MonoBehaviour
{

    public static string LevelToLoad = "";
    public float waitTime;
    public bool lvlLoadingDone;
    public static void LoadLevelAsync(string level, float waitTime)
    {
        GameObject G = Instantiate(Resources.Load("Object/LoadObject"), Vector3.zero,Quaternion.identity) as GameObject;

        G.GetComponent<LoadObject>().BeginLoading(level, waitTime);
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(lvlLoadingDone)
        {
            waitTime -= Time.deltaTime;
            if(waitTime<0 && waitTime > -1)
            {
                waitTime = -10;
                StartCoroutine(Loader(LevelToLoad));
            }
        }
    }

    public void BeginLoading(string lvl, float waitTime)
    {
        this.waitTime = waitTime;
        LevelToLoad = lvl;
        StartCoroutine(Loader("Game-Load"));
    }

    IEnumerator Loader(string levelName)
    {
        AsyncOperation async = Application.LoadLevelAsync(levelName);
        while (!async.isDone)
        {
            yield return async.isDone;
            //LoadingBar.setValue(async.progress);
            //Debug.Log("Loading at " + async.progress);
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("Completed Loading of :: " + levelName);
        if (lvlLoadingDone)
            Destroy(gameObject);
        else
            lvlLoadingDone = true;
    }
}
