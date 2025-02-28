using NUnit.Framework;
using System.Collections.Generic;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

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

        [Test]
        public void CopyTo_NonRecursiveWithSubfolder_DoesNotCopySubfolder()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var sourceSubDir = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir", "SubDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            sourceSubDir.Create();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.Directory.Exists(sourceSubDir.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            source.CopyTo(dest);

            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(dest.FullName, "SubDir"));
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsFalse(fs.Directory.Exists(destSubDir.FullName));

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_NonRecursiveWithSubfolderWithFiles_DoesNotCopySubfolderAndSubfolderFiles()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var sourceSubDir = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir", "SubDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            sourceSubDir.Create();

            //create files
            var sourceFile = fs.FileInfo.New(fs.Path.Combine(source.FullName, "file.txt"));
            var sourceSubDirFile = fs.FileInfo.New(fs.Path.Combine(sourceSubDir.FullName, "file.txt"));

            sourceFile.Create().Dispose();
            sourceSubDirFile.Create().Dispose();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.Directory.Exists(sourceSubDir.FullName));
            Assert.IsTrue(fs.File.Exists(sourceFile.FullName));
            Assert.IsTrue(fs.File.Exists(sourceSubDirFile.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            source.CopyTo(dest);

            var destFile = fs.FileInfo.New(fs.Path.Combine(dest.FullName, "file.txt"));
            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(dest.FullName, "SubDir"));
            var destSubDirFile = fs.FileInfo.New(fs.Path.Combine(destSubDir.FullName, "file.txt"));
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsTrue(fs.File.Exists(destFile.FullName));
            Assert.IsFalse(fs.Directory.Exists(destSubDir.FullName));
            Assert.IsFalse(fs.File.Exists(destSubDirFile.FullName));

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_RecursiveWithSubfolder_DoesCopySubfolder()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var sourceSubDir = fs.DirectoryInfo.New(fs.Path.Combine(source.FullName, "SubDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            sourceSubDir.Create();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.Directory.Exists(sourceSubDir.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            source.CopyTo(dest, recursive: true);

            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(dest.FullName, "SubDir"));
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsTrue(fs.Directory.Exists(destSubDir.FullName));

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_RecursiveWithSubfolderWithFiles_DoesCopySubfolderAndSubfolderFiles()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var sourceSubDir = fs.DirectoryInfo.New(fs.Path.Combine(source.FullName, "SubDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            sourceSubDir.Create();

            //create files
            var sourceFile = fs.FileInfo.New(fs.Path.Combine(source.FullName, "file.txt"));
            var sourceSubDirFile = fs.FileInfo.New(fs.Path.Combine(sourceSubDir.FullName, "file.txt"));

            sourceFile.Create().Dispose();
            sourceSubDirFile.Create().Dispose();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.Directory.Exists(sourceSubDir.FullName));
            Assert.IsTrue(fs.File.Exists(sourceFile.FullName));
            Assert.IsTrue(fs.File.Exists(sourceSubDirFile.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            source.CopyTo(dest, recursive: true);

            var destFile = fs.FileInfo.New(fs.Path.Combine(dest.FullName, "file.txt"));
            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(dest.FullName, "SubDir"));
            var destSubDirFile = fs.FileInfo.New(fs.Path.Combine(destSubDir.FullName, "file.txt"));
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsTrue(fs.File.Exists(destFile.FullName));
            Assert.IsTrue(fs.Directory.Exists(destSubDir.FullName));
            Assert.IsTrue(fs.File.Exists(destSubDirFile.FullName));

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_SourceDirDoesNotExists_ThrowsDirectoryNotFoundException()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            //make sure everything is set up as expected
            Assert.IsFalse(fs.Directory.Exists(source.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            Assert.That(() => source.CopyTo(dest), Throws.Exception.TypeOf<DirectoryNotFoundException>().And.Message.Contains(source.FullName));

            Assert.IsFalse(fs.File.Exists(source.FullName));
            Assert.IsFalse(fs.File.Exists(dest.FullName));
        }

        [Test]
        public void CopyTo_TargetDirAndParentDoesNotExist_CreatesTargetDirectoryHierarchy()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "ParentDir", "DestDir"));

            source.Create();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsFalse(fs.Directory.Exists(dest.FullName));

            //act
            source.CopyTo(dest);

            //assert
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_Overwrite_OverwritesWhenSet()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            dest.Create();

            //create files
            var sourceFile = fs.FileInfo.New(fs.Path.Combine(source.FullName, "file.txt"));
            var destFile = fs.FileInfo.New(fs.Path.Combine(dest.FullName, "file.txt"));

            var sourceFileContent = new[] { nameof(sourceFile) };
            sourceFile.WriteLines(sourceFileContent);
            var destFileContent = new[] { nameof(destFile) };
            destFile.WriteLines(destFileContent);

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.File.Exists(sourceFile.FullName));
            Assert.AreEqual(fs.File.ReadAllLines(sourceFile.FullName), sourceFileContent);
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsTrue(fs.File.Exists(destFile.FullName));
            Assert.AreEqual(fs.File.ReadAllLines(destFile.FullName), destFileContent);

            //act
            source.CopyTo(dest, overwrite: true);

            //assert
            Assert.AreEqual(fs.File.ReadAllLines(destFile.FullName), sourceFileContent);

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }

        [Test]
        public void CopyTo_Overwrite_DoesNotOverwritesWhenNotSet()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            dest.Create();

            //create files
            var sourceFile = fs.FileInfo.New(fs.Path.Combine(source.FullName, "file.txt"));
            var destFile = fs.FileInfo.New(fs.Path.Combine(dest.FullName, "file.txt"));

            var sourceFileContent = new[] { nameof(sourceFile) };
            sourceFile.WriteLines(sourceFileContent);
            var destFileContent = new[] { nameof(destFile) };
            destFile.WriteLines(destFileContent);

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.File.Exists(sourceFile.FullName));
            Assert.AreEqual(fs.File.ReadAllLines(sourceFile.FullName), sourceFileContent);
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));
            Assert.IsTrue(fs.File.Exists(destFile.FullName));
            Assert.AreEqual(fs.File.ReadAllLines(destFile.FullName), destFileContent);

            //act
            Assert.That(() => source.CopyTo(dest, overwrite: false), Throws.Exception.TypeOf<IOException>().And.Message.Contains(destFile.FullName));

            //assert
            Assert.AreEqual(fs.File.ReadAllLines(destFile.FullName), destFileContent);

            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
        }
    }
}
