using HarmonyLib;

namespace MarkMode
{
    /* These patches disable the lobby tabs to configure the player. 
     * Right now just blocking initialization.
     * Not yet sure how to handle if we want to disable them during runtime.  
     */

    // Color
    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    public static class DisablePlayerTabPatch
    {
        public static bool Prefix(PlayerTab __instance)
        {
            // Colors and hats are always disabled if mod is active.That is the reason for the mod.
            return !MarkModeMain.ModActive.GetValue();
        }
    }

    // Hat
    [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
    public static class DisableHatsTabPatch
    {
        public static bool Prefix(HatsTab __instance)
        {
            // Colors and hats are always disabled if mod is active. That is the reason for the mod.
            return !MarkModeMain.ModActive.GetValue();
        }
    }

    // PetsTabs does not work right now. OxygenFilter does not recognize the class name.
    //[HarmonyPatch(typeof(PetsTab), nameof(PetsTab.OnEnable))]
    //public static class DisablePetsTabPatch
    //{
    //    public static bool Prefix(PetsTab __instance)
    //    {
    //        return !MarkModeMain.ModActive.GetValue() || MarkModeMain.PetsAllowed.GetValue();
    //    }
    //}

    // Skins
    [HarmonyPatch(typeof(SkinsTab), nameof(SkinsTab.OnEnable))]
    public static class DisableSkinsTabPatch
    {
        public static bool Prefix(SkinsTab __instance)
        {
            return !MarkModeMain.ModActive.GetValue() || MarkModeMain.SkinsAllowed.GetValue();
        }
    }
}
