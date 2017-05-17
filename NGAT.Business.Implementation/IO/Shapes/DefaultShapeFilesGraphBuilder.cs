using System;
using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Domain.Core;
using NGAT.Business.Implementation.IO.Shapes.Inputs;

namespace NGAT.Business.Implementation.IO.Shapes
{
    public class DefaultShapeFilesGraphBuilder : IGraphBuilder<DefaultShapesFileGraphBuilderInput>
    {
        public Graph Build(DefaultShapesFileGraphBuilderInput input)
        {
            throw new NotImplementedException();
        }

        public Task<Graph> BuildAsync(DefaultShapesFileGraphBuilderInput input)
        {
            throw new NotImplementedException();
        }
    }
}
