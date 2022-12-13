using NUnit.Framework;

namespace System.IO.Abstractions.Tests
{
    [TestFixture]
    public class DirectoryInfoExtensionsTests
    {
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
            var newDest = source.CopyTo(dest.FullName);

            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(newDest.FullName, "SubDir"));
            Assert.IsTrue(fs.Directory.Exists(newDest.FullName));
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
            var newDest = source.CopyTo(dest.FullName);

            var destFile = fs.FileInfo.New(fs.Path.Combine(newDest.FullName, "file.txt"));
            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(newDest.FullName, "SubDir"));
            var destSubDirFile = fs.FileInfo.New(fs.Path.Combine(destSubDir.FullName, "file.txt"));
            Assert.IsTrue(fs.Directory.Exists(newDest.FullName));
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
            var newDest = source.CopyTo(dest.FullName, recursive: true);

            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(newDest.FullName, "SubDir"));
            Assert.IsTrue(fs.Directory.Exists(newDest.FullName));
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
            var newDest = source.CopyTo(dest.FullName, recursive: true);

            var destFile = fs.FileInfo.New(fs.Path.Combine(newDest.FullName, "file.txt"));
            var destSubDir = fs.DirectoryInfo.New(fs.Path.Combine(newDest.FullName, "SubDir"));
            var destSubDirFile = fs.FileInfo.New(fs.Path.Combine(destSubDir.FullName, "file.txt"));
            Assert.IsTrue(fs.Directory.Exists(newDest.FullName));
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
            Assert.That(() => source.CopyTo(dest.FullName), Throws.Exception.TypeOf<DirectoryNotFoundException>().And.Message.EqualTo($"Source directory not found: '{source.FullName}'"));

            Assert.IsFalse(fs.File.Exists(source.FullName));
            Assert.IsFalse(fs.File.Exists(dest.FullName));
        }

        [Test]
        public void CopyTo_TargetDirExists_ThrowsIOException()
        {
            //arrange
            var fs = new FileSystem();
            var workingDir = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory()).CreateSubdirectory(Guid.NewGuid().ToString());

            //create directories
            var source = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "SourceDir"));
            var dest = fs.DirectoryInfo.New(fs.Path.Combine(workingDir.FullName, "DestDir"));

            source.Create();
            dest.Create();

            //make sure everything is set up as expected
            Assert.IsTrue(fs.Directory.Exists(source.FullName));
            Assert.IsTrue(fs.Directory.Exists(dest.FullName));

            //act
            Assert.That(() => source.CopyTo(dest.FullName), Throws.Exception.TypeOf<IOException>().And.Message.EqualTo($"The directory '{dest.FullName}' already exists."));
            
            //cleanup
            workingDir.Delete(recursive: true);

            Assert.IsFalse(fs.File.Exists(workingDir.FullName));
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

        [Test]
        public void SubDirectory_Extension_Test()
        {
            //arrange
            var fs = new FileSystem();
            var current = fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
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
    }
}