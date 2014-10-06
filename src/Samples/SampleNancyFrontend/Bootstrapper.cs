using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Net.Nancy;
using Net.Text;
using SampleNancyFrontend.App_Start;
using Serilog;

namespace SampleNancyFrontend
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        public string RootPath { get; set; }

        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                if (RootPath.HasValue())
                    return new StaticRootPathProvider(RootPath);

                return new DefaultRootPathProvider();
            }
        }


        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            Log.Debug("Starting Nancy...");
            base.ApplicationStartup(container, pipelines);
            Log.Debug("Nancy started.", new object[0]);
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return AutofacConfig.Initialize(GetType());
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts", @"scripts"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts", @"fonts"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("~/js"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("~/css"));
        }


    }
}