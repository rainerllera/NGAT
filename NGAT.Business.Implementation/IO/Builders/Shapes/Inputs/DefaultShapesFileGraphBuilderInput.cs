using System;
using System.IO;

namespace NGAT.Business.Implementation.IO.Shapes.Inputs
{
    /// <summary>
    /// Represents the arguments for a ShapeFileGraphBuilder.BuildGraph method
    /// </summary>
    public class DefaultShapesFileGraphBuilderInput
    {
        public DefaultShapesFileGraphBuilderInput(string filePath, string fromPointColumn, string toPointColumn)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException("The file specified doesn't exists or is invalid", "filePath");
            FilePath = filePath;

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Source point column name cannot be null");
            FromPointColumn = fromPointColumn;

            if(string.IsNullOrWhiteSpace(toPointColumn))
                throw new ArgumentException("Source point column name cannot be null");
            ToPointColumn = toPointColumn;


        }
        /// <summary>
        /// The Path to .shp file
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// The Origin Column Name in .shp file records
        /// </summary>
        public string FromPointColumn { get; protected set; }

        /// <summary>
        /// The Destination Column Name in .shp file records
        /// </summary>
        public string ToPointColumn { get; protected set; }
    }
}
