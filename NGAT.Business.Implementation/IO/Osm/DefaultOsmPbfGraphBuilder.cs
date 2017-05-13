using System;
using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Domain.Core;
using NGAT.Business.Implementation.IO.Osm.Inputs;

namespace NGAT.Business.Implementation.IO.Osm
{
    public class DefaultOsmPbfGraphBuilder : IGraphBuilder<DefaultOsmPbfGraphBuilderInput>
    {
        public Graph Build(DefaultOsmPbfGraphBuilderInput input)
        {
            throw new NotImplementedException();
        }

        public Task<Graph> BuildAsync(DefaultOsmPbfGraphBuilderInput input)
        {
            throw new NotImplementedException();
        }
    }
}
