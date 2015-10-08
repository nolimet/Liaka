using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour
{
    public delegate void VoidDelegate();
    public VoidDelegate onGoodHit, onBadHit, onPerfectHit;

    public float HP;
    public float MaxHP;

   void Start()
    {
        if (GameManager.uiControler)
            GameManager.uiControler.attackSlider.onAttack += AttackSlider_onAttack;
    }

    void Destroy()
    {
        if (GameManager.uiControler)
            GameManager.uiControler.attackSlider.onAttack -= AttackSlider_onAttack;
    }

    private void AttackSlider_onAttack(float f, AttackSlider.state preformance)
    {
        switch (preformance)
        {
            case AttackSlider.state.perfect:
                if (onPerfectHit != null)
                    onPerfectHit();

                break;

            case AttackSlider.state.good:
                if (onGoodHit != null)
                    onGoodHit();

                break;

            case AttackSlider.state.bad:
                if (onBadHit != null)
                    onBadHit();

                break;
        }
    }
}
