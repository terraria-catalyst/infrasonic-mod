using Terraria.ModLoader;
using InfraSonic.IO;

namespace InfraSonic.Systems;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PreSaveAndQuit()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.ForceSave();
    }
}
