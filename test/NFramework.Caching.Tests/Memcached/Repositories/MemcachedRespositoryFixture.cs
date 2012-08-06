using System;
using System.Collections.Generic;
using NSoft.NFramework.Json;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Caching.Memcached.Repositories {
    [TestFixture]
    public class MemcachedRespositoryFixture {
        [TestCase(typeof(BinarySerializer))]
        [TestCase(typeof(JsonByteSerializer))]
        [TestCase(typeof(BsonSerializer))]
        public void Serialized_Caching(Type serializerType) {
            var serializer = (ISerializer)ActivatorTool.CreateInstance(serializerType);
            var repository = new MemcachedRepository(serializer);

            var tasks = new List<TaskCacheItem>();
            var cachedTasks = new List<TaskCacheItem>();

            for(var i = 0; i < 10; i++) {
                var task = new TaskCacheItem()
                           {
                               IsDone = false,
                               Summary = "Task " + i + " to cached.",
                               Data = ArrayTool.GetRandomBytes(0x0100)
                           };

                tasks.Add(task);
                repository.Set(task.Id.ToString(), task);
            }

            foreach(var task in tasks) {
                var cachedTask = repository.Get(task.Id.ToString()) as TaskCacheItem;

                cachedTask.Should().Not.Be.Null();
                cachedTasks.Add(cachedTask);
            }
        }
    }
}