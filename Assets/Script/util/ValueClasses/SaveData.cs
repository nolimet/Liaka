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
        /// <summary>
        /// coins you can currently spend
        /// </summary>
        public int CoinsCurrent = 0;
        /// <summary>
        /// number of coins collected intotal
        /// </summary>
        public int CoinsTotal { get { return _CoinsTotal; } }
        int _CoinsTotal = 0;
        /// <summary>
        /// The high ammount of coins the player ever had at one point
        /// </summary>
        public int HighestAmountOfCoinsHeld { get { return _HighestAmountOfCoinsHeld; } }
        int _HighestAmountOfCoinsHeld = 0;
        /// <summary>
        /// return the highest ammount gained in a single run
        /// </summary>
        public int HighestAmountGainedInSingleRun {  get { return _HighestAmountGainedInSingleRun; } }
        int _HighestAmountGainedInSingleRun;
        /// <summary>
        /// add x number of coins to the collected coins pool
        /// </summary>
        /// <param name="numb">Now many will be added</param>
        public void addCoins(int numb)
        {
            Debug.Log(numb);
            CoinsCurrent += numb;
            _CoinsTotal += numb;
            if (numb > HighestAmountGainedInSingleRun)
                _HighestAmountGainedInSingleRun = numb;

            if (CoinsCurrent > _HighestAmountOfCoinsHeld)
                _HighestAmountOfCoinsHeld = CoinsCurrent;
        }
    }

    public optionsData options = new optionsData();
    public gameData game = new gameData();

    string GameVersionLastStartup = "";
    const string CurrentGameVersion = "0.6f";

    bool versionTest()
    {
        if (CurrentGameVersion != GameVersionLastStartup)
        {
            GameVersionLastStartup = CurrentGameVersion;
            return true;
        }
        return false;
    }
}
