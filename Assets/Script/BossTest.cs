using UnityEngine;
using System.Collections;

public class BossTest : MonoBehaviour
{
    [SerializeField]
    Vector3 posDist,PlayerStart,SelfStart;
    float tx;

    void Start()
    {
        BaseObject.onHitBose += hit;
        AttackSlider.onAttack += AttackSlider_onAttack;
        PlayerControler.onPlayerCreated += PlayerControler_Created;
        
        SelfStart = transform.position;
    }

    public void OnDestroy()
    {
        AttackSlider.onAttack -= AttackSlider_onAttack;
        BaseObject.onHitBose -= hit;
        PlayerControler.onPlayerCreated -= PlayerControler_Created;
    }

    private void PlayerControler_Created(PlayerControler player)
    {
        Debug.Log("cake");
        PlayerStart = player.transform.position;
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

        GameManager.playerControler.ChangePlayerPos(PlayerStart.x + Mathf.Lerp(0, 10, tx));
        transform.position = SelfStart + new Vector3(Mathf.Lerp(0, 10, tx), 0);
    }

    void hit(BaseObject.objectType t)
    {
        if (tx > 1)
            tx = 0;
        tx += 1f / 40;
        moveBoss();
    }
}
