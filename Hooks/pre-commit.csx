#!/usr/bin/env dotnet-script

#r "nuget: System.Runtime.Loader, 4.3.0"

using System.IO;
using System.Runtime.Loader;

var hooksDirectory = Path.Combine(Environment.CurrentDirectory, "gitHookAssemblies");

var assemblyPath = Path.Combine(hooksDirectory, "GitHooksCollector.dll");

AssemblyLoadContext.Default.Resolving += (context, assemblyName) => {
    var assemblyPath = Path.Combine(hooksDirectory, $"{assemblyName.Name}.dll");
    if(File.Exists(assemblyPath))
    {
        return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
    }
    return null;
};

var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
if(assembly == null)
{
    Console.WriteLine($"Can't load assembly from '{assemblyPath}'.");
}

var collectorsType = assembly.GetType("GitHooksCollector.Collectors");
if(collectorsType == null)
{
    Console.WriteLine("Can't find collector's type.");
}

var method = collectorsType.GetMethod("RunPreCommitHooks", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
if(method == null)
{
    Console.WriteLine("Can't find collector's method for pre-commit hooks.");
}

int exitCode = (int) method.Invoke(null, new object[] { Args, hooksDirectory });

Environment.Exit(exitCode);