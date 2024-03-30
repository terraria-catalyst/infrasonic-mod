using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using POpusCodec;
using POpusCodec.Enums;
using System;
using Terraria.Audio;

namespace TerraVoice.Systems;

public class PlayerSpeaker : IDisposable
{
    public const string DummySound = "InfraSonic:PlayerSpeakerDummy";

    public DynamicSoundEffectInstance SoundEffectInstance { get; private set; }

    public float Volume
    {
        get => SoundEffectInstance.Volume;
        set => SoundEffectInstance.Volume = value;
    }

    public float Pan
    {
        get => SoundEffectInstance.Pan;
        set => SoundEffectInstance.Pan = value;
    }

    private readonly int whoAmI;

    private readonly OpusDecoder decoder;

    private ActiveSound activeSound;

    public PlayerSpeaker(int whoAmI) 
    {
        this.whoAmI = whoAmI;

        SoundEffectInstance = new DynamicSoundEffectInstance(VoiceInputSystem.SampleRate, AudioChannels.Mono);

        decoder = new(SamplingRate.Sampling48000, Channels.Mono);
    }

    public void Dispose() 
    {
        SoundEffectInstance?.Dispose();
        SoundEffectInstance = null;
    }

    public void PlayAsActiveSound()
    {
        SoundStyle dummyStyle = new($"{DummySound}:{whoAmI}");

        // The sound path passed in is that of a dummy sound.
        // This notifies the IL edit that the given sound should not be played.
        // Instead, the style's pitch variance (whoAmI) will be used to substitute in a PlayerSpeaker sound.
        activeSound = new ActiveSound(dummyStyle);
    }

    public void UpdatePosition(Vector2 playerPosition)
    {
        // The constructor isn't called on the same thread as this method, specifically when in test mode (the microphone runs on another thread).
        if (activeSound != null)
        {
            activeSound.Position = playerPosition;
        }
    }

    public void SubmitBuffer(byte[] buffer)
    {
        short[] samples = decoder.DecodePacket(buffer);

        byte[] decoded = new byte[samples.Length * 2];

        Buffer.BlockCopy(samples, 0, decoded, 0, decoded.Length);

        SoundEffectInstance.SubmitBuffer(decoded);
    }
}