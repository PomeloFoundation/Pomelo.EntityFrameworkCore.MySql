using System;
using System.Collections.Generic;
using System.Linq;
#if NET461
using System.Reflection;
#endif
using IOPath = System.IO.Path;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;

namespace Pomelo.EntityFrameworkCore.TestUtilities
{
    public class BuildReference
    {
        private BuildReference(IEnumerable<MetadataReference> references, bool copyLocal = false, string path = null)
        {
            References = references;
            CopyLocal = copyLocal;
            Path = path;
        }

        public IEnumerable<MetadataReference> References { get; }

        public bool CopyLocal { get; }
        public string Path { get; }

        public static BuildReference ByName(string name, bool copyLocal = false)
        {
#if NET461
            var assembly = Assembly.Load(name);
            return new BuildReference(
                new[] { MetadataReference.CreateFromFile(assembly.Location) },
                copyLocal,
                new Uri(assembly.CodeBase).LocalPath);
#elif NETCOREAPP2_0 || NETCOREAPP2_1
            var references = (from l in DependencyContext.Default.CompileLibraries
                from r in l.ResolveReferencePaths()
                where IOPath.GetFileNameWithoutExtension(r) == name
                select MetadataReference.CreateFromFile(r)).ToList();
            if (references.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Assembly '{name}' not found.");
            }

            return new BuildReference(
                references,
                copyLocal);
#else
#error target frameworks need to be updated.
#endif
        }

        public static BuildReference ByPath(string path)
            => new BuildReference(new[] { MetadataReference.CreateFromFile(path) }, path: path);
    }
}
