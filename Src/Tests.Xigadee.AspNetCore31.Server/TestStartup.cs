using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Test.Xigadee;
using Xigadee;
namespace Tests.Xigadee
{
    /// <summary>
    /// This is the base application class.
    /// </summary>
    /// <seealso cref="Xigadee.ApiMicroserviceStartupBase{Tests.Xigadee.TestStartupContext}" />
    //public class TestStartup : JwtApiMicroserviceStartupBase<TestStartupContext>
    //{
    //    #region Constructor
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="TestStartup"/> class.
    //    /// </summary>
    //    /// <param name="env">The environment.</param>
    //    public TestStartup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env) : base(env)
    //    {
    //    } 
    //    #endregion

    //    /// <summary>
    //    /// This method configures the repositories.
    //    /// </summary>
    //    protected override void MicroserviceConfigure()
    //    {
    //        var eOpts = Context.Directives.RepositoryProcessExtract();

    //        Pipeline.AdjustPolicyTaskManagerForDebug();

    //        var channelIncoming = Pipeline.AddChannelIncoming("testin");

    //        eOpts.ForEach(r => channelIncoming.AttachPersistenceRepositoryDirective(r, RepositoryResolve));

    //        Pipeline.Service.Events.StartCompleted += Service_StartCompleted; 
    //    }

    //    private RepositoryBase RepositoryResolve(RepositoryDirective directive)
    //    {
    //        if (directive.RepositoryType == typeof(IRepositoryAsync<Guid, MondayMorningBlues>))
    //        {
    //            var repo = new RepositoryMemory<Guid, MondayMorningBlues>();
    //            repo.SearchAdd(new RepositoryMemorySearch<Guid, MondayMorningBlues>("default"), true);
    //            return repo;
    //        }

    //        return directive.RepositoryCreateMemory();
    //    }

    //    protected override void ConfigureLoggingSubscribers(IApplicationBuilder app, ApplicationLoggerProvider provider)
    //    {
    //        base.ConfigureLoggingSubscribers(app, provider);
    //        provider.Publisher.Subscribe(new TraceLoggingSubscriber());
    //    }

    //    private void ProcessRepository(RepositoryDirective rd, IPipelineChannelIncoming<MicroservicePipeline> channelIncoming)
    //    {
    //        channelIncoming
    //            .AttachPersistenceManagerHandlerMemory(
    //                (MondayMorningBlues e) => e.Id
    //                , s => new Guid(s)
    //                , versionPolicy: ((e) => $"{e.VersionId:N}", (e) => e.VersionId = Guid.NewGuid(), true)
    //                , propertiesMaker: (e) => e.ToReferences2()
    //                , searches: new[] { new RepositoryMemorySearch<Guid, MondayMorningBlues>("default") }
    //                , searchIdDefault: "default");
    //    }

    //    private RepositoryBase ResolveRepository(Type repoType)
    //    {
    //        return null;
    //    }

    //    private void Service_StartCompleted(object sender, StartEventArgs e)
    //    {
    //        //var rs = _mmbClient.Create(new MondayMorningBlues() { Id = new Guid("9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC") }).Result;
    //    }

    //    /// <summary>
    //    /// Adds the MondayMorningBlues test repo to the collection..
    //    /// </summary>
    //    /// <param name="services">The services.</param>
    //    protected override void ConfigureSingletons(IServiceCollection services)
    //    {
    //        base.ConfigureSingletons(services);
    //        //services.AddSingleton<IRepositoryAsync<Guid, MondayMorningBlues>>(_mmbClient);
    //    }

    //    /// <summary>
    //    /// Configures the authorization policy
    //    /// </summary>
    //    /// <param name="services">The services.</param>
    //    protected override void ConfigureSecurityAuthorization(IServiceCollection services)
    //    {
    //        var policy = new AuthorizationPolicyBuilder()
    //            //.AddAuthenticationSchemes("Cookie, Bearer")
    //            .RequireAuthenticatedUser()
    //            .RequireRole("paul")
    //            //.RequireAssertion(ctx =>
    //            //{
    //            //    return ctx.User.HasClaim("editor", "contents") ||
    //            //            ctx.User.HasClaim("level", "senior");
    //            //}
    //            //)
    //            .Build();

    //        services.AddAuthorization(options =>
    //        {
    //            options.AddPolicy("adminp", policy);
    //                //policy =>
    //                //{
    //                //    policy.RequireAuthenticatedUser();
    //                //    policy.RequireRole("admin");
    //                //});

    //        })
    //        ;
    //    }

    //}



}
