using System.IO;

namespace Microting.EntityFrameworkCore.TestUtilities
{
    public class BuildFileResult
    {
        public BuildFileResult(string targetPath)
        {
            TargetPath = targetPath;
            TargetDir = Path.GetDirectoryName(targetPath);
            TargetName = Path.GetFileNameWithoutExtension(targetPath);
        }

        public string TargetPath { get; }

        public string TargetDir { get; }

        public string TargetName { get; }
    }
}
