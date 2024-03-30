using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.UI;
using InfraSonic.UI.Abstract;

namespace InfraSonic.UI.ControlPanel;

internal class SwitchButton : SmartUIElement
{
    public const int SwitchWidth = 140;
    public const int SwitchHeight = 120;

    private readonly Texture2D icon;

    private readonly LocalizedText label;

    private readonly Ref<bool> setting;

    public SwitchButton(Texture2D icon, string label, Ref<bool> setting) : base(label)
    {
        this.icon = icon;
        this.label = Language.GetText($"Mods.InfraSonic.UI.{label}");
        this.setting = setting;

        Width.Set(SwitchWidth, 0);
        Height.Set(SwitchHeight, 0);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        Texture2D texture = setting.Value ? ModAsset.Switch_On.Value : ModAsset.Switch_Off.Value;

        spriteBatch.Draw(texture, position, Color.White);

        DrawLabel(spriteBatch, position);
        DrawIcon(spriteBatch, position);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        setting.Value = !setting.Value;

        SoundStyle sound = new($"InfraSonic/Assets/Sounds/UI/Switch{(setting.Value ? "On" : "Off")}")
        {
            Volume = 0.5f
        };

        SoundEngine.PlaySound(sound);
    }

    private void DrawLabel(SpriteBatch spriteBatch, Vector2 position)
    {
        // Offset needs to be an even number to prevent weird scaling issues.
        float stringWidth = InfraSonic.Font.MeasureString(label.Value).X;
        Vector2 textPosition = position + new Vector2((Width.Pixels / 2) - (stringWidth / 2), Height.Pixels + 1);

        spriteBatch.DrawString(InfraSonic.Font, label.Value, textPosition, InfraSonic.Pink);
    }

    private void DrawIcon(SpriteBatch spriteBatch, Vector2 position)
    {
        Vector2 iconMiddle = position + new Vector2(99, 80);
        Vector2 halfIconSize = new(icon.Width / 2, icon.Height / 2);

        spriteBatch.Draw(icon, iconMiddle - halfIconSize, Color.White);
    }
}
