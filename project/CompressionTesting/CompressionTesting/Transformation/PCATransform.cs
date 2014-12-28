using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;
using MathNet.Numerics.LinearAlgebra;

namespace CompressionTesting.Transformation
{
    class PCATransform
    {
        public static void Forward(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                Matrix<float> m = BuildMatrix(l,pointOffset);
                PCA pca = new PCA(m, true);
                Matrix<float> t = pca.transform(m, PCA.TransformationType.ROTATION);
                CopyToLine(l, t, pointOffset);
                l.pcaTransform = pca.v.Inverse();
                l.means = pca.means;
            }
        }

        public static void Backward(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                Matrix<float> m = BuildMatrix(l, pointOffset);
                PCA pca = new PCA(l.pcaTransform,l.means);
                Matrix<float> t = pca.inverseTransform(m, PCA.TransformationType.ROTATION);
                CopyToLine(l, t, pointOffset);
            }
        }

        private static Matrix<float> BuildMatrix(PFSSLine l, int offset)
        {
            Matrix<float> answer = Matrix<float>.Build.Dense(l.points.Count-offset, 3);
            for (int i = offset; i < l.points.Count; i++)
            {
                answer[i-offset, 0] = l.points[i].x;
                answer[i - offset, 1] = l.points[i].y;
                answer[i - offset, 2] = l.points[i].z;
            }
            return answer;
        }

        private static void CopyToLine(PFSSLine l, Matrix<float> matrix, int offset)
        {
            for (int i = offset; i < l.points.Count; i++)
            {
                l.points[i].x = matrix[i - offset, 0];
                l.points[i].y = matrix[i - offset, 1];
                l.points[i].z = matrix[i - offset, 2];
            }
        }
    }
}
