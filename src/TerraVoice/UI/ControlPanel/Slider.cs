using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraVoice.UI.Abstract;
using ReLogic.Graphics;
using Terraria.UI;
using Terraria;
using System;
using Terraria.Localization;

namespace TerraVoice.UI.ControlPanel;

internal class Slider : SmartUIElement
{
    private readonly Ref<int> setting;

    private const int SliderWidth = 530;
    private const int SliderHeight = 32;
    private const int BaseHeight = 12;
    private const int KnobWidth = 12;

    private readonly int maxRange;

    private float factor;

    private bool sliding;

    public Slider(int maxRange, Ref<int> setting) : base("Slider")
    {
        this.maxRange = maxRange;
        this.setting = setting;

        Width.Set(SliderWidth, 0);
        Height.Set(SliderHeight, 0);

        factor = setting.Value / (float)maxRange;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle drawBox = GetDimensions().ToRectangle();

        DrawSlider(spriteBatch, drawBox);
        DrawIndicator(spriteBatch, drawBox);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        if (Main.gamePaused)
        {
            return;
        }

        Rectangle drawBox = GetDimensions().ToRectangle();

        if (drawBox.Contains(Main.MouseScreen.ToPoint()))
        {
            sliding = true;
        }
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        if (Main.gamePaused)
        {
            return;
        }

        Rectangle drawBox = GetDimensions().ToRectangle();

        float width = GetDimensions().Width - KnobWidth;

        if (sliding)
        {
            factor = ((int)Main.MouseScreen.X - drawBox.X) / width;
            factor = MathHelper.Clamp(factor, 0, 1);
        }

        if (!Main.mouseLeft)
        {
            sliding = false;
        }

        setting.Value = (int)MathF.Floor(factor * maxRange);
    }

    private void DrawSlider(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int gap = (drawBox.Height - BaseHeight) / 2;

        Vector2 sliderBasePosition = new(drawBox.X, drawBox.Y + gap);

        spriteBatch.Draw(ModAsset.Slider.Value, sliderBasePosition, Color.White);

        spriteBatch.Draw(ModAsset.RangeMarks.Value, sliderBasePosition - new Vector2(0, 8), Color.White);

        Vector2 sliderKnobPosition = new(drawBox.X + (factor * (drawBox.Width - KnobWidth)), drawBox.Y - 6);

        spriteBatch.Draw(ModAsset.SliderKnob.Value, sliderKnobPosition, Color.White);
    }

    private void DrawIndicator(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int x = drawBox.X + drawBox.Width + VoiceControlPanel.Spacing - 2;

        Rectangle indicatorBox = new(x, drawBox.Y, 64, drawBox.Height);

        spriteBatch.Draw(ModAsset.RangeWidget.Value, new Vector2(indicatorBox.X, indicatorBox.Y), Color.White);

        string infText = Language.GetTextValue($"Mods.TerraVoice.UI.Inf");
        string text = setting.Value == 0 ? infText : setting.Value.ToString();

        Vector2 boxMiddle = new(indicatorBox.X + (indicatorBox.Width / 2), indicatorBox.Y + (indicatorBox.Height / 2));

        Vector2 halfTextSize = TerraVoice.Font.MeasureString(text).RoundEven() / 2;

        spriteBatch.DrawString(TerraVoice.Font, text, boxMiddle - halfTextSize, TerraVoice.Cyan);
    }
}
