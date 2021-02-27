using HarmonyLib;

namespace MarkMode
{
    /* These patches change the servers response to name and color check,
     * so that every player will has the same name and same color.
     */

    // This patch sets the name to the client
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckName))]
    public static class CheckNamePatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!MarkModeMain.ModActive.GetValue())
                return true;

            __instance.RpcSetName(MarkModeMain.PlayerName.Value);

            return false;
        }
    }

    // This patch sets the color to the client
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    public static class CheckColorPatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!MarkModeMain.ModActive.GetValue())
                return true;

            __instance.RpcSetColor((byte)MarkModeMain.PlayerColor.Value);

            return false;
        }
    }
}
