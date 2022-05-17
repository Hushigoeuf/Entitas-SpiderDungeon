using System;
using System.Collections.Generic;
using DG.Tweening;
using EasyMobile;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GameEngine
{
    public sealed class MobileServices : MonoBehaviour
    {
        private static readonly List<string> Unlocks = new List<string>();
        private static bool _allAchievementCompleted;
        private static int _initializeCount = 0;

        [BoxGroup("_initialized", false)] [SerializeField]
        private bool _initialized;

        [BoxGroup("_initDelay", false)] [MinValue(0)] [EnableIf("$_initialized")] [SerializeField]
        private float _initDelay;

        [BoxGroup("_initRepeated", false)] [EnableIf("$_initialized")] [SerializeField]
        private bool _initRepeated;

        [BoxGroup("_executed", false)] [SerializeField]
        private bool _executed;

        [BoxGroup("_executeDelay", false)] [MinValue(0)] [EnableIf("$_executed")] [SerializeField]
        private float _executeDelay;

        [BoxGroup("_settings", false)] [EnableIf("$_executed")] [SerializeField]
        private MobileServicesData _settings;

        [BoxGroup("_trapSettings", false)] [EnableIf("$_executed")] [SerializeField]
        private TrapData _trapSettings;

        [BoxGroup("_contentSettings", false)] [EnableIf("$_executed")] [SerializeField]
        private ContentSettingsObject _contentSettings;

        private float _totalDelay;
        private int _reportScore;

        // -------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            _totalDelay = 0f;

            if (_initialized)
                if (!IsInitialized() && (_initializeCount == 0 || _initRepeated))
                {
                    _initializeCount++;
                    _StartInit();
                    return;
                }

            if (_executed)
                if (IsInitialized())
                    _StartExecute();
        }

        private void _StartInit()
        {
            if (_initDelay != 0)
            {
                _totalDelay += _initDelay;

                var seq = DOTween.Sequence();
                seq.AppendInterval(_initDelay);
                seq.OnComplete(_StopInit);
                return;
            }

            _StopInit();
        }

        private void _StopInit()
        {
            GameServices.UserLoginSucceeded += _InitOnUserLoginSucceeded;
            Init();
        }

        private void _InitOnUserLoginSucceeded()
        {
            GameServices.UserLoginSucceeded -= _InitOnUserLoginSucceeded;
            if (_executed && IsInitialized()) _StartExecute();
        }

        private void _StartExecute()
        {
            if (_executeDelay - _totalDelay > 0)
            {
                _totalDelay += _executeDelay;

                var seq = DOTween.Sequence();
                seq.AppendInterval(_executeDelay - _totalDelay);
                seq.OnComplete(_StopExecute);
                return;
            }

            _StopExecute();
        }

        private void _StopExecute()
        {
            //return;

            var highScore = _settings.HighScoreParameter.Value;

            // Score Leaderboard (1)
            if (_settings.LastScoreParameter.Value == highScore)
                GameServices.ReportScore(highScore, EM_GameServicesConstants.Leaderboard_leaderboard_score);

            if (_allAchievementCompleted) return;

            var deadCount = _settings.DeadCountParameter.Value;
            var diamondCount = _settings.DiamondCountParameter.Value;

            // Score Achievements (5)
            var scoreRegions = _trapSettings.ScoreRegions;
            if (highScore >= scoreRegions[0])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_score_1);
            if (highScore >= scoreRegions[1])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_score_2);
            if (highScore >= scoreRegions[2])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_score_3);
            if (highScore >= scoreRegions[3])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_score_4);
            if (highScore >= scoreRegions[4])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_score_5);

            // Dead Achievements (3)
            var deadConditions = _settings.DeadConditions;
            if (deadCount >= deadConditions[0])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_dead_1);
            if (deadCount >= deadConditions[1])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_dead_2);
            if (deadCount >= deadConditions[2])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_dead_3);

            // Diamond Achievements (3)
            var diamondConditions = _settings.DiamondConditions;
            if (diamondCount >= diamondConditions[0])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_diamond_1);
            if (diamondCount >= diamondConditions[1])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_diamond_2);
            if (diamondCount >= diamondConditions[2])
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_diamond_3);

            // Other Achievements (4)
            if (_settings.LongestScoreRaceParameter.Value >= _settings.LongestRaceScoreSize)
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_longest_no_dead_race);
            if (_settings.TerminalStatusParameter.Value)
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_terminal);
            if (_settings.FightStatusParameter.Value)
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_fight);
            foreach (var item in _contentSettings.ItemList)
            {
                if (item.Level < 3) continue;
                _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_max_upgrade);
                break;
            }

            // All Achievement
            /*var all_count = EM_Settings.GameServices.Achievements.Length;
            if (all_count > 10)
            {*/
            var all = GameServices.GetAchievementByName(EM_GameServicesConstants.Achievement_achievement_all);
            Social.LoadAchievements(achievements =>
            {
                var totalCompleteCount = 0;
                for (var i = 0; i < achievements.Length; i++)
                    /*if (achievements[i].id == all.Id) continue;
                        if (achievements[i].id == all.AndroidId) continue;
                        if (achievements[i].id == all.IOSId) continue;*/
                    if (achievements[i].completed)
                        totalCompleteCount++;

                if (totalCompleteCount >= 17)
                {
                    _UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_all);
                    _allAchievementCompleted = true;
                }
            });
            //}
        }

        private static void _UnlockAchievement(string achievementName)
        {
            //return;
            if (Unlocks.Contains(achievementName)) return;
            Unlocks.Add(achievementName);
            GameServices.UnlockAchievement(achievementName);
        }

        private static void UnlockAchievement(string achievementId)
        {
            //return;
            if (_allAchievementCompleted) return;
            if (!IsInitialized()) return;
            _UnlockAchievement(achievementId);
        }

        public static void UnlockTerminalAchievement()
        {
            UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_terminal);
        }

        public static void UnlockMaxItemAchievement()
        {
            UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_max_item);
        }

        public static void UnlockMaxBonusAchievement()
        {
            UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_max_bonus);
        }

        public static void UnlockMaxUpgradeAchievement()
        {
            UnlockAchievement(EM_GameServicesConstants.Achievement_achievement_max_upgrade);
        }

        public void ShowLeaderboardUiOrInitPopup()
        {
            ShowLeaderboardUi(true);
        }

        public void ShowAchievementsUiOrInitPopup()
        {
            ShowAchievementsUi(true);
        }

        // -------------------------------------------------------------------------------------------------------------

        public static bool IsInitialized()
        {
#if UNITY_EDITOR
            return false;
#endif
            return GameServices.IsInitialized();
        }

        public static void Init()
        {
#if UNITY_EDITOR
            return;
#endif
            if (!IsInitialized())
                GameServices.Init();
        }

        public static void ShowLeaderboardUi(bool useInitPopup)
        {
            if (IsInitialized())
                GameServices.ShowLeaderboardUI();
            else if (useInitPopup) InitPopup();
        }

        public static void ShowAchievementsUi(bool useInitPopup)
        {
            if (IsInitialized())
                GameServices.ShowAchievementsUI();
            else if (useInitPopup) InitPopup();
        }

        // -------------------------------------------------------------------------------------------------------------

        public static readonly UnityEvent OnInitPopupCompleted = new UnityEvent();

        public static void InitPopup(Action callbackCompleted = null)
        {
            if (IsInitialized())
            {
                _OnInitPopupCompleted();
                return;
            }

            var alert = NativeUI.ShowTwoButtonAlert(
                DefaultServices.Localization.GetTranslation("NativeUi_MSTitle"),
                DefaultServices.Localization.GetTranslation("NativeUi_MSConfirm"),
                DefaultServices.Localization.GetTranslation("NativeUi_MSConnectButton"),
                DefaultServices.Localization.GetTranslation("NativeUi_CancelButton"));

            if (alert == null)
            {
                _OnInitPopupCompleted();
                return;
            }

            alert.OnComplete += buttonIndex =>
            {
                if (buttonIndex != 0)
                {
                    _OnInitPopupCompleted();
                    return;
                }

                GameServices.UserLoginSucceeded += _OnInitSucceededAfterAlert;
                GameServices.UserLoginFailed += _OnInitFailedAfterAlert;
                Init();
            };
        }

        private static void _OnInitSucceededAfterAlert()
        {
            GameServices.UserLoginSucceeded -= _OnInitSucceededAfterAlert;
            _ShowMessageAlert("NativeUi_MSInitSucceeded");
        }

        private static void _OnInitFailedAfterAlert()
        {
            GameServices.UserLoginFailed -= _OnInitFailedAfterAlert;
            _ShowMessageAlert("NativeUi_MSInitFailed");
        }

        private static void _ShowMessageAlert(string translationKey)
        {
            var alert = NativeUI.Alert(DefaultServices.Localization.GetTranslation("NativeUi_MSTitle"),
                DefaultServices.Localization.GetTranslation(translationKey));
            alert.OnComplete += buttonIndex => { _OnInitPopupCompleted(); };
        }

        private static void _OnInitPopupCompleted()
        {
            OnInitPopupCompleted.Invoke();
            OnInitPopupCompleted.RemoveAllListeners();
        }
    }
}