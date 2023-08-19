using NUnit.Framework;
using System.Collections.Generic;

namespace System.IO.Abstractions.Extensions.Tests
{
    [TestFixture]
    public class DirectoryInfoExtensionsTests
    {
        [Test]
        public void SubDirectory_Extension_Test()
        {
            //arrange
            var fs = new FileSystem();
            var current =  fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var expectedPath = fs.Path.Combine(current.FullName, guid);

            //make sure directory doesn't exists
            Assert.IsFalse(fs.Directory.Exists(expectedPath));

            //create directory
            var created = current.SubDirectory(guid);
            created.Create();

            //assert it exists
            Assert.IsTrue(fs.Directory.Exists(expectedPath));
            Assert.AreEqual(expectedPath, created.FullName);

            //delete directory
            created.Delete();
            Assert.IsFalse(fs.Directory.Exists(expectedPath));
        }

        [TestCase("test1", "test2")]
        [TestCase("test1", "", "test2")]
        [TestCase("test1", null, "test2")]
        public void SubDirectoryWithParams_Extension_Test(params string[] subFolders)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var expectedPath = fs.Path.Combine(current.FullName, "test1", "test2");

            //make sure directory doesn't exists
            Assert.IsFalse(fs.Directory.Exists(expectedPath));

            //create directory
            var created = current.SubDirectory(subFolders);
            created.Create();

            //assert it exists
            Assert.IsTrue(fs.Directory.Exists(expectedPath));
            Assert.AreEqual(expectedPath, created.FullName);

            //delete directory
            created.Delete();
            Assert.IsFalse(fs.Directory.Exists(expectedPath));
        }

        [TestCase("test1", "test2")]
        [TestCase("test1", "", "test2")]
        [TestCase("test1", null, "test2")]
        public void SubDirectoryWithIEnumerable_Extension_Test(params string[] names)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var expectedPath = fs.Path.Combine(current.FullName, "test1", "test2");

            //make sure directory doesn't exists
            Assert.IsFalse(fs.Directory.Exists(expectedPath));

            //create directory
            var list = new List<string>(names);
            var created = current.SubDirectory(list);
            created.Create();

            //assert it exists
            Assert.IsTrue(fs.Directory.Exists(expectedPath));
            Assert.AreEqual(expectedPath, created.FullName);

            //delete directory
            created.Delete();
            Assert.IsFalse(fs.Directory.Exists(expectedPath));
        }

        [Test]
        public void File_Extension_Test()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var expectedPath = fs.Path.Combine(current.FullName, guid);

            //make sure file doesn't exists
            Assert.IsFalse(fs.File.Exists(expectedPath));

            //create file
            var created = current.File(guid);
            using (var stream = created.Create())
            {
                stream.Dispose();
            }

            //assert it exists
            Assert.IsTrue(fs.File.Exists(expectedPath));
            Assert.AreEqual(expectedPath, created.FullName);

            //delete file
            created.Delete();
            Assert.IsFalse(fs.File.Exists(expectedPath));
        }

        [TestCase("test1", "test2", "test.txt")]
        [TestCase("test1", "", "test2", "test.txt")]
        [TestCase("test1", null, "test2", "test.txt")]

        public void FileWithParams_Extension_Test(params string[] names)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var expectedPath = fs.Path.Combine(current.FullName, "test1", "test2", "test.txt");

            //make sure file doesn't exists
            Assert.IsFalse(fs.File.Exists(expectedPath));

            //act, create file
            var created = current.File(names);
            created.Directory.Create();
            using (var stream = created.Create())
            {
                stream.Dispose();
            }

            //assert it exists
            Assert.IsTrue(fs.File.Exists(expectedPath));
            Assert.AreEqual(expectedPath, created.FullName);

            //delete file
            created.Delete();
            created.Directory.Delete();
            Assert.IsFalse(fs.File.Exists(expectedPath));
        }

        [Test]
        public void ThrowIfNotFound_IfDirectoryDoesNotExists_ThrowsException()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var directory = current.SubDirectory(guid);

            //act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => directory.ThrowIfNotFound());

            //assert
            Assert.IsTrue(exception.Message.Contains(directory.FullName));
        }

        [Test]
        public void ThrowIfNotFound_IfDirectoryExists_DoesNotThrowException()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var directory = current.SubDirectory(guid);

            //act
            directory.Create();
            directory.ThrowIfNotFound();

            //assert
            Assert.IsTrue(directory.Exists);

            //cleanup
            directory.Delete();
        }
    }
}
