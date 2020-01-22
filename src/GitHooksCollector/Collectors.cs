using GitHooksInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GitHooksCollector
{
    public class Collectors
    {
        private class PreCommitHooks
        {
            [ImportMany(typeof(IPreCommitHook))]
            public IPreCommitHook[] Hooks { get; set; }
        }

        public static int RunPreCommitHooks(IList<string> args, string directory)
        {
            Console.WriteLine("Collecting hooks from directory...");

            //var assembly = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "GitHooks.dll"));

            //var catalog = new AssemblyCatalog(assembly);
            var catalog = new DirectoryCatalog(directory, "*Hooks.dll");
            var container = new CompositionContainer(catalog);
            var obj = new PreCommitHooks();
            container.ComposeParts(obj);

            bool success = true;

            foreach(var hook in obj.Hooks)
            {
                success &= hook.Process(args);
            }

            return success ? 0 : 1;
        }
    }
}
