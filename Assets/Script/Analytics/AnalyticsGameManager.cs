using UnityEngine;
using UnityEngine.Analytics;
using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;

public class AnalyticsGameManager : MonoBehaviour
{
    public void OnLevelWasLoaded(int level)
    {
        Analytics.CustomEvent("LevelSwitch", new Dictionary<string, object>
                {
                    {"level_id", "Lvl " + level }
                });
    }

    void Awake()
    {
        GameAnalytics.NewDesignEvent("Game-Started");
        //UnityEngine.
    }

    public static void ThrowSystemError(System.Exception e)
    {
        GameAnalytics.NewErrorEvent(GA_Error.GAErrorSeverity.GAErrorSeverityError, e.Message);
    }
}
