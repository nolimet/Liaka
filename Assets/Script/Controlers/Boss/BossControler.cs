using UnityEngine;
using System.Collections;

public class BossControler : MonoBehaviour
{
    public delegate void VoidDelegate();
    public static event VoidDelegate onPlayerHit, onEnemyHit, onDefeated;


    public BossFight bossFight;
    public BossMove bossMove;
    [SerializeField]
    BossAnimation bossAnimator;

    //number of enemies that hit the boss;
    int noEnemiesThatHitBoss;


    void Awake()
    {
        if (!bossMove)
            bossMove = GetComponent<BossMove>();
        if (!bossAnimator)
            bossAnimator = GetComponent<BossAnimation>();
        if (!bossFight)
            bossFight = GetComponent<BossFight>();
    }
    void Start()
    {
        BaseObject.onHitBose += BaseObject_onHitBose;

        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;

        bossFight.onOutOfHP += bossFight_onOutOfHP;

        bossMove.onMoveChange += bossMove_onMoveChange;

        bossFight.onBadHit += () => bossAnimator.Fight_onBadHit();
        bossFight.onGoodHit += () => bossAnimator.Fight_onGoodHit();
        bossFight.onPerfectHit += () => bossAnimator.Fight_onPerfectHit();
    }

    public void OnEnable()
    {
        bossFight.gameObject.SetActive(true);
        bossMove.gameObject.SetActive(true);
        bossAnimator.gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        bossFight.gameObject.SetActive(false);
        bossMove.gameObject.SetActive(false);
        bossAnimator.gameObject.SetActive(false);

    }

    public void OnDestroy()
    {
        BaseObject.onHitBose -= BaseObject_onHitBose;

        StageControler.onBossBattleBegins -= StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds -= StageControler_onBossBattleEnds;
    }

    private void bossMove_onMoveChange(BossMove.moveDir moveDir)
    {
        bossAnimator.MoveChange((int)moveDir);
    }

    private void BaseObject_onHitBose(BaseObject.objectType o)
    {
        if (o == BaseObject.objectType.Enemy)
        {
            noEnemiesThatHitBoss++;
            bossAnimator.OnEnemyHit();
            if (onEnemyHit != null)
                onEnemyHit();
        }
    }

    private void StageControler_onBossBattleEnds()
    {
        bossFight.enabled = false;
        bossMove.enabled = true;
    }

    private void StageControler_onBossBattleBegins()
    {
        bossFight.enabled = true;
        bossMove.enabled = false;
    }

    private void bossFight_onOutOfHP()
    {
        if (onDefeated != null)
            onDefeated();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == TagManager.Player)
        {
            if (onPlayerHit != null)
                onPlayerHit();
            bossAnimator.OnPlayerHit();
        }

        if (collision.transform.tag == TagManager.Ground)
        {
            gameObject.layer = LayerMask.NameToLayer("Boss");
            Rigidbody2D ri = GetComponent<Rigidbody2D>();
            ri.isKinematic = true;
            ri.gravityScale = 0;
        }
    }
}
