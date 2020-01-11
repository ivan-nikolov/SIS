using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.Identity;

    public class SisViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewContent, T model, Principal user = null)
        {
            string csharpHtmlCode = GetSharpCode(viewContent);

            string code = $@"
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using SIS.MvcFramework.ViewEngine;
using SIS.MvcFramework.Identity;
namespace AppViewCodeNamespace
{{
    public class AppViewCode : IView
    {{
        public string GetHtml(object model, Principal user)
        {{
            var Model = {(model == null ? "new {}" : "model as " + GetModelType(model))};
            var User = user;

            var html = new StringBuilder();
            
            {csharpHtmlCode}
            
            return html.ToString();
        }}
    }}
}}
";
            var view = CompileAndInstance(code, model?.GetType().Assembly);
            var htmlResult = view?.GetHtml(model, user);

            return htmlResult;
        }

        private IView CompileAndInstance(string code, Assembly modelAssembly)
        {
            modelAssembly ??= Assembly.GetEntryAssembly();

            var compilation = CSharpCompilation.Create("AppViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(modelAssembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.GetEntryAssembly().Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location));

            var netStandardAssembly = Assembly.Load(new AssemblyName("netstandard"));
            compilation.AddReferences(MetadataReference.CreateFromFile(netStandardAssembly.Location));

            var netStandardAssemblies = netStandardAssembly.GetReferencedAssemblies();
            foreach (var referencedAssembly in netStandardAssemblies)
            {
                compilation = compilation.AddReferences(
                    MetadataReference.CreateFromFile(Assembly.Load(referencedAssembly).Location));
            }

            compilation = compilation.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(code));

            using var memoryStream = new MemoryStream();
            var compilationResult = compilation.Emit(memoryStream);

            if (!compilationResult.Success)
            {
                foreach (var error in compilationResult.Diagnostics)
                {
                    Console.WriteLine(error.GetMessage());
                }

                return null;
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var assemblyBytes = memoryStream.ToArray();
            var assembly = Assembly.Load(assemblyBytes);

            var type = assembly.GetType("AppViewCodeNamespace.AppViewCode");
            if (type == null)
            {
                Console.WriteLine("AppViewCode not found.");

                return null;
            }

            IView instance = Activator.CreateInstance(type) as IView;
            if (instance == null)
            {
                Console.WriteLine("AppViewCode cannot by instanciated.");

                return null;
            }

            return instance;
        }

        private string GetSharpCode(string viewContent)
        {
            var lines = viewContent.Split(new string[] { "\n\r", "\r\n", "\n"}, StringSplitOptions.None);
            var cSharpCode = new StringBuilder();
            var supportedOperators = new[] { "for", "if", "else" };
            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("{") || line.TrimStart().StartsWith("}"))
                {
                    cSharpCode.AppendLine(line);
                }
                else if (supportedOperators.Any(x => line.TrimStart().StartsWith("@" + x)))
                {
                    var index = line.IndexOf('@');
                    var currentLine = line.Remove(index, 1);

                    cSharpCode.AppendLine(currentLine);
                }
                else
                {
                    if (line.Contains("@RenderBody()"))
                    {
                        var cSharpLine = $"html.AppendLine(@\"{line.Replace("\"", "\"\"")}\");";
                        cSharpCode.AppendLine(cSharpLine);
                    }
                    else
                    {
                        int startIndex = 0;
                        var cSharpStringToAppend = "html.AppendLine(@\"";
                        var restOfLine = line;
                        while (restOfLine.Contains("@"))
                        {
                            var charIndex = restOfLine.IndexOf('@', startIndex);
                            var plainText = restOfLine.Substring(0, charIndex).Replace("\"", "\"\"");
                            var cSharpCodeRegex = new Regex(@"[^\s<""\&]+", RegexOptions.Compiled);
                            var cSharpExpression = cSharpCodeRegex.Match(restOfLine.Substring(charIndex + 1)).Value;

                            charIndex += cSharpExpression.Length + 1;

                            if (cSharpExpression.StartsWith("{") && cSharpExpression.EndsWith("}"))
                            {
                                cSharpExpression = cSharpExpression.Substring(1, cSharpExpression.LastIndexOf("}") - 1);
                            }

                            cSharpStringToAppend += plainText + "\" + " + cSharpExpression + " + @\"";

                            if (restOfLine.Length <= charIndex)
                            {
                                restOfLine = string.Empty;
                            }
                            else
                            {
                                restOfLine = restOfLine.Substring(charIndex);
                            }
                        }

                        cSharpStringToAppend += $"{restOfLine.Replace("\"", "\"\"")}\");";
                        cSharpCode.AppendLine(cSharpStringToAppend);
                    }
                }
            }

            return cSharpCode.ToString();
        }

        private string GetModelType<T>(T model)
        {
            if (model is IEnumerable)
            {
                return $"IEnumerable<{model.GetType().GetGenericArguments()[0].FullName}>";
            }

            return model.GetType().FullName;
        }
    }
}
