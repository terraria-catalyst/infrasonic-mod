using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;
using InfraSonic.UI.Abstract;

namespace InfraSonic.UI.ControlPanel;

internal class DualKnob : SmartUIElement
{
    public const int KnobWidth = 128;
    public const int KnobHeight = 128;

    public float SmallKnobValue => (float)MathF.Round(MathHelper.Lerp(min, max, Math.Abs(InverseLerp(MinAngle, MaxAngle, angle))), 2);

    public int LargeKnobPosition => channel.Value;

    private const float MinAngle = MathHelper.Pi + MathHelper.PiOver2;
    private const float MaxAngle = -MathHelper.PiOver2;

    private const float SpinSensitivity = 1 / 3f;

    private float CursorOffsetFromCenterX => Main.MouseScreen.X - (GetDimensions().Position().X + GetDimensions().Width / 2);

    private float angle;

    private bool dragging;

    private float startOffset;

    private float startAngle;

    private readonly float min;
    private readonly float max;

    private readonly Ref<float> smallValue;

    private readonly Ref<int> channel;

    public DualKnob(float min, float max, Ref<float> smallValue, Ref<int> channel) : base("DualKnob")
    {
        this.min = min;
        this.max = max;
        this.smallValue = smallValue;
        this.channel = channel;

        angle = MathHelper.Lerp(MinAngle, MaxAngle, InverseLerp(min, max, smallValue.Value));

        Width.Set(KnobWidth, 0);
        Height.Set(KnobHeight, 0);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        spriteBatch.Draw(ModAsset.KnobBaseplate.Value, position, Color.White);

        DrawMarkings(spriteBatch, position);

        spriteBatch.Draw(ModAsset.Knob.Value, position, Color.White);

        DrawIndicator(spriteBatch, position);
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        PlayerInput.LockVanillaMouseScroll("InfraSonic/VoiceControlState");

        if (!Main.mouseLeft && dragging)
        {
            dragging = false;
            startOffset = 0;
        }

        if (dragging)
        {
            float currentOffset = CursorOffsetFromCenterX - startOffset;

            float radiansOffset = -MathHelper.ToRadians(currentOffset) * SpinSensitivity;

            angle = startAngle + radiansOffset;
        }

        angle = MathHelper.Clamp(angle, MaxAngle, MinAngle);

        smallValue.Value = SmallKnobValue;

        base.SafeUpdate(gameTime);
    }

    public override void SafeScrollWheel(UIScrollWheelEvent evt)
    {
        int sign = Math.Sign(evt.ScrollWheelValue);

        int oldPosition = channel.Value;

        channel.Value += sign;
        channel.Value = (int)MathHelper.Clamp(channel.Value, 0, 7);

        if (channel.Value != oldPosition)
        {
            PlaySound();
        }
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        Rectangle drawBox = GetDimensions().ToRectangle();

        if (drawBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
        {
            dragging = true;

            startOffset = CursorOffsetFromCenterX;
            startAngle = angle;
        }
    }

    private void DrawIndicator(SpriteBatch spriteBatch, Vector2 position)
    {
        Texture2D turns = ModAsset.KnobTurns.Value;

        Rectangle sourceRectangle = new(turns.Width / 8 * channel.Value, 0, turns.Width / 8, turns.Height);

        spriteBatch.Draw(turns, position, sourceRectangle, Color.White);
    }

    private void DrawMarkings(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(ModAsset.KnobMarkings.Value, position + new Vector2(64, 62), null, Color.White, -angle, new Vector2(64, 62), 1, SpriteEffects.None, 0);
    }

    private void PlaySound()
    {
        SoundStyle sound = new("InfraSonic/Assets/Sounds/UI/SwitchOn")
        {
            Volume = 0.5f,
            Pitch = 0.8f
        };

        SoundEngine.PlaySound(sound);
    }

    private float InverseLerp(float a, float b, float value) => (value - a) / (b - a);
}
