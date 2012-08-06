using System.IO;
using NUnit.Framework;

namespace NSoft.NFramework.Licensing {
    public abstract class AbstractLicenseFixture {
        public static readonly string public_and_private;
        public static readonly string public_only;
        public static readonly string floating_private;
        public static readonly string floating_public;

        private static readonly string ResourceHeader = typeof(AbstractLicenseFixture).Namespace + ".Licenses.";

        static AbstractLicenseFixture() {
            var assembly = typeof(AbstractLicenseFixture).Assembly;
            Assert.IsNotNull(assembly);

            public_and_private =
                new StreamReader(assembly.GetManifestResourceStream(ResourceHeader + "public_and_private.xml")).ReadToEnd();
            public_only = new StreamReader(assembly.GetManifestResourceStream(ResourceHeader + "public_only.xml")).ReadToEnd();
            floating_private = new StreamReader(assembly.GetManifestResourceStream(ResourceHeader + "floating_private.xml")).ReadToEnd();
            floating_public = new StreamReader(assembly.GetManifestResourceStream(ResourceHeader + "floating_public.xml")).ReadToEnd();
        }
    }
}