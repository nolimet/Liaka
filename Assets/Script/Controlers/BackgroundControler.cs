﻿using UnityEngine;
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
    bool GamePaused = false;
    void Start()
    {
        if(GameManager.instance)
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

    void Update()
    {
        if (GamePaused)
            return;
        foreach (layer l in Layers)
        {
            l.Layer.moveLayer(l.speed);
        }
    }
}
