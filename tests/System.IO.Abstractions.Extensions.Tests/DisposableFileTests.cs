using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class DisposableFileTests
    {
        [Test]
        public void DisposableFile_Throws_ArgumentNullException_For_Null_IFileInfo_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new DisposableFile(null));
        }

        [Test]
        public void DisposableFile_Delete_On_Dispose_Test()
        {
            // Arrange
            var fs = new FileSystem();
            var path = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), fs.Path.GetRandomFileName());
            var fileInfo = fs.FileInfo.New(path);
            fileInfo.Create().Dispose();

            // Assert file exists
            Assert.IsTrue(fs.File.Exists(path), "File exists");
            Assert.IsTrue(fileInfo.Exists, "IFileInfo.Exists should be true");

            // Act
            var disposableFile = new DisposableFile(fileInfo);
            disposableFile.Dispose();

            // Assert directory is deleted
            Assert.IsFalse(fs.File.Exists(path), "File does not exist");
            Assert.IsFalse(fileInfo.Exists, "IFileInfo.Exists should be false");

            // Assert a second dispose does not throw
            Assert.DoesNotThrow(() => disposableFile.Dispose());
        }
    }
}
