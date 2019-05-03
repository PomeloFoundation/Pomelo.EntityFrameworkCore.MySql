using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests
{
	public class SkipAppVeyorAndMariaDbFact : FactAttribute {

		public SkipAppVeyorAndMariaDbFact() {
			if(AppConfig.AppVeyor  || new ServerVersion(AppConfig.Config["Data:ServerVersion"]).Type == Infrastructure.ServerType.MariaDb) {
				Skip = "Test does not work with AppVeyor's MySQL version";
			}
		}

	}
}
