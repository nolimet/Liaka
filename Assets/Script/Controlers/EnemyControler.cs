using UnityEngine;
using System.Collections;

public class EnemyControler : MonoBehaviour
{
    
    [System.Serializable]
    public struct twoT
    {
        public Transform Low, High;
    }
    public Transform GroundSpawnPoints;
    public twoT FlyingSpawnPoints;

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
        EnemyBase b = EnemyPool.GetObject(EnemyBase.Enemytype.flying);
        switch (b.etype)
        {
            case EnemyBase.Enemytype.flying:
                b.transform.position = new Vector3(FlyingSpawnPoints.Low.position.x, Random.Range(FlyingSpawnPoints.Low.position.y, FlyingSpawnPoints.High.position.y), FlyingSpawnPoints.Low.position.z);
                break;

            case EnemyBase.Enemytype.walking:
                b.transform.position = GroundSpawnPoints.position;
                break;
        }   
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
