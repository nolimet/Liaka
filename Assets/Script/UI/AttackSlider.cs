using UnityEngine;
using System.Linq;
using System.Collections;

public class AttackSlider : MonoBehaviour
{

    public delegate void FloatDelegate(float f, state preformance);
    public event FloatDelegate onAttack;

    [SerializeField]
    RectTransform cicleMov;
    [SerializeField]
    slidePart Good, Bad1, Bad2, Perfect;
    [SerializeField]
    float MoveMin, MoveMax, moveRange;
    bool paused;

    public enum state
    {
        good,bad,perfect
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
    public void OnEnable()
    {
        GameManager.inputManager.onTap += InputManager_onTap;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
        t = 0;
        Update_TriggerStatus();
    }

    private void Instance_onPauseGame(bool b)
    {
        paused = b;
    }

    public void OnDisable()
    {
        GameManager.inputManager.onTap -= InputManager_onTap;
        GameManager.instance.onPauseGame -= Instance_onPauseGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            InputManager_onTap(Vector2.zero);

        if (run)
        {
            curve = Mathf.Lerp(MoveMin, MoveMax, (1 + Mathf.Cos(t * 3f)) / 2);
            cicleMov.rotation = Quaternion.Euler(0, 0, curve);
            t += Time.deltaTime;

            // Debug.Log(-(curve - (moveRange / 2f)) / moveRange);
            // Debug.Log(curve);
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
            if (called > counter  )
                heighest = i;

            counter += a[i].triggerPoint / total;
        }
        return a[heighest].State;
    }

    private void InputManager_onTap(Vector2 pos)
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
}
