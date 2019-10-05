﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pomelo.EntityFrameworkCore.TestUtilities
{
    public class BuildSource
    {
        public ICollection<BuildReference> References { get; } = new List<BuildReference>
        {
#if NET461
            BuildReference.ByName("mscorlib"),
            BuildReference.ByName("netstandard", true),
            BuildReference.ByName("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
            BuildReference.ByName("System.Collections", true),
            BuildReference.ByName("System.Collections.Concurrent", true),
            BuildReference.ByName("System.ComponentModel", true),
            BuildReference.ByName("System.ComponentModel.Annotations", true),
            BuildReference.ByName("System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
            BuildReference.ByName("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
            BuildReference.ByName("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
            BuildReference.ByName("System.Data.Common", true),
            BuildReference.ByName("System.Globalization", true),
            BuildReference.ByName("System.Linq", true),
            BuildReference.ByName("System.Reflection", true),
            BuildReference.ByName("System.Reflection.Extensions", true),
            BuildReference.ByName("System.Runtime", true),
            BuildReference.ByName("System.Runtime.Extensions", true),
            BuildReference.ByName("System.Threading", true),
            BuildReference.ByName("System.Threading.Tasks", true),
            BuildReference.ByName("System.ValueTuple", true)
#else
            BuildReference.ByName("netstandard"),
            BuildReference.ByName("System.Collections"),
            BuildReference.ByName("System.ComponentModel.Annotations"),
            BuildReference.ByName("System.Data.Common"),
            BuildReference.ByName("System.Linq.Expressions"),
            BuildReference.ByName("System.Runtime"),
            BuildReference.ByName("System.Text.RegularExpressions")
#endif
        };

        public string TargetDir { get; set; }
        public ICollection<string> Sources { get; set; } = new List<string>();

        public BuildFileResult Build()
        {
            var projectName = Path.GetRandomFileName();
            var references = new List<MetadataReference>();

            foreach (var reference in References)
            {
                if (reference.CopyLocal)
                {
                    if (string.IsNullOrEmpty(reference.Path))
                    {
                        throw new InvalidOperationException("Could not find path for reference " + reference);
                    }
                    File.Copy(reference.Path, Path.Combine(TargetDir, Path.GetFileName(reference.Path)), overwrite: true);
                }

                references.AddRange(reference.References);
            }

            var compilation = CSharpCompilation.Create(
                projectName,
                Sources.Select(s => SyntaxFactory.ParseSyntaxTree(s)),
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var targetPath = Path.Combine(TargetDir ?? Path.GetTempPath(), projectName + ".dll");

            using (var stream = File.OpenWrite(targetPath))
            {
                var result = compilation.Emit(stream);
                if (!result.Success)
                {
                    throw new InvalidOperationException(
                        $"Build failed. Diagnostics: {string.Join(Environment.NewLine, result.Diagnostics)}");
                }
            }

            return new BuildFileResult(targetPath);
        }

        public Assembly BuildInMemory()
        {
            var projectName = Path.GetRandomFileName();
            var references = new List<MetadataReference>();

            foreach (var reference in References)
            {
                references.AddRange(reference.References);
            }

            var compilation = CSharpCompilation.Create(
                projectName,
                Sources.Select(s => SyntaxFactory.ParseSyntaxTree(s)),
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Assembly assembly;
            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);
                if (!result.Success)
                {
                    throw new InvalidOperationException(
                        $"Build failed. Diagnostics: {string.Join(Environment.NewLine, result.Diagnostics)}");
                }

                assembly = Assembly.Load(stream.ToArray());
            }

            return assembly;
        }
    }
}
