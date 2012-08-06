using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;

namespace NSoft.NFramework.Data.MongoDB {
    /// <summary>
    /// Mongo DB에 대한 Repository 패턴을 제공하는 클래스입니다.
    /// </summary>
    public class MongoRepositoryImpl : IMongoRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly CommandResult EmptyResult = new CommandResult();

        #region	<< Constructors >>

        public MongoRepositoryImpl() : this(MongoTool.DefaultConnectionString) {}
        public MongoRepositoryImpl(string connectionString) : this(new MongoConnectionStringBuilder(connectionString)) {}

        public MongoRepositoryImpl(MongoConnectionStringBuilder builder) {
            builder.ShouldNotBeNull("builder");

            if(IsDebugEnabled)
                log.Debug("MongoRepositoryImpl 인스턴스를 생성합니다... connectionString=[{0}]", builder.ConnectionString);

            Server = builder.CreateMongoServer();
            DatabaseName = builder.DatabaseName;
            CollectionName = MongoTool.DefaultCollectionName;
        }

        #endregion

        /// <summary>
        /// Mongo DB 서버
        /// </summary>
        public MongoServer Server { get; private set; }

        private string _databaseName;

        /// <summary>
        /// 대상 Database 명
        /// </summary>
        public string DatabaseName {
            get { return _databaseName; }
            set {
                value.ShouldNotBeWhiteSpace("DatabaseName");
                _databaseName = value;
            }
        }

        public MongoDatabase Database {
            get { return Server[DatabaseName]; }
        }

        public string CollectionName { get; set; }

        public MongoCollection<BsonDocument> Collection {
            get { return GetCollection(CollectionName ?? MongoTool.DefaultCollectionName); }
        }

        public MongoGridFS GridFS {
            get { return Database.GridFS; }
        }

        /// <summary>
        /// 서버에 접속 중인가?
        /// </summary>
        public virtual bool IsServerConnected {
            get { return (Server != null) && (Server.State == MongoServerState.Connected); }
        }

        public SafeMode SafeMode {
            get { return Server.Settings.SafeMode; }
        }

        //! ==============================================================

        /// <summary>
        /// <paramref name="databaseName"/>의 DB가 존재하는지 검사합니다.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public bool DatabaseExists(string databaseName) {
            return Server.DatabaseExists(databaseName);
        }

        public CommandResult DropDatabase(string databaseName) {
            return Server.DropDatabase(databaseName);
        }

        //public void CopyDatabase(string fromDatabaseName, string toDatabaseName)
        //{
        //    Server.CopyDatabase(fromDatabaseName, toDatabaseName);
        //}

        public void Reconnect() {
            Server.Reconnect();
        }

        //! ==============================================================

        /// <summary>
        /// 지정된 이름의 컬렉션을 생성합니다.
        /// </summary>
        /// <param name="collectionName">생성할 컬렉션 명</param>
        /// <returns></returns>
        public CommandResult CreateCollection(string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");

            if(CollectionExists(collectionName) == false) {
                if(IsDebugEnabled)
                    log.Debug("컬렉션을 생성합니다... collectionName=[{0}]", collectionName);

                return Database.CreateCollection(collectionName);
            }

            return EmptyResult;
        }

        /// <summary>
        /// 지정한 collection name에 해당하는 컬렉션을 DB에서 삭제합니다. (Generic이 아니므로, 여러가지 수형을 담을 수 있는 컬렉션입니다.)
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public CommandResult DropCollection(string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");

            if(CollectionExists(collectionName)) {
                if(IsDebugEnabled)
                    log.Debug("컬렉션을 제거합니다... collectionName=[{0}]", collectionName);

                return Database.DropCollection(collectionName);
            }
            return EmptyResult;
        }

        /// <summary>
        /// 현 <see cref="Database"/>의 모든 컬렉션을 삭제합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CommandResult> DropAllCollection() {
            lock(DatabaseName) {
                return
                    GetAllCollectionNames()
                        .Select(name => Database.DropCollection(name))
                        .ToArray();
            }
        }

        public bool CollectionExists(string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");
            return Database.CollectionExists(collectionName);
        }

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <param name="collectionName">컬렉션 명</param>
        /// <returns></returns>
        public MongoCollection<BsonDocument> GetCollection(string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");
            return Database.GetCollection(collectionName);
        }

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public MongoCollection GetCollection(Type documentType, string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");
            return Database.GetCollection(documentType, collectionName);
        }

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">컬렉션 명</param>
        /// <returns></returns>
        public MongoCollection<T> GetCollectionAs<T>(string collectionName) {
            collectionName.ShouldNotBeWhiteSpace("collectionName");
            return Database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// <paramref name="srcName"/>의 컬렉션의 이름을 <paramref name="destName"/>으로 변경합니다.
        /// </summary>
        /// <param name="srcName"></param>
        /// <param name="destName"></param>
        /// <returns></returns>
        public CommandResult RenameCollection(string srcName, string destName) {
            srcName.ShouldNotBeNull("srcName");
            destName.ShouldNotBeNull("destName");

            if(IsDebugEnabled)
                log.Debug("컬렉션 명을 변경합니다. srcName=[{0}], destName=[{1}]", srcName, destName);

            try {
                return Database.RenameCollection(srcName, destName);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Collection 이름 변경에 실패했습니다. [{0}] => [{1}]", srcName, destName);
                    log.Warn(ex);
                }
            }
            return EmptyResult;
        }

        /// <summary>
        /// <see cref="Database"/>의 모든 Collection의 아름을 열거합니다.
        /// </summary>
        public string[] GetAllCollectionNames() {
            lock(DatabaseName) {
                return
                    Database
                        .GetCollectionNames()
                        .Where(name => name != MongoTool.SystemIndexesCollectionName)
                        .ToArray();
            }
        }

        /// <summary>
        /// Database에 있는 모든 Collection을 가져온다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MongoCollection<BsonDocument>> GetAllCollection() {
            return GetAllCollectionNames().Select(name => GetCollection(name));
        }

        /// <summary>
        /// Database의 모든 컬렉션의 정보를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MongoCollectionSettings> GetAllCollectionSettings() {
            return GetAllCollection().Select(coll => coll.Settings);
        }

        //! ==============================================================

        /// <summary>
        /// 지정된 컬렉션의 Document 수를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public long Count() {
            return Collection.Count();
        }

        /// <summary>
        /// 지정된 컬렉션의 조건에 맞는 Document 수를 반환합니다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public long Count(IMongoQuery query) {
            return Collection.Count(query);
        }

        /// <summary>
        /// 컬렉션에서 <paramref name="key"/> 의 값들을 중복되지 않도록 가져옵니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<BsonValue> Distinct(string key) {
            key.ShouldNotBeWhiteSpace("key");
            return Collection.Distinct(key);
        }

        /// <summary>
        /// 컬렉션에서 <paramref name="key"/> 의 값들을 중복되지 않도록 가져옵니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<BsonValue> Distinct(string key, IMongoQuery query) {
            key.ShouldNotBeWhiteSpace("key");
            return Collection.Distinct(key, query ?? Query.Null);
        }

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다. 
        /// </summary>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        public BsonDocument FindOne(IMongoQuery query) {
            return Collection.FindOne(query ?? Query.Null);
        }

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다. 
        /// </summary>
        /// <param name="documentType">Document Type</param>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        public object FindOneAs(Type documentType, IMongoQuery query) {
            return Collection.FindOneAs(documentType, query ?? Query.Null);
        }

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다.
        /// </summary>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        public T FindOneAs<T>(IMongoQuery query) {
            return Collection.FindOneAs<T>(query ?? Query.Null);
        }

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <param name="id">Id 값</param>
        /// <returns></returns>
        public BsonDocument FindOneById(BsonValue id) {
            return Collection.FindOneById(id);
        }

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public object FindOneByIdAs(Type documentType, BsonValue id) {
            return Collection.FindOneByIdAs(documentType, id);
        }

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindOneByIdAs<T>(BsonValue id) {
            return (T)FindOneByIdAs(typeof(T), id);
        }

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        public MongoCursor<BsonDocument> Find(IMongoQuery query) {
            return Collection.Find(query);
        }

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <param name="documentType">Document 수형</param>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        public MongoCursor FindAs(Type documentType, IMongoQuery query) {
            documentType.ShouldNotBeNull("documentType");
            return Collection.FindAs(documentType, query);
        }

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        public MongoCursor<T> FindAs<T>(IMongoQuery query) {
            return Collection.FindAs<T>(query);
        }

        /// <summary>
        /// 컬렉션의 모든 Document를 반환합니다.
        /// </summary>
        public MongoCursor<BsonDocument> FindAll() {
            return Collection.FindAll();
        }

        /// <summary>
        /// 컬렉션의 모든 Document를 반환합니다.
        /// </summary>
        public MongoCursor FindAllAs(Type documentType) {
            return Collection.FindAllAs(documentType);
        }

        /// <summary>
        /// 컬렉션의 모든 Document를 지정된 형식으로 반환합니다.
        /// </summary>
        public MongoCursor<T> FindAllAs<T>() {
            return Collection.FindAllAs<T>();
        }

        /// <summary>
        /// 조회한 Document 정보를 갱신합니다.
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="sortBy">정렬 규칙</param>
        /// <param name="update">Update </param>
        /// <param name="returnNew">Update된 Document를 반환할 것인가?</param>
        /// <returns>대상 결과</returns>
        public FindAndModifyResult FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool? returnNew) {
            return Collection.FindAndModify(query, sortBy, update, returnNew ?? true);
        }

        /// <summary>
        /// 조회한 Document 정보를 갱신합니다.
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="sortBy">정렬 규칙</param>
        /// <param name="update">Update </param>
        /// <param name="returnNew">Update된 Document를 반환할 것인가?</param>
        /// <param name="upsert">Update할 게 없으면 Insert를 할 것인가?</param>
        /// <returns>대상 결과</returns>
        public FindAndModifyResult FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool? returnNew,
                                                 bool? upsert) {
            return Collection.FindAndModify(query, sortBy, update, returnNew ?? true, upsert ?? true);
        }

        /// <summary>
        /// 질의에 해당하는 Document를 찾아서 삭제합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <returns>수행 결과</returns>
        public FindAndModifyResult FindAndRemove(IMongoQuery query) {
            return Collection.FindAndRemove(query, SortBy.Null);
        }

        /// <summary>
        /// 질의에 해당하는 Document를 찾아서 삭제합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <param name="sortBy">정렬 방식</param>
        /// <returns>수행 결과</returns>
        public FindAndModifyResult FindAndRemove(IMongoQuery query, IMongoSortBy sortBy) {
            return Collection.FindAndRemove(query, sortBy);
        }

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="key">Grouping 기준 키</param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        public IEnumerable<BsonDocument> Group(IMongoQuery query, string key, BsonDocument initial, BsonJavaScript reduce,
                                               BsonJavaScript finalize) {
            return Collection.Group(query, key, initial, reduce, finalize);
        }

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="groupBy">Grouping 기준 키 (예: GroupBy.Keys("x"), GroupBy.Function("function(doc) { return { x: doc.x }; }") )</param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        public IEnumerable<BsonDocument> Group(IMongoQuery query, IMongoGroupBy groupBy, BsonDocument initial, BsonJavaScript reduce,
                                               BsonJavaScript finalize) {
            return Collection.Group(query, groupBy, initial, reduce, finalize);
        }

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="keyFunction">Grouping 기준 키를 선택하는 Javascript (예: "function(doc) { return { x: doc.x }; }") </param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        public IEnumerable<BsonDocument> Group(IMongoQuery query, BsonJavaScript keyFunction, BsonDocument initial,
                                               BsonJavaScript reduce, BsonJavaScript finalize) {
            return Collection.Group(query, keyFunction, initial, reduce, finalize);
        }

        //! ==============================================================

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <param name="document">추가할 Document</param>
        public SafeModeResult Insert(BsonDocument document) {
            return Collection.Insert(document, SafeMode);
        }

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <param name="documentType">추가할 Document의 수형</param>
        /// <param name="document">추가할 Document</param>
        public SafeModeResult Insert(Type documentType, object document) {
            return Collection.Insert(documentType, document, SafeMode);
        }

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <typeparam name="T">추가할 Document의 수형</typeparam>
        /// <param name="document">추가할 Document</param>
        public SafeModeResult Insert<T>(T document) {
            return Collection.Insert<T>(document, SafeMode);
        }

        /// <summary>
        /// 엔티티들을 새로 추가합니다.
        /// </summary>
        /// <param name="documentType">요소의 수형</param>
        /// <param name="documents">추가할 요소의 시퀀스</param>
        public IEnumerable<SafeModeResult> InsertBatch(Type documentType, IEnumerable<object> documents) {
            return Collection.InsertBatch(documentType, documents);
        }

        /// <summary>
        /// 엔티티들을 새로 추가합니다.
        /// </summary>
        /// <param name="documents">추가할 요소의 시퀀스</param>
        public IEnumerable<SafeModeResult> InsertBatch<T>(IEnumerable<T> documents) {
            return Collection.InsertBatch<T>(documents, SafeMode);
        }

        /// <summary>
        /// Document 를 저장합니다. Document는 [BsonId] 를 가지는 Id 속성을 가져야 합니다.
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public SafeModeResult Save(Type documentType, object document) {
            return Collection.Save(documentType, document, SafeMode);
        }

        /// <summary>
        /// Document 를 저장합니다. Document는 [BsonId] 를 가지는 Id 속성을 가져야 합니다.
        /// </summary>
        /// <typeparam name="T">추가할 Document의 수형</typeparam>
        /// <param name="document">추가할 Document</param>
        public SafeModeResult Save<T>(T document) {
            return Collection.Save<T>(document, SafeMode);
        }

        public SafeModeResult Remove(BsonDocument document) {
            return RemoveById(document[MongoTool.IdString]);
        }

        /// <summary>
        /// 지정된 엔티티를 삭제합니다.
        /// </summary>
        /// <param name="query">삭제할 요소의 조건</param>
        public SafeModeResult Remove(IMongoQuery query) {
            return Collection.Remove(query, SafeMode);
        }

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <returns></returns>
        public SafeModeResult RemoveById(BsonValue id) {
            return Remove(Query.EQ(MongoTool.IdString, id));
        }

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <param name="documentType">Document 수형</param>
        /// <param name="id">Document Id</param>
        public SafeModeResult RemoveByIdAs(Type documentType, BsonValue id) {
            return GetCollection(documentType, CollectionName).Remove(Query.EQ(MongoTool.IdString, id));
        }

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <typeparam name="T">Document 수형</typeparam>
        /// <param name="id">Document Id</param>
        public SafeModeResult RemoveByIdAs<T>(BsonValue id) {
            return RemoveByIdAs(typeof(T), id);
        }

        /// <summary>
        /// 컬렉션에서 모든 엔티티를 삭제합니다.
        /// </summary>
        public SafeModeResult RemoveAll() {
            return Collection.RemoveAll();
        }

        #region << IDisposable >>

        public bool IsDisposed { get; protected set; }

        ~MongoRepositoryImpl() {
            Dispose(false);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                try {
                    if(IsServerConnected) {
                        if(IsDebugEnabled)
                            log.Debug("MongoServer[{0}]의 연결을 끊습니다.", Server);

                        Server.Disconnect();
                    }
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled) {
                        log.Warn("MongoRepositoryImpl 인스턴스를 Disposing하는 동안에 예외가 발생했습니다.");
                        log.Warn(ex);
                    }
                }
            }
            IsDisposed = true;
        }

        #endregion
    }
}