using MongoDB.Driver;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoTool {
        /// <summary>
        /// <see cref="IMongoRepository"/> 를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static IMongoRepository CreateRepository() {
            if(IoC.IsInitialized) {
                var repository = IoC.Resolve<IMongoRepository>();
                if(repository != null)
                    return repository;
            }

            return CreateRepository(DefaultConnectionString);
        }

        /// <summary>
        /// <see cref="IMongoRepository"/>를 생성합니다.
        /// </summary>
        /// <param name="connectionString">Mongo DB Connection String (예: server=localhost;database=default;safe=true;)</param>
        /// <returns></returns>
        public static IMongoRepository CreateRepository(string connectionString) {
            connectionString.ShouldNotBeWhiteSpace("connectionString");
            return new MongoRepositoryImpl(connectionString);
        }

        /// <summary>
        /// <see cref="IMongoRepository"/>를 생성합니다. 
        /// </summary>
        /// <param name="connectionBuilder"></param>
        /// <returns></returns>
        public static IMongoRepository CreateRepository(this MongoConnectionStringBuilder connectionBuilder) {
            connectionBuilder.ShouldNotBeNull("connectionBuilder");
            return new MongoRepositoryImpl(connectionBuilder);
        }
    }
}