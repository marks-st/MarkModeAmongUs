using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkMode
{
    /* These patches change the servers response to name and color check,
     * so that every player will has the same name and same color.
     */

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckName))]
    public static class CheckNamePatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            __instance.RpcSetName(MarkModeMain.playerName);

            // When a new player joins (asks for his name), hats are reassigned.
            // This could also only be done on game start, but I like it this way.
            MarkModeMain.assignHats();
            MarkModeMain.sendHatAssignments();

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    public static class CheckColorPatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            __instance.RpcSetColor((byte)MarkModeMain.playerColor);

            return false;
        }
    }
}
