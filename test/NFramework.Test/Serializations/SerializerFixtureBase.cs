using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Serializations {
    public abstract class SerializerFixtureBase : AbstractFixture {
        protected readonly SerializationOptions[] Options
            = new SerializationOptions[]
              {
#if !SILVERLIGHT
                  SerializationOptions.Binary,
                  SerializationOptions.CompressedBinary,
#endif
                  SerializationOptions.Json,
                  SerializationOptions.CompressedJson,
                  SerializationOptions.Bson,
                  SerializationOptions.CompressedBson
              };

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }
    }
}