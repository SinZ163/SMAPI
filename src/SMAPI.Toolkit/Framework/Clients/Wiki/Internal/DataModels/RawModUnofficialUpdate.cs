namespace StardewModdingAPI.Toolkit.Framework.Clients.Wiki.Internal.DataModels;

/// <summary>As part of <see cref="RawModEntry"/>, an unofficial update which fixes compatibility with the latest Stardew Valley and SMAPI versions.</summary>
internal class RawModUnofficialUpdate
{
    /// <summary>The version of the unofficial update.</summary>
    public string? Version { get; set; }

    /// <summary>The URL to the unofficial update.</summary>
    public string? Url { get; set; }
}
