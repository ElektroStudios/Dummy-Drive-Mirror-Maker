#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

Imports DevCase.Core.IO.Tools
Imports DevCase.Interop.Win32.Enums

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " ReparsePointTests "

''' <summary>
''' Unit tests for the symbolic link and mount point (junction) utilities
''' of <see cref="Files"/> and <see cref="Directories"/>.
''' </summary>
''' <remarks>
''' The tests that create symbolic links are marked as inconclusive when the current user
''' does not hold the privilege to create them (not elevated and Windows Developer Mode disabled).
''' </remarks>
<TestClass>
Public NotInheritable Class ReparsePointTests

#Region " Fields "

    ''' <summary>
    ''' The temporary directory used by the current test.
    ''' </summary>
    Private tempDir As TemporaryDirectory

#End Region

#Region " Initialization / Cleanup "

    <TestInitialize>
    Public Sub TestInitialize()
        Me.tempDir = New TemporaryDirectory()
    End Sub

    <TestCleanup>
    Public Sub TestCleanup()
        Me.tempDir.Dispose()
    End Sub

#End Region

#Region " Regular Files and Directories "

    <TestMethod>
    Public Sub DirectoriesIsSymbolicLink_RegularDirectory_ReturnsFalse()
        Dim dirPath As String = Me.tempDir.CreateDirectory("Regular")

        Assert.IsFalse(Directories.IsSymbolicLink(dirPath))
        Assert.IsFalse(Directories.IsSymbolicLink(New DirectoryInfo(dirPath)))
    End Sub

    <TestMethod>
    Public Sub DirectoriesIsMountPoint_RegularDirectory_ReturnsFalse()
        Dim dirPath As String = Me.tempDir.CreateDirectory("Regular")

        Assert.IsFalse(Directories.IsMountPoint(dirPath))
        Assert.IsFalse(Directories.IsMountPoint(New DirectoryInfo(dirPath)))
    End Sub

    <TestMethod>
    Public Sub FilesIsSymbolicLink_RegularFile_ReturnsFalse()
        Dim filePath As String = Me.tempDir.CreateFile("Regular.txt")

        Assert.IsFalse(Files.IsSymbolicLink(filePath))
        Assert.IsFalse(Files.IsSymbolicLink(New FileInfo(filePath)))
    End Sub

    <TestMethod>
    Public Sub FilesIsSymbolicLink_RegularFileWithLongPathPrefix_ReturnsFalse()
        Dim filePath As String = Me.tempDir.CreateFile("Regular.txt")

        Assert.IsFalse(Files.IsSymbolicLink(MirrorRules.AddLongPathPrefix(filePath)))
    End Sub

    <TestMethod>
    Public Sub DirectoriesIsSymbolicLink_MissingDirectory_ThrowsDirectoryNotFoundException()
        Dim missingPath As String = Me.tempDir.Combine("Missing")

        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() Directories.IsSymbolicLink(missingPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesIsMountPoint_MissingDirectory_ThrowsDirectoryNotFoundException()
        Dim missingPath As String = Me.tempDir.Combine("Missing")

        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() Directories.IsMountPoint(missingPath))
    End Sub

    <TestMethod>
    Public Sub FilesIsSymbolicLink_MissingFile_ThrowsFileNotFoundException()
        Dim missingPath As String = Me.tempDir.Combine("Missing.txt")

        Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Files.IsSymbolicLink(missingPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetSymbolicLinkDestination_MissingDirectory_ThrowsDirectoryNotFoundException()
        Dim missingPath As String = Me.tempDir.Combine("Missing")

        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() Directories.GetSymbolicLinkDestination(missingPath))
    End Sub

    <TestMethod>
    Public Sub FilesGetSymbolicLinkDestination_MissingFile_ThrowsFileNotFoundException()
        Dim missingPath As String = Me.tempDir.Combine("Missing.txt")

        Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Files.GetSymbolicLinkDestination(missingPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetSymbolicLinkDestination_RegularDirectory_ThrowsNotAReparsePointError()
        Dim dirPath As String = Me.tempDir.CreateDirectory("Regular")

        Dim ex As Win32Exception = Assert.ThrowsExactly(Of Win32Exception)(Sub() Directories.GetSymbolicLinkDestination(dirPath))
        Assert.AreEqual(CInt(Win32ErrorCode.ERROR_NOT_A_REPARSE_POINT), ex.NativeErrorCode)
    End Sub

    <TestMethod>
    Public Sub FilesGetSymbolicLinkDestination_RegularFile_ThrowsNotAReparsePointError()
        Dim filePath As String = Me.tempDir.CreateFile("Regular.txt")

        Dim ex As Win32Exception = Assert.ThrowsExactly(Of Win32Exception)(Sub() Files.GetSymbolicLinkDestination(filePath))
        Assert.AreEqual(CInt(Win32ErrorCode.ERROR_NOT_A_REPARSE_POINT), ex.NativeErrorCode)
    End Sub

#End Region

#Region " Junctions (Mount Points) "

    <TestMethod>
    Public Sub DirectoriesIsMountPoint_Junction_ReturnsTrue()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        LinkFactory.CreateJunction(junctionPath, targetPath)

        Assert.IsTrue(Directories.IsMountPoint(junctionPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesIsSymbolicLink_Junction_ReturnsFalse()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        LinkFactory.CreateJunction(junctionPath, targetPath)

        Assert.IsFalse(Directories.IsSymbolicLink(junctionPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetSymbolicLinkDestination_Junction_ThrowsArgumentException()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        LinkFactory.CreateJunction(junctionPath, targetPath)

        Assert.ThrowsExactly(Of ArgumentException)(Sub() Directories.GetSymbolicLinkDestination(junctionPath))
    End Sub

#End Region

#Region " CreateSymbolicLinkAt - Argument Validation "

    <TestMethod>
    <DataRow("")>
    <DataRow("  ")>
    Public Sub CreateSymbolicLinkAt_EmptyLinkPath_ThrowsArgumentNullException(linkPath As String)
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Directories.CreateSymbolicLinkAt(linkPath, targetPath))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.CreateSymbolicLinkAt(linkPath, targetPath))
    End Sub

    <TestMethod>
    Public Sub CreateSymbolicLinkAt_NullTargetPath_ThrowsArgumentNullException()
        Dim linkPath As String = Me.tempDir.Combine("Link")

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Directories.CreateSymbolicLinkAt(linkPath, Nothing))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.CreateSymbolicLinkAt(linkPath, Nothing))
    End Sub

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLinkAt_ExistingDirectoryAtLinkPath_ThrowsIOException()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim existingPath As String = Me.tempDir.CreateDirectory("Existing")

        Assert.ThrowsExactly(Of IOException)(Sub() Directories.CreateSymbolicLinkAt(existingPath, targetPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLinkAt_ExistingFileAtLinkPath_ThrowsIOException()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim existingPath As String = Me.tempDir.CreateFile("Existing")

        Assert.ThrowsExactly(Of IOException)(Sub() Directories.CreateSymbolicLinkAt(existingPath, targetPath))
    End Sub

    <TestMethod>
    Public Sub FilesCreateSymbolicLinkAt_ExistingFileAtLinkPath_ThrowsIOException()
        Dim targetPath As String = Me.tempDir.CreateFile("Target.txt")
        Dim existingPath As String = Me.tempDir.CreateFile("Existing.txt")

        Assert.ThrowsExactly(Of IOException)(Sub() Files.CreateSymbolicLinkAt(existingPath, targetPath))
        Assert.AreEqual("content", File.ReadAllText(existingPath), "The existing file must not be modified.")
    End Sub

    <TestMethod>
    Public Sub FilesCreateSymbolicLink_LinkWithSameNameAlreadyInDestination_ThrowsIOException()
        Dim srcFile As String = Me.tempDir.CreateFile("Source\File.txt")
        Dim dstDir As String = Me.tempDir.CreateDirectory("Destination")
        Me.tempDir.CreateFile("Destination\File.txt")

        Assert.ThrowsExactly(Of IOException)(Sub() Files.CreateSymbolicLink(srcFile, dstDir))
    End Sub

#End Region

#Region " Directory Symbolic Links "

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLinkAt_ValidPaths_CreatesDirectorySymbolicLink()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Me.tempDir.CreateFile("Target\Inner.txt", "inner")
        Dim linkPath As String = Me.tempDir.Combine("Link")

        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=True)

        Assert.IsTrue(Directory.Exists(linkPath))
        Assert.IsTrue(Directories.IsSymbolicLink(linkPath))
        Assert.IsFalse(Directories.IsMountPoint(linkPath))
        Assert.AreEqual("inner", File.ReadAllText(Path.Combine(linkPath, "Inner.txt")))
    End Sub

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLinkAt_LinkNameDifferentFromTarget_KeepsLinkName()
        ' Regression test: mirroring a directory symbolic link created the new link with the name of its target.
        Dim targetPath As String = Me.tempDir.CreateDirectory("Real Target Name")
        Dim mirrorDir As String = Me.tempDir.CreateDirectory("Mirror")
        Dim linkPath As String = Path.Combine(mirrorDir, "My Link")

        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=True)

        CollectionAssert.AreEqual({"My Link"}, Directory.GetDirectories(mirrorDir).Select(Function(p As String) Path.GetFileName(p)).ToArray())
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetSymbolicLinkDestination_AbsoluteLink_ReturnsTargetPath()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim linkPath As String = Me.tempDir.Combine("Link")
        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=True)

        Assert.AreEqual(targetPath, Directories.GetSymbolicLinkDestination(linkPath))
        Assert.AreEqual(targetPath, Directories.GetSymbolicLinkDestination(New DirectoryInfo(linkPath)).FullName)
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetSymbolicLinkDestination_RelativeLink_ReturnsRelativeTargetPath()
        Me.tempDir.CreateDirectory("Target")
        Dim linkPath As String = Me.tempDir.Combine("Links\Link")
        Me.tempDir.CreateDirectory("Links")
        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, "..\Target", isDirectory:=True)

        Assert.AreEqual("..\Target", Directories.GetSymbolicLinkDestination(linkPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLinkAt_ReplicatedLink_PointsToSameTarget()
        ' Simulates the mirror operation of a directory symbolic link:
        ' the replica must have the same name as the original link and point to the same target.
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim originalLink As String = Me.tempDir.Combine("Source\Original Link")
        Me.tempDir.CreateDirectory("Source")
        LinkFactory.CreateSymbolicLinkOrInconclusive(originalLink, targetPath, isDirectory:=True)

        Dim replicaLink As String = MirrorRules.BuildDestinationPath(Me.tempDir.Combine("Source"), Me.tempDir.Combine("Mirror"), originalLink)
        Directory.CreateDirectory(Me.tempDir.Combine("Mirror"))
        Directories.CreateSymbolicLinkAt(replicaLink, Directories.GetSymbolicLinkDestination(originalLink))

        Dim replicaPlainPath As String = Me.tempDir.Combine("Mirror\Original Link")
        Assert.IsTrue(Directories.IsSymbolicLink(replicaPlainPath))
        Assert.AreEqual(targetPath, Directories.GetSymbolicLinkDestination(replicaPlainPath))
    End Sub

    <TestMethod>
    Public Sub DirectoriesCreateSymbolicLink_SourceAndDestinationDirectory_CreatesLinkNamedAfterSource()
        Dim srcDir As String = Me.tempDir.CreateDirectory("Source Folder")
        Dim dstDir As String = Me.tempDir.CreateDirectory("Destination")

        Try
            Directories.CreateSymbolicLink(New DirectoryInfo(srcDir), New DirectoryInfo(dstDir))
        Catch ex As Win32Exception When ex.NativeErrorCode = Win32ErrorCode.ERROR_PRIVILEGE_NOT_HELD
            Assert.Inconclusive("The current user does not hold the privilege to create symbolic links.")
        End Try

        Dim linkPath As String = Path.Combine(dstDir, "Source Folder")
        Assert.IsTrue(Directories.IsSymbolicLink(linkPath))
        Assert.AreEqual(srcDir, Directories.GetSymbolicLinkDestination(linkPath))
    End Sub

#End Region

#Region " File Symbolic Links "

    <TestMethod>
    Public Sub FilesCreateSymbolicLinkAt_ValidPaths_CreatesFileSymbolicLink()
        Dim targetPath As String = Me.tempDir.CreateFile("Target.txt", "target content")
        Dim linkPath As String = Me.tempDir.Combine("Link.txt")

        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=False)

        Assert.IsTrue(File.Exists(linkPath))
        Assert.IsTrue(Files.IsSymbolicLink(linkPath))
        Assert.AreEqual("target content", File.ReadAllText(linkPath))
    End Sub

    <TestMethod>
    Public Sub FilesGetSymbolicLinkDestination_AbsoluteLink_ReturnsTargetPath()
        Dim targetPath As String = Me.tempDir.CreateFile("Target.txt")
        Dim linkPath As String = Me.tempDir.Combine("Link.txt")
        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=False)

        Assert.AreEqual(targetPath, Files.GetSymbolicLinkDestination(linkPath))
        Assert.AreEqual(targetPath, Files.GetSymbolicLinkDestination(New FileInfo(linkPath)).FullName)
    End Sub

    <TestMethod>
    Public Sub FilesCreateSymbolicLinkAt_ReplicatedLink_PointsToOriginalTargetInsteadOfOriginalLink()
        ' Regression test: mirroring a file symbolic link created a link that pointed to the source link itself
        ' (a chain of links that breaks when the source drive is not available), instead of to the original target.
        Dim targetPath As String = Me.tempDir.CreateFile("Target.txt")
        Dim originalLink As String = Me.tempDir.Combine("Source\Link.txt")
        Me.tempDir.CreateDirectory("Source")
        LinkFactory.CreateSymbolicLinkOrInconclusive(originalLink, targetPath, isDirectory:=False)

        Dim replicaLink As String = Me.tempDir.Combine("Mirror\Link.txt")
        Me.tempDir.CreateDirectory("Mirror")
        Files.CreateSymbolicLinkAt(MirrorRules.AddLongPathPrefix(replicaLink), Files.GetSymbolicLinkDestination(originalLink))

        Assert.IsTrue(Files.IsSymbolicLink(replicaLink))
        Assert.AreEqual(targetPath, Files.GetSymbolicLinkDestination(replicaLink))
    End Sub

    <TestMethod>
    Public Sub FilesCreateSymbolicLinkAt_NonExistentTarget_CreatesDanglingLink()
        Dim targetPath As String = Me.tempDir.Combine("DoesNotExist.txt")
        Dim linkPath As String = Me.tempDir.Combine("Dangling.txt")

        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=False)

        Assert.IsTrue(Files.IsSymbolicLink(linkPath))
        Assert.AreEqual(targetPath, Files.GetSymbolicLinkDestination(linkPath))
    End Sub

#End Region

End Class

#End Region
