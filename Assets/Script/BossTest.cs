using UnityEngine;
using System.Collections;

public class BossTest : MonoBehaviour
{
    [SerializeField]
    Vector3 posDist,SelfStart;
    float tx;

    void Start()
    {
        BaseObject.onHitBose += hit;
        GameManager.uiControler.attackSlider.onAttack += AttackSlider_onAttack;
        
        SelfStart = transform.position;
    }

    public void OnDestroy()
    {
        GameManager.uiControler.attackSlider.onAttack -= AttackSlider_onAttack;
        BaseObject.onHitBose -= hit;
    }

    private void AttackSlider_onAttack(float f, AttackSlider.state preformance)
    {
        //event handler for attack Slider;
        if (preformance == AttackSlider.state.good)
            tx -= 5 / 40f;
        if (preformance == AttackSlider.state.perfect)
            tx -= 10 / 40f;
        if (preformance == AttackSlider.state.bad)
            tx += 5 / 40f;
        moveBoss();
    }

    void moveBoss()
    {
        if (tx < 0)
            tx = 0;

        if (tx > 1)
            tx = 1;

        //GameManager.playerControler.ChangePlayerPos(PlayerStart.x + Mathf.Lerp(0, 10, tx));
        transform.position = SelfStart + new Vector3(Mathf.Lerp(0, 10, tx), 0);
    }

    void hit(BaseObject.objectType t)
    {
        if (tx > 1)
            tx = 0;
        tx += 1f / 2;
        moveBoss();
    }
}
