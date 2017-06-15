using System;
using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Contracts.IO.Filters;
using NGAT.Business.Domain.Core;
using NGAT.Business.Implementation.IO.Shapes.Inputs;

namespace NGAT.Business.Implementation.IO.Shapes
{
    public class DefaultShapeFilesGraphBuilder : IGraphBuilder
    {
        public Uri DigitalMapURI { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAttributeFilterCollection LinkFilters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAttributesFetcherCollection NodeAttributesFetchers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAttributesFetcherCollection LinkAttributesFetchers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string DigitalMapFormatID => throw new NotImplementedException();

        public Graph Build()
        {
            throw new NotImplementedException();
        }

        public Task<Graph> BuildAsync()
        {
            throw new NotImplementedException();
        }
    }
}
