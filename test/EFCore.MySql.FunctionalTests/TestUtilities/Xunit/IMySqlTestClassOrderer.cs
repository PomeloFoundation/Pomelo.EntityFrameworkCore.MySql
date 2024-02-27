using System.Collections.Generic;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit;

public interface IMySqlTestClassOrderer
{
    IEnumerable<ITestClass> OrderTestClasses(IEnumerable<ITestClass> testClasses);
}
