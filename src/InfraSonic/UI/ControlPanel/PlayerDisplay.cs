using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using InfraSonic.IO;
using InfraSonic.UI.Abstract;

namespace InfraSonic.UI.ControlPanel;

internal class PlayerDisplay : SmartUIElement
{
    private const int DisplayWidth = 344;
    private const int DisplayHeight = 80;

    private readonly Effect headShader;

    private int currentPlayerWhoAmI;

    public PlayerDisplay() : base("PlayerDisplay")
    {
        headShader = ModContent.Request<Effect>("InfraSonic/Assets/Effects/HeadShader", AssetRequestMode.ImmediateLoad).Value;

        Width.Set(DisplayWidth, 0);
        Height.Set(DisplayHeight, 0);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle drawBox = GetDimensions().ToRectangle();

        spriteBatch.Draw(ModAsset.PlayerScreen.Value, new Vector2(drawBox.X, drawBox.Y), Color.White);

        DrawCurrentPlayer(spriteBatch, drawBox);
    }

    public void NextPlayer()
    {
        List<int> onlineWhoAmI = new();

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];

            if (player.active && player.whoAmI != Main.myPlayer)
            {
                onlineWhoAmI.Add(i);
            }
        }

        int index = onlineWhoAmI.IndexOf(currentPlayerWhoAmI);

        if (index == -1)
        {
            index = 0;
        }
        else
        {
            index++;

            if (index >= onlineWhoAmI.Count)
            {
                index = 0;
            }
        }

        currentPlayerWhoAmI = onlineWhoAmI.Count > 0 ? onlineWhoAmI[index] : 0;
    }

    private void DrawCurrentPlayer(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            DrawMidpointString(spriteBatch, Language.GetTextValue($"Mods.InfraSonic.UI.NotOnline"), drawBox);

            return;
        }

        int onlinePlayers = GetPlayerCount();

        if (onlinePlayers <= 1)
        {
            DrawMidpointString(spriteBatch, Language.GetTextValue("Mods.InfraSonic.UI.NoOtherPlayers"), drawBox);

            return;
        }

        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        Player player = Main.player[currentPlayerWhoAmI];

        if (!player.active || currentPlayerWhoAmI == Main.myPlayer)
        {
            NextPlayer();
        }

        Vector2 drawPosition = new(drawBox.X + 7, drawBox.Y + 3);

        // One tile in terraria = 2ft.
        int distanceFeet = (int)(Main.LocalPlayer.Distance(player.Center) / 8);

        spriteBatch.DrawString(InfraSonic.Font, Language.GetTextValue("Mods.InfraSonic.UI.CurrentlyTracking"), drawPosition, InfraSonic.Pink);

        drawPosition.Y += 20;

        spriteBatch.DrawString(InfraSonic.Font, $"{player.name}", drawPosition, InfraSonic.Cyan);

        drawPosition.Y += 16;

        DrawPlayerHead(spriteBatch, player, drawPosition + new Vector2(280, -12), InfraSonic.Pink);

        spriteBatch.DrawString(InfraSonic.Font, Language.GetTextValue("Mods.InfraSonic.UI.Distance", distanceFeet), drawPosition, InfraSonic.Pink);

        drawPosition.Y += 16;

        bool playerMuted = data.TemporaryMuteList.Contains(Main.player[currentPlayerWhoAmI].name);

        spriteBatch.DrawString(InfraSonic.Font,
            Language.GetTextValue($"Mods.InfraSonic.UI.Muted{(playerMuted ? "Yes" : "No")}"),
            drawPosition,
            playerMuted ? InfraSonic.Cyan : InfraSonic.Pink);
    }

    private void DrawMidpointString(SpriteBatch spriteBatch, string display, Rectangle drawBox)
    {
        Vector2 middle = new(drawBox.X + (drawBox.Width / 2), drawBox.Y + (drawBox.Height / 2));

        Vector2 halfSize = InfraSonic.Font.MeasureString(display).RoundEven() / 2;

        spriteBatch.DrawString(InfraSonic.Font, display, middle - halfSize, InfraSonic.Cyan);
    }

    private int GetPlayerCount()
    {
        int count = 0;

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            if (Main.player[i].active)
                count++;
        }

        return count;
    }

    private void DrawPlayerHead(SpriteBatch spriteBatch, Player drawPlayer, Vector2 position, Color borderColor)
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

        headShader.Parameters["threshold"].SetValue(0.6f);
        headShader.Parameters["shade"].SetValue(InfraSonic.Pink.ToVector4());

        spriteBatch.End();
        spriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            headShader,
            Main.UIScaleMatrix
        );

        spriteBatch.Draw(target, position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);

        spriteBatch.End();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.LinearClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            Main.UIScaleMatrix
        );
    }

    public void ToggleMuteSelected()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient || currentPlayerWhoAmI == Main.myPlayer)
            return;

        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        string name = Main.player[currentPlayerWhoAmI].name;

        if (data.TemporaryMuteList.Contains(name))
        {
            data.TemporaryMuteList.Remove(name);
        }
        else
        {
            data.TemporaryMuteList.Add(name);
        }
    }
}
