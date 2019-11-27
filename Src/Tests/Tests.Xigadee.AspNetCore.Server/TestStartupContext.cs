using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xigadee;
namespace Tests.Xigadee
{
    /// <summary>
    /// This class holds the application settings.
    /// </summary>
    /// <seealso cref="Xigadee.ApiMicroserviceStartUpContext" />
    public class TestStartupContext : JwtApiMicroserviceStartUpContext
    {
        [RegisterAsSingleton(typeof((string, string)))]
        public virtual (string, string) GetSomething() => ("hello", DateTime.UtcNow.ToString("o"));

        protected override IApiUserSecurityModule UserSecurityModuleCreate()
        {
            var usm = new UserSecurityModule<TestUser>()
                .SetAsMemoryBased();

            //Add test security accounts here.
            var user = new TestUser() { Username = "paul" };
            var rs = usm.Users.Create(user).Result;

            var rsv = usm.Users.ReadByRef(TestUser.KeyUsername, "paul").Result;

            var uSec = new UserSecurity() { Id = user.Id };
            uSec.AuthenticationSet("", "123Enter.");
            var rs2 = usm.UserSecurities.Create(uSec).Result;

            var ur = new UserRoles() { Id = user.Id };
            ur.RoleAdd("paul");
            var rs3 = usm.UserRoles.Create(ur).Result;

            //uSec.

            return usm;
        }

        public override void ModulesCreate(IServiceCollection services)
        {
            base.ModulesCreate(services);

            ModuleMondayMorningBlues = new MondayMorningBluesModule();
        }

        public override void Connect(ILoggerFactory lf)
        {
            base.Connect(lf);

            ModuleMondayMorningBlues.Logger = lf.CreateLogger<MondayMorningBluesModule>();
        }

        [ModuleStartStop]
        [RepositoriesProcess]
        [RegisterAsSingleton]
        public MondayMorningBluesModule ModuleMondayMorningBlues { get; set; }
    }
}
