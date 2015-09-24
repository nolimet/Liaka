using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public class optionsData
    {
        public float soundVolume = 1f;
        public float musicVolume = 1f;
        public float interfaceVolume = 0.4f;
    }

    public optionsData options = new optionsData();

    public int Coins = 0;
}
