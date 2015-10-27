using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using Microsoft.CodeAnalysis.Emit;

namespace SemanticsCompilation
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessAsync().Wait();
        }

        static async Task ProcessAsync()
        {
            string source = File.ReadAllText("HelloWorld.cs");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = (await tree.GetRootAsync()) as CompilationUnitSyntax;

            // get Hello method
            MethodDeclarationSyntax helloMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.ValueText == "Hello").FirstOrDefault();
            // get hello variable
            VariableDeclaratorSyntax helloVariable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().Where(v => v.Identifier.ValueText == "hello").FirstOrDefault();


            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);
            EmitResult result = compilation.Emit("HelloWorld.exe");
            

            ISymbol helloVariableSymbol1 = compilation.GetSymbolsWithName(name => name == "Hello").FirstOrDefault();
            

            SemanticModel model = compilation.GetSemanticModel(tree);


            ISymbol helloVariableSymbol = model.GetDeclaredSymbol(helloVariable);
            IMethodSymbol helloMethodSymbol = model.GetDeclaredSymbol(helloMethod);

            ShowSymbol(helloVariableSymbol);
            ShowSymbol(helloMethodSymbol);


        }

        private static void ShowSymbol(ISymbol symbol)
        {
            WriteLine(symbol.Name);
            WriteLine(symbol.Kind);
            WriteLine(symbol.ContainingSymbol);
            WriteLine(symbol.ContainingType);
            WriteLine((symbol as IMethodSymbol)?.MethodKind);
            WriteLine();
        }
    }
}
