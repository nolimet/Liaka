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
    bool gamePaused, bossBattle, GameLoopStarted;

    public bool keepDisabled;

    void Awake()
    {
        GameManager.instance.onPauseGame += GamePaused;
        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;
    }

    void Start()
    {
        //StartCoroutine(gameLoop());

        EnemyBase e;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                e = EnemyPool.GetObject((EnemyBase.Enemytype)i);
                e.transform.position = new Vector3(e.transform.position.x, e.transform.position.y, 0);
                e.RemoveObject(0, 0);
            }
        }

        EnemyPool.RemoveAllImmediate();
    }



    private void StageControler_onBossBattleEnds()
    {
        bossBattle = false;
        if (this && enabled)
            StartCoroutine(gameLoop());
        else
        {
            StageControler.onBossBattleBegins -= StageControler_onBossBattleBegins;
            StageControler.onBossBattleEnds -= StageControler_onBossBattleEnds;
        }
    }

    private void StageControler_onBossBattleBegins()
    {
        bossBattle = true;
        EnemyPool.RemoveAll();
    }

    void Destory()
    {
        GameManager.instance.onPauseGame -= GamePaused;
        StageControler.onBossBattleBegins -= StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds -= StageControler_onBossBattleEnds;
    }

    public void OnEnable()
    {
        if (keepDisabled)
            enabled = false;
        else
            StartCoroutine(gameLoop());
    }

    void GamePaused(bool b)
    {
        if (!this)
            return;

        gamePaused = b;
    }

    void Spawn()
    {
        EnemyBase b = EnemyPool.GetObject((EnemyBase.Enemytype)Random.Range(0,3));
        //EnemyBase b = EnemyPool.GetObject(EnemyBase.Enemytype.walking);
        Vector3 p = Vector3.zero;
        switch (b.etype)
        {
            case EnemyBase.Enemytype.flying:
                p = new Vector3(FlyingSpawnPoints.Low.position.x, Random.Range(FlyingSpawnPoints.Low.position.y, FlyingSpawnPoints.High.position.y), FlyingSpawnPoints.Low.position.z);
                break;

            case EnemyBase.Enemytype.walking:
                p = GroundSpawnPoints.position;
                break;

            case EnemyBase.Enemytype.flying_high:
                p = FlyingSpawnPoints.High.position;
                break;

            default:
                Debug.LogError("unknown enemy: " + b.etype);
                break;
        }

        b.setPosition(p);
    }

    IEnumerator gameLoop()
    {
        float d = 2f;
        while (true)
        {
            if (!gamePaused && !bossBattle)
            {
                if (d <= 0)
                {
                    Spawn();
                    d = Random.Range(0.3f, 5 / Mathf.Pow(1.2f, diff));
                }

                d -= Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
