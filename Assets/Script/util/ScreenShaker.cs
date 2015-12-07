using UnityEngine;
using System.Collections;

public class ScreenShaker : MonoBehaviour
{

    /// <summary>
    /// Object instance
    /// </summary>
    public static ScreenShaker instance;

    public static bool Shaking { get { return instance.shaking; } }
    public static Vector2 currenOffSet { get { return instance._currentOffSet; } }

    protected Vector2 _currentOffSet = Vector2.zero;
    protected bool shaking = false;
    bool paused = false;

    void Start()
    {
        shaking = false;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
    }

    private void Instance_onPauseGame(bool b)
    {
        paused = b;
    }

    void Update()
    {
        if (!instance)
            instance = this;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShakeScreen(2f, 0.2f, 0.05f);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShakeScreen(2f, 0.2f, 0.1f);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShakeScreen(2f, 0.4f, 0.05f);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ShakeScreen(2f, 0.7f, 0.05f);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ShakeScreen(2f, 1f, 0.04f);
#endif
    }



    /// <summary>
    /// Shakes the screen for a period of time
    /// </summary>
    /// <param name="duration">how long with the screen shake</param>
    /// <param name="intensity">how intense is the shaking</param>
    /// <param name="timePerShake">how long does each shake last</param>
    /// <returns></returns>
    public static void ShakeScreen(float duration, float intensity, float timePerShake)
    {
        if (instance)
        {
            instance.StartCoroutine(instance.shake(duration, intensity, timePerShake));
        }
    }

    /// <summary>
    /// Shakes the screen for a period of time
    /// </summary>
    /// <param name="duration">how long with the screen shake</param>
    /// <param name="intensity">how intense is the shaking</param>
    /// <param name="timePerShake">how long does each shake last</param>
    /// <returns></returns>
    IEnumerator shake(float duration, float intensity, float timePerShake)
    {
        //init
        if (shaking)
            yield break;

        shaking = true;

        //timer
        float t = duration;
        //counter that is used for moving the cam withing the lerp
        float shakeLerp = 0;
        //shaking posistion keeping
        Vector2 screenShakeTarget = new Vector2(intensity * Random.Range(-1f, 1f), intensity * Random.Range(-1f, 1f)), LastScreenShakeTarget = Vector2.zero;
        //camera main transform;
        Transform camTrans = Camera.main.transform;
        //Time Delta Calculator
        System.DateTime timeLast = System.DateTime.Now;
        float timeDelta = 0;
        //shakes the screen
        while (t > 0)
        {
            //when the game is pause it does not shake the screen 
            if (!paused)
            {
                //moves the camera around with a liniar interpolation
                _currentOffSet = Vector2.Lerp(LastScreenShakeTarget, screenShakeTarget, shakeLerp / timePerShake);
                camTrans.localPosition = _currentOffSet;
                //does a count down
                t -= timeDelta;
                
                shakeLerp += timeDelta;
                if (shakeLerp >= timePerShake)
                {
                    shakeLerp = 0;

                    LastScreenShakeTarget = screenShakeTarget;
                    screenShakeTarget = new Vector2(intensity * Random.Range(-1f, 1f), intensity * Random.Range(-1f, 1f));
                }
            }
            yield return new WaitForSeconds(0.014f);
            
            timeDelta = (float)(System.DateTime.Now - timeLast).TotalSeconds;
            timeLast = System.DateTime.Now;
            Debug.Log(timeDelta);
        }
        
        
        t = 0;
        //make sure the screen is back to where it started
        while (t < timePerShake)
        {
            _currentOffSet = Vector2.Lerp(screenShakeTarget, Vector2.zero, t / timePerShake);
            camTrans.localPosition = _currentOffSet;
            t += timeDelta;
            
            yield return new WaitForSeconds(0.014f);

            timeDelta = (float)(System.DateTime.Now - timeLast).TotalSeconds;
            timeLast = System.DateTime.Now;
        }
        shaking = false;
        _currentOffSet = Vector2.zero;
        camTrans.localPosition = Vector2.zero;
       
    }
}
