using HarmonyLib;

namespace MarkMode
{

    /* These patches disable the sending of players hat, pet and skin settings. 
     * Only the host will set these parameters.
     */

    // Hat
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetHat))]
    public static class SetHatPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint hatId)
        {
            // Colors and hats are always disabled. That is the reason for the mod.
            return false;
        }
    }

    // Pet
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetPet))]
    public static class SetPetPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint petId)
        {
            if (MarkModeMain.petsDisabled)
                return false;

            return true;
        }
    }

    // Skin
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetSkin))]
    public static class SetSkinPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] uint skinId)
        {
            if (MarkModeMain.skinsDisabled)
                return false;

            return true;
        }
    }
}
