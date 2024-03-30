using Microsoft.Xna.Framework;
using POpusCodec;
using POpusCodec.Enums;
using System;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.IO;
using TerraVoice.Native;

namespace TerraVoice.Systems;

[Autoload(Side = ModSide.Client)]
internal sealed class VoiceProcessingSystem : ModSystem
{

    public event Action<short[]> OnTestBufferReceived;

    // https://wiki.xiph.org/Opus_Recommended_Settings recommends 24Kb/s for fullband VoIP.
    private const int VoIPBitrate = 24_000;

    private OpusEncoder encoder;

    private VoiceOutputSystem outputSystem;

    public override void PostSetupContent()
    {
        rnnoise.rnnoise_create();

        encoder = new(SamplingRate.Sampling48000, Channels.Mono, OpusApplicationType.Voip, Delay.Delay20ms)
        {
            Bitrate = VoIPBitrate
        };

        outputSystem = ModContent.GetInstance<VoiceOutputSystem>();
    }

    public override void Unload()
    {
        rnnoise.rnnoise_destroy();
    }

    public void SubmitBuffer(short[] buffer)
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        if (data.NoiseSuppression.Value)
            rnnoise.rnnoise_process_frame(buffer);

        AmplifyBuffer(buffer, data.Amplification.Value);

        byte[] encoded = encoder.Encode(buffer);

        // When testing, submit the encoded buffer as though it was recieved as a packet.
        // This is more accurate for testing purposes as it factors in the encode/decode other players will hear.
        if (data.TestMode.Value)
        {
            outputSystem.RecieveBuffer(encoded, Main.myPlayer);

            // Invoke with the full buffer for use in the visualiser.
            OnTestBufferReceived?.Invoke(buffer);
        }

        // Don't send voice input if PTT is enabled and the key isn't pressed.
        if (data.PushToTalk.Value && !TerraVoice.PushToTalkActivated)
        {
            return;
        }

        TerraVoice.Instance.PushVoiceBuffer(encoded);
    }

    private void AmplifyBuffer(short[] buffer, float amplification)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            float sample = buffer[i];

            buffer[i] = (short)MathHelper.Clamp(buffer[i] * amplification, short.MinValue, short.MaxValue);
        }
    }
}
