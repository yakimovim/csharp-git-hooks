using System.Collections.Generic;

namespace GitHooksInterfaces
{
    public interface IPreCommitHook
    {
        bool Process(IList<string> args);
    }
}
