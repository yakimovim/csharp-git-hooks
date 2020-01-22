using LibGit2Sharp;
using System;
using System.Security.Principal;

namespace GitHooks
{
    public class RunHooks
    {
        public static void RunPreCommitHook()
        {
            using var repo = new Repository(Environment.CurrentDirectory);

            Console.WriteLine(repo.Info.WorkingDirectory);
        }
    }
}
