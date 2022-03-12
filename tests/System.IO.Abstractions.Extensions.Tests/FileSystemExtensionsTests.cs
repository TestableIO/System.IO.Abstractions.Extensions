using NUnit.Framework;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class FileSystemExtensionsTests
    {
        [Test]
        public void CurrentDirectoryTest()
        {
            var fs = new FileSystem();
            var fullName = fs.CurrentDirectory().FullName;

            Assert.IsFalse(String.IsNullOrWhiteSpace(fullName));
            Assert.AreEqual(Environment.CurrentDirectory, fullName);
        }
    }
}
