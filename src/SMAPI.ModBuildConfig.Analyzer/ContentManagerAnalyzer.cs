using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace StardewModdingAPI.ModBuildConfig.Analyzer
{
    public class VersionModel
    {
        public VersionModel(Dictionary<string, string> assetMap, HashSet<string> missingAssets)
        {
            this.AssetMap = assetMap;
            this.FormerAssets = missingAssets;
        }

        public Dictionary<string, string> AssetMap { get; set; }
        public HashSet<string> FormerAssets { get; set; }
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentManagerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        public VersionModel OneSixRules { get; }


        /// <summary>The diagnostic info for an avoidable net field access.</summary>
        private readonly DiagnosticDescriptor AvoidBadTypeRule = new(
            id: "AvoidContentManagerBadType",
            title: "Avoid incorrectly typing ContentManager Loads",
            messageFormat: "'{0}' uses the {1} type, but {2} is in use instead. See https://smapi.io/package/avoid-contentmanager-type for details.",
            category: "SMAPI.CommonErrors",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: "https://smapi.io/package/avoid-contentmanager-type"
        );

        public ContentManagerAnalyzer()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("StardewModdingAPI.ModBuildConfig.Analyzer.1.6.0.23268.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                this.OneSixRules = JsonConvert.DeserializeObject<VersionModel>(reader.ReadToEnd());
            }
            this.SupportedDiagnostics = ImmutableArray.CreateRange(new[] { this.AvoidBadTypeRule });
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                this.AnalyzeContentManagerLoads,
                SyntaxKind.InvocationExpression
            );
        }

        private void AnalyzeContentManagerLoads(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null || memberAccess.Name.Identifier.ValueText != "Load")
                return;
            // "Data\\Fish" -> Data\Fish
            string assetName = invocation.ArgumentList.Arguments[0].ToString().Replace("\"", "").Replace("\\\\", "\\");

            var formatter = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
            string genericArgument = context.SemanticModel.GetTypeInfo((memberAccess.Name as GenericNameSyntax).TypeArgumentList.Arguments[0]).Type.ToDisplayString(formatter).Replace(" ", "");

            if (this.OneSixRules.AssetMap.TryGetValue(assetName, out string expectedType))
            {
                expectedType = Regex.Replace(expectedType, "`\\d+", "");
                if (genericArgument != expectedType)
                {
                    context.ReportDiagnostic(Diagnostic.Create(this.AvoidBadTypeRule, context.Node.GetLocation(), assetName, expectedType, genericArgument));
                }
            }
        }
    }
}
