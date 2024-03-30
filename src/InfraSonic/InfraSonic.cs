using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReLogic.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using InfraSonic.Systems;
using InfraSonic.UI;
using InfraSonic.UI.ControlPanel;

namespace InfraSonic;

public partial class InfraSonic : Mod
{
    public static DynamicSpriteFont Font =>
        Language.ActiveCulture.Name switch {
            "en-US" => englishFont,
            "zh-Hans" => chineseFont,
            _ => FontAssets.MouseText.Value
        };

    private static DynamicSpriteFont englishFont;
    private static DynamicSpriteFont chineseFont;

    // Kept to avoid settings from legacy versions getting replaced.
    public static readonly string CachePath = Path.Combine(Main.SavePath, "TerraVoice");

    public static readonly Color Cyan = new(130, 233, 229);

    public static readonly Color Pink = new(226, 114, 175);

    public static InfraSonic Instance { get; private set; }

    public static bool PushToTalkActivated { get; private set; }

    private static ModKeybind voiceBind;
    private static ModKeybind pushToTalk;

    public override void Load() 
    {
        Instance = this;

        if (!Main.dedServ)
        {
            voiceBind = KeybindLoader.RegisterKeybind(this, "VoiceControlPanel", "J");
            pushToTalk = KeybindLoader.RegisterKeybind(this, "PushToTalk", "V");

            englishFont = Assets.Request<DynamicSpriteFont>("Assets/Fonts/MP3-12", AssetRequestMode.ImmediateLoad).Value;
            chineseFont = Assets.Request<DynamicSpriteFont>("Assets/Fonts/LanaPixel-15", AssetRequestMode.ImmediateLoad).Value;
        }
    }

    [Autoload(Side = ModSide.Client)]
    private class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            VoiceControlState state = InfraSonicUILoader.GetUIState<VoiceControlState>();
            VoiceInputSystem inputSystem = ModContent.GetInstance<VoiceInputSystem>();

            if (voiceBind.JustPressed)
            {
                state.Visible = !state.Visible;

                state.Recalculate();

                SoundEngine.PlaySound(state.Visible ? SoundID.MenuOpen : SoundID.MenuClose);
            }

            PushToTalkActivated = false;

            if (pushToTalk.Current)
            {
                PushToTalkActivated = true;
            }
        }
    }
}