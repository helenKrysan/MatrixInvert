using Parcs;
using System.Linq;
using System.Threading;

namespace InverseMatrix
{
    internal class InverseMatrix : IModule
    {
        public void Run(ModuleInfo info, CancellationToken token = default)
        {
            while (true)
            {
                Matrix m = (Matrix)info.Parent.ReadObject(typeof(Matrix));
                if (m.Width > 1)
                {
                    var splitedMatrix = SplitMatrixHelper.SplitMatrix(m).ToArray();
                    info.Parent.WriteObject(splitedMatrix);
                }
                else
                {
                    Matrix[] res = new Matrix[] { new Matrix(1, 1, false) };
                    if (m[0, 0] != 0)
                    {
                        res[0][0, 0] = 1.0 / m[0, 0];
                    }
                    else
                    {
                        res[0][0, 0] = 0;
                    }

                    info.Parent.WriteObject(res);
                }
            }
        }
    }
}