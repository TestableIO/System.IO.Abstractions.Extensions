﻿using NUnit.Framework;
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
                foreach (var line in content)
                    writer.WriteLine(line);
            }

            //act
            var actual = file.EnumerateLines().ToArray();

            //assert
            Assert.AreEqual(content.Length, actual.Length);
            for (int i = 0; i < content.Length; i++)
            {
                Assert.AreEqual(content[i], actual[i]);
            }
        }

        [TestCase("line1", "line2", "line3")]
        [TestCase("line1", "", "line3")]
        public void WriteLines_WriteLinesToNewFile_LinesAreWritten(params string[] content)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);

            //act
            Assert.IsFalse(file.Exists);
            file.WriteLines(content);
            var actual = file.EnumerateLines().ToArray();

            //assert
            Assert.AreEqual(content.Length, actual.Length);
            for (int i = 0; i < content.Length; i++)
            {
                Assert.AreEqual(content[i], actual[i]);
            }
        }

        [TestCase("line1", "line2", "line3")]
        [TestCase("line1", "", "line3")]
        public void WriteLines_WriteLinesToExistingFileWithOverwriteDisabled_ThrowsIOException(params string[] content)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);

            //create empty file
            using (var stream = file.OpenWrite())
            {
                stream.Dispose();
            }

            //act & assert
            Assert.IsTrue(file.Exists);
            //call WriteLines with both overwrite parameter ommitted or set to false
            var ex1 = Assert.Throws<IOException>(() => file.WriteLines(content));
            var ex2 = Assert.Throws<IOException>(() => file.WriteLines(content, false));

            Assert.IsTrue(ex1.Message.Contains(file.FullName));
            Assert.IsTrue(ex2.Message.Contains(file.FullName));
        }

        [TestCase("line1", "line2", "line3")]
        [TestCase("line1", "", "line3")]
        public void WriteLines_WriteLinesToExistingFileWithOverwriteEnabled_FileIsTruncatedAndLinesAreWritten(params string[] content)
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
            var guid = Guid.NewGuid().ToString();
            var file = current.File(guid);

            //create file with long content
            var data = Encoding.UTF8.GetBytes("line5 line4 line3 line2 line1");
            using (var stream = file.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
                stream.Dispose();
            }

            //act
            Assert.IsTrue(file.Exists);
            file.WriteLines(content, overwrite: true);
            var actual = file.EnumerateLines().ToArray();

            //assert
            Assert.AreEqual(content.Length, actual.Length);
            for (int i = 0; i < content.Length; i++)
            {
                Assert.AreEqual(content[i], actual[i]);
            }
        }
    }
}