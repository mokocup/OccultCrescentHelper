using Dalamud.Configuration;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace OccultCrescentHelper;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool ShowFateToast = true;
    public bool PlayFateSfx = true;
    public uint FateSfx = Constants.PlayableSoundEffect[0].EffectId;

    public bool ShowFTToast = true;
    public bool PlayCESfx = true;
    public uint CESfx = Constants.PlayableSoundEffect[0].EffectId;

    public bool PlayFTSfx = true;
    public uint FTSfx = Constants.PlayableSoundEffect[0].EffectId;

    public void Save()
    {
        if (!Constants.PlayableSoundEffect.Exists(cSound => cSound.EffectId == FateSfx))
        {
            OccultCrescentHelper.Log.Debug($"FateSFX: {FateSfx} does not exist in Constants List, reset to default");
            FateSfx = Constants.PlayableSoundEffect[0].EffectId;
        }

        if (!Constants.PlayableSoundEffect.Exists(cSound => cSound.EffectId == CESfx))
        {
            OccultCrescentHelper.Log.Debug($"CESfx: {FateSfx} does not exist in Constants List, reset to default");
            CESfx = Constants.PlayableSoundEffect[0].EffectId;
        }

        if (!Constants.PlayableSoundEffect.Exists(cSound => cSound.EffectId == FTSfx))
        {
            OccultCrescentHelper.Log.Debug($"FTSfx: {FateSfx} does not exist in Constants List, reset to default");
            FTSfx = Constants.PlayableSoundEffect[0].EffectId;
        }

        OccultCrescentHelper.PluginInterface.SavePluginConfig(this);
    }
}
