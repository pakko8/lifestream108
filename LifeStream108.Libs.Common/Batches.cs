using System;
using System.Collections;
using System.Collections.Generic;

namespace LifeStream108.Libs.Common
{
    public class Batches<T> : IEnumerable<T[]>, IEnumerator<T[]>
    {
        private int batchSize;
        private T[] array;

        private int currentIndex;

        public Batches(T[] array, int batchSize)
        {
            this.array = array;
            this.batchSize = batchSize;
        }

        private T[] GetCurrent()
        {
            int size = batchSize;
            if (array.Length - currentIndex < batchSize) size = array.Length - currentIndex;
            T[] result = new T[size];
            Array.Copy(array, currentIndex, result, 0, size);
            currentIndex += result.Length;
            //
            return result;
        }

        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator()
        {
            currentIndex = 0;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            currentIndex = 0;
            return this;
        }

        T[] IEnumerator<T[]>.Current
        {
            get { return GetCurrent(); }
        }

        void IDisposable.Dispose()
        {
            array = null;
        }

        object IEnumerator.Current
        {
            get { return GetCurrent(); }
        }

        bool IEnumerator.MoveNext()
        {
            return currentIndex <= array.Length - 1;
        }

        void IEnumerator.Reset()
        {
            currentIndex = 0;
        }
    }
}
