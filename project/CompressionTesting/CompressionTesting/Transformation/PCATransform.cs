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
                Matrix<float> m = BuildMatrix(l);
                PCA pca = new PCA(m, true);
                Matrix<float> t = pca.transform(m, PCA.TransformationType.ROTATION);
                CopyToLine(l, t);
                l.pcaTransform = pca.zerosRotationTransformation;
                l.means = pca.means;
            }
        }

        public static void Backwards(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                Matrix<float> m = BuildMatrix(l);
                PCA pca = new PCA(l.pcaTransform,l.means);
                Matrix<float> t = pca.inverseTransform(m, PCA.TransformationType.ROTATION);
                CopyToLine(l, t);
            }
        }

        private static Matrix<float> BuildMatrix(PFSSLine l)
        {
            Matrix<float> answer = Matrix<float>.Build.Dense(l.points.Count, 3);
            for (int i = 0; i < l.points.Count; i++)
            {
                answer[i, 0] = l.points[i].x;
                answer[i, 1] = l.points[i].y;
                answer[i, 2] = l.points[i].z;
            }
            return answer;
        }

        private static void CopyToLine(PFSSLine l, Matrix<float> matrix)
        {
            for (int i = 0; i < l.points.Count; i++)
            {
                l.points[i].x = matrix[i, 0];
                l.points[i].y = matrix[i, 1];
                l.points[i].z = matrix[i, 2];
            }
        }
    }
}
