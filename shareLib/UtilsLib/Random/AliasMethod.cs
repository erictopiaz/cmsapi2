using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLib.Random
{
    public class AliasMethod
    {
        private int[] _alias;
        private double[] _probability;
        public AliasMethod()
        {

        }
        public AliasMethod(List<Double> probabilities)
        {

            _probability = new double[probabilities.Count];
            _alias = new int[probabilities.Count];

            double average = 1.0 / probabilities.Count;

            var small = new Stack<int>();
            var large = new Stack<int>();

            for (int i = 0; i < probabilities.Count; ++i)
            {
                if (probabilities[i] >= average)
                    large.Push(i);
                else
                    small.Push(i);
            }

            while (small.Count > 0 && large.Count > 0)
            {
                int less = small.Pop();
                int more = large.Pop();

                _probability[less] = probabilities[less] * probabilities.Count;
                _alias[less] = more;

                probabilities[more] = (probabilities[more] + probabilities[less] - average);

                if (probabilities[more] >= average)
                    large.Push(more);
                else
                    small.Push(more);
            }

            while (small.Count > 0)
                _probability[small.Pop()] = 1.0;
            while (large.Count > 0)
                _probability[large.Pop()] = 1.0;
        }
        /// <summary>
        /// app new not use construct
        /// </summary>
        /// <param name="probabilities"></param>
        public void ConfigMethod(List<Double> probabilities)
        {

            _probability = new double[probabilities.Count];
            _alias = new int[probabilities.Count];

            double average = 1.0 / probabilities.Count;

            var small = new Stack<int>();
            var large = new Stack<int>();

            for (int i = 0; i < probabilities.Count; ++i)
            {
                if (probabilities[i] >= average)
                    large.Push(i);
                else
                    small.Push(i);
            }

            while (small.Count > 0 && large.Count > 0)
            {
                int less = small.Pop();
                int more = large.Pop();

                _probability[less] = probabilities[less] * probabilities.Count;
                _alias[less] = more;

                probabilities[more] = (probabilities[more] + probabilities[less] - average);

                if (probabilities[more] >= average)
                    large.Push(more);
                else
                    small.Push(more);
            }

            while (small.Count > 0)
                _probability[small.Pop()] = 1.0;
            while (large.Count > 0)
                _probability[large.Pop()] = 1.0;
        }
        public int next()
        {
            long tick = DateTime.Now.Ticks;
            var seed = ((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            unchecked
            {
                seed = (seed + Guid.NewGuid().GetHashCode() + new System.Random().Next(0, 100));
            }
            var random = new System.Random(seed);
            int column = random.Next(_probability.Length);

            bool coinToss = random.NextDouble() < _probability[column];

            return coinToss ? column : _alias[column];
        }
    }
}
