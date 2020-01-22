using GitHooksInterfaces;
using LibGit2Sharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Security.Principal;
using Newtonsoft.Json;

namespace GitHooks
{
    [Export(typeof(IPreCommitHook))]
    public class SqlStepsHook : IPreCommitHook
    {
        private static readonly Regex _expr = new Regex("\\bver(\\d+)\\b");

        public bool Process(IList<string> args)
        {
            using var repo = new Repository(Environment.CurrentDirectory);

            var items = repo.RetrieveStatus()
                .Where(i => !i.State.HasFlag(FileStatus.Ignored))
                .Where(i => i.State.HasFlag(FileStatus.NewInIndex))
                .Where(i => i.FilePath.StartsWith(@"sql"));

            var versions = new HashSet<int>(
                items
                .Select(i => _expr.Match(i.FilePath))
                .Where(m => m.Success)
                .Select(m => m.Groups[1].Value)
                .Select(d => int.Parse(d))
                );

            foreach(var version in versions)
            {
                if (!ListItemOwnerChecker.DoesCurrentUserOwnListItem(1, version))
                    return false;
            }

            return true;
        }
    }
}
