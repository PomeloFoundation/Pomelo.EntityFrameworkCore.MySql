using System.Collections.Generic;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit;

public interface IMySqlTestClassOrderer
{
    IEnumerable<ITestClass> OrderTestClasses(IEnumerable<ITestClass> testClasses);
}
