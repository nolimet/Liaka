using UnityEngine;
using System.Collections;

public class PickupControler : MonoBehaviour
{
    [Tooltip("Pickups spawn between low spawn and high spawn"), SerializeField]
    Transform HighSpawn = null, LowSpawn = null;
    [SerializeField]
    PickupBase.PickupType[] spawnable = null;
    [SerializeField]
    float MinVelocity=0, MaxVelocity=0;

    bool gameloopRunning = false;
    bool gamePaused = false;
    [SerializeField]
    bool bossCharging = false;
    bool bossAttacking = false;

    public bool keepDisabled;

    // Use this for initialization
    void Awake()
    {
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;
    }

    void Start()
    {
        FindObjectOfType<BossControler>().bossMove.onMoveChangeEarly += setlocalMoveDirBoss;

        foreach (PickupBase.PickupType p in spawnable)
        {
            for (int i = 0; i < 10; i++)
            {
                PickupPool.GetObject(p);
            }
        }

        PickupPool.RemoveAllImmediate();
    }

    public void OnEnable()
    {
        if (keepDisabled)
            enabled = false;
        else
            StartCoroutine(gameLoop());
    }

    public void OnDisable()
    {
        gameloopRunning = false;
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

        while (gameloopRunning && isActiveAndEnabled)
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
