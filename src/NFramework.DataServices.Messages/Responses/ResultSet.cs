using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 응답 결과의 정보를 DataSet 처럼 표현합니다.
    /// </summary>
    [Serializable]
    public class ResultSet : List<ResultRow> {
        private readonly object _syncLock = new object();

        public object this[int index, string fieldName] {
            get { return this[index][fieldName]; }
            set {
                lock(_syncLock) {
                    if(this[index].ContainsKey(fieldName))
                        this[index][fieldName] = value;
                    else
                        this[index].Add(fieldName, value);
                }
            }
        }

        private IList<string> _fieldNames;

        /// <summary>
        /// 필드(컬럼) 명들
        /// </summary>
        public IList<string> FieldNames {
            get {
                if(_fieldNames == null)
                    lock(_syncLock)
                        if(_fieldNames == null) {
                            var names = (Count == 0) ? new List<string>() : this[0].Keys.ToList();
                            Thread.MemoryBarrier();
                            _fieldNames = names;
                        }

                return _fieldNames;
            }
            set { _fieldNames = value; }
        }

        /// <summary>
        /// <see cref="ResultRow"/> 들을 <see cref="targetType"/>의 인스턴스로 매핑합니다.
        /// </summary>
        /// <param name="targetType">인스턴스의 수형</param>
        /// <returns><see cref="targetType"/>의 인스턴스의 컬렉션</returns>
        public IEnumerable<object> GetMappedObjects(Type targetType) {
            return this;
            //lock(_syncLock)
            //	return this.Select(r => ObjectMapper.Map(r, () => targetType.CreateInstance(true)));
        }
    }
}