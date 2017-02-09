using Mono.Cecil;
using Mono.Cecil.Cil;
using StardewModdingAPI.AssemblyRewriters.Framework;
using StardewValley;

namespace StardewModdingAPI.AssemblyRewriters.Rewriters
{
    /// <summary>Rewrites field references to <see cref="Game1.activeClickableMenu"/>.</summary>
    /// <remarks>Stardew Valley changed the <see cref="Game1.activeClickableMenu"/> field to a property, which broke many mods that reference it.</remarks>
    public class ActiveClickableMenuRewriter : BaseFieldRewriter
    {
        /*********
        ** Protected methods
        *********/
        /// <summary>Get whether a field reference should be rewritten.</summary>
        /// <param name="instruction">The IL instruction.</param>
        /// <param name="fieldRef">The field reference.</param>
        /// <param name="platformChanged">Whether the mod was compiled on a different platform.</param>
        protected override bool ShouldRewrite(Instruction instruction, FieldReference fieldRef, bool platformChanged)
        {
            return
                (instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Stsfld) // static field
                && fieldRef.DeclaringType.FullName == typeof(Game1).FullName
                && fieldRef.Name == nameof(Game1.activeClickableMenu);
        }

        /// <summary>Rewrite a method for compatibility.</summary>
        /// <param name="module">The module being rewritten.</param>
        /// <param name="cil">The CIL rewriter.</param>
        /// <param name="instruction">The instruction which references the field.</param>
        /// <param name="fieldRef">The field reference invoked by the <paramref name="instruction"/>.</param>
        /// <param name="assemblyMap">Metadata for mapping assemblies to the current platform.</param>
        protected override void Rewrite(ModuleDefinition module, ILProcessor cil, Instruction instruction, FieldReference fieldRef, PlatformAssemblyMap assemblyMap)
        {
            string methodPrefix = instruction.OpCode == OpCodes.Ldsfld ? "get" : "set";
            MethodReference propertyRef = module.Import(typeof(Game1).GetMethod($"{methodPrefix}_{nameof(Game1.activeClickableMenu)}"));
            cil.Replace(instruction, cil.Create(OpCodes.Call, propertyRef));
        }
    }
}
