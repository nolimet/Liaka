using UnityEngine;
using System.Collections;

public class BackgroundControler : MonoBehaviour
{
    [System.Serializable]
    public struct layer
    {
        [Tooltip("Object that will be moving")]
        public LayerControler Layer;
        [Tooltip("Distance it will travel each second")]
        public Vector3 speed;
    }

    [SerializeField]
    layer[] Layers = new layer[0];
    public float SpeedMult = 1f;
    bool GamePaused = false;
    bool playerHitTrap = false;
    

    void Start()
    {
        if (GameManager.instance)
            GameManager.instance.onPauseGame += onGamePaused;

    }

    void OnDestory()
    {
        if (GameManager.instance)
            GameManager.instance.onPauseGame -= onGamePaused;
    }

    void onGamePaused(bool b)
    {
        GamePaused = b;
    }

    void onPlayerHitTrap()
    {
        playerHitTrap = true;
    }

    void Update()
    {
        if (GamePaused)
            return;
        foreach (layer l in Layers)
        {
            l.Layer.moveLayer(l.speed * SpeedMult);
        }
    }

    //public void FixedUpdate()
    //{
    //    if (GamePaused)
    //        return;
    //    foreach (layer l in Layers)
    //    {
    //        l.Layer.FixedLayerMove(l.speed);
    //    }
    //}

    void LateUpdate()
    {
            foreach (layer l in Layers)
                l.Layer.LateUpdateLoop();
    }
}
