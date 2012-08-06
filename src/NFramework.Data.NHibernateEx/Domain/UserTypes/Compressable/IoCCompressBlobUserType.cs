using System.Threading;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// IoC 환경설정에 지정된 <see cref="ICompressor"/>를 사용하도록 합니다. 환경설정에 설정된 값이 없다면, <see cref="SharpGZipCompressor"/>를 사용합니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		// Hbm
    /// 
    ///		<class name="className">
    ///			...
    /// 
    ///		    <property name="CompressedData" type="NSoft.NFramework.Data.Domain.UserTypes.IoCCompressBlobUserType, NSoft.NFramework.Data.NHibernateEx">
    ///				<column nmae="CompressedData" length="9999" />
    ///			</property>
    /// 
    ///		</class>
    /// 
    /// 
    /// 	// Fluent
    /// 	Map(x=>x.CompressedData)
    /// 		.CustomType{IoCCompressBlobUserType}()
    /// 		.Columns.Clear()
    /// 		.Columns.Add(new ColumnMapping {Name = "CompressedData", Length = 9999});
    /// 
    /// 
    /// 
    /// 
    /// </code>
    /// </example>
    public class IoCCompressBlobUserType : AbstractCompressBlobUserType {
        private static ICompressor _compressor;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get {
                if(_compressor == null)
                    lock(_syncLock)
                        if(_compressor == null) {
                            var compressor = IoC.TryResolve<ICompressor, SharpGZipCompressor>();
                            Thread.MemoryBarrier();
                            _compressor = compressor;
                        }
                return _compressor;
            }
        }
    }
}