using GitHooksInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace GitHooks
{
    [Export(typeof(IPreCommitHook))]
    public class MessageHook : IPreCommitHook
    {
        public bool Process(IList<string> args)
        {
            Console.WriteLine("Message hook...");

            if(args != null)
            {
                Console.WriteLine("Arguments are:");
                foreach(var arg in args)
                {
                    Console.WriteLine(arg);
                }
            }

            return true;
        }
    }
}
