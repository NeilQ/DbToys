using Netcool.DbToys.Core.Database;
using Netcool.DbToys.Core.Scriban;
using Scriban;
using Scriban.Functions;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Netcool.DbToys.Core;

public class CodeGenerateResult
{
    public string Filename;

    public string Codes;

    public CodeGenerateResult(string filename, string codes)
    {
        Filename = filename;
        Codes = codes;
    }
}

public interface ICodeGenerator
{
    public CodeGenerateResult GenerateFromTable(Table table, string templateText);

}

public class CodeGenerator : ICodeGenerator
{
    public CodeGenerateResult GenerateFromTable(Table table, string templateText)
    {
        var scriptVisitor = new CodeGeneratorScriptVisitor();
        var scriptObject = new ScriptObject { { "table", table } };
        //scriptObject.Import(new CustomScribanStringFunctions());
        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        //context.BuiltinObject["string"] = new CustomScribanStringFunctions();
        var thing = (ScriptObject)context.BuiltinObject["string"];
        thing.Import(new CustomScribanStringFunctions());

        var template = Template.Parse(templateText);
        template.Page.Accept(scriptVisitor);
        var output = template.Render(context);

        string filename = null;
        if (scriptVisitor.Variables.TryGetValue("filename", out var variable))
        {
            filename = (string)variable.GetValue(context);
        }

        return new CodeGenerateResult(filename, output);
    }
}

public class CodeGeneratorScriptVisitor : ScriptVisitor
{
    public Dictionary<string, ScriptVariable> Variables { get; } = new();

    public override void Visit(ScriptVariableGlobal node)
    {
        Variables.TryAdd(node.Name, node);
        base.Visit(node);
    }

    public override void Visit(ScriptVariableLocal node)
    {
        Variables.TryAdd(node.Name, node);
        base.Visit(node);
    }

}