using System.Collections.Generic;

namespace McpSmyrilLine.Model
{
    public interface IBugsRepository
    {
        IEnumerable<Bug> GetBugs();
    }
}