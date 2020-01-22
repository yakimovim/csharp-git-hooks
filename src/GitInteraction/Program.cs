using ColorConsole;
using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GitInteraction
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new ConsoleWriter();

            console.WriteLine("Interaction with Git...", ConsoleColor.Black, ConsoleColor.White);

            var repoPath = @"c:\Users\IvanY\source\repos\GitHooksOnCSharp\";

            using var repo = new Repository(repoPath);

            foreach (var item in repo.RetrieveStatus().Where(i => !i.State.HasFlag(FileStatus.Ignored)))
            {
                console.WriteLine(item.FilePath, ConsoleColor.DarkBlue, ConsoleColor.White);

                using (var content = new StreamReader(repo.Info.WorkingDirectory + Path.DirectorySeparatorChar + item.FilePath, Encoding.UTF8))
                {
                    Console.WriteLine("\n\n~~~~ Current file ~~~~");
                    Console.WriteLine(content.ReadToEnd());
                }

                var blob = repo.Head.Tip[item.FilePath]?.Target as Blob;
                if (blob != null)
                {
                    using (var content = new StreamReader(blob.GetContentStream(), Encoding.UTF8))
                    {
                        Console.WriteLine("\n\n~~~~ Original file ~~~~");
                        Console.WriteLine(content.ReadToEnd());
                    }
                }

                if (item.State.HasFlag(FileStatus.ModifiedInIndex))
                {
                    var id = repo.Index[item.FilePath].Id;

                    var itemBlob = repo.Lookup<Blob>(id);

                    using (var content = new StreamReader(itemBlob.GetContentStream(), Encoding.UTF8))
                    {
                        Console.WriteLine("\n\n~~~~ Staged file ~~~~");
                        Console.WriteLine(content.ReadToEnd());
                    }
                }
            }
        }
    }
}
