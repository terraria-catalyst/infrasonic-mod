using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class RadioButton : SmartUIElement
{
    public const int ButtonWidth = 56;
    public const int ButtonHeight = 56;

    private readonly List<RadioButton> buttons;

    private readonly Ref<bool> setting;

    private readonly Texture2D icon;

    public RadioButton(List<RadioButton> buttons, Ref<bool> setting, Texture2D icon, string tooltip) : base(tooltip)
    {
        this.buttons = buttons;
        this.setting = setting;
        this.icon = icon;

        buttons.Add(this);

        Width.Set(ButtonWidth, 0);
        Height.Set(ButtonHeight, 0);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        if (!setting.Value)
        {
            position -= new Vector2(0, 8);
        }

        Rectangle sourceRectangle = new(setting.Value ? 0 : (icon.Width / 2), 0, icon.Width / 2, icon.Height);

        spriteBatch.Draw(icon, position, sourceRectangle, Color.White);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        if (setting.Value)
        {
            return;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != this)
            {
                buttons[i].setting.Value = false;
            }
        }

        setting.Value = true;

        SoundStyle sound = new("TerraVoice/Assets/Sounds/UI/SwitchOn")
        {
            Volume = 0.5f,
            Pitch = 0.8f
        };

        SoundEngine.PlaySound(sound);
    }
}
