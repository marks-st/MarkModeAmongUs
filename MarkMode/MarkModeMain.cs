using BepInEx.Configuration;
using Essentials.CustomOptions;
using HarmonyLib;
using System.Collections.Generic;

namespace MarkMode
{
    /* This class holds settings and static data for the mod. 
     * It also includes more complex logic, that can be excluded from the patches.
     */

    [HarmonyPatch]
    public class MarkModeMain
    {
        // Settings
        public static ConfigEntry<string> PlayerName { get; set; }
        public static ConfigEntry<ushort> PlayerColor { get; set; }
        public static CustomToggleOption ModActive = CustomOption.AddToggle("MarkMode", false);
        public static CustomToggleOption PetsAllowed = CustomOption.AddToggle("MarkMode Pets", false);
        public static CustomToggleOption SkinsAllowed = CustomOption.AddToggle("MarkMode Skins", false);

        // Stored data
        public static Dictionary<byte, uint> hatAssignments = new Dictionary<byte, uint>();

        // Init unfortunately needed to attach event listeners. Is there a better way?
        public static void init()
        {
            ModActive.ValueChanged += ModActiveValueChanged;
            PetsAllowed.ValueChanged += PetsAllowed_ValueChanged;
            SkinsAllowed.ValueChanged += SkinsAllowed_ValueChanged;
        }

        // Handling of mod activation / deactivation
        private static void ModActiveValueChanged(object sender, OptionValueChangedEventArgs e)
        {
            // Communicate change to other players
            if (AmongUsClient.Instance.AmHost)
                CustomRpcSender.RpcSetModActive(ModActive.GetValue());

            // Handle it ourselfes
            if (ModActive.GetValue()) modActivated();
            else modDeactivated();
        }

        private static void modActivated()
        {
            PlayerControl player = PlayerControl.LocalPlayer;

            // Get new name and color from host
            player.CmdCheckName(SaveManager.PlayerName);
            player.CmdCheckColor(SaveManager.BodyColor);

            // Remember settings
            SaveManager.LastHat = player.Data.HatId;
            SaveManager.LastPet = player.Data.PetId;
            SaveManager.LastSkin = player.Data.SkinId;

            // Reset hat on begin
            AlternativeRpcSender.RpcSetHat(0);

            // Handle pet setting
            if (PetsAllowed.GetValue()) petsActivated();
            else petsDeactivated();

            // Handle skin setting
            if (SkinsAllowed.GetValue()) skinsActivated();
            else skinsDeactivated();
        }

        private static void modDeactivated()
        {
            PlayerControl player = PlayerControl.LocalPlayer;

            // Get new name and color from host
            player.CmdCheckName(SaveManager.PlayerName);
            player.CmdCheckColor(SaveManager.BodyColor);

            // Send hat, pet and skin to other players
            AlternativeRpcSender.RpcSetHat(SaveManager.LastHat);
            AlternativeRpcSender.RpcSetPet(SaveManager.LastPet);
            AlternativeRpcSender.RpcSetSkin(SaveManager.LastSkin);
        }

        // Handling of pet activation / deactivation
        private static void PetsAllowed_ValueChanged(object sender, OptionValueChangedEventArgs e)
        {
            // Communicate change to other players
            if (AmongUsClient.Instance.AmHost)
                CustomRpcSender.RpcSetPetsAllowed(PetsAllowed.GetValue());

            // Handle it ourselfes
            if (PetsAllowed.GetValue()) petsActivated();
            else petsDeactivated();
        }

        private static void petsActivated()
        {
            // Only change pet settings, when mod is active
            if (ModActive.GetValue())
                AlternativeRpcSender.RpcSetPet(SaveManager.LastPet);
        }

        private static void petsDeactivated()
        {
            // Only change pet settings, when mod is active
            if (ModActive.GetValue())
            {
                SaveManager.LastPet = PlayerControl.LocalPlayer.Data.PetId;
                AlternativeRpcSender.RpcSetPet(0);
            }
        }

        // Handling of skin activation / deactivation
        private static void SkinsAllowed_ValueChanged(object sender, OptionValueChangedEventArgs e)
        {
            // Communicate change to other players
            if (AmongUsClient.Instance.AmHost)
                CustomRpcSender.RpcSetSkinsAllowed(SkinsAllowed.GetValue());

            // Handle it ourselfes
            if (SkinsAllowed.GetValue()) skinsActivated();
            else skinsDeactivated();
        }

        private static void skinsActivated()
        {
            // Only change pet settings, when mod is active
            if (ModActive.GetValue())
                AlternativeRpcSender.RpcSetSkin(SaveManager.LastSkin);
        }

        private static void skinsDeactivated()
        {
            // Only change skin settings, when mod is active
            if (ModActive.GetValue())
            {
                SaveManager.LastSkin = PlayerControl.LocalPlayer.Data.SkinId;
                AlternativeRpcSender.RpcSetSkin(0);
            }
        }

        // Handling of hats
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
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    AlternativeRpcSender.RpcSetHat(hatId);

                // Send to other players
                CustomRpcSender.RpcHostSetHat(player.PlayerId, hatId);
            }
        }
    }
}
