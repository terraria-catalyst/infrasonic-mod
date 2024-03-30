using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using InfraSonic.UI.Abstract;

namespace InfraSonic.UI.ControlPanel;

internal class VoiceControlState : SmartUIState
{
    public VoiceControlPanel Panel { get; private set; }

    public override int InsertionIndex(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

    public override void OnInitialize()
    {
        Panel = new(this);

        Append(Panel);
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        if (Main.gamePaused)
        {
            Visible = false;
        }
    }
}
