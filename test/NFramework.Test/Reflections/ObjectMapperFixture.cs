using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 인스턴스끼리 Dynamic Accessor를 이용하여 속성값을 복사합니다.
    /// </summary>
    [TestFixture]
    public class ObjectMapperFixture : AbstractFixture {
        public const int ObjectCount = 100;

        private readonly IList<SourceData> SourceDatas = new List<SourceData>(ObjectCount);
        private readonly IList<TargetData> TargetDatas = new List<TargetData>(ObjectCount);

        [TestFixtureSetUp]
        public void ClassSetUp() {
            for(int i = 0; i < ObjectCount; i++) {
                SourceDatas.Add(new SourceData(i));
                TargetDatas.Add(new TargetData(i));
            }
        }

        /// <summary>
        /// 서로 다른 형식의 인스턴스의 속성 정보를 복사한다.
        /// </summary>
        [Test]
        public void ObjectMap() {
            using(new OperationTimer("Object Map")) {
                foreach(var source in SourceDatas) {
                    var target = new TargetData();

                    ObjectMapper.Map(source, target, true, true, new[] { "OtherDataOnly" });

                    Assert.AreEqual(source.Id, target.Id, "Id가 매핑되지 않았습니다.");
                    Assert.AreEqual(source.Name, target.Name);
                    Assert.AreEqual(source.Guid, target.Guid);
                    Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                    Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                    Assert.AreEqual((decimal)source.NumberLong.GetValueOrDefault(), target.NumberLong.GetValueOrDefault());

                    Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                }
            }
        }

        /// <summary>
        /// 복사한 인스턴스 정보를 원본에 다시 복사한다.
        /// </summary>
        [Test]
        public void ObjectMapReverse() {
            using(new OperationTimer("Object Map Reverse")) {
                foreach(var target in TargetDatas) {
                    var source = new SourceData();

                    ObjectMapper.Map(target, source, true, true, new[] { "DataOnly" });

                    Assert.AreEqual(source.Id, target.Id, "Id가 매핑되지 않았습니다.");
                    Assert.AreEqual(source.Name, target.Name, "Name이 매핑되지 않았습니다.");
                    Assert.AreEqual(source.Guid, target.Guid);
                    Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                    Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                    Assert.AreEqual(source.NumberLong.GetValueOrDefault(), (float)target.NumberLong.GetValueOrDefault());

                    Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                }
            }
        }

        /// <summary>
        /// 서로 다른 형식의 인스턴스의 속성 정보를 복사한다.
        /// </summary>
        [Test]
        public void ObjectMapProperty() {
            using(new OperationTimer("Object MapProperty")) {
                SourceDatas
                    .AsParallel()
                    .AsOrdered()
                    .Run(source => {
                             var target = source.MapProperty(() => new TargetData());

                             Assert.AreEqual(source.Id, target.Id);
                             Assert.AreEqual(source.Name, target.Name);
                             Assert.AreEqual(source.Guid, target.Guid);
                             Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                             Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                             Assert.AreEqual((decimal)source.NumberLong.GetValueOrDefault(), target.NumberLong.GetValueOrDefault());

                             Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                         });
            }
        }

        /// <summary>
        /// 복사한 인스턴스 정보를 원본에 다시 복사한다.
        /// </summary>
        [Test]
        public void ObjectMapPropertyReverse() {
            using(new OperationTimer("Object MapProperty Reverse")) {
                TargetDatas
                    .AsParallel()
                    .AsOrdered()
                    .Run((target) => {
                             var source = target.MapProperty(() => new SourceData());

                             Assert.AreEqual(source.Id, target.Id);
                             Assert.AreEqual(source.Name, target.Name);
                             Assert.AreEqual(source.Guid, target.Guid);
                             Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                             Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                             Assert.AreEqual(source.NumberLong.GetValueOrDefault(), (float)target.NumberLong.GetValueOrDefault());

                             Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                         });
            }
        }

        [Test]
        public void ObjectMapProperty_ReturnTarget() {
            using(new OperationTimer("Object MapProperty<T>")) {
                foreach(var source in SourceDatas) {
                    var target = ObjectMapper.MapProperty<TargetData>(source, () => new TargetData(100));

                    Assert.AreEqual(source.Id, target.Id);
                    Assert.AreEqual(source.Name, target.Name);
                    Assert.AreEqual(source.Guid, target.Guid);
                    Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                    Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                    Assert.AreEqual((decimal)source.NumberLong.GetValueOrDefault(), target.NumberLong.GetValueOrDefault());

                    Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                }
            }
        }

        [Test]
        public void ObjectMapProperty_ReturnTarget_Options() {
            using(new OperationTimer("Object MapProperty<T>")) {
                foreach(var source in SourceDatas) {
                    var target = ObjectMapper.MapProperty<TargetData>(source, () => new TargetData(100), MapPropertyOptions.Default);

                    Assert.AreEqual(source.Id, target.Id);
                    Assert.AreEqual(source.Name, target.Name);
                    Assert.AreEqual(source.Guid, target.Guid);
                    Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                    Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                    Assert.AreEqual((decimal)source.NumberLong.GetValueOrDefault(), target.NumberLong.GetValueOrDefault());

                    Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                }
            }
        }

        [Test]
        public void Fasterflect_DelegateForMap() {
            using(new OperationTimer("Fasterflect DelegateForMap")) {
                var mapper = Fasterflect.MapExtensions.DelegateForMap(typeof(SourceData), typeof(TargetData));
                SourceDatas
                    .AsParallel()
                    .Run(source => {
                             var target = new TargetData();
                             mapper(source, target);

                             // Id 값은 수형이 달라, 매핑되지 않습니다. 그래서 명시적으로 수행해 주어야 합니다.
                             target.Id = source.Id;

                             Assert.AreEqual(source.Id, target.Id);
                             Assert.AreEqual(source.Name, target.Name);
                             Assert.AreEqual(source.Guid, target.Guid);
                             Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                             Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                             Assert.AreEqual(source.NumberLong.GetValueOrDefault(), (float)target.NumberLong.GetValueOrDefault());

                             Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                         });
            }
        }

        [Test]
        public void Fasterflect_MapProperties() {
            using(new OperationTimer("Fasterflect MapProperties")) {
                SourceDatas
                    .AsParallel()
                    .Run(source => {
                             var target = new TargetData();
                             source.MapProperties(target);

                             // Id 값은 수형이 달라, 매핑되지 않습니다. 그래서 명시적으로 수행해 주어야 합니다.
                             target.Id = source.Id;

                             Assert.AreEqual(source.Id, target.Id);
                             Assert.AreEqual(source.Name, target.Name);
                             Assert.AreEqual(source.Guid, target.Guid);
                             Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                             Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                             Assert.AreEqual(source.NumberLong.GetValueOrDefault(), (float)target.NumberLong.GetValueOrDefault());

                             Assert.AreNotEqual(source.DataOnly, target.OtherDataOnly);
                         });
            }
        }

        [Test]
        public void MapObject_With_CustomMap() {
            var targets =
                SourceDatas
                    .Select(source => {
                                var target = source.MapObject<SourceData, TargetData>(ActivatorTool.CreateInstance<TargetData>,
                                                                                      MapPropertyOptions.Default,
                                                                                      (s, t) => t.OtherDataOnly = s.DataOnly);

                                Assert.AreEqual(source.DataOnly, target.OtherDataOnly);

                                Assert.AreEqual(source.Id, (int)target.Id);
                                Assert.AreEqual(source.Name, target.Name);
                                Assert.AreEqual(source.Guid, target.Guid);
                                Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                                Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);

                                return target;
                            })
                    .ToList();

            Assert.AreEqual(SourceDatas.Count, targets.Count);
        }

        [Test]
        public void MapObjects_With_CustomMap() {
            var targets =
                SourceDatas
                    .MapObjects<SourceData, TargetData>(ActivatorTool.CreateInstance<TargetData>,
                                                        MapPropertyOptions.Default,
                                                        (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();
        }

        [Test]
        public void MapObjectsAsParallel_With_CustomMap() {
            var targets =
                SourceDatas
                    .MapObjectsAsParallel<SourceData, TargetData>(ActivatorTool.CreateInstance<TargetData>,
                                                                  (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();
        }

        [Test]
        public void MapObjectsAsParallel_To_Proxy_With_CustomMap() {
            var targets =
                SourceDatas
                    .MapObjectsAsParallel<SourceData, TargetData>(() => DynamicProxyTool.CreateNotifyPropertyChanged<TargetData>(),
                                                                  (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();
        }

        [Test]
        public void MapObject_With_CustomMap_Options() {
            var targets =
                SourceDatas
                    .Select(source => {
                                var target = source.MapObject<SourceData, TargetData>(() => new TargetData(),
                                                                                      (s, t) => t.OtherDataOnly = s.DataOnly);

                                Assert.AreEqual(source.Id, target.Id);
                                Assert.AreEqual(source.Name, target.Name);
                                Assert.AreEqual(source.Guid, target.Guid);
                                Assert.AreEqual(source.CreatedDate, target.CreatedDate);
                                Assert.AreEqual(source.ElapsedTimeSpan, target.ElapsedTimeSpan);
                                Assert.AreEqual(source.DataOnly, target.OtherDataOnly);

                                return target;
                            })
                    .ToList();

            Assert.AreEqual(SourceDatas.Count, targets.Count);
        }

        [Test]
        public void MapObjects_With_CustomMap_Options() {
            var targets =
                SourceDatas
                    .MapObjects<SourceData, TargetData>(() => new TargetData(), (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();

            Assert.AreEqual(SourceDatas.Count, targets.Count);
        }

        [Test]
        public void MapObjectsAsParallel_With_CustomMap_Options() {
            var targets =
                SourceDatas
                    .MapObjectsAsParallel<SourceData, TargetData>(() => new TargetData(),
                                                                  (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();

            Assert.AreEqual(SourceDatas.Count, targets.Count);
        }

        [Test]
        public void MapObjectsAsParallel_To_Proxy_With_CustomMap_Options() {
            var targets =
                SourceDatas
                    .MapObjectsAsParallel<SourceData, TargetData>(() => DynamicProxyTool.CreateNotifyPropertyChanged<TargetData>(),
                                                                  (s, t) => t.OtherDataOnly = s.DataOnly)
                    .ToList();

            Assert.AreEqual(SourceDatas.Count, targets.Count);
        }

        #region << 테스트용 엔티티 >>

        [Serializable]
        public class SourceData : ValueObjectBase {
            public SourceData() : this(0) {}

            public SourceData(int id) {
                Id = id;
                Name = "Data";
                Guid = System.Guid.NewGuid();
                CreatedDate = DateTime.Now;
                ElapsedTimeSpan = TimeSpan.FromMinutes(60);

                NumberLong = 12345.6789F;

                DataOnly = 1;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public Guid? Guid { get; set; }
            public DateTime? CreatedDate { get; set; }
            public TimeSpan? ElapsedTimeSpan { get; set; }
            public float? NumberLong { get; set; }

            public int? DataOnly { get; set; }

            public override int GetHashCode() {
                return HashTool.Compute(Id, Guid);
            }
        }

        [Serializable]
        public class TargetData : ValueObjectBase {
            public TargetData() : this(-1) {}

            public TargetData(int id) {
                Id = id;
                Name = "TargetData";
                Guid = System.Guid.NewGuid();
                CreatedDate = DateTime.Now.AddDays(1);
                ElapsedTimeSpan = TimeSpan.FromMinutes(120);

                NumberLong = 12345.6789M;

                OtherDataOnly = 2;
            }

            public long Id { get; set; }
            public string Name { get; set; }
            public Guid? Guid { get; set; }
            public DateTime? CreatedDate { get; set; }
            public TimeSpan? ElapsedTimeSpan { get; set; }
            public decimal? NumberLong { get; set; }

            public int? OtherDataOnly { get; set; }

            public override int GetHashCode() {
                return HashTool.Compute(Id, Guid);
            }
        }

        #endregion
    }
}