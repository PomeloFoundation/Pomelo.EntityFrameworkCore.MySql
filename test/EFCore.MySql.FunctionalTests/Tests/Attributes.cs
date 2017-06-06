using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests
{
	public class SkipAppVeyorFact : FactAttribute {

		public SkipAppVeyorFact() {
			if(AppConfig.AppVeyor) {
				Skip = "Test does not work with AppVeyor's MySQL version";
			}
		}

	}
}
