using HarmonyLib;

namespace MarkMode
{

    /* These patches disable the sending of clients hat, pet and skin settings. 
     * We need to block these, because they are directly called by the tabs in the game settings.
     * To still communicate changes to others we need alternative versions, which are in HandleRpc.cs
     */

    // Hat
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetHat))]
    public static class SetHatPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint hatId)
        {
            // Colors and hats are always disabled. That is the reason for the mod.
            return !MarkModeMain.ModActive.GetValue();
        }
    }

    // Pet
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetPet))]
    public static class SetPetPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint petId)
        {
            return !MarkModeMain.ModActive.GetValue() || MarkModeMain.PetsAllowed.GetValue();
        }
    }

    // Skin
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetSkin))]
    public static class SetSkinPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint skinId)
        {
            return !MarkModeMain.ModActive.GetValue() || MarkModeMain.SkinsAllowed.GetValue();
        }
    }
}
