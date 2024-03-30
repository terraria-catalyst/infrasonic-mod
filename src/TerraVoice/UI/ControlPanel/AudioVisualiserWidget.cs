using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraVoice.Systems;
using TerraVoice.IO;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class AudioVisualiserWidget : SmartUIElement
{
    private readonly VoiceControlPanel panel;

    private readonly short[] testBuffer;

    public AudioVisualiserWidget(VoiceControlPanel panel) : base("Visualizer")
    {
        this.panel = panel;

        testBuffer = new short[(int)(VoiceInputSystem.SampleRate * (VoiceInputSystem.MicrophoneInputDurationMs / 1000f))];

        ModContent.GetInstance<VoiceProcessingSystem>().OnTestBufferReceived += SubmitTestBuffer;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        spriteBatch.Draw(ModAsset.AudioVisualiserWidget.Value, position, Color.White);

        DrawScreen(spriteBatch, position);
    }

    private void SubmitTestBuffer(short[] buffer)
    {
        Buffer.BlockCopy(buffer, 0, testBuffer, 0, buffer.Length * sizeof(short));
    }

    private void DrawScreen(SpriteBatch spriteBatch, Vector2 position)
    {
        Vector2 middle = position + new Vector2((Width.Pixels - 128) / 2, Height.Pixels / 2);

        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        string text = "";

        if (!data.TestMode.Value || !data.MicrophoneEnabled.Value)
        {
            text = Language.GetTextValue("Mods.TerraVoice.UI.EnableTestMode");
        }

        // Offset needs to be an even number to prevent weird scaling issues.
        Vector2 halfTextSize = TerraVoice.Font.MeasureString(text).RoundEven() / 2;

        spriteBatch.DrawString(TerraVoice.Font, text, middle - halfTextSize, TerraVoice.Cyan);

        if (data.TestMode.Value && data.MicrophoneEnabled.Value)
        {
            DrawAudioBars(spriteBatch, position + new Vector2(4));
        }

        DrawInfo(spriteBatch, position + new Vector2(4));
    }

    private void DrawInfo(SpriteBatch spriteBatch, Vector2 position)
    {
        int volumePercent = (int)(panel.ChannelAmplificationDualKnob.SmallKnobValue * 100);
        int channel = panel.ChannelAmplificationDualKnob.LargeKnobPosition;

        string volumeString = Language.GetTextValue($"Mods.TerraVoice.UI.VolumeDisplay");
        string channelString = Language.GetTextValue($"Mods.TerraVoice.UI.ChannelDisplay");

        Vector2 drawPosition = position + new Vector2(Width.Pixels - 128 + 8, 3);

        spriteBatch.DrawString(TerraVoice.Font, $"{volumeString} {volumePercent}%", drawPosition, TerraVoice.Pink);

        drawPosition.Y += TerraVoice.Font.LineSpacing - 1;

        spriteBatch.DrawString(TerraVoice.Font, $"{channelString} {channel}", drawPosition, TerraVoice.Pink);
    }

    private void DrawAudioBars(SpriteBatch spriteBatch, Vector2 position)
    {
        int blocks = ((int)Width.Pixels - 128) / 4;
        int currentBlock = 0;
        int visualiserHeight = (int)Height.Pixels - 8;
        int sensitivity = 6;
        int minHeight = 4;

        for (int offset = 0; offset < Width.Pixels - 128; offset += 6)
        {
            int positionStart = (int)((float)currentBlock / blocks * testBuffer.Length);
            int blockLength = testBuffer.Length / blocks;

            float value = (float)Average(testBuffer, positionStart, blockLength) / short.MaxValue;

            int height = (int)(value * (visualiserHeight - minHeight) * sensitivity) + minHeight;
            height = (int)MathHelper.Clamp(height, 4, visualiserHeight);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + offset, (int)position.Y + visualiserHeight / 2 - height / 2, 4, height / 2), TerraVoice.Cyan);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + offset, (int)position.Y + visualiserHeight / 2, 4, height / 2), TerraVoice.Cyan);

            currentBlock++;
        }
    }

    private short Average(short[] array, int start, int length)
    {
        int total = 0;

        for (int i = start; i < start + length; i++)
        {
            total += array[i];
        }

        return (short)(total / length);
    }
}
