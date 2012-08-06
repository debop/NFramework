using System;
using System.Data;
using System.Text;
using NHibernate;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 문자열을 압축하여 저장하는 User Type의 기본 형식입니다.
    /// </summary>
    public abstract class AbstractCompressStringUserType : AbstractCompressUserType {
        /// <summary>
        /// Text 인코딩 방식
        /// </summary>
        protected virtual Encoding TextEncoder {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// 문자열을 압축합니다.
        /// </summary>
        protected byte[] CompressValue(string value) {
            if(value.IsEmpty())
                return null;

            return Compressor.Compress(TextEncoder.GetBytes(value));
        }

        /// <summary>
        /// 압축된 정보를 복원하여 문자열로 반환합니다.
        /// </summary>
        protected string DecompressValue(byte[] value) {
            if(value == null)
                return null;

            return TextEncoder.GetString(Compressor.Decompress(value)).Trim('\0');
        }

        #region Implementation of IUserType

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        /// Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param><param name="names">column names</param><param name="owner">the containing entity</param>
        /// <returns/>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public override object NullSafeGet(IDataReader rs, string[] names, object owner) {
            var value = NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0]);
            return DecompressValue(value as byte[]);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param><param name="value">the object to write</param><param name="index">command parameter index</param><exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public override void NullSafeSet(IDbCommand cmd, object value, int index) {
            var compressed = CompressValue(value as string);
            NHibernateUtil.BinaryBlob.NullSafeSet(cmd, compressed, index);
        }

        /// <summary>
        /// The type returned by <c>NullSafeGet()</c>
        /// </summary>
        public override Type ReturnedType {
            get { return typeof(string); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public override bool IsMutable {
            get { return NHibernateUtil.String.IsMutable; }
        }

        #endregion
    }
}