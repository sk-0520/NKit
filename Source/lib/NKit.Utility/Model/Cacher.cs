using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    /// <summary>
    /// 生成データを保持しておく。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Cacher<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        ///非スレッドセーフでキャッシュ構築。
        /// </summary>
        public Cacher()
            : this(false)
        { }

        /// <summary>
        /// キャッシュ構築。
        /// </summary>
        /// <param name="isSynchronized">スレッドセーフにするか。</param>
        public Cacher(bool isSynchronized)
        {
            IsSynchronized = isSynchronized;

            if(IsSynchronized) {
                Cache = new ConcurrentDictionary<TKey, TValue>();
            } else {
                Cache = new Dictionary<TKey, TValue>();
            }
        }

        #region property

        /// <summary>
        /// スレッドセーフか。
        /// </summary>
        public bool IsSynchronized { get; }

        /// <summary>
        /// キャッシュデータ。
        /// </summary>
        protected IDictionary<TKey, TValue> Cache { get; private set; }

        #endregion

        #region function

        /// <summary>
        /// 指定キーからデータを取得する。
        /// <para>指定キーにデータがなければデータを生成してキャッシュに入れる。</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public TValue Get(TKey key, Func<TValue> creator)
        {
            TValue result;
            if(!Cache.TryGetValue(key, out result)) {
                result = creator();
                Cache[key] = result;
            }

            return result;
        }

        public void Add(TKey key, TValue value)
        {
            Cache.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return Cache.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 指定キーのキャッシュをクリア。
        /// </summary>
        /// <param name="key"></param>
        /// <returns>クリア出来れば真。</returns>
        public bool ClearCache(TKey key)
        {
            TValue result;
            if(Cache.TryGetValue(key, out result)) {
                return Cache.Remove(key);
            } else {
                return false;
            }
        }

        /// <summary>
        /// 全キャッシュデータのクリア。
        /// </summary>
        public void Clear()
        {
            foreach(var key in Cache.Keys.ToArray()) {
                ClearCache(key);
            }

            Cache.Clear();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Cache.GetEnumerator();
        }

        #endregion
    }

    public class CacheItem<T>
    {
        #region variable

        T _value;

        #endregion

        public CacheItem(T value)
        {
            Value = value;
        }

        #region property

        public T Value
        {
            get
            {
                LastUsedUtcTimestamp = DateTime.UtcNow;
                return this._value;
            }
            private set { this._value = value; }
        }

        public DateTime LastUsedUtcTimestamp { get; private set; }

        #endregion
    }
}
