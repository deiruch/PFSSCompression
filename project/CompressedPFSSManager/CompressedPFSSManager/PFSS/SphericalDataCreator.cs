using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
	/// <summary>
	/// Creates the PFSSData structure containing points in the spherical coordinate system
	/// </summary>
	class SphericalDataCreator
	{
		protected double l0;
		protected double b0;
		protected float[] ptx;
		protected short[] ptr_nz_len;
		protected float[] pty;
		protected float[] ptz;

		public SphericalDataCreator(double _l0, double _b0, float[] _ptr, short[] _ptr_nz_len, float[] _ptph, float[] _ptth)
		{
			Debug.Assert(_ptr.Length == _ptth.Length);
			Debug.Assert(_ptth.Length == _ptph.Length);

			l0 = _l0;
			b0 = _b0;
			ptr_nz_len = _ptr_nz_len;

			ptx = new float[_ptr.Length];
			pty = new float[_ptr.Length];
			ptz = new float[_ptr.Length];

			for (var i = 0; i < _ptr.Length; i++)
			{
				ptx[i] = (float)(_ptr[i] * PFSSPoint.SUN_RADIUS * Math.Sin(_ptth[i]) * Math.Sin(_ptph[i])); 	//x
				pty[i] = (float)(_ptr[i] * PFSSPoint.SUN_RADIUS * Math.Cos(_ptth[i])); 				//y     
				ptz[i] = (float)(_ptr[i] * PFSSPoint.SUN_RADIUS * Math.Sin(_ptth[i]) * Math.Cos(_ptph[i])); 	//z
			}
		}

		protected LineType GetType(int lineStart, int lineEnd)
		{
			//error: somewhere between IDL and Fits there is something which eats the last few floats, but only floats!
			if (lineEnd > ptx.Length)
			{
				Console.WriteLine("Warning: Truncated line");
				lineEnd = ptx.Length - 1;
			}

            var rStart = Math.Sqrt(Math.Pow(ptx[lineStart], 2) + Math.Pow(pty[lineStart], 2) + Math.Pow(ptz[lineStart], 2));
            var rEnd = Math.Sqrt(Math.Pow(ptx[lineEnd], 2) + Math.Pow(pty[lineEnd], 2) + Math.Pow(ptz[lineEnd], 2));

            if (rStart > PFSSPoint.SUN_RADIUS * 1.05)
				return LineType.OUTSIDE_TO_SUN;
            else if (rEnd > PFSSPoint.SUN_RADIUS * 1.05)
				return LineType.SUN_TO_OUTSIDE;
			else
				return LineType.SUN_TO_SUN;
		}

		public PFSSData Create()
		{
			var lines = new List<PFSSLine>(ptr_nz_len.Length);

			int lineEnd = ptr_nz_len[0] - 1;
			int lineStart = 0;

			var type = GetType(lineStart, lineEnd);

			int vertexIndex = 0;
			for (int i = 0; i < ptr_nz_len.Length; i++)
			{
				var lineSize = ptr_nz_len[i];
				var line = new List<PFSSPoint>(lineSize);
				var t = GetType(vertexIndex, vertexIndex + lineSize - 1);

				var maxSize = vertexIndex + lineSize;
				for (; vertexIndex < maxSize; vertexIndex++)
					line.Add(new PFSSPoint(ptx[vertexIndex], pty[vertexIndex], ptz[vertexIndex]));

				lines.Add(new PFSSLine(t, line));
			}

			return new PFSSData(l0, b0,lines);
		}
	}
}
