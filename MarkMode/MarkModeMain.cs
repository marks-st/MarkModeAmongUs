using HarmonyLib;
using Hazel;
using System.Collections.Generic;

namespace MarkMode
{
    /* This class holds settings and static data for the mod. 
     * It also includes more complex logic, that can be excluded from the patches itself.
     */
    [HarmonyPatch]
    public class MarkModeMain
    {
        // Settings
        public const string playerName = "Mark";
        public const Color playerColor = Color.Orange;
        public const bool petsDisabled = true;
        public const bool skinsDisabled = true;

        // Static data
        public static Dictionary<byte, uint> hatAssignments = new Dictionary<byte, uint>();
        
        public static void assignHats()
        {
            hatAssignments.Clear();

            // Assign a hat to every player
            var rand = new System.Random();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                // Find random hat, which is not already used
                uint hatId;
                do
                {
                    hatId = (uint)rand.Next(1, 94);
                } while (hatAssignments.ContainsValue(hatId));

                // Store assignment
                hatAssignments.Add(player.PlayerId, hatId);
            }
        }

        public static void sendHatAssignments()
        {
            // Send hat assignments for each player
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                uint hatId = hatAssignments[player.PlayerId];

                // Set for host
                player.SetHat(hatId, (int)Color.Orange);

                // Send to other players
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(player.NetId, (byte)RPC.SetHat, SendOption.Reliable);
                messageWriter.WritePacked(hatId);
                messageWriter.EndMessage();            
            }
        }
    }
}
