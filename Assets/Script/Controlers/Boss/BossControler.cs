using UnityEngine;
using System.Collections;

public class BossControler : MonoBehaviour
{
    public delegate void VoidDelegate();
    public static event VoidDelegate onPlayerHit;


    public BossFight bossFight;
    public BossMove bossMove;
    [SerializeField]
    BossAnimation bossAnimator;
    
    //number of enemies that hit the boss;
    int noEnemiesThatHitBoss;

    void Start()
    {
        if (!bossMove)
            bossMove = GetComponent<BossMove>();
        if (!bossAnimator)
            bossAnimator = GetComponent<BossAnimation>();
        if (!bossFight)
            bossFight = GetComponent<BossFight>();

        BaseObject.onHitBose += BaseObject_onHitBose;

        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;

        if (bossMove)
            bossMove.onMoveChange += bossMove_onMoveChange;
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == TagManager.Player)
        {
            if (onPlayerHit != null)
                onPlayerHit();
        }
    }
}
