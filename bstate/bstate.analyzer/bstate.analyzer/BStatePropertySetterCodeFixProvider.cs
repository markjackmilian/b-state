using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace bstate.analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BStatePropertySetterCodeFixProvider)), Shared]
public class BStatePropertySetterCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create(BStatePropertySetterAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() => 
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        
        // Find the property declaration identified by the diagnostic
        var propertyDeclaration = root.FindToken(diagnosticSpan.Start)
            .Parent.AncestorsAndSelf()
            .OfType<PropertyDeclarationSyntax>()
            .First();
        
        // Register a code action that will invoke the fix
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make property setter private",
                createChangedDocument: c => MakeSetterPrivateAsync(context.Document, propertyDeclaration, c),
                equivalenceKey: nameof(BStatePropertySetterCodeFixProvider)),
            diagnostic);
    }

    private async Task<Document> MakeSetterPrivateAsync(Document document, 
        PropertyDeclarationSyntax propertyDecl, CancellationToken cancellationToken)
    {
        // Get the current root and model
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        
        PropertyDeclarationSyntax newPropertyDecl;
        
        // Check if the property has an accessor list
        if (propertyDecl.AccessorList != null)
        {
            // Find the set accessor
            var setAccessor = propertyDecl.AccessorList.Accessors
                .FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword));
                
            if (setAccessor != null)
            {
                // Create a new set accessor with private modifier
                var newSetAccessor = setAccessor.WithModifiers(
                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                    .WithLeadingTrivia(setAccessor.GetLeadingTrivia())
                    .WithTrailingTrivia(setAccessor.GetTrailingTrivia());
                
                // Replace the old accessor with the new one
                var newAccessorList = propertyDecl.AccessorList.ReplaceNode(setAccessor, newSetAccessor);
                newPropertyDecl = propertyDecl.WithAccessorList(newAccessorList);
            }
            else
            {
                // This case shouldn't happen given our analyzer, but handling it for completeness
                return document;
            }
        }
        else if (propertyDecl.ExpressionBody != null)
        {
            // If it's an expression-bodied property, convert to one with accessors
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(propertyDecl.ExpressionBody)
                .WithSemicolonToken(propertyDecl.SemicolonToken);
                
            var privateSetAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                
            var accessorList = SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[] { getAccessor, privateSetAccessor }));
                
            newPropertyDecl = propertyDecl
                .WithExpressionBody(null)
                .WithAccessorList(accessorList)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        else if (propertyDecl.Initializer != null)
        {
            // For properties with initializers
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                
            var privateSetAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                
            var accessorList = SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[] { getAccessor, privateSetAccessor }));
                
            newPropertyDecl = propertyDecl
                .WithAccessorList(accessorList);
        }
        else
        {
            // Can't determine how to fix
            return document;
        }

        // Format the new property declaration
        newPropertyDecl = newPropertyDecl.WithAdditionalAnnotations(Formatter.Annotation);
        
        // Replace the old property declaration with the new one
        var newRoot = root.ReplaceNode(propertyDecl, newPropertyDecl);
        
        // Return the new document
        return document.WithSyntaxRoot(newRoot);
    }
}
