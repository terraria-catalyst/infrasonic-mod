using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using InfraSonic.IO;
using Terraria.GameContent;

namespace InfraSonic.Systems;

internal class IconDrawingSystem : ModSystem
{
    private readonly int[] playerSpeaking = new int[Main.maxPlayers];

    private readonly float[] iconOpacity = new float[Main.maxPlayers];

    public override void PostUpdateTime()
    {
        for (var i = 0; i < Main.maxPlayers; i++)
        {
            ref int speakRemainingTime = ref playerSpeaking[i];

            ref float opacity = ref iconOpacity[i];

            speakRemainingTime--;

            bool speaking = speakRemainingTime > 0;

            if (speaking)
                opacity += 0.2f;
            else
                opacity -= 0.2f;

            opacity = Math.Clamp(opacity, 0f, 1f);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        LegacyGameInterfaceLayer speakingPlayersLayer = new("InfraSonic: VoiceIcons", DrawIcons, InterfaceScaleType.UI);

        int index = layers.FindIndex(l => l.Name == "Vanilla: Player Chat");

        if (index != -1)
            layers.Insert(index, speakingPlayersLayer);
    }

    public void SetPlayerSpeaking(int player, int value) => playerSpeaking[player] = value;

    public List<int> GetSpeakingPlayers()
    {
        List<int> speaking = new();

        for (int i = 0; i < playerSpeaking.Length; i++)
        {
            if (playerSpeaking[i] > 0)
                speaking.Add(i);
        }

        return speaking;
    }

    private bool DrawIcons()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        if (data.NoIcons.Value)
        {
            return true;
        }

        int timer = (int)Main.GameUpdateCount;

        Texture2D texture = ModAsset.Speaking.Value;

        int frameY = timer / 12 % 3 * (texture.Height / 3);

        Rectangle frame = new(0, frameY, texture.Width, texture.Height / 3);

        int x = 8;
        int y = Main.screenHeight - 8;

        for (var i = 0; i < Main.maxPlayers; i++)
        {
            float opacity = iconOpacity[i];

            if (opacity <= 0)
                continue;

            y -= frame.Height + 4;

            Vector2 position = new(x, y);

            var iconColor = i == Main.myPlayer ? Main.OurFavoriteColor : Color.White;

            iconColor *= opacity;

            Main.spriteBatch.Draw(texture, position, frame, iconColor);

            position.X += frame.Width + 4;

            DrawPlayerHead(Main.player[i], ref position, opacity, 0.8f);
        }

        // This icon is not necessary. Player head drawn on the left is enough
        // DrawMicrophoneIcon(data);

        return true;
    }

    private bool DrawMicrophoneIcon(UserDataStore data)
    {
        Texture2D texture = ModAsset.Microphone.Value;

        int frameY = data.MicrophoneEnabled.Value ? 0 : texture.Height / 2;

        Rectangle frame = new(0, frameY, texture.Width, texture.Height / 2);

        Vector2 position = Main.ScreenSize.ToVector2() - frame.Size() - new Vector2(6);

        Main.spriteBatch.Draw(texture, position, frame, Color.White);

        return true;
    }

    private void DrawPlayerHead(Player drawPlayer, ref Vector2 position, float opacity = 1f, float scale = 1f)
    {
        PlayerHeadDrawRenderTargetContent playerHeadDrawRenderTargetContent =
            Main.MapPlayerRenderer._playerRenders[drawPlayer.whoAmI];

        playerHeadDrawRenderTargetContent.UsePlayer(drawPlayer);
        playerHeadDrawRenderTargetContent.Request();

        Main.MapPlayerRenderer._anyDirty = true;
        Main.MapPlayerRenderer._drawData.Clear();

        if (!playerHeadDrawRenderTargetContent.IsReady)
            return;

        RenderTarget2D target = playerHeadDrawRenderTargetContent.GetTarget();

        Vector2 origin = target.Size() / 2f;

        position += new Vector2(10f, 8f);

        Main.spriteBatch.Draw(target, position, null, Color.White * opacity, 0, origin, scale, SpriteEffects.None, 0);
    }
}
