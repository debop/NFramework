using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoRepository {
        public static void DownloadFile(this IMongoRepository repository, Stream stream, MongoGridFSFileInfo fileInfo) {
            stream.ShouldNotBeNull("stream");
            fileInfo.ShouldNotBeNull("fileinfo");

            repository.GridFS.Download(stream, fileInfo);
        }

        public static void DownloadFile(this IMongoRepository repository, Stream stream, string remoteFilename) {
            stream.ShouldNotBeNull("stream");
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            repository.GridFS.Download(stream, remoteFilename);
        }

        public static void DownloadFile(this IMongoRepository repository, Stream stream, string remoteFilename, int version) {
            stream.ShouldNotBeNull("stream");
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            repository.GridFS.Download(stream, remoteFilename, version);
        }

        public static void DownloadFile(this IMongoRepository repository, Stream stream, IMongoQuery query) {
            stream.ShouldNotBeNull("stream");
            repository.GridFS.Download(stream, query ?? Query.Null);
        }

        public static void DownloadFile(this IMongoRepository repository, Stream stream, IMongoQuery query, int version) {
            stream.ShouldNotBeNull("stream");
            repository.GridFS.Download(stream, query ?? Query.Null, version);
        }

        public static MongoGridFSStream OpenFile(this IMongoRepository repository, string remoteFilename) {
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            if(repository.FileExists(remoteFilename))
                return repository.GridFS.OpenRead(remoteFilename);
            return null;
        }

        public static StreamReader OpenFileText(this IMongoRepository repository, string remoteFilename) {
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            if(repository.FileExists(remoteFilename))
                return repository.GridFS.OpenText(remoteFilename);
            return null;
        }

        public static MongoGridFSFileInfo UploadFile(this IMongoRepository repository, string localFilename) {
            Guard.Assert(File.Exists(localFilename), "파일을 찾을 수 없습니다. localFilename=[{0}]", localFilename);
            return repository.GridFS.Upload(localFilename);
        }

        public static MongoGridFSFileInfo UploadFile(this IMongoRepository repository, string localFilename, string remoteFilename) {
            Guard.Assert(File.Exists(localFilename), "파일을 찾을 수 없습니다. localFilename=[{0}]", localFilename);
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            return repository.GridFS.Upload(localFilename, remoteFilename);
        }

        public static MongoGridFSFileInfo UploadFile(this IMongoRepository repository, string remoteFilename, Stream stream) {
            stream.ShouldNotBeNull("stream");
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            return repository.GridFS.Upload(stream, remoteFilename);
        }

        public static MongoGridFSFileInfo UploadFile(this IMongoRepository repository, string remoteFilename, Stream stream,
                                                     MongoGridFSCreateOptions createOptions) {
            stream.ShouldNotBeNull("stream");
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            return repository.GridFS.Upload(stream, remoteFilename, createOptions);
        }

        public static bool FileExists(this IMongoRepository repository, string remoteFilename) {
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            return repository.GridFS.Exists(remoteFilename);
        }

        public static bool FileExists(this IMongoRepository repository, IMongoQuery query) {
            return repository.GridFS.Exists(query);
        }

        public static bool FileExistsById(this IMongoRepository repository, BsonValue id) {
            return repository.GridFS.ExistsById(id);
        }

        public static MongoGridFSFileInfo FindOneFile(this IMongoRepository repository, string remoteFilename) {
            return repository.GridFS.FindOne(remoteFilename);
        }

        public static MongoGridFSFileInfo FindOneFile(this IMongoRepository repository, string remoteFilename, int version) {
            return repository.GridFS.FindOne(remoteFilename, version);
        }

        public static MongoGridFSFileInfo FindOneFile(this IMongoRepository repository, IMongoQuery query) {
            return repository.GridFS.FindOne(query);
        }

        public static MongoGridFSFileInfo FindOneFileById(this IMongoRepository repository, BsonValue id) {
            return repository.GridFS.FindOneById(id);
        }

        public static MongoCursor<MongoGridFSFileInfo> FindFile(this IMongoRepository repository, string remoteFilename) {
            return repository.GridFS.Find(remoteFilename);
        }

        public static MongoCursor<MongoGridFSFileInfo> FindFile(this IMongoRepository repository, IMongoQuery query) {
            return repository.GridFS.Find(query);
        }

        public static MongoCursor<MongoGridFSFileInfo> FindAllFile(this IMongoRepository repository) {
            return repository.GridFS.FindAll();
        }

        public static void DeleteFile(this IMongoRepository repository, string remoteFilename) {
            repository.GridFS.Delete(remoteFilename);
        }

        public static void DeleteFile(this IMongoRepository repository, IMongoQuery query) {
            repository.GridFS.Delete(query);
        }

        public static void DeleteFileById(this IMongoRepository repository, BsonValue id) {
            repository.GridFS.DeleteById(id);
        }

        /// <summary>
        /// 모든 파일을 삭제합니다.
        /// </summary>
        /// <param name="repository"></param>
        public static void DeleteAllFile(this IMongoRepository repository) {
            try {
                repository.GridFS.Delete(Query.Null);

                repository.GridFS.Chunks.RemoveAll();
                repository.GridFS.Chunks.ResetIndexCache();
                repository.GridFS.Files.RemoveAll();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Repository의 모든 파일을 삭제하는데 실패했습니다.", ex);
            }
        }
    }
}