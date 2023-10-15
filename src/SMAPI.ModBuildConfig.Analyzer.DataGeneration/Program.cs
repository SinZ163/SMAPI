using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using StardewModdingAPI.ModBuildConfig.Analyzer;
using StardewModdingAPI.Toolkit;

Console.WriteLine("Hello, World!");

var stardew = AssemblyDefinition.ReadAssembly("Stardew Valley.dll");

Dictionary<string, string> AssetMap = new Dictionary<string, string>();

List<string> messagesForLater = new();
foreach (var module in stardew.Modules)
{
    var types = module.GetTypes().Where(type => type.BaseType != null);

    foreach (var type in types)
    {
        foreach (var method in type.Methods)
        {
            if (method.HasBody)
            {

                ILProcessor cil = method.Body.GetILProcessor();
                Collection<Instruction> instructions = cil.Body.Instructions;

                Instruction prevInstruction = Instruction.Create(OpCodes.Nop);
                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode.Code == Code.Nop)
                        continue;
                    if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt || instruction.OpCode == OpCodes.Newobj)
                    {
                        var methodInstruction = (MethodReference)instruction.Operand;
                        if (methodInstruction.Name == "Load") {
                            if (methodInstruction.DeclaringType.FullName != "StardewValley.LocalizedContentManager")
                            {
                                Console.WriteLine("Found a Load that was not StardewValley.LocalizedContentManager " + method.DeclaringType.FullName);
                                continue;
                            }
                            var genericMethod = (GenericInstanceMethod)methodInstruction;
                            string genericParameter = genericMethod.GenericArguments[0]?.FullName ?? "<unknown>";
                            string? assetName = null;
                            if (prevInstruction.OpCode == OpCodes.Ldstr)
                            {
                                assetName = (string)prevInstruction.Operand;
                            }
                            else
                            {
                                messagesForLater.Add($"{method.FullName} is calling Load {methodInstruction} (prev instruction: {prevInstruction})");
                            }
                            if (assetName != null)
                            {
                                if (AssetMap.TryGetValue(assetName, out string existingType))
                                {
                                    if (genericParameter != existingType)
                                    {
                                        Console.WriteLine($"Found a new use of {assetName}: Previously {existingType} now {genericParameter}");
                                    }
                                }
                                else
                                {
                                    AssetMap[assetName] = genericParameter;
                                }
                            }
                        }
                    }
                    prevInstruction = instruction;
                }
            }
        }
    }
}


Directory.CreateDirectory("outputs");
var currentVersion = new SemanticVersion(stardew.Name.Version);
HashSet<string> missingAssets = new();
foreach (string file in Directory.GetFiles("outputs"))
{
    if (file.EndsWith(stardew.Name.Version + ".json")) continue;
    var otherVersion = new SemanticVersion(Path.GetFileNameWithoutExtension(file), true);
    if (otherVersion.IsNewerThan(currentVersion)) continue;
    try
    {
        var versionInfo = JsonSerializer.Deserialize<VersionModel>(File.ReadAllText(file));
        foreach (var (asset, type) in versionInfo.AssetMap)
        {
            if (!AssetMap.ContainsKey(asset))
            {
                missingAssets.Add(asset);
            }
        }
    }
    catch { }
}
string jsonOutput = JsonSerializer.Serialize(new VersionModel(AssetMap, missingAssets), new JsonSerializerOptions
{
    WriteIndented = true,
});
File.WriteAllText($"outputs/{stardew.Name.Version}.json", jsonOutput);




