using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        [TestCase("line1", "line2", "line3")]
        [TestCase("line1", "", "line3")]
        public void EnumerateLines_ReadFromExistingFile_ReturnsLines(params string[] content)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);
            //create file
            using (var stream = file.OpenWrite())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                foreach(var line in content)
                    writer.WriteLine(line);
            }

            //act
            var actual = file.EnumerateLines().ToArray();

            //assert
            for(int i=0; i<content.Length; i++)
            {
                Assert.AreEqual(content[i], actual[i]);
            }
        }
    }
}
