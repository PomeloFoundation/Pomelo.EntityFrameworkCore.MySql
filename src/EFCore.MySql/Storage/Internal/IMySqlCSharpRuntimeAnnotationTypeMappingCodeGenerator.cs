// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Design.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public interface IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator
{
    void Create(
        CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
        CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies);
}
