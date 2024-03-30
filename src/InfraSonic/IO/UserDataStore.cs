using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace InfraSonic.IO;

internal class UserDataStore : PersistentDataStore
{
    public override string FileName => "voice_settings.dat";

    public Ref<bool> MicrophoneEnabled = new();
    public Ref<bool> TestMode = new();
    public Ref<bool> NoiseSuppression = new();
    public Ref<bool> NoIcons = new();
    public Ref<bool> OpenMic = new();
    public Ref<bool> PushToTalk = new(true);

    public Ref<float> Amplification = new(1);

    public Ref<int> ProximityDistance = new(64);
    public Ref<int> Channel = new();
    
    public Ref<string> Device = new();

    public List<string> TemporaryMuteList { get; private set; } = new();

    public override void LoadGlobal(TagCompound tag)
    {
        LoadTag(tag, nameof(MicrophoneEnabled), MicrophoneEnabled);
        LoadTag(tag, nameof(TestMode), TestMode);
        LoadTag(tag, nameof(NoiseSuppression), NoiseSuppression);
        LoadTag(tag, nameof(NoIcons), NoIcons);
        LoadTag(tag, nameof(OpenMic), OpenMic);
        LoadTag(tag, nameof(PushToTalk), PushToTalk);

        LoadTag(tag, nameof(Amplification), Amplification);

        LoadTag(tag, nameof(ProximityDistance), ProximityDistance);
        LoadTag(tag, nameof(Channel), Channel);

        LoadTag(tag, nameof(Device), Device);
    }

    public override void SaveGlobal(TagCompound tag)
    {
        tag[nameof(MicrophoneEnabled)] = MicrophoneEnabled.Value;
        tag[nameof(TestMode)] = TestMode.Value;
        tag[nameof(NoiseSuppression)] = NoiseSuppression.Value;
        tag[nameof(NoIcons)] = NoIcons.Value;
        tag[nameof(OpenMic)] = OpenMic.Value;
        tag[nameof(PushToTalk)] = PushToTalk.Value;

        tag[nameof(Amplification)] = Amplification.Value;

        tag[nameof(ProximityDistance)] = ProximityDistance.Value;
        tag[nameof(Channel)] = Channel.Value;

        tag[nameof(Device)] = Device.Value;
    }

    private void LoadTag<T>(TagCompound tag, string name, Ref<T> property)
    {
        if (tag.ContainsKey(name))
        {
            property.Value = tag.Get<T>(name);
        }
    }
}
