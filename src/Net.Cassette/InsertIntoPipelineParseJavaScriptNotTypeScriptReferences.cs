using System.Linq;
using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Net.Cassette
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
}