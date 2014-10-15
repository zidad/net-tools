// MIT Licensed from http://github.com/zidad/net-tools
using System.Linq;
using Cassette;
using Cassette.BundleProcessing;
using Cassette.Scripts;
using Cassette.Stylesheets;
using Net.Cassette;

namespace SampleMessages
{

    public class InsertIntoPipelineParseJavaScriptNotTypeScriptReferences : IBundlePipelineModifier<ScriptBundle>
    {
        public IBundlePipeline<ScriptBundle> Modify(IBundlePipeline<ScriptBundle> pipeline)
        {
            var index = pipeline.IndexOf(pipeline.OfType<ParseJavaScriptReferences>().Single());
            pipeline.RemoveAt(index);
            pipeline.Insert<ParseJavaScriptNotTypeScriptReferences>(index);
            return pipeline;
        }
    }

    public class BundleConfig : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            
            bundles.AddPerSubDirectory<StylesheetBundle>("~/content", new FileSearch { Pattern = "*.css" });

            //bundles.AddPerSubDirectory<ScriptBundle>("scripts", new FileSearch { Pattern = "*.js" } );
            bundles.Add<ScriptBundle>("~/scripts/app", new RecursiveFileSearch(new FileSearch { Pattern = "*.js" }));
            bundles.Add<ScriptBundle>("scripts", new AggregateFileSearch(
                new IFileSearch[] { 
                    new FileSearch { Pattern = "jquery-*.js" }, 
                    new FileSearch { Pattern = "angular.js" }, 
                    new FileSearch { Pattern = "angular-*.js" },
                }));
        }
    }
}