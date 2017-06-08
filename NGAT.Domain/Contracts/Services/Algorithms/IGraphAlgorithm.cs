using NGAT.Business.Contracts.Common;

namespace NGAT.Business.Contracts.Services.Algorithms
{
    /// <summary>
    /// Represents an algorithm that receives a Graph as an input argument
    /// </summary>
    /// <typeparam name="TInput">The type of the Input paramateres (A wrapper object for params)</typeparam>
    /// <typeparam name="TOutput">The type of this algorithm response (A wrapper for output)</typeparam>
    public interface IGraphAlgorithm<TInput, TOutput> : IAlgorithm<TInput,TOutput> where TInput:IGraphContainer
    {
    }
}
