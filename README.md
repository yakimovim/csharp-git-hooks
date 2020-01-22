# Example of implementation of Git hooks using C#

The folder `Hooks` contains C# scripts that implements Git hooks. To use them you must remove `.csx` extension and place them into `.git\hooks` folder of your project. The `Hooks` folder contains only implementation of some Git hooks. Scripts for other Git hooks can be implemented the same way. The difference is minor.

The folder `gitHookAssemblies` contains assemblies that are used by current implementation of it hook handlers.

In the `src` folder you can find Visual Studio solution containing:

* Interfaces for Git hooks (`GitHooksInterfaces` project).
* Collector of handlers for Git hooks (`GitHooksCollector` project).
* Implementation of handlers for GitHooks (`GitHooks` project).
* Simple ASP.NET Core Web Service providing some information about owners of list items (`GitHooks` project).

