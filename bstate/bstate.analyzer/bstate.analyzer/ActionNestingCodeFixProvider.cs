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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ActionNestingCodeFixProvider)), Shared]
public class ActionNestingCodeFixProvider : CodeFixProvider
{
    private const string TitleFormat = "Move action inside a State class";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create(ActionNestingAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() => 
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic
        var typeDeclaration = root?.FindToken(diagnosticSpan.Start)
            .Parent?.AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();

        if (typeDeclaration == null)
            return;

        // Register a code action that will create a new state class and move the action inside it
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleFormat,
                createChangedDocument: c => CreateStateAndMoveActionAsync(context.Document, typeDeclaration, c),
                equivalenceKey: TitleFormat),
            diagnostic);
    }

    private async Task<Document> CreateStateAndMoveActionAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
    {
        // Get the current compilation
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        if (semanticModel == null)
            return document;

        // Get namespace information
        var namespaceDecl = typeDecl.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        var fileScopedNamespace = typeDecl.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
        var namespaceName = namespaceDecl?.Name.ToString() ?? fileScopedNamespace?.Name.ToString() ?? "YourNamespace";

        // Create a name for the new state class
        string stateClassName = $"{typeDecl.Identifier.Text}State";

        // Create a new state class with the action moved inside it
        var stateClassDecl = SyntaxFactory.ClassDeclaration(stateClassName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName("bstate.core.BState")))
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(
                new[] { typeDecl.WithoutTrivia() }))
            .WithAdditionalAnnotations(Formatter.Annotation);

        // Create a constructor for the state class
        var constructorDecl = SyntaxFactory.ConstructorDeclaration(stateClassName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("actionChannel"))
                    .WithType(SyntaxFactory.ParseTypeName("bstate.core.Services.IActionBus")))
            .WithInitializer(
                SyntaxFactory.ConstructorInitializer(
                    SyntaxKind.BaseConstructorInitializer,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("actionChannel"))))))
            .WithBody(SyntaxFactory.Block())
            .WithAdditionalAnnotations(Formatter.Annotation);

        // Add the constructor to the state class
        stateClassDecl = stateClassDecl.AddMembers(constructorDecl);

        // Add a protected override Initialize method
        var initializeMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), 
                "Initialize")
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
            .WithBody(SyntaxFactory.Block())
            .WithAdditionalAnnotations(Formatter.Annotation);

        stateClassDecl = stateClassDecl.AddMembers(initializeMethod);

        // Get the root node
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null)
            return document;

        // Replace the original type declaration with the new state class
        var newRoot = root.ReplaceNode(typeDecl, stateClassDecl);

        // Add required using directives if needed
        if (!newRoot.DescendantNodes().OfType<UsingDirectiveSyntax>()
                .Any(u => u.Name.ToString().Contains("bstate.core")))
        {
            var bstateUsingDirective = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("bstate.core"))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            var servicesUsingDirective = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("bstate.core.Services"))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            // Find the position for the new using statements
            var firstUsing = newRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().FirstOrDefault();
            if (firstUsing != null)
            {
                // Insert the new using directives before the existing ones
                newRoot = newRoot.InsertNodesBefore(firstUsing, new[] { bstateUsingDirective, servicesUsingDirective });
            }
            else
            {
                // If there are no existing using directives, add them at the beginning of the file
                newRoot = newRoot.InsertNodesBefore(newRoot.ChildNodes().First(),
                    new[] { bstateUsingDirective, servicesUsingDirective });
            }
        }

        // Create a new document with the updated root
        return document.WithSyntaxRoot(newRoot);
    }
}