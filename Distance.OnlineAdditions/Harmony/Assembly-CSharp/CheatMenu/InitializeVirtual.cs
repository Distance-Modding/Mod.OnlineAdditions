using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CheatMenu), "InitializeVirtual")]
    internal class InitializeVirtual__CheatMenu
    {
        [HarmonyPrefix]
        //Literally rewriting an entire function for this lmao
        internal static bool CheckMotorcycleCheat(CheatMenu __instance)
        {
            //Disable motorcycle mode in multiplayer because motorcycle mode is cringe
            __instance.cm_ = G.Sys.CheatsManager_;
            __instance.am_ = G.Sys.Achievements_;
            if (!(bool)__instance.cm_)
                return false;
            __instance.initialValues_ = __instance.cm_.EnabledFlags_;
            for (ECheat c = ECheat.CampaignPlus; c < ECheat.Count_; ++c)
            {
                CheatsManager.Cheat cheat = __instance.cm_.GetCheat(c);
                if (cheat.name == null)
                    continue;
                string upper = cheat.name.ToUpper();
                string str1 = "Gameplay".Colorize(Colors.teal);
                string str2 = "Visual".Colorize(Colors.yellowGreen);
                if (!__instance.cm_.IsUnlocked(c))
                {
                    string lockedText = GUtils.GetLockedText(upper);
                    for (int index = 0; (ECheat)index < c; ++index)
                        lockedText += (string)(object)' ';
                    string str3 = string.Empty;
                    if (cheat.acheivement != EAchievements.None)
                        str3 = string.Format("Complete: {0}", __instance.am_.GetAchievement(cheat.acheivement).name_);
                    __instance.TweakAction(lockedText.Colorize(UnityEngine.Color.gray), null, "To Unlock: ".Colorize(Colors.tomato) + str3);
                }
                else if (cheat.cheatID == ECheat.CampaignPlus && !__instance.cm_.CampaignPlusRecognized_)
                {
                    string description = "This cheat only works in Campaign.";
                    __instance.TweakAction(upper.Colorize(Colors.tomato), null, description);
                }
                else if (cheat.affectsGameplay && !__instance.cm_.GameplayCheatsRecognized_ && !Mod.Instance.Config.EnableCheatsInMultiplayer)
                {
                    string description = "This cheat affects gameplay and is disabled in online play.";
                    __instance.TweakAction(upper.Colorize(Colors.tomato), null, description);
                }
                else if (cheat.cheatID == ECheat.MotorcycleMode && Mod.Instance.Config.EnableCheatsInMultiplayer)
                {
                    //This is the important part. Making motorcycle mode automatically be disabled in multiplayer

                    string description = "This cheat will explode people in real life when playing in multiplayer.";
                    __instance.TweakAction(upper.Colorize(Colors.tomato), null, description);
                }
                else
                {
                    string description = string.Format("{0}: {1}", !cheat.affectsGameplay ? str2 : str1, cheat.description);
                    if (cheat.cheatID == ECheat.CampaignPlus && G.Sys.GameManager_.IsCampaignMode_)
                        description += "\n You will need to restart the level for this to fully take effect.".Colorize(Colors.tomato);
                    __instance.TweakBool(upper, __instance.cm_.IsEnabled(c), x => __instance.Set(c, x), description);
                }
            }
            return false;
        }
    }
}
