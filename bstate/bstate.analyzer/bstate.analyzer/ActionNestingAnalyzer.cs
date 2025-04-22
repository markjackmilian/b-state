using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace bstate.analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ActionNestingAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BS0001";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Action must be nested inside State",
        messageFormat: "The Action '{0}' is not nested inside a State class.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Actions implementing IAction must be nested inside a BState class."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        // Only class or record types
        if (namedTypeSymbol.TypeKind != TypeKind.Class && !namedTypeSymbol.IsRecord)
            return;

        // Must implement bstate.core.Classes.IAction
        if (!namedTypeSymbol.AllInterfaces.Any(i =>
                i.Name == "IAction" &&
                (i.ContainingNamespace.ToDisplayString() == "bstate.core" ||
                 i.ContainingNamespace.ToDisplayString() == "bstate.core.Classes")))
            return;

        // Check if nested inside a BState
        var containingType = namedTypeSymbol.ContainingType;
        if (containingType == null)
        {
            var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
            return;
        }

// Check if containing type inherits from bstate.core.BState
        var currentType = containingType;
        bool inheritsFromBState = false;
        while (currentType != null && currentType.BaseType != null)
        {
            if (currentType.BaseType.ToDisplayString() == "bstate.core.BState")
            {
                inheritsFromBState = true;
                break;
            }

            currentType = currentType.BaseType;
        }

        if (!inheritsFromBState)
        {
            var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}