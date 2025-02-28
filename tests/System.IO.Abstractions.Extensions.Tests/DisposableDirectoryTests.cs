using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class DisposableDirectoryTests
    {
        [Test]
        public void DisposableDirectory_Throws_ArgumentNullException_For_Null_IDirectoryInfo_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new DisposableDirectory(null));
        }

        [Test]
        public void DisposableDirectory_DeleteRecursive_On_Dispose_Test()
        {
            // Arrange
            var fs = new FileSystem();
            var path = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), fs.Path.GetRandomFileName());
            var dirInfo = fs.DirectoryInfo.New(path);

            // Create a subdirectory to ensure recursive delete
            dirInfo.CreateSubdirectory(Guid.NewGuid().ToString());

            // Assert directory exists
            Assert.IsTrue(fs.Directory.Exists(path), "Directory should exist");
            Assert.IsTrue(dirInfo.Exists, "IDirectoryInfo.Exists should be true");

            // Act
            var disposableDirectory = new DisposableDirectory(dirInfo);
            disposableDirectory.Dispose();

            // Assert directory is deleted
            Assert.IsFalse(fs.Directory.Exists(path), "Directory should not exist");
            Assert.IsFalse(dirInfo.Exists, "IDirectoryInfo.Exists should be false");

            // Assert a second dispose does not throw
            Assert.DoesNotThrow(() => disposableDirectory.Dispose());
        }
    }
}
