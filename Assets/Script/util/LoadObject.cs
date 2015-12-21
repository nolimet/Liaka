using UnityEngine;
using System.Collections;
namespace util
{
    public class LoadObject : MonoBehaviour
    {

        public static string LevelToLoad = "";
        public float waitTime;
        public float waitTimeMax;
        public bool lvlLoadingDone;

        SegmentedBar b;

        public static void LoadLevelAsync(string level, float waitTime)
        {
            GameObject G = Instantiate(Resources.Load("Object/LoadObject"), Vector3.zero, Quaternion.identity) as GameObject;

            G.GetComponent<LoadObject>().BeginLoading(level, waitTime);
        }

        void Start()
        {
            DontDestroyOnLoad(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (lvlLoadingDone)
            {

                waitTime -= Time.deltaTime;
                b.Value = ((waitTimeMax - waitTime) / waitTimeMax) * 0.6f;
                if (waitTime < 0 && waitTime > -1)
                {
                    waitTime = -10;
                    StartCoroutine(Loader(LevelToLoad));
                }
            }
        }

        public void BeginLoading(string lvl, float waitTime)
        {
            this.waitTime = waitTime;
            waitTimeMax = waitTime;
            LevelToLoad = lvl;
            StartCoroutine(Loader("Game-Load"));
        }

        IEnumerator Loader(string levelName)
        {
            AsyncOperation async = Application.LoadLevelAsync(levelName);
            while (!async.isDone)
            {
                if (lvlLoadingDone)
                    b.Value = 0.6f + (async.progress) * 0.4f;
                yield return async.isDone;
                //LoadingBar.setValue(async.progress);
                //Debug.Log("Loading at " + async.progress);
                yield return new WaitForEndOfFrame();
            }

            //Debug.Log("Completed Loading of :: " + levelName);
            if (lvlLoadingDone)
                Destroy(gameObject);
            else
            {
                lvlLoadingDone = true;
                b = FindObjectOfType<SegmentedBar>();
            }
        }
    }
}