using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using System.Linq;
using System.Runtime.InteropServices;
using static Terraria.ModLoader.Core.TmodFile;
using System;
using Terraria.ModLoader.UI;
using System.Reflection;
using POpusCodec;
using Terraria.Localization;

namespace TerraVoice.Native;

[Autoload(Side = ModSide.Client)]
internal class NativeFeatureSystem : ModSystem
{
    private static readonly Dictionary<string, Assembly> assemblyHints = new()
    {
        ["libopus"] = typeof(OpusEncoder).Assembly,
        ["librnnoise"] = typeof(rnnoise).Assembly
    };

    private readonly Dictionary<string, string> _loadLibrariesTextLocalization = new() {
        {"default", "Loading native libraries..."},
        {"zh-Hans", "正在加载本地程序集..."}
    };

    private readonly Dictionary<string, IntPtr> loadedLibs = new();

    public override void Load()
    {
        // Interface.loadMods.SetLoadStage($"{Mod.DisplayName}: Loading native libraries...", -1);
        Interface.loadMods.SubProgressText = Language.ActiveCulture.Name is "zh-Hans"
                ? _loadLibrariesTextLocalization["zh-Hans"]
                : _loadLibrariesTextLocalization["default"];

        Directory.CreateDirectory(TerraVoice.CachePath);

        Dictionary<string, string> binaryMap = ExtractPlatformBinaries();

        foreach (string key in assemblyHints.Keys)
        {
            Assembly assembly = assemblyHints[key];

            string path = binaryMap[key];

            NativeLibrary.SetDllImportResolver(assembly, (name, assembly, dllSearchPath) =>
            {
                if (!loadedLibs.ContainsKey(key))
                {
                    IntPtr handle = NativeLibrary.Load(path);

                    Mod.Logger.Info($"Successfully loaded native library: {path}. Handle: {handle}");

                    loadedLibs[key] = handle;
                }

                return loadedLibs[key];
            });
        }

        ALMonoMicrophone.LoadOpenAL(binaryMap["libopenal"]);
    }

    public override void Unload()
    {
        foreach (IntPtr handle in loadedLibs.Values)
        {
            NativeLibrary.Free(handle);
        }

        ALMonoMicrophone.UnloadOpenAL();
    }

    private void CopyLibFromTmod(FileEntry entry, string path)
    {
        using Stream stream = Mod.File.GetStream(entry);

        byte[] bytes = new byte[entry.Length];

        stream.Read(bytes, 0, bytes.Length);

        File.WriteAllBytes(path, bytes);

        Mod.Logger.Info($"Successfully installed native library: {path}.");
    }

    private Dictionary<string, string> ExtractPlatformBinaries()
    {
        Dictionary<string, string> binaries = new();

        // Don't need to use system path separators as this only refers to the tmod file.
        string path = $"lib/Native/";

        // No macOS for the forseeable future.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path += "win64";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            path += "linux64";
        }

        IEnumerable<FileEntry> nativeLibraries = Mod.File.files.Where(f => f.Key.StartsWith(path)).Select(f => f.Value);

        foreach (FileEntry fileEntry in nativeLibraries)
        {
            string destinationPath = Path.Combine(TerraVoice.CachePath, Path.GetFileName(fileEntry.Name));

            binaries.Add(Path.GetFileNameWithoutExtension(fileEntry.Name), destinationPath);

            if (!File.Exists(destinationPath))
            {
                CopyLibFromTmod(fileEntry, destinationPath);
            }
        }

        return binaries;
    }
}
