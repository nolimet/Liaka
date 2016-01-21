using UnityEngine;
using System.Collections;

public class StageControler : MonoBehaviour
{
    public delegate void StageControlerEvent(StageControler stage);
    public static event StageControlerEvent onStageCreated, onStageDestroyed;
    public delegate void DelegateVoid();
    public delegate void DelegateBool(bool b);
    public static event DelegateVoid onStageTimerEnded, onBossBattleBegins, onBossBattleEnds;

    [Tooltip("The time in seconds the stage takes")]
    public float StageLength, BossBattleLength;
    public bool bossFighting
    {
        get
        {
            return _bossFighting;
        }
    }
    public bool bossDefeated
    {
        get
        {
            return _bossDefeated;
        }
    }

    public int coinsCollected
    {
        get
        {
            return _coinsCollected;
        }
    }

    public string NextStageName = "";

    float TimeLeft;
    bool _bossFighting, _bossDefeated;

#if UNITY_EDITOR
    public bool DisableTimer;
#endif

    int _coinsCollected;

    public PortalControler portalControler;
    public PlayerDeath PlayerDeathAnimationControler;
    public EnemyControler eControl;
    public PickupControler pControl;
    public BossControler bossControler;

    // Use this for initialization
    void Awake()
    {
        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        BossControler.onDefeated += BossControler_Defeated;

        onBossBattleBegins += StageControler_onBossBattleBegins;
        onBossBattleEnds += StageControler_onBossBattleEnds;
    }

    void Start()
    {
        TimeLeft = StageLength;

        if (onStageCreated != null)
            onStageCreated(this);

        if (onBossBattleEnds != null)
            onBossBattleEnds();

        Debug.LogWarning("BOSS BATTLE DISABLED IN STAGE CONTROLER STARTUP!!! DO NOT FORGET YOU FOOL!");
        BossBattleLength = 0.1f;

        _bossDefeated = true;

        if (portalControler == null)
            portalControler = FindObjectOfType<PortalControler>();

        if (PlayerDeathAnimationControler == null)
            PlayerDeathAnimationControler = FindObjectOfType<PlayerDeath>();
    }

    public void OnDestroy()
    {
        if (onStageDestroyed != null)
            onStageDestroyed(this);

        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
        BossControler.onDefeated -= BossControler_Defeated;

        onBossBattleBegins -= StageControler_onBossBattleBegins;
        onBossBattleEnds -= StageControler_onBossBattleEnds;

        if (GameManager.instance)
            GameManager.instance.GameEnd(_coinsCollected);
    }

    #region Events

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        p.onDeath += Player_onDeath;
        p.onEnergyZero += Player_onEnergyZero;
        p.onCoinPickup += P_onCoinPickup;
        p.onCoinsLost += P_onCoinsLost;
    }

    private void P_onCoinsLost(int i)
    {
        int o = i;
        _coinsCollected -= i;
        if (_coinsCollected < 0)
        {
            o += _coinsCollected;
            _coinsCollected = 0;
        }

        if (o > 0)
            dropCoins(o * 3); 
    }

    private void P_onCoinPickup()
    {
        _coinsCollected++;
    }

    private void Player_onEnergyZero()
    {
        playerDefeated();
    }

    private void Player_onDeath()
    {
        playerDefeated();
    }

    private void StageControler_onBossBattleEnds()
    {
        _bossFighting = false;

        if (_bossDefeated)
        {
            playerWon();
        }

    }

    private void StageControler_onBossBattleBegins()
    {
        _bossFighting = true;
    }

    private void BossControler_Defeated()
    {
        _bossDefeated = true;
        
    }
    #endregion
    void Update()
    {
        if (GameManager.gamePaused)
            return;
#if UNITY_EDITOR
        if(!DisableTimer)
#endif
        if(TimeLeft>0)
            TimeLeft -= Time.deltaTime;

        if (TimeLeft<0 && TimeLeft>-1)
        {
            TimeLeft = -20;
            if (onStageTimerEnded != null)
                onStageTimerEnded();

            if (_bossFighting)
            {
                if (onBossBattleEnds != null)
                    onBossBattleEnds();

                TimeLeft = StageLength;
            }
            else
            {
                if (onBossBattleBegins != null)
                    onBossBattleBegins();

                TimeLeft = BossBattleLength;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (onBossBattleEnds != null)
                onBossBattleEnds();

            TimeLeft = StageLength;
            _bossFighting = false;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (onBossBattleBegins != null)
                onBossBattleBegins();

            _bossFighting = true;
            TimeLeft = BossBattleLength;
        }

        if (Input.GetKeyDown(KeyCode.X))
            dropCoins(10);

        if (Input.GetKeyDown(KeyCode.C))
            _coinsCollected += 4;

        if (Input.GetKeyDown(KeyCode.V))
            TimeLeft = 1f;


        util.Debugger.Log("coins" , " current: " + coinsCollected);
    }

    public float NormalizedTimeLeft()
    {
        if (!_bossFighting)
            return TimeLeft / StageLength;
        else
            return TimeLeft / BossBattleLength;
    }

    void dropCoins(int l)
    {
        PickupBase p;
        for (int i = 0; i < l; i++)
        {
             p = PickupPool.GetObject(PickupBase.PickupType.Coin);

            p.ignorPlayer();
            p.transform.position = GameManager.playerControler.transform.position;
            p.SetMovementType(PickupBase.Movement.Dynamic);
            p.setVelocity(Random.Range(6f, 9f) * util.Common.AngleToVector(Random.Range(45f, 45 + 90f)));

            p.FadeOutStart(0.4f, 2f);
        }
    }

    void playerDefeated(bool animPlayed = false)
    {
        if (animPlayed)
            util.LoadObject.LoadLevelAddative("Game-End-Lose");
        else
        {
            if (deathAnimePlaying)
                return;

            eControl.keepDisabled = true;
            pControl.keepDisabled = true;


            eControl.gameObject.SetActive(false);
            pControl.gameObject.SetActive(false);
            bossControler.enabled = false;

            PlayerDeathAnimationControler.onAnimationDone += PlayerDeathAnimationDone;
            PlayerDeathAnimationControler.StartAni();
            deathAnimePlaying = true;
        }
        //Application.LoadLevel("Game-End-Lose");
    }

    void playerWon(bool animPlayed = false)
    {
        if (animPlayed)
        {
            if (NextStageName == "")
                //util.LoadObject.LoadLevelAsync("Game-End-Win", 1.5f);
                util.LoadObject.LoadLevelAddative("Game-End-Win");
            //Application.LoadLevel("Game-End-Win");
            else
                util.LoadObject.LoadLevelAsync(NextStageName, 1.5f);
            //Application.LoadLevel(NextStageName);

        }
        else
        {
            if (portalWorking)
                return;

            eControl.keepDisabled = true;
            pControl.keepDisabled = true;
           

            eControl.gameObject.SetActive(false);
            pControl.gameObject.SetActive(false);
            bossControler.enabled = false;

            portalControler.StartMove();
            portalControler.onMoveDone += PortalAniDone;
            portalWorking = true;
        }
    }

    bool portalWorking = false;
    void PortalAniDone()
    {
        portalControler.onMoveDone -= PortalAniDone;
        playerWon(true);
    }

    bool deathAnimePlaying = false;
    void PlayerDeathAnimationDone()
    {
        PlayerDeathAnimationControler.onAnimationDone -= PlayerDeathAnimationDone;
        playerDefeated(true);
        enabled = false;
    }
}
