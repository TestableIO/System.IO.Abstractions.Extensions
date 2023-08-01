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

        [Test]
        public void CreateDisposableDirectory_Temp_Path_Test()
        {
            // Arrange
            var fs = new FileSystem();
            string path;

            // Act
            using (_ = fs.CreateDisposableDirectory(out var dir))
            {
                path = dir.FullName;

                Assert.IsTrue(dir.Exists, "Directory should exist");
                Assert.IsTrue(
                    path.StartsWith(fs.Path.GetTempPath(), StringComparison.Ordinal),
                    "Directory should be in temp path");
            }

            // Assert directory is deleted
            Assert.IsFalse(fs.Directory.Exists(path), "Directory should not exist");
        }

        [Test]
        public void CreateDisposableDirectory_Already_Exists_Test()
        {
            // Arrange
            var fs = new FileSystem();
            var path = fs.Path.Combine(fs.Path.GetTempPath(), fs.Path.GetRandomFileName());
            fs.Directory.CreateDirectory(path);

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => fs.CreateDisposableDirectory(path, out _));
            Assert.True(ex.Data["path"].ToString() == path, "Exception data should contain colliding path to aid with debugging");

            // Delete colliding directory
            fs.Directory.Delete(path);
            Assert.IsFalse(fs.Directory.Exists(path), "Directory should not exist");
        }

        [Test]
        public void CreateDisposableFile_Temp_Path_Test()
        {
            // Arrange
            var fs = new FileSystem();
            string path;

            // Act
            using (_ = fs.CreateDisposableFile(out var file))
            {
                path = file.FullName;

                Assert.IsTrue(file.Exists, "File should exist");
                Assert.IsTrue(
                    path.StartsWith(fs.Path.GetTempPath(), StringComparison.Ordinal),
                    "File should be in temp path");
            }

            // Assert file is deleted
            Assert.IsFalse(fs.File.Exists(path), "File should not exist");
        }

        [Test]
        public void CreateDisposableFile_Already_Exists_Test()
        {
            // Arrange
            var fs = new FileSystem();
            var path = fs.Path.Combine(fs.Path.GetTempPath(), fs.Path.GetRandomFileName());
            fs.File.Create(path).Dispose();

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => fs.CreateDisposableFile(path, out _));
            Assert.True(ex.Data["path"].ToString() == path, "Exception data should contain colliding path to aid with debugging");

            // Delete colliding file
            fs.File.Delete(path);
            Assert.IsFalse(fs.File.Exists(path), "File should not exist");
        }
    }
}
