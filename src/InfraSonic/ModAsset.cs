using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace InfraSonic;

internal static class ModAsset
{
    private static readonly AssetRepository _repo;

    static ModAsset()
    {
        _repo = ModLoader.GetMod("InfraSonic").Assets;
    }

    public const string MicrophonePath = @"Assets/Microphone";
    public static Asset<Texture2D> Microphone => _repo.Request<Texture2D>(MicrophonePath, AssetRequestMode.ImmediateLoad);

    public const string SpeakingPath = @"Assets/Speaking";
    public static Asset<Texture2D> Speaking => _repo.Request<Texture2D>(SpeakingPath, AssetRequestMode.ImmediateLoad);

    public const string AudioVisualiserWidgetPath = @"Assets/UI/AudioVisualiserWidget";
    public static Asset<Texture2D> AudioVisualiserWidget => _repo.Request<Texture2D>(AudioVisualiserWidgetPath, AssetRequestMode.ImmediateLoad);

    public const string DenoisePath = @"Assets/UI/Denoise";
    public static Asset<Texture2D> Denoise => _repo.Request<Texture2D>(DenoisePath, AssetRequestMode.ImmediateLoad);

    public const string DeviceSwitcherButtonPath = @"Assets/UI/DeviceSwitcherButton";
    public static Asset<Texture2D> DeviceSwitcherButton => _repo.Request<Texture2D>(DeviceSwitcherButtonPath, AssetRequestMode.ImmediateLoad);

    public const string DeviceSwitcherWidgetPath = @"Assets/UI/DeviceSwitcherWidget";
    public static Asset<Texture2D> DeviceSwitcherWidget => _repo.Request<Texture2D>(DeviceSwitcherWidgetPath, AssetRequestMode.ImmediateLoad);

    public const string IndicatorPath = @"Assets/UI/Indicator";
    public static Asset<Texture2D> Indicator => _repo.Request<Texture2D>(IndicatorPath, AssetRequestMode.ImmediateLoad);

    public const string KnobPath = @"Assets/UI/Knob";
    public static Asset<Texture2D> Knob => _repo.Request<Texture2D>(KnobPath, AssetRequestMode.ImmediateLoad);

    public const string KnobBaseplatePath = @"Assets/UI/KnobBaseplate";
    public static Asset<Texture2D> KnobBaseplate => _repo.Request<Texture2D>(KnobBaseplatePath, AssetRequestMode.ImmediateLoad);

    public const string KnobMarkingsPath = @"Assets/UI/KnobMarkings";
    public static Asset<Texture2D> KnobMarkings => _repo.Request<Texture2D>(KnobMarkingsPath, AssetRequestMode.ImmediateLoad);

    public const string KnobTurnsPath = @"Assets/UI/KnobTurns";
    public static Asset<Texture2D> KnobTurns => _repo.Request<Texture2D>(KnobTurnsPath, AssetRequestMode.ImmediateLoad);

    public const string MainPanelPath = @"Assets/UI/MainPanel";
    public static Asset<Texture2D> MainPanel => _repo.Request<Texture2D>(MainPanelPath, AssetRequestMode.ImmediateLoad);

    public const string MicPath = @"Assets/UI/Mic";
    public static Asset<Texture2D> Mic => _repo.Request<Texture2D>(MicPath, AssetRequestMode.ImmediateLoad);

    public const string MuteButtonPath = @"Assets/UI/MuteButton";
    public static Asset<Texture2D> MuteButton => _repo.Request<Texture2D>(MuteButtonPath, AssetRequestMode.ImmediateLoad);

    public const string NoIconsPath = @"Assets/UI/NoIcons";
    public static Asset<Texture2D> NoIcons => _repo.Request<Texture2D>(NoIconsPath, AssetRequestMode.ImmediateLoad);

    public const string OpenMicPath = @"Assets/UI/OpenMic";
    public static Asset<Texture2D> OpenMic => _repo.Request<Texture2D>(OpenMicPath, AssetRequestMode.ImmediateLoad);

    public const string PlayerScreenPath = @"Assets/UI/PlayerScreen";
    public static Asset<Texture2D> PlayerScreen => _repo.Request<Texture2D>(PlayerScreenPath, AssetRequestMode.ImmediateLoad);

    public const string PushToTalkPath = @"Assets/UI/PushToTalk";
    public static Asset<Texture2D> PushToTalk => _repo.Request<Texture2D>(PushToTalkPath, AssetRequestMode.ImmediateLoad);

    public const string RangeMarksPath = @"Assets/UI/RangeMarks";
    public static Asset<Texture2D> RangeMarks => _repo.Request<Texture2D>(RangeMarksPath, AssetRequestMode.ImmediateLoad);

    public const string RangeWidgetPath = @"Assets/UI/RangeWidget";
    public static Asset<Texture2D> RangeWidget => _repo.Request<Texture2D>(RangeWidgetPath, AssetRequestMode.ImmediateLoad);

    public const string SliderPath = @"Assets/UI/Slider";
    public static Asset<Texture2D> Slider => _repo.Request<Texture2D>(SliderPath, AssetRequestMode.ImmediateLoad);

    public const string SliderKnobPath = @"Assets/UI/SliderKnob";
    public static Asset<Texture2D> SliderKnob => _repo.Request<Texture2D>(SliderKnobPath, AssetRequestMode.ImmediateLoad);

    public const string Switch_OffPath = @"Assets/UI/Switch_Off";
    public static Asset<Texture2D> Switch_Off => _repo.Request<Texture2D>(Switch_OffPath, AssetRequestMode.ImmediateLoad);

    public const string Switch_OnPath = @"Assets/UI/Switch_On";
    public static Asset<Texture2D> Switch_On => _repo.Request<Texture2D>(Switch_OnPath, AssetRequestMode.ImmediateLoad);

    public const string TestPath = @"Assets/UI/Test";
    public static Asset<Texture2D> Test => _repo.Request<Texture2D>(TestPath, AssetRequestMode.ImmediateLoad);
}
