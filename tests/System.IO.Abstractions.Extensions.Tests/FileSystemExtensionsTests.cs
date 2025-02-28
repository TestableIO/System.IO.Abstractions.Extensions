using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class FileSystemExtensionsTests
    {
        private class CustomDisposableDirectory : DisposableDirectory
        {
            public bool DeleteFileSystemInfoWasCalled { get; private set; }

            public CustomDisposableDirectory(IDirectoryInfo directoryInfo) : base(directoryInfo)
            {
            }

            protected override void DeleteFileSystemInfo()
            {
                DeleteFileSystemInfoWasCalled = true;
                base.DeleteFileSystemInfo();
            }
        }

        private class CustomDisposableFile : DisposableFile
        {
            public bool DeleteFileSystemInfoWasCalled { get; private set; }

            public CustomDisposableFile(IFileInfo fileInfo) : base(fileInfo)
            {
            }

            protected override void DeleteFileSystemInfo()
            {
                DeleteFileSystemInfoWasCalled = true;
                base.DeleteFileSystemInfo();
            }
        }

        [Test]
        public void CurrentDirectoryTest()
        {
            var fs = new FileSystem();
            var fullName = fs.CurrentDirectory().FullName;

            Assert.IsFalse(String.IsNullOrWhiteSpace(fullName));
            NUnit.Framework.Assert.That(fullName, Is.EqualTo(Environment.CurrentDirectory));
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
        public void CreateDisposableDirectory_Custom_IDisposable_Test()
        {
            // Arrange
            var fs = new FileSystem();
            string path = null;

            // Act
            CustomDisposableDirectory customDisposable;
            using (customDisposable = fs.CreateDisposableDirectory(dir => new CustomDisposableDirectory(dir), out var dirInfo))
            {
                path = dirInfo.FullName;

                Assert.IsTrue(dirInfo.Exists, "Directory should exist");
                Assert.IsFalse(customDisposable.DeleteFileSystemInfoWasCalled, "Delete should not have been called yet");
            }

            // Assert directory is deleted
            Assert.IsNotNull(path);
            Assert.IsFalse(fs.Directory.Exists(path), "Directory should not exist");
            Assert.IsTrue(customDisposable.DeleteFileSystemInfoWasCalled, "Custom disposable delete should have been called");
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

                Assert.That(file.Exists, Is.True, "File should exist");
                Assert.IsTrue(
                    path.StartsWith(fs.Path.GetTempPath(), StringComparison.Ordinal),
                    "File should be in temp path");
            }

            // Assert file is deleted
            Assert.That(fs.File.Exists(path), Is.False, "File should not exist");
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

        [Test]
        public void CreateDisposableFile_Custom_IDisposable_Test()
        {
            // Arrange
            var fs = new FileSystem();
            string path = null;

            // Act
            CustomDisposableFile customDisposable;
            using (customDisposable = fs.CreateDisposableFile(dir => new CustomDisposableFile(dir), out var fileInfo))
            {
                path = fileInfo.FullName;

                Assert.IsTrue(fileInfo.Exists, "File should exist");
                Assert.IsFalse(customDisposable.DeleteFileSystemInfoWasCalled, "Delete should not have been called yet");
            }

            // Assert file is deleted
            Assert.IsNotNull(path);
            Assert.IsFalse(fs.File.Exists(path), "File should not exist");
            Assert.IsTrue(customDisposable.DeleteFileSystemInfoWasCalled, "Custom disposable delete should have been called");
        }
    }
}
