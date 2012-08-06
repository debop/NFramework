using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace NSoft.NFramework.Data.MongoDB {
    /// <summary>
    /// MongoDB에 대한 Data 처리를 위한 Repository를 표현합니다.
    /// </summary>
    public interface IMongoRepository : IDisposable {
        /// <summary>
        /// MongoDB 서버
        /// </summary>
        MongoServer Server { get; }

        /// <summary>
        /// Database 명
        /// </summary>
        string DatabaseName { get; set; }

        /// <summary>
        /// MongoDB Database 인스턴스
        /// </summary>
        MongoDatabase Database { get; }

        /// <summary>
        /// 컬렉션 명
        /// </summary>
        string CollectionName { get; set; }

        /// <summary>
        /// 컬렉션
        /// </summary>
        MongoCollection<BsonDocument> Collection { get; }

        /// <summary>
        /// Mongo Grid File System
        /// </summary>
        MongoGridFS GridFS { get; }

        /// <summary>
        /// 서버에 접속 중인가?
        /// </summary>
        bool IsServerConnected { get; }

        /// <summary>
        /// 안전 모드인가?
        /// </summary>
        SafeMode SafeMode { get; }

        /// <summary>
        /// <paramref name="databaseName"/>의 Database가 존재하는지 여부
        /// </summary>
        /// <param name="databaseName">Database 명</param>
        /// <returns></returns>
        bool DatabaseExists(string databaseName);

        /// <summary>
        /// <paramref name="databaseName"/>의 Database를 삭제하도록 명령합니다.
        /// </summary>
        /// <param name="databaseName">삭제할 Database 명</param>
        /// <returns>삭제 결과</returns>
        CommandResult DropDatabase(string databaseName);

        // TODO: Mongo C# Driver에서 아직 구현하지 않았습니다.
        // void CopyDatabase(string fromDatabaseName, string toDatabaseName);

        void Reconnect();

        /// <summary>
        /// 지정된 이름의 컬렉션을 생성합니다.
        /// </summary>
        /// <param name="collectionName">생성할 컬렉션 명</param>
        /// <returns></returns>
        CommandResult CreateCollection(string collectionName);

        /// <summary>
        /// 지정한 collection name에 해당하는 컬렉션을 DB에서 삭제합니다. (Generic이 아니므로, 여러가지 수형을 담을 수 있는 컬렉션입니다.)
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        CommandResult DropCollection(string collectionName);

        /// <summary>
        /// 현 <see cref="Database"/>의 모든 컬렉션을 삭제합니다.
        /// </summary>
        /// <returns>삭제 수행 결과의 컬렉션</returns>
        IEnumerable<CommandResult> DropAllCollection();

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <param name="collectionName">컬렉션 명</param>
        /// <returns>컬렉션 명의 컬렉션 반환</returns>
        MongoCollection<BsonDocument> GetCollection(string collectionName);

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        MongoCollection GetCollection(Type documentType, string collectionName);

        /// <summary>
        /// <paramref name="collectionName"/>의 Collection을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">컬렉션 명</param>
        /// <returns></returns>
        MongoCollection<T> GetCollectionAs<T>(string collectionName);

        /// <summary>
        /// <paramref name="srcName"/>의 컬렉션의 이름을 <paramref name="destName"/>으로 변경합니다.
        /// </summary>
        /// <param name="srcName"></param>
        /// <param name="destName"></param>
        /// <returns></returns>
        CommandResult RenameCollection(string srcName, string destName);

        /// <summary>
        /// <see cref="Database"/>의 모든 Collection의 아름을 열거합니다.
        /// </summary>
        string[] GetAllCollectionNames();

        /// <summary>
        /// Database에 있는 모든 Collection을 가져온다.
        /// </summary>
        /// <returns></returns>
        IEnumerable<MongoCollection<BsonDocument>> GetAllCollection();

        /// <summary>
        /// Database의 모든 컬렉션의 정보를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        IEnumerable<MongoCollectionSettings> GetAllCollectionSettings();

        /// <summary>
        /// 지정된 컬렉션의 Document 수를 반환합니다.
        /// </summary>
        /// <returns></returns>
        long Count();

        /// <summary>
        /// 지정된 컬렉션의 조건에 맞는 Document 수를 반환합니다.
        /// </summary>
        /// <param name="query">질의 객체</param>
        /// <returns></returns>
        long Count(IMongoQuery query);

        /// <summary>
        /// 컬렉션에서 <paramref name="key"/> 의 값들을 중복되지 않도록 가져옵니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<BsonValue> Distinct(string key);

        /// <summary>
        /// 컬렉션에서 <paramref name="key"/> 의 값들을 중복되지 않도록 가져옵니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<BsonValue> Distinct(string key, IMongoQuery query);

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다. 
        /// </summary>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        BsonDocument FindOne(IMongoQuery query);

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다. 
        /// </summary>
        /// <param name="documentType">Document Type</param>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        object FindOneAs(Type documentType, IMongoQuery query);

        /// <summary>
        /// 조건에 맞는 첫번째 요소를 반환합니다.
        /// </summary>
        /// <param name="query">조건 절 (없으면 null을 입력하세요)</param>
        /// <returns></returns>
        T FindOneAs<T>(IMongoQuery query);

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BsonDocument FindOneById(BsonValue id);

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        object FindOneByIdAs(Type documentType, BsonValue id);

        /// <summary>
        /// Id로 Document 조회하기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindOneByIdAs<T>(BsonValue id);

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        MongoCursor<BsonDocument> Find(IMongoQuery query);

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <param name="documentType">Document 수형</param>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        MongoCursor FindAs(Type documentType, IMongoQuery query);

        /// <summary>
        /// <paramref name="query"/> 조건에 맞는 Document를 조회합니다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="query">질의</param>
        /// <returns>질의 결과 컬렉션</returns>
        MongoCursor<T> FindAs<T>(IMongoQuery query);

        /// <summary>
        /// 컬렉션의 모든 Document를 반환합니다.
        /// </summary>
        MongoCursor<BsonDocument> FindAll();

        /// <summary>
        /// 컬렉션의 모든 Document를 반환합니다.
        /// </summary>
        MongoCursor FindAllAs(Type documentType);

        /// <summary>
        /// 컬렉션의 모든 Document를 지정된 형식으로 반환합니다.
        /// </summary>
        MongoCursor<T> FindAllAs<T>();

        /// <summary>
        /// 조회한 Document 정보를 갱신합니다.
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="sortBy">정렬 규칙</param>
        /// <param name="update">Update </param>
        /// <param name="returnNew">Update된 Document를 반환할 것인가?</param>
        /// <returns>대상 결과</returns>
        FindAndModifyResult FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool? returnNew);

        /// <summary>
        /// 조회한 Document 정보를 갱신합니다.
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="sortBy">정렬 규칙</param>
        /// <param name="update">Update </param>
        /// <param name="returnNew">Update된 Document를 반환할 것인가?</param>
        /// <param name="upsert">Update할 게 없으면 Insert를 할 것인가?</param>
        /// <returns>대상 결과</returns>
        FindAndModifyResult FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool? returnNew, bool? upsert);

        /// <summary>
        /// 질의에 해당하는 Document를 찾아서 삭제합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <returns>수행 결과</returns>
        FindAndModifyResult FindAndRemove(IMongoQuery query);

        /// <summary>
        /// 질의에 해당하는 Document를 찾아서 삭제합니다.
        /// </summary>
        /// <param name="query">질의</param>
        /// <param name="sortBy">정렬 방식</param>
        /// <returns>수행 결과</returns>
        FindAndModifyResult FindAndRemove(IMongoQuery query, IMongoSortBy sortBy);

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="key">Grouping 기준 키</param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        IEnumerable<BsonDocument> Group(IMongoQuery query, string key, BsonDocument initial, BsonJavaScript reduce,
                                        BsonJavaScript finalize);

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="groupBy">Grouping 기준 키 (예: GroupBy.Keys("x"), GroupBy.Function("function(doc) { return { x: doc.x }; }") )</param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        IEnumerable<BsonDocument> Group(IMongoQuery query, IMongoGroupBy groupBy, BsonDocument initial, BsonJavaScript reduce,
                                        BsonJavaScript finalize);

        /// <summary>
        /// Group By를 수행합니다. (Find() 후에 LINQ 의 GroupBy를 사용하는게 더 편할 것 같습니다)
        /// </summary>
        /// <param name="query">필터링</param>
        /// <param name="keyFunction">Grouping 기준 키를 선택하는 Javascript (예: "function(doc) { return { x: doc.x }; }") </param>
        /// <param name="initial">초기값에 해당하는 Document</param>
        /// <param name="reduce">Grouping 시의 처리에 대한 로직을 가진 Javascript (예: "function(doc, prev) { prev.count += 1; }")</param>
        /// <param name="finalize">마지막 정리 시의 실행해야 할 Javascript</param>
        /// <returns>Grouping된 Document의 컬렉션</returns>
        IEnumerable<BsonDocument> Group(IMongoQuery query, BsonJavaScript keyFunction, BsonDocument initial, BsonJavaScript reduce,
                                        BsonJavaScript finalize);

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <param name="document">추가할 Document</param>
        SafeModeResult Insert(BsonDocument document);

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <param name="documentType">추가할 Document의 수형</param>
        /// <param name="document">추가할 Document</param>
        SafeModeResult Insert(Type documentType, object document);

        /// <summary>
        /// 엔티티를 새로 추가합니다.
        /// </summary>
        /// <typeparam name="T">추가할 Document의 수형</typeparam>
        /// <param name="document">추가할 Document</param>
        SafeModeResult Insert<T>(T document);

        /// <summary>
        /// 엔티티들을 새로 추가합니다.
        /// </summary>
        /// <param name="documentType">요소의 수형</param>
        /// <param name="documents">추가할 요소의 시퀀스</param>
        IEnumerable<SafeModeResult> InsertBatch(Type documentType, IEnumerable<object> documents);

        /// <summary>
        /// 엔티티들을 새로 추가합니다.
        /// </summary>
        /// <param name="documents">추가할 요소의 시퀀스</param>
        IEnumerable<SafeModeResult> InsertBatch<T>(IEnumerable<T> documents);

        /// <summary>
        /// Document 를 저장합니다. Document는 [BsonId] 를 가지는 Id 속성을 가져야 합니다.
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        SafeModeResult Save(Type documentType, object document);

        /// <summary>
        /// Document 를 저장합니다. Document는 [BsonId] 를 가지는 Id 속성을 가져야 합니다.
        /// </summary>
        /// <typeparam name="T">추가할 Document의 수형</typeparam>
        /// <param name="document">추가할 Document</param>
        SafeModeResult Save<T>(T document);

        /// <summary>
        /// 삭제한다.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        SafeModeResult Remove(BsonDocument document);

        /// <summary>
        /// 질의 조건에 해당하는 Document를 삭제합니다.
        /// </summary>
        /// <param name="query">삭제할 요소의 조건</param>
        SafeModeResult Remove(IMongoQuery query);

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <returns></returns>
        SafeModeResult RemoveById(BsonValue id);

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <param name="documentType">Document 수형</param>
        /// <param name="id">Document Id</param>
        SafeModeResult RemoveByIdAs(Type documentType, BsonValue id);

        /// <summary>
        /// <paramref name="id"/> 값을 가지는 Document를 삭제합니다
        /// </summary>
        /// <typeparam name="T">Document 수형</typeparam>
        /// <param name="id">Document Id</param>
        SafeModeResult RemoveByIdAs<T>(BsonValue id);

        /// <summary>
        /// 컬렉션에서 모든 엔티티를 삭제합니다.
        /// </summary>
        SafeModeResult RemoveAll();
    }
}