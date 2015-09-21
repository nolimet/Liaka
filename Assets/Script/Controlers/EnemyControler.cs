using UnityEngine;
using System.Collections;

public class EnemyControler : MonoBehaviour
{  
    public Transform[] spawnPositions;
    [Range(1,10), Tooltip("increases spawning speed")]
    public int diff = 1;
    bool gamePaused;
    void Start()
    {
        GameManager.instance.onPauseGame += GamePaused;
        StartCoroutine(gameLoop());
    }

    void Destory()
    {
        GameManager.instance.onPauseGame -= GamePaused;
    }

    void GamePaused(bool b)
    {
        if (!this)
            return;

        gamePaused = b;
        if (!b)
            StartCoroutine(gameLoop());
    }

    void Spawn()
    {
        BaseObject b = BasePool.GetObject(BaseObject.objectType.Enemy);
        if (spawnPositions.Length > 0)
            b.transform.position = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
    }

    IEnumerator gameLoop()
    {
        float d = 2f;
        while (!gamePaused)
        {
            if (d <= 0)
            {
                Spawn();
                d = Random.Range(0.3f, 5 / Mathf.Pow(1.2f,diff));
            }

            d -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
