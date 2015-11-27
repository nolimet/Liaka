using UnityEngine;
using System.Linq;
using System.Collections;

public class AttackSlider : MonoBehaviour
{

    public delegate void FloatDelegate(float f, state preformance);
    public event FloatDelegate onAttack;

    [SerializeField]
    RectTransform cicleMov = new RectTransform();
    [SerializeField]
    slidePart Good = new slidePart(), Bad1 = new slidePart(), Bad2 = new slidePart(), Perfect = new slidePart();
    [Header("MovementRange")]
    public float MoveMin = 0;
    public float MoveMax = 0;
    public float moveRange = 0;
    bool paused = false;

    public enum state
    {
        good, bad, perfect
    }

    [System.Serializable]
    struct slidePart
    {
        public RectTransform t;
        public UnityEngine.UI.Image Image;
        [Range(0f, 1f)]
        public float triggerPoint;
        public state State;
    }

    bool run = true;

    float t, curve;

    void Awake()
    {
        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
    }

    public void OnDestroy()
    {
        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
    }

    public void OnEnable()
    {

        GameManager.instance.onPauseGame += Instance_onPauseGame;

        t = 0;
        Update_TriggerStatus();
    }

    public void OnDisable()
    {
        GameManager.instance.onPauseGame -= Instance_onPauseGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            P_onBossBattleAttack();

        if (run)
        {
            curve = Mathf.Lerp(MoveMin, MoveMax, (1 + Mathf.Cos(t * 3f)) / 2);
            cicleMov.rotation = Quaternion.Euler(0, 0, curve);
            t += Time.deltaTime;
        }
        else
            t = 0;

#if UNITY_EDITOR
        Update_TriggerStatus();
#endif
    }

    private void Update_TriggerStatus()
    {
        slidePart[] a = new slidePart[] { Bad1, Good, Perfect, Bad2 };

        float r = moveRange / 360f;
        float total = a.Select(x => x.triggerPoint).Sum();
        if (total == 0)
            return;
        float rot = 0;
        for (int i = 0; i < a.Length; i++)
        {
            a[i].t.localRotation = Quaternion.Euler(0, 0, rot);
            a[i].Image.fillAmount = r * (a[i].triggerPoint / total);
            rot -= moveRange * (a[i].triggerPoint / total);
        }
    }

    state statusCheck()
    {
        slidePart[] a = new slidePart[] { Bad1, Good, Perfect, Bad2 };

        float total = a.Select(x => x.triggerPoint).Sum();
        if (total == 0)
            return state.bad;
        float called = -(curve - (moveRange / 2f)) / moveRange;
        float counter = 0;
        int heighest = 0;

        for (int i = 0; i < a.Length; i++)
        {
            if (called > counter)
                heighest = i;

            counter += a[i].triggerPoint / total;
        }
        return a[heighest].State;
    }

    private void P_onBossBattleAttack()
    {
        run = !run;
        if (run)
            return;
        if (onAttack != null)
        {
            float f = (curve - (moveRange / 2f)) / moveRange;
            if (f < 0)
                f *= -1;
            onAttack(f, statusCheck());
        }
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        p.onBossBattleAttack += P_onBossBattleAttack;
    }

    private void Instance_onPauseGame(bool b)
    {
        paused = b;
    }


}
