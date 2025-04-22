using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace bstate.analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BStatePropertySetterAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BS0002";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Properties in BState classes must have private setters",
        messageFormat: "The property '{0}' in BState-derived class must have a private setter",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All public properties declared inside a class that extends bstate.core.BState must have private setters."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        
        // Skip non-public properties
        if (propertySymbol.DeclaredAccessibility != Accessibility.Public)
            return;
            
        // Skip properties without setters
        if (propertySymbol.SetMethod == null)
            return;
            
        // Skip properties with private setters
        if (propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Private)
            return;
            
        // Get containing type
        var containingType = propertySymbol.ContainingType;
        if (containingType == null)
            return;
            
        // Check if the containing type derives from BState
        if (!InheritsBState(containingType))
            return;
            
        // Report diagnostic
        var diagnostic = Diagnostic.Create(Rule, propertySymbol.Locations[0], propertySymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
    
    private bool InheritsBState(INamedTypeSymbol typeSymbol)
    {
        var currentType = typeSymbol;
        while (currentType != null && currentType.BaseType != null)
        {
            if (currentType.BaseType.ToDisplayString() == "bstate.core.BState")
            {
                return true;
            }
            
            currentType = currentType.BaseType;
        }
        
        return false;
    }
}
