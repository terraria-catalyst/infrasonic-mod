using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace InfraSonic.Systems;

public class BgmFadeSystem : ModSystem
{
    private float bgmFadeOut;

    public override void Load()
    {
        On_LegacyAudioSystem.UpdateCommonTrack += ApplyDucking;
        On_LegacyAudioSystem.UpdateCommonTrackTowardStopping += ApplyDuckingTowardStopping;
    }

    public override void PostUpdateTime()
    {
        IconDrawingSystem iconSystem = ModContent.GetInstance<IconDrawingSystem>();

        // Background music ducking is applied if any players are speaking, and they aren't the user.
        List<int> speakingPlayers = iconSystem.GetSpeakingPlayers();

        int necessaryCountToFade = speakingPlayers.Contains(Main.myPlayer) ? 2 : 1;

        bool anyPlayerSpeaking = speakingPlayers.Count > necessaryCountToFade;

        if (anyPlayerSpeaking)
            bgmFadeOut += 0.05f;
        else
            bgmFadeOut -= 0.05f;

        bgmFadeOut = Math.Clamp(bgmFadeOut, 0f, 0.5f);
    }

    private void ApplyDucking(On_LegacyAudioSystem.orig_UpdateCommonTrack orig,
        LegacyAudioSystem self, bool active, int i, float volume, ref float fade)
    {
        volume *= 1f - bgmFadeOut;

        orig(self, active, i, volume, ref fade);
    }

    private void ApplyDuckingTowardStopping(
        On_LegacyAudioSystem.orig_UpdateCommonTrackTowardStopping orig,
        LegacyAudioSystem self, int i, float volume, ref float fade, bool audible)
    {
        volume *= 1f - bgmFadeOut;

        orig(self, i, volume, ref fade, audible);
    }
}