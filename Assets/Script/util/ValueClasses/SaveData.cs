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

    [System.Serializable]
    public class gameData
    {
        public bool firstStartup = true;
        public int CoinsCurrent = 0;
        public int CoinsTotal = 0;
    }

    public optionsData options = new optionsData();
    public gameData game = new gameData();

    string GameVersionLastStartup = "";
    const string CurrentGameVersion = "0.1f";
}
