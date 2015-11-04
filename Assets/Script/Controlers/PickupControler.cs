using UnityEngine;
using System.Collections;

public class PickupControler : MonoBehaviour
{
    [Tooltip("Pickups spawn between low spawn and high spawn"), SerializeField]
    Transform HighSpawn, LowSpawn;
    [SerializeField]
    PickupBase.PickupType[] spawnable;
    [SerializeField]
    float MinVelocity, MaxVelocity;

    bool gameloopRunning;
    bool gamePaused;
    [SerializeField]
    bool bossCharging = false;
    bool bossAttacking = false;
    // Use this for initialization
    void Start()
    {
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        FindObjectOfType<BossControler>().bossMove.onMoveChangeEarly += setlocalMoveDirBoss;

        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;

        StartCoroutine(gameLoop());

        foreach(PickupBase.PickupType p in spawnable)
        {
            for (int i = 0; i < 10; i++)
            {
                PickupPool.GetObject(p);
            }    
        }

        PickupPool.RemoveAllImmediate();
    }

    private void StageControler_onBossBattleEnds()
    {
        bossAttacking = false;
    }

    private void StageControler_onBossBattleBegins()
    {
        bossAttacking = true;
        PickupPool.RemoveAll();
    }

    public void OnDestroy()
    {
        //GameManager.instance.onPauseGame -= Instance_onPauseGame;
        //FindObjectOfType<BossControler>().bossMove.onMoveChangeEarly -= setlocalMoveDirBoss;

        StageControler.onBossBattleBegins -= StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds -= StageControler_onBossBattleEnds;
    }

    private void setlocalMoveDirBoss(BossMove.moveDir d)
    {
        if (d == BossMove.moveDir.right)
        {
            bossCharging = true;
            Spawn_Boost();
        }
        else
            bossCharging = false;
    }

    private void Instance_onPauseGame(bool b)
    {
        gamePaused = b;
    }

    IEnumerator gameLoop()
    {
        if (gameloopRunning)
            yield break;

        gameloopRunning = true;

        while (gameloopRunning)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            while (gamePaused || bossAttacking)
                yield return new WaitForEndOfFrame();
            if (bossCharging)
                Spawn_Boost();
            Spawn();
        }
        gameloopRunning = false;
    }


    void Spawn()
    {
        setupPickup(PickupPool.GetObject(spawnable[Random.Range(0, spawnable.Length)]));
    }

    public void Spawn_Boost()
    {
        setupPickup(PickupPool.GetObject(PickupBase.PickupType.SpeedUp));
    }

    void setupPickup(PickupBase p)
    {
        p.transform.position = GetPos();
        p.SetMovementType(PickupBase.Movement.Static);
        p.setVelocity(new Vector2(-Random.Range(MinVelocity, MaxVelocity), 0));
    }

    Vector3 GetPos()
    {
        return new Vector3(LowSpawn.position.x, Random.Range(HighSpawn.position.y, LowSpawn.position.y), LowSpawn.position.z);
    }
}
