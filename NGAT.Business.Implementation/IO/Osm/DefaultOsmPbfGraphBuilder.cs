using System;
using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Domain.Core;
using NGAT.Business.Implementation.IO.Osm.Inputs;
using OsmSharp.IO.PBF;

namespace NGAT.Business.Implementation.IO.Osm
{
    public class DefaultOsmPbfGraphBuilder : IGraphBuilder<DefaultOsmPbfGraphBuilderInput>
    {
        public Graph Build(DefaultOsmPbfGraphBuilderInput input)
        {
            return InternalBuild(input);
        }

        public Task<Graph> BuildAsync(DefaultOsmPbfGraphBuilderInput input)
        {
            return new Task<Graph>(() => InternalBuild(input));
        }

        private Graph InternalBuild(DefaultOsmPbfGraphBuilderInput input)
        {

        }
    }
}
