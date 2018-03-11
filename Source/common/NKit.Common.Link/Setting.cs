using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public interface IReadOnlySetting: ICloneable
    { }

    [Serializable, DataContract]
    public abstract class SettingBase : IReadOnlySetting
    {
        #region ICloneable

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    public interface IReadOnlyIdSetting<TId>: IReadOnlySetting
    {
        TId Id { get; }
    }

    public interface IReadOnlyGuidIdSetting : IReadOnlyIdSetting<Guid>
    { }

    [Serializable, DataContract]
    public abstract class IdSettingBase<TId> : SettingBase, IReadOnlyIdSetting<TId>
    {
        #region IReadOnlyIdSetting

        [DataMember]
        public TId Id { get; set; }


        #endregion
    }

    [Serializable, DataContract]
    public abstract class GuidIdSettingBase : IdSettingBase<Guid>, IReadOnlyGuidIdSetting
    {
        public GuidIdSettingBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
