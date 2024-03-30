using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class ClickButton : SmartUIElement
{
    private const int ButtonWidth = 32;
    private const int ButtonHeight = 32;
    private const int PressDuration = 7;

    private readonly Texture2D texture;

    private readonly Action onClick;

    private int pressedTimer;

    public ClickButton(Texture2D texture, Action onClick) : base(null)
    {
        this.texture = texture;
        this.onClick = onClick;

        Width.Set(ButtonWidth, 0);
        Height.Set(ButtonHeight, 0);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        onClick?.Invoke();

        SoundStyle sound = new("TerraVoice/Assets/Sounds/UI/SwitchOn")
        {
            Volume = 0.5f,
            Pitch = 0.8f
        };

        SoundEngine.PlaySound(sound);

        pressedTimer = PressDuration;
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        pressedTimer--;

        if (pressedTimer < 0)
        {
            pressedTimer = 0;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        Rectangle sourceRectangle = new(pressedTimer > 0 ? 0 : (texture.Width / 2), 0, texture.Width / 2, texture.Height);

        spriteBatch.Draw(texture, position - new Vector2(0, pressedTimer > 0 ? 0 : 4), sourceRectangle, Color.White);
    }
}
