using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    /// <summary>
    /// enumのしょうもない処理ユーティリティ。
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// メンバ一覧を取得。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<TEnum> GetMembers<TEnum>(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<TEnum>();
        }

        /// <summary>
        /// 型からメンバ一覧を取得。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnum> GetMembers<TEnum>()
        {
            return GetMembers<TEnum>(typeof(TEnum));
        }

        /// <summary>
        /// 指定値を正規化された enum の値に変換する。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns>指定値がEnumに存在すれば指定値、存在しなければ<paramref name="defaultValue"/>を返す。</returns>
        public static TEnum GetNormalization<TEnum>(object value, TEnum defaultValue)
        {
            if(Enum.IsDefined(typeof(TEnum), value)) {
                return (TEnum)value;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        ///列挙体メンバのパース。
        ///<para><see cref="Enum.Parse(Type, string)"/>のキャスト周りを簡略化した版。</para>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">変換する名前または値が含まれている文字列。</param>
        /// <param name="ignoreCase">大文字と小文字を区別しない場合は true。大文字と小文字を区別する場合は false。</param>
        /// <returns></returns>
        public static TEnum Parse<TEnum>(string value, bool ignoreCase = true)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }
    }
}
