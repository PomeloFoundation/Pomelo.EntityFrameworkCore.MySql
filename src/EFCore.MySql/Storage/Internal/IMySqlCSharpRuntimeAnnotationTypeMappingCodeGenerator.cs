// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Design.Internal;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal;

public interface IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator
{
    void Create(
        CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
        CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies);
}
