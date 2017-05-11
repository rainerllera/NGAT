using System.Threading.Tasks;

namespace NGAT.Business.Algorithms
{
    /// <summary>
    /// Represents an algorithm with input and output
    /// </summary>
    /// <typeparam name="TInput">The type of the Input paramateres (A wrapper object for params)</typeparam>
    /// <typeparam name="TOutput">The type of this algorithm response (A wrapper for output)</typeparam>
    public interface IAlgorithm<TInput, TOutput>
    {
        /// <summary>
        /// Executes the algorithm.
        /// </summary>
        /// <param name="input">The input for this algorithm</param>
        /// <returns>The output of this algorithm.</returns>
        TOutput Run(TInput input);

        /// <summary>
        /// Executes the algorithm asynchronously
        /// </summary>
        /// <param name="input">The input for this algorithm</param>
        /// <returns>The task that executes this algorithm</returns>
        Task<TOutput> RunAsync(TInput input);
    }

}
