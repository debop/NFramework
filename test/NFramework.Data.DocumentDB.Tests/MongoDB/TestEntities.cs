using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace NSoft.NFramework.Data.MongoDB {
    internal class ReduceProduct {
        public ReduceProduct() {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public float Price { get; set; }
    }

    internal class ProductSum {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    internal class ProductSumObjectId {
        public ObjectId Id { get; set; }
        public int Value { get; set; }
    }

    public class GenericSuperClass<T> {}

    public class TestClass {
        public TestClass() {
            Id = Guid.NewGuid();
        }

        public Guid? Id { get; set; }

        public double? ADouble { get; set; }
        public string AString { get; set; }
        public int? AInteger { get; set; }
        public IList<string> AStringList { get; set; }
    }

    internal class Post {
        public Post() {
            Id = ObjectId.GenerateNewId();
            Comments = new List<Comment>();
            Tags = new List<string>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Title { get; set; }
        public int Score { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<string> Tags { get; set; }

        public override string ToString() {
            return string.Format("Post# Id=[{0}], Title=[{1}], Score=[{2}]", Id, Title, Score);
        }
    }

    internal class Post2 {
        public Post2() {
            Id = ObjectId.GenerateNewId();
            Comments = new List<Comment>();
            Tags = new List<string>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Title { get; set; }
        public int Score { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<string> Tags { get; set; }
    }

    internal class Comment {
        public string Text { get; set; }
        public string Name { get; set; }
        public bool IsOld { get; set; }
        public IList<Tag> CommentTags { get; set; }
        public IList<string> CommentTagsSimple { get; set; }
    }

    internal class Tag {
        public string TagName { get; set; }
    }

    internal class CheeseClubContact {
        public CheeseClubContact() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public string FavoriteCheese { get; set; }
    }

    internal class ProductReference {
        public ProductReference() {}

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public MongoDBRef[] ProductsOrdered { get; set; }
    }

    internal class User3 {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
    }

    internal class Role {
        public string Id { get; set; }
        public List<MongoDBRef> Users { get; set; }
    }

    internal class Person {
        public Person() {
            Id = ObjectId.GenerateNewId();
            Address = new Address();
            Relatives = new List<string>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
        public DateTime? LastContact { get; set; }
        public IList<string> Relatives { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Age { get; set; }
    }

    internal class Address {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    internal class ExpandoAddress {
        public string Street { get; set; }
        public string City { get; set; }
    }

    internal class Supplier {
        public Supplier() {
            Address = new Address();
            CreatedOn = DateTime.Now;
        }

        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public Address Address { get; set; }
        public int RefNum { get; set; }
    }

    internal class InventoryChange {
        public int AmountChanged { get; set; }
        public DateTime CreatedOn { get; set; }

        public InventoryChange() {
            CreatedOn = DateTime.Now;
        }
    }

    internal class TestProduct {
        public TestProduct() {
            Supplier = new Supplier();
            _id = ObjectId.GenerateNewId();
            Inventory = new List<InventoryChange>();
            UniqueID = Guid.NewGuid();
        }

        public List<InventoryChange> Inventory { get; set; }
        public ObjectId _id { get; set; }
        public Guid UniqueID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime Available { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsStillAvailable { get; set; }
    }

    internal class TestProductSummary {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    internal class TestIntGeneration {
        public int? _id { get; set; }
        public string Name { get; set; }
    }

    public class FakeObject {
        public FakeObject() {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
    }

    public enum Flags32 {
        FlagNone = 0,
        FlagOn = 1,
        FlagOff = 2
    }

    public enum Flags64 : long {
        FlagNone = 0,
        FlagOn = 1,
        FlagOff = 2
    }

    public class MiniObject {
        [BsonId]
        public ObjectId _id { get; set; }
    }

    public class PrivateSetter {
        public int Id { get; private set; }

        public PrivateSetter() {}

        public PrivateSetter(int id) {
            Id = id;
        }
    }

    public class ReadOnlyList {
        private IList<string> _names;

        public IList<string> Names {
            get {
                if(_names == null) {
                    _names = new List<string>();
                }
                return _names;
            }
        }
    }

    public class HashSetList {
        private HashSet<string> _names;

        public ICollection<string> Names {
            get { return _names ?? (_names = new HashSet<string>()); }
        }
    }

    public class DictionaryObject {
        private Dictionary<string, int> _lookup;

        public Dictionary<string, int> Names {
            get { return _lookup ?? (_lookup = new Dictionary<string, int>()); }
            set { _lookup = value; }
        }
    }

    public class IDictionaryObject {
        private IDictionary<string, int> _lookup;

        public IDictionary<string, int> Names {
            get { return _lookup ?? (_lookup = new Dictionary<string, int>()); }
            set { _lookup = value; }
        }
    }

    public class ReadOnlyDictionary {
        private IDictionary<string, int> _lookup;

        public IDictionary<string, int> Names {
            get { return _lookup ?? (_lookup = new Dictionary<string, int>()); }
        }
    }

    public class SerializerTest {
        public int Id { get; set; }

        [DefaultValue("Test")]
        public string Message { get; set; }

        [DefaultValue(typeof(DateTime), "00:00:00.0000000, January 1, 0001")]
        public DateTime MagicDate { get; set; }
    }

    public class GeneralDTO {
        public double? Pi { get; set; }
        public int? AnInt { get; set; }
        public String Title { get; set; }
        public bool? ABoolean { get; set; }
        public byte[] Bytes { get; set; }
        public String[] Strings { get; set; }
        public Guid? AGuid { get; set; }
        public Regex ARex { get; set; }
        public DateTime? ADateTime { get; set; }
        public List<string> AList { get; set; }
        public GeneralDTO Nester { get; set; }
        public float? AFloat { get; set; }
        public Flags32? Flags32 { get; set; }
        public Flags64? Flags64 { get; set; }
        internal IEnumerable<Person> AnIEnumerable { get; set; }

        public int IgnoredProperty { get; set; }
    }

    public class ChildGeneralDTO : GeneralDTO {
        public bool IsOver9000 { get; set; }
    }

    public class CultureInfoDTO {
        public CultureInfo Culture { get; set; }
    }

    public class NonSerializableClass {
        public NonSerializableValueObject Value { get; set; }
        public string Text { get; set; }
    }

    public class NonSerializableValueObject {
        // Stuff a few properties in here that Norm normally cannot handle
        private ArgumentException ex { get; set; }
        private NonSerializableValueObject MakeNormCrashReference { get; set; }

        public string Number { get; private set; }

        public NonSerializableValueObject(string number) {
            Number = number;
            MakeNormCrashReference = this;
        }
    }

    public class SuperClassObject {
        public SuperClassObject() {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }

        public string Title { get; set; }
    }

    public class SubClassedObject : SuperClassObject {
        public bool ABool { get; set; }
    }

    internal interface IDiscriminated {
        Guid Id { get; }
    }

    internal class InterfaceDiscriminatedClass : IDiscriminated {
        public InterfaceDiscriminatedClass() {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }
    }

    internal class InterfacePropertyContainingClass {
        public InterfacePropertyContainingClass() {
            Id = Guid.NewGuid();
            InterfaceProperty = new NotDiscriminatedClass();
        }

        public Guid Id { get; set; }

        public INotDiscriminated InterfaceProperty { get; set; }
    }

    internal interface INotDiscriminated {
        string Something { get; set; }
    }

    internal class NotDiscriminatedClass : INotDiscriminated {
        public string Something { get; set; }
    }

    internal interface IDTOWithNonDefaultId {
        Guid MyId { get; }
    }

    internal class DtoWithNonDefaultIdClass : IDTOWithNonDefaultId {
        public DtoWithNonDefaultIdClass() {
            MyId = Guid.NewGuid();
        }

        public Guid MyId { get; protected set; }
    }

    public class PrivateConstructor {
        public string Name { get; set; }

        private PrivateConstructor() {}

        public static PrivateConstructor Create(string name) {
            return new PrivateConstructor { Name = name };
        }
    }

    public class Forum {
        public ObjectId Id { get; set; }
    }

    public class MessageThread {
        public ObjectId ForumId { get; set; }
    }

    internal class Shopper {
        public Shopper() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public Cart Cart { get; set; }
    }

    internal class Cart {
        public Cart() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public TestProduct Product { get; set; }
        public Supplier[] CartSuppliers { get; set; }
    }

    internal class User {
        public User() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    internal class User2 {
        public User2() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    internal class IdMap0 {
        public ObjectId _ID { get; set; }
    }

    internal class IdMap1 {
        public ObjectId TheID { get; set; }
    }

    internal class IdMap2 {
        public ObjectId ID { get; set; }
    }

    internal class IdMap3 {
        public ObjectId id { get; set; }
    }

    internal class IdMap4 {
        public ObjectId Id { get; set; }
    }
}