using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public interface IReadOnlyRange<T>
        where T : IComparable
    {
        #region property

        T Head { get; }
        T Tail { get; }

        #endregion
    }

    /// <summary>
    /// 範囲持ちアイテム。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct Range<T> : IReadOnlyRange<T>
        where T : IComparable
    {
        public Range(T head, T tail)
        {
            Head = head;
            Tail = tail;
        }

        #region IReadOnlyRange

        /// <summary>
        /// 範囲の開始点。
        /// </summary>
        [DataMember]
        public T Head { get; set; }
        /// <summary>
        /// 範囲の終了点。
        /// </summary>
        [DataMember]
        public T Tail { get; set; }

        #endregion

    }

    /// <summary>
    /// ヘルパ。
    /// </summary>
    public static class Range
    {
        public static Range<T> Create<T>(T head, T tail)
            where T : IComparable
        {
            return new Range<T>(head, tail);
        }

        public static Range<T> Parse<T>(string value)
            where T : IComparable
        {
            var values = value.Split(',');

            if(values.Length != 2) {
                throw new ArgumentException($"{nameof(value)}: illegal, {value}");
            }

            var rawRanges = values
                .Select(s => (T)Convert.ChangeType(s.Trim(), typeof(T)))
                .ToArray()
            ;

            return Create(rawRanges[0], rawRanges[1]);
        }

        public static bool TryParse<T>(string value, out Range<T> result)
            where T : IComparable
        {
            try {
                result = Parse<T>(value);
                return true;
            } catch {
                result = default(Range<T>);
                return false;
            }
        }

    }

    public static class RangeExtensions
    {
        #region function

        /// <summary>
        /// 指定された値が範囲内にあるか・。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this IReadOnlyRange<T> range, T value)
            where T : IComparable
        {
            return 0 <= value.CompareTo(range.Head) && value.CompareTo(range.Tail) <= 0;
        }

        #endregion
    }
}
