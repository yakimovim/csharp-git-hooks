using GitHooksInterfaces;
using LibGit2Sharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GitHooks
{
    [Export(typeof(IPreCommitHook))]
    public class ConstantValuesHook : IPreCommitHook
    {
        public bool Process(IList<string> args)
        {
            using var repo = new Repository(Environment.CurrentDirectory);

            var constantsItem = repo.RetrieveStatus()
                .Staged
                .FirstOrDefault(i => i.FilePath == @"src/GitInteraction/Constants.cs");

            if (constantsItem == null)
                return true;

            if (!constantsItem.State.HasFlag(FileStatus.NewInIndex)
                && !constantsItem.State.HasFlag(FileStatus.ModifiedInIndex))
                return true;

            var initialContent = GetInitialContent(repo, constantsItem);
            var indexContent = GetIndexContent(repo, constantsItem);

            var initialConstantValues = GetConstantValues(initialContent);
            var indexConstantValues = GetConstantValues(indexContent);

            indexConstantValues.ExceptWith(initialConstantValues);

            if (indexConstantValues.Count == 0)
                return true;

            foreach (var version in indexConstantValues)
            {
                if (!ListItemOwnerChecker.DoesCurrentUserOwnListItem(2, version))
                    return false;
            }

            return true;
        }

        private string GetInitialContent(Repository repo, StatusEntry item)
        {
            var blob = repo.Head.Tip[item.FilePath]?.Target as Blob;

            if (blob == null)
                return null;

            using var content = new StreamReader(blob.GetContentStream(), Encoding.UTF8);

            return content.ReadToEnd();
        }

        private string GetIndexContent(Repository repo, StatusEntry item)
        {
            var id = repo.Index[item.FilePath]?.Id;
            if (id == null)
                return null;

            var itemBlob = repo.Lookup<Blob>(id);
            if (itemBlob == null)
                return null;

            using var content = new StreamReader(itemBlob.GetContentStream(), Encoding.UTF8);

            return content.ReadToEnd();
        }

        private ISet<int> GetConstantValues(string fileContent)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
                return new HashSet<int>();

            var tree = CSharpSyntaxTree.ParseText(fileContent);

            var root = tree.GetCompilationUnitRoot();

            var enumDeclaration = root.DescendantNodes().OfType<EnumDeclarationSyntax>()
                .FirstOrDefault(e => e.Identifier.Text == "Constants");

            if(enumDeclaration == null)
                return new HashSet<int>();

            var result = new HashSet<int>();

            foreach (var member in enumDeclaration.Members)
            {
                if(int.TryParse(member.EqualsValue.Value.ToString(), out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }
}
