using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using CSG2STL;
using System.Diagnostics;

Console.WriteLine("CSG2STL V0.1");

if (args.Length == 0)
{
	Console.Error.WriteLine("Usage: CSG2STL <script.csx>");
	return 1;
}

string scriptPath = args[0];
if (!File.Exists(scriptPath))
{
	Console.Error.WriteLine($"Script file not found: {scriptPath}");
	return 1;
}

string code = File.ReadAllText(scriptPath);

ScriptOptions options = ScriptOptions.Default
	.AddImports("System", "CSG2STL")
	.AddReferences(Assembly.GetExecutingAssembly());

Stopwatch stopwatch = Stopwatch.StartNew();
try
{
	await CSharpScript.RunAsync(code, options);
	stopwatch.Stop();
}
catch (CompilationErrorException ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.Error.WriteLine("Script compilation errors:");
	foreach (var diag in ex.Diagnostics)
		Console.Error.WriteLine($"  {diag}");
	Console.ForegroundColor = ConsoleColor.White;
	Console.WriteLine();
	Console.WriteLine("Press any key to continue.");
	while(!Console.KeyAvailable) { }
	return 1;
}
catch (Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.Error.WriteLine($"Script error: {ex.Message}");
	Console.ForegroundColor = ConsoleColor.White;
	Console.WriteLine();
	Console.WriteLine("Press any key to continue.");
	while(!Console.KeyAvailable) { }
	return 1;
}

Console.WriteLine($"Finished in {stopwatch.ElapsedMilliseconds}ms.");
Console.WriteLine("Press any key to continue.");
while(!Console.KeyAvailable) { }

return 0;
