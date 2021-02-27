using HarmonyLib;
using Hazel;

namespace MarkMode
{
    /* This file includes very much everything that directly handles Rpc messages.
     */

    // This patch catches our custom Rpc messages after the normal ones were processed by the original HandleRpc.
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (callId)
            {
                case (byte)CustomRpc.HostSetHat:
                    {
                        uint playerId = reader.ReadPackedUInt32();
                        if (playerId == PlayerControl.LocalPlayer.PlayerId)
                            AlternativeRpcSender.RpcSetHat(reader.ReadPackedUInt32());
                        break;
                    }

                case (byte)CustomRpc.HostSetPet:
                    {
                        uint playerId = reader.ReadPackedUInt32();
                        if (playerId == PlayerControl.LocalPlayer.PlayerId)
                            AlternativeRpcSender.RpcSetPet(reader.ReadPackedUInt32());
                        break;
                    }

                case (byte)CustomRpc.HostSetSkin:
                    {
                        uint playerId = reader.ReadPackedUInt32();
                        if (playerId == PlayerControl.LocalPlayer.PlayerId)
                            AlternativeRpcSender.RpcSetSkin(reader.ReadPackedUInt32());
                        break;
                    }
                case (byte)CustomRpc.SetModActive:
                    MarkModeMain.ModActive.SetValue(reader.ReadBoolean());
                    break;
                case (byte)CustomRpc.SetPetsAllowed:
                    MarkModeMain.PetsAllowed.SetValue(reader.ReadBoolean());
                    break;
                case (byte)CustomRpc.SetSkinsAllowed:
                    MarkModeMain.SkinsAllowed.SetValue(reader.ReadBoolean());
                    break;
            }
        }
    }

    /* This class encapsulates the MessageWriter stuff, so we can use a simple one line call anywhere else, where we
     * want to send CustomRpc packages.   
     */
    static class CustomRpcSender
    {
        public static void RpcHostSetHat(byte playerId, uint hatId)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.HostSetHat, SendOption.Reliable);
            messageWriter.WritePacked(playerId);
            messageWriter.WritePacked(hatId);
            messageWriter.EndMessage();
        }

        public static void RpcHostSetPet(byte playerId, uint petId)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.HostSetPet, SendOption.Reliable);
            messageWriter.WritePacked(playerId);
            messageWriter.WritePacked(petId);
            messageWriter.EndMessage();
        }

        public static void RpcHostSetSkin(byte playerId, uint skinId)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.HostSetSkin, SendOption.Reliable);
            messageWriter.WritePacked(playerId);
            messageWriter.WritePacked(skinId);
            messageWriter.EndMessage();
        }

        public static void RpcSetModActive(bool active)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetModActive, SendOption.Reliable);
            messageWriter.Write(active);
            messageWriter.EndMessage();
        }

        public static void RpcSetPetsAllowed(bool active)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetPetsAllowed, SendOption.Reliable);
            messageWriter.Write(active);
            messageWriter.EndMessage();
        }

        public static void RpcSetSkinsAllowed(bool active)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSkinsAllowed, SendOption.Reliable);
            messageWriter.Write(active);
            messageWriter.EndMessage();
        }
    }

    /* This class is needed to replace the functions of PlayerControl that set hat, pet and skin.
     * These functions are deactivated in ClientRpcDisablePatches.cs since they would allow the
     * players to change these settings, since they are communicated directly from the client.
     * But since we still need to communicate changes from the clients we have to use these functions.
     * Maybe they can be removed, if we find a way to securely hide all settings panels in the lobby.
     */
    static class AlternativeRpcSender
    {
        public static void RpcSetHat(uint hatId)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (AmongUsClient.Instance.AmClient)
            {
                GameData.PlayerInfo data = player.Data;
                int colorId = (int)((data != null) ? data.ColorId : 0);
                player.SetHat(hatId, colorId);
            }
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(player.NetId, (byte)Rpc.SetHat, SendOption.Reliable);
            messageWriter.WritePacked(hatId);
            messageWriter.EndMessage();
        }

        public static void RpcSetPet(uint petId)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (AmongUsClient.Instance.AmClient)
            {
                player.SetPet(petId);
            }
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(player.NetId, (byte)Rpc.SetPet, SendOption.Reliable);
            messageWriter.WritePacked(petId);
            messageWriter.EndMessage();
        }

        public static void RpcSetSkin(uint skinId)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (AmongUsClient.Instance.AmClient)
            {
                player.SetSkin(skinId);
            }
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(player.NetId, (byte)Rpc.SetSkin, SendOption.Reliable);
            messageWriter.WritePacked(skinId);
            messageWriter.EndMessage();
        }
    }
}
