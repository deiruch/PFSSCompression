using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class MovingAverage
    {
        private float[] movAv;
        private float average;
        private int index;
        private int count;
        public MovingAverage(int size)
        {
            movAv = new float[size];
            index = 0;
            count = 0;
        }

        public void Add(float value)
        {
            if (count >= movAv.Length)
            {
                average -= movAv[index];
                average += value;
                movAv[index++] = value;
                
            }
            else
            {
                movAv[index++] = value;
                average += value;
                count++;
            }
            index = index % movAv.Length;
        }

        public float GetAverage()
        {
            return average / count;
        }
    }
}
