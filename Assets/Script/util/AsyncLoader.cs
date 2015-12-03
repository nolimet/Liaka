using UnityEngine;
using System.Collections;

public class AsyncLoader : MonoBehaviour {
    public MaskBar LoadingBar;

    void Start()
    {
        gameObject.SetActive(false);
    }

	public void Load(string level)
    {
        gameObject.SetActive(true);
        StartCoroutine(Loader(level));
    }

    IEnumerator Loader(string levelName)
    {
        
        AsyncOperation async = Application.LoadLevelAsync(levelName);
        while (!async.isDone)
        {
            yield return async.isDone;
            LoadingBar.setValue(async.progress);
            //Debug.Log("Loading at " + async.progress);
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("Completed Loading of :: " + levelName);

        gameObject.SetActive(false);
    }
}
