using HarmonyLib;

namespace MarkMode
{
    /* These Patches are only to trigger actions. Not changing the behaviour of the functions themselfes.
     */

    // This patch randomizes the hats just before the game start, right when the imposters are communicated.
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class InfectedPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                MarkModeMain.assignHats();
                MarkModeMain.sendHatAssignments();
            }
        }
    }
}
