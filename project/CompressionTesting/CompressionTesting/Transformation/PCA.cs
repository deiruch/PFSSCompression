using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CompressionTesting.Transformation
{/** The class responsible mainly for preparing the PCA transformation parameters 
 * based on training data and executing the actual transformation on test data.
 * @author Mateusz Kobos
 */
public class PCA {
	/** Type of the possible data transformation.
	 * ROTATION: rotate the data matrix to get a diagonal covariance matrix. 
	 * This transformation is sometimes simply called PCA.
	 * WHITENING: rotate and scale the data matrix to get 
	 * the unit covariance matrix
	 */
	public enum TransformationType { ROTATION, WHITENING };
	
	/** Whether the input data matrix should be centered. */
	private bool centerMatrix;
	
	/** Number of input dimensions. */
	private int inputDim;
	
	private Matrix<float> whiteningTransformation;
    internal Matrix<float> pcaRotationTransformation;
    internal Matrix<float> v;
	/** Part of the original SVD vector that is responsible for transforming the
	 * input data into a vector of zeros.*/
    internal Matrix<float> zerosRotationTransformation;
	internal float[] means;
	
	
	/** Create the PCA transformation
	 * @param data data matrix used to compute the PCA transformation. 
	 * Rows of the matrix are the instances/samples, columns are dimensions.
	 * @param evdCalc method of computing eigenvalue decomposition of data's
	 * covariance matrix
	 * @param center should the data matrix be centered before doing the
	 * calculations?
	 */
    public PCA(Matrix<float> data, bool center)
    {
		/** Determine if input matrix should be centered */
		this.centerMatrix = center;
		/** Get the number of input dimensions. */
        this.inputDim = data.ColumnCount;
		this.means = 
            getColumnsMeans(data);

        Matrix<float> centeredData = data;
		/** Center the data matrix columns about zero */
		if(centerMatrix){
			centeredData = shiftColumns(data, means);
		}
		//debugWrite(centeredData, "centeredData.csv");

		this.v = run(centeredData);

        this.pcaRotationTransformation = v;	
	}

    public PCA(Matrix<float> transformation, float[] means)
    {
        this.v = transformation;
        this.pcaRotationTransformation = transformation;
        this.means = means;
        centerMatrix = true;
    }

    public Matrix<float> run(Matrix<float> centeredData)
    {
        int m = centeredData.RowCount;
        int n = centeredData.ColumnCount;
        
		MathNet.Numerics.LinearAlgebra.Factorization.Svd<float> svd = centeredData.Svd();
        Vector<float> singularValues = svd.S;
        /*Matrix<float> d = Matrix<float>.Build.DenseIdentity(n, n);
		for(int i = 0; i < n; i++){
			double val;
			if(i < m) val = singularValues[i];
			else val = 0;
			
			d.set(i, i, 1.0/(m-1) * Math.pow(val, 2));
		}*/
        return svd.VT.Transpose();
	}

	
	/**
	 * Execute selected transformation on given data.
	 * @param data data to transform. Rows of the matrix are the 
	 * instances/samples, columns are dimensions. 
	 * If the original PCA data matrix was set to be centered, this
	 * matrix will also be centered using the same parameters.
	 * @param type transformation to apply
	 * @return transformed data
	 */
	public Matrix<float> transform(Matrix<float> data, TransformationType type){
		Matrix<float> centeredData = data;
		if(centerMatrix){
			centeredData = shiftColumns(data, means);
		}
		Matrix<float> transformation = getTransformation(type); 
		return centeredData * transformation;
	}


    public Matrix<float> inverseTransform(Matrix<float> data, TransformationType type)
    {
        
        Matrix<float> transformation = getTransformation(type);
        Matrix<float> inverse = data * transformation;
        if (centerMatrix)
        {
            inverse = inverseShiftColumns(inverse, means);
        }

        return inverse;
    }
	
	
	
	private Matrix<float> getTransformation(TransformationType type){
		switch(type)
        {
		    case TransformationType.ROTATION: 
                return pcaRotationTransformation;

            case TransformationType.WHITENING: 
                return whiteningTransformation;

            default:
                return null;
		}
	}
	
	private static Matrix<float> shiftColumns(Matrix<float> data, float[] shifts){
		Matrix<float> m = Matrix<float>.Build.Dense(
				data.RowCount, data.ColumnCount);
		for(int c = 0; c < data.ColumnCount; c++)
			for(int r = 0; r < data.RowCount; r++)
				m[r, c] = (float)(data[r, c] - shifts[c]);
		return m;
	}

    private static Matrix<float> inverseShiftColumns(Matrix<float> data, float[] shifts)
    {
        Matrix<float> m = Matrix<float>.Build.Dense(
                data.RowCount, data.ColumnCount);
        for (int c = 0; c < data.ColumnCount; c++)
            for (int r = 0; r < data.RowCount; r++)
                m[r, c] = (float)(data[r, c] + shifts[c]);
        return m;
    }
	
	private static float[] getColumnsMeans(Matrix<float> m){
		float[] means = new float[m.ColumnCount];
        for (int c = 0; c < m.ColumnCount; c++)
        {
			double sum = 0;
			for(int r = 0; r < m.RowCount; r++)
				sum += m[r, c];
			means[c] = (float)sum/m.RowCount;
		}
		return means;
	}
	
	private static Matrix<float> sqrtDiagonalMatrix(Matrix<float> m){

        Matrix<float> newM = Matrix<float>.Build.Dense(m.RowCount, m.ColumnCount);
        for (int i = 0; i < m.RowCount; i++)
			newM[i, i] =  (float)Math.Sqrt(m[i, i]);
		return newM;
	}
	
	private static Matrix<float> inverseDiagonalMatrix(Matrix<float> m){
        Matrix<float> newM = Matrix<float>.Build.Dense(m.RowCount, m.ColumnCount);
		for(int i = 0; i < m.RowCount; i++)
			newM[i, i] =  1/m[i, i];
		return newM;
	}
	
}




}
