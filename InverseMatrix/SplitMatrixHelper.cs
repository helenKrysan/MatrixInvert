using System;
using System.Collections.Generic;

namespace InverseMatrix
{
    internal static class SplitMatrixHelper
    {
        public static IEnumerable<Matrix> SplitMatrix(Matrix a)
        {
            yield return a.SubMatrix(0, 0, a.Height / 2, a.Width / 2);
            yield return a.SubMatrix(0, a.Width / 2, a.Height / 2, a.Width / 2);
            yield return a.SubMatrix(a.Height / 2, 0, a.Height / 2, a.Width / 2);
            yield return a.SubMatrix(a.Height / 2, a.Width / 2, a.Height / 2, a.Width / 2);
        }

        public static Matrix JoinMatrix(Matrix resMatrix, IList<Matrix> matrixes)
        {
            resMatrix.FillSubMatrix(matrixes[0], 0, 0);
            resMatrix.FillSubMatrix(matrixes[1], 0, resMatrix.Width / 2);
            resMatrix.FillSubMatrix(matrixes[2], resMatrix.Height / 2, 0);
            resMatrix.FillSubMatrix(matrixes[3], resMatrix.Height / 2, resMatrix.Width / 2);
            return resMatrix;
        }
    }
}