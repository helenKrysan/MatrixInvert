using Parcs.Module.CommandLine;

namespace InverseMatrix
{
    using CommandLine;

    public class CommandLineOptions : BaseModuleOptions
    {
        [Option("f", Required = true, HelpText = "File path to the matrix.")]
        public string File { get; set; }
        [Option("p", Required = true, HelpText = "Number of points.")]
        public int PointsNum { get; set; }
        [Option("n", Required = false, HelpText = "Size of matrix")]
        public int N { get; set; }
    }
}
