using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public interface ICounter
    {
        #region property

        int MaxCount { get; }
        int CurrentCount { get; }

        bool IsFirst { get; }
        bool IsLast { get; }

        bool Complete { get; }

        #endregion
    }

    public class Counter : ICounter, IEnumerable<ICounter>
    {
        public Counter(int maxCount)
        {
            MaxCount = maxCount;
        }

        #region ICounter

        public int MaxCount { get; }

        public int CurrentCount { get; private set; }

        public bool IsFirst { get; private set; }
        public bool IsLast => CurrentCount == MaxCount;

        public bool Complete { get; set; }

        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ICounter> GetEnumerator()
        {
            Complete = false;

            foreach(var i in Enumerable.Range(1, MaxCount)) {
                IsFirst = i == 1;

                CurrentCount = i;
                yield return this;
            }

            Complete = true;
        }

        #endregion
    }
}
