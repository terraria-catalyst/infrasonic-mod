using Terraria.ModLoader;
using TerraVoice.IO;

namespace TerraVoice.Systems;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PreSaveAndQuit()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.ForceSave();
    }
}
