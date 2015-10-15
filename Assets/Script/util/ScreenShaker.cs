using UnityEngine;
using System.Collections;

public class ScreenShaker : MonoBehaviour {

    /// <summary>
    /// Object instance
    /// </summary>
    public static ScreenShaker instance;
    bool shaking = false;

    void Update()
    {
        if (!instance)
            instance = this;

    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShakeScreen(2f, 0.2f, 0.05f);
        }
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
        if(instance)
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
        //shakeDelayTimer
        float shakeLerp = 0;
        //shaking posistion keeping
        Vector2 screenShakeTarget = new Vector2(intensity * Random.Range(-1f,1f), intensity * Random.Range(-1f, 1f)), LastScreenShakeTarget = Vector2.zero;
        //camera main transform;
        Transform camTrans = Camera.main.transform;

        while (t > 0)
        {

            camTrans.localPosition = Vector2.Lerp(LastScreenShakeTarget, screenShakeTarget, shakeLerp / timePerShake);

            t -= Time.deltaTime;
            shakeLerp += Time.deltaTime;
            if (shakeLerp >= timePerShake)
            {
                shakeLerp = 0;

                LastScreenShakeTarget = screenShakeTarget;
                screenShakeTarget = new Vector2(intensity * Random.Range(-1f, 1f), intensity * Random.Range(-1f, 1f));
            }
            yield return new WaitForEndOfFrame();
        }

        t = 0;
        while(t<timePerShake)
        {
            camTrans.localPosition = Vector2.Lerp(screenShakeTarget, Vector2.zero, t / timePerShake);
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        camTrans.localPosition = Vector2.zero;
        shaking = false;
    }
}
