using System.Collections.Generic;
using UnityEngine.Analytics;

public static class AnalyticsService
{
    public static void SendEvent(string customEventName)
    {
        return;
        Analytics.CustomEvent(customEventName);
    }

    public static void SendEvent(string customEventName, IDictionary<string, object> eventData)
    {
        return;
        Analytics.CustomEvent(customEventName, eventData);
    }

    public static void OnBuyContainer(int cost)
    {
        SendEvent("buy_container_" + cost);
    }

    public static void OnShowRewardAd()
    {
        SendEvent("show_reward_ad");
    }

    public static void OnShowForcedAd()
    {
        SendEvent("show_forced_ad");
    }

    public static void OnGameStart(int cash)
    {
        SendEvent("game_start", new Dictionary<string, object>
        {
            {"cash", cash}
        });
    }

    public static void OnGameStop(int score, int cash)
    {
        SendEvent("game_stop", new Dictionary<string, object>
        {
            {"score", score},
            {"cash", cash}
        });
    }

    public static void OnItemEquip(string itemName)
    {
        SendEvent("item_equip", new Dictionary<string, object>
        {
            {"item_name", itemName}
        });
    }

    public static void OnItemTakeOff(string itemName)
    {
        SendEvent("item_take_off", new Dictionary<string, object>
        {
            {"item_name", itemName}
        });
    }

    public static void OnItemLevelUp(string itemName)
    {
        SendEvent("item_level_up", new Dictionary<string, object>
        {
            {"item_name", itemName}
        });
    }

    public static void OnDropBlueprint(int cost)
    {
        SendEvent("drop_blueprint_" + cost);
    }

    public static void OnSwitchTerminal()
    {
        SendEvent("switch_terminal");
    }

    public static void OnFightStart()
    {
        SendEvent("fight_start");
    }

    public static void OnFightStop(bool state)
    {
        SendEvent("fight_stop", new Dictionary<string, object>
        {
            {"state", state ? 0 : 1}
        });
    }
}