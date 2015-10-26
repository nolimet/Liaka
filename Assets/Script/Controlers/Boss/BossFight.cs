using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour
{
    public delegate void VoidDelegate();
    public VoidDelegate onGoodHit, onBadHit, onPerfectHit, onOutOfHP;

    public delegate void FloatDelegate(float f);
    public FloatDelegate onHPChange;

    public float HP = 5;
    public float MaxHP = 5;
    public TextMesh textm;

   void Start()
    {
        if (GameManager.uiControler)
            GameManager.uiControler.attackSlider.onAttack += AttackSlider_onAttack;

        textm.text = "BossHP: " + HP.ToString() + " / " + MaxHP.ToString();
    }

    void Destroy()
    {
        if (GameManager.uiControler)
            GameManager.uiControler.attackSlider.onAttack -= AttackSlider_onAttack;
    }

    void onEnable()
    {

    }

    private void AttackSlider_onAttack(float f, AttackSlider.state preformance)
    {
        switch (preformance)
        {
            case AttackSlider.state.perfect:
                if (onPerfectHit != null)
                    onPerfectHit();
                HP -= 3;
                break;

            case AttackSlider.state.good:
                if (onGoodHit != null)
                    onGoodHit();
                HP--;
                break;

            case AttackSlider.state.bad:
                if (onBadHit != null)
                    onBadHit();

                break;
        }

        if (HP <= 0)
        {
            if (onOutOfHP != null)
                onOutOfHP();

            HP = 0;
        }

        if (onHPChange != null)
            onHPChange(HP / MaxHP);

        textm.text = "BossHP: " + HP.ToString() + " / " + MaxHP.ToString();
    }
}
