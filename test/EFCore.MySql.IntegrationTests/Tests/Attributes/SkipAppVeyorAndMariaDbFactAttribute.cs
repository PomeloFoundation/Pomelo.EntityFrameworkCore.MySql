using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests.Attributes
{
	public class SkipAppVeyorAndMariaDbFactAttribute : FactAttribute {

		public SkipAppVeyorAndMariaDbFactAttribute() {
			if(AppConfig.AppVeyor  || new ServerVersion(AppConfig.Config["Data:ServerVersion"]).Type == Infrastructure.ServerType.MariaDb) {
				Skip = "Test does not work with AppVeyor's MySQL version";
			}
		}

	}
}
