using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Framework.ModLoading.Framework;
using StardewValley;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member: This is internal code to support rewriters and shouldn't be called directly.

namespace StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_6
{
    /// <summary>Maps Stardew Valley 1.5.6's <see cref="Game1"/> methods to their newer form to avoid breaking older mods.</summary>
    /// <remarks>This is public to support SMAPI rewriting and should never be referenced directly by mods. See remarks on <see cref="ReplaceReferencesRewriter"/> for more info.</remarks>
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = SuppressReasons.MatchesOriginal)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = SuppressReasons.MatchesOriginal)]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaRewriting)]
    public class Game1Facade
    {
        /*********
        ** Accessors
        *********/
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        public static bool menuUp { get; set; } // field was mostly unused and always false


        /*********
        ** Public methods
        *********/
        public static NPC getCharacterFromName(string name, bool mustBeVillager = true, bool useLocationsListOnly = false)
        {
            return Game1.getCharacterFromName(name, mustBeVillager);
        }

        public static string GetSeasonForLocation(GameLocation location)
        {
            Season season = Game1.GetSeasonForLocation(location);
            return season.ToString();
        }

        public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number)
        {
            Game1.createMultipleObjectDebris(index.ToString(), xTile, yTile, number);
        }

        public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number, GameLocation location)
        {
            Game1.createMultipleObjectDebris(index.ToString(), xTile, yTile, number, location);
        }

        public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number, float velocityMultiplier)
        {
            Game1.createMultipleObjectDebris(index.ToString(), xTile, yTile, number, velocityMultiplier);
        }

        public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number, long who)
        {
            Game1.createMultipleObjectDebris(index.ToString(), xTile, yTile, number, who);
        }

        public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number, long who, GameLocation location)
        {
            Game1.createMultipleObjectDebris(index.ToString(), xTile, yTile, number, who, location);
        }

        public static void createObjectDebris(int objectIndex, int xTile, int yTile, long whichPlayer)
        {
            Game1.createObjectDebris(objectIndex.ToString(), xTile, yTile, whichPlayer);
        }

        public static void createObjectDebris(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
        {
            Game1.createObjectDebris(objectIndex.ToString(), xTile, yTile, whichPlayer, location);
        }

        public static void createObjectDebris(int objectIndex, int xTile, int yTile, GameLocation location)
        {
            Game1.createObjectDebris(objectIndex.ToString(), xTile, yTile, location);
        }

        public static void createObjectDebris(int objectIndex, int xTile, int yTile, int groundLevel = -1, int itemQuality = 0, float velocityMultiplyer = 1f, GameLocation? location = null)
        {
            Game1.createObjectDebris(objectIndex.ToString(), xTile, yTile, groundLevel, itemQuality, velocityMultiplyer, location);
        }

        public static void drawDialogue(NPC speaker, string dialogue)
        {
            Game1.DrawDialogue(new Dialogue(speaker, null, dialogue));
        }

        public static void drawDialogue(NPC speaker, string dialogue, Texture2D overridePortrait)
        {
            Game1.DrawDialogue(new Dialogue(speaker, null, dialogue) { overridePortrait = overridePortrait });
        }


        /*********
        ** Private methods
        *********/
        private Game1Facade()
        {
            RewriteHelper.ThrowFakeConstructorCalled();
        }
    }
}
