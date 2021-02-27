using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace MarkMode
{
    /* This file includes everything to get BepInEx working and start the patching.     
     */
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class HarmonyMain : BasePlugin
    {
        public const string Id = "local.marks-st.markmode";

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            MarkModeMain.PlayerName = Config.Bind("Custom", "Player name", "Mark");
            MarkModeMain.PlayerColor = Config.Bind("Custom", "Player color", (ushort)Color.Orange);

            MarkModeMain.init();

            Harmony.PatchAll();
        }
    }
}
