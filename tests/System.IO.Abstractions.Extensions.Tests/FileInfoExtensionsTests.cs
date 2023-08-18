using NUnit.Framework;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class FileInfoExtensionsTests
    {
        [Test]
        public void ThrowIfNotFound_IfFileDoesNotExists_ThrowsException()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);

            //act
            var exception = Assert.Throws<FileNotFoundException>(() => file.ThrowIfNotFound());

            //assert
            Assert.IsTrue(exception.Message.Contains(file.FullName));
        }

        [Test]
        public void ThrowIfNotFound_IfFileExists_DoesNotThrowException()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);

            //act
            using (var stream = file.Create())
            {
                stream.Dispose();
            }
            file.ThrowIfNotFound();

            //assert
            Assert.IsTrue(file.Exists);

            //cleanup
            file.Delete();
        }
    }
}
