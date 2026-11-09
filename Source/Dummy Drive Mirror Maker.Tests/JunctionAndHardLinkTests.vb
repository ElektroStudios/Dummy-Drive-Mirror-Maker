#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Text

Imports DevCase.Core.IO.Tools

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " JunctionAndHardLinkTests "

''' <summary>
''' Unit tests for the directory junction and hard link utilities
''' (<see cref="MountPointReparseData"/>, <see cref="Directories.CreateJunction"/>,
''' <see cref="Files.GetLinkInfo"/>, <see cref="Files.CreateHardLink"/>).
''' <para></para>
''' Junctions and hard links do not require any special privilege, so these tests always run.
''' </summary>
<TestClass>
Public NotInheritable Class JunctionAndHardLinkTests

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

#Region " MountPointReparseData "

    <TestMethod>
    <DataRow("C:\Target", "C:\Target")>
    <DataRow("C:\Target\", "C:\Target")>
    <DataRow("c:/Some Folder/Sub", "c:\Some Folder\Sub")>
    <DataRow("\\?\D:\Target", "D:\Target")>
    <DataRow("D:\", "D:\")>
    Public Sub MountPointReparseData_BuildThenParse_RoundTrips(targetPath As String, expectedTarget As String)
        Dim buffer As Byte() = MountPointReparseData.BuildJunctionBuffer(targetPath)
        Dim data As MountPointReparseData = MountPointReparseData.Parse(buffer, buffer.Length)

        Assert.AreEqual(expectedTarget, data.TargetPath)
        Assert.AreEqual(expectedTarget, data.PrintName)
        Assert.AreEqual($"\??\{expectedTarget}", data.SubstituteName)
        Assert.IsFalse(data.IsVolumeMountPoint)
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_BuildJunctionBuffer_HasExpectedHeader()
        Dim buffer As Byte() = MountPointReparseData.BuildJunctionBuffer("C:\T")

        Assert.AreEqual(MountPointReparseData.MountPointTag, BitConverter.ToUInt32(buffer, 0))
        Assert.AreEqual(buffer.Length - 8, CInt(BitConverter.ToUInt16(buffer, 4)), "ReparseDataLength.")
        Assert.AreEqual(0, CInt(BitConverter.ToUInt16(buffer, 8)), "SubstituteNameOffset.")
        Assert.AreEqual(Encoding.Unicode.GetByteCount("\??\C:\T"), CInt(BitConverter.ToUInt16(buffer, 10)), "SubstituteNameLength.")
        Assert.AreEqual(Encoding.Unicode.GetByteCount("C:\T"), CInt(BitConverter.ToUInt16(buffer, 14)), "PrintNameLength.")
    End Sub

    <TestMethod>
    <DataRow("Relative\Path")>
    <DataRow("\Rooted\Without\Drive")>
    <DataRow("\\Server\Share\Folder")>
    <DataRow("C:")>
    Public Sub MountPointReparseData_BuildJunctionBuffer_NotFullyQualifiedLocalPath_ThrowsArgumentException(targetPath As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MountPointReparseData.BuildJunctionBuffer(targetPath))
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_BuildJunctionBuffer_EmptyPath_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MountPointReparseData.BuildJunctionBuffer(" "))
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_BuildJunctionBuffer_TooLongPath_ThrowsArgumentException()
        Dim longPath As String = $"C:\{New String("a"c, 9000)}"
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MountPointReparseData.BuildJunctionBuffer(longPath))
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_Parse_VolumeMountPoint_IsDetected()
        ' A volume mount point has the same layout as a junction, with a volume GUID path as substitute name.
        Const volumePath As String = "\??\Volume{12345678-1234-1234-1234-123456789ABC}\"
        Dim substituteBytes As Byte() = Encoding.Unicode.GetBytes(volumePath)
        Dim buffer(16 + substituteBytes.Length + 4 - 1) As Byte
        BitConverter.GetBytes(MountPointReparseData.MountPointTag).CopyTo(buffer, 0)
        BitConverter.GetBytes(CUShort(buffer.Length - 8)).CopyTo(buffer, 4)
        BitConverter.GetBytes(CUShort(substituteBytes.Length)).CopyTo(buffer, 10)
        BitConverter.GetBytes(CUShort(substituteBytes.Length + 2)).CopyTo(buffer, 12)
        substituteBytes.CopyTo(buffer, 16)

        Dim data As MountPointReparseData = MountPointReparseData.Parse(buffer, buffer.Length)

        Assert.IsTrue(data.IsVolumeMountPoint)
        Assert.AreEqual("\\?\Volume{12345678-1234-1234-1234-123456789ABC}\", data.TargetPath)
        Assert.AreEqual(String.Empty, data.PrintName)
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_Parse_OtherReparseTag_ThrowsArgumentException()
        Dim buffer As Byte() = MountPointReparseData.BuildJunctionBuffer("C:\T")
        BitConverter.GetBytes(&HA000000CUI).CopyTo(buffer, 0) ' IO_REPARSE_TAG_SYMLINK.

        Assert.ThrowsExactly(Of ArgumentException)(Sub() MountPointReparseData.Parse(buffer, buffer.Length))
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_Parse_MalformedOrTruncatedBuffer_ThrowsArgumentException()
        Dim buffer As Byte() = MountPointReparseData.BuildJunctionBuffer("C:\Target")

        Assert.ThrowsExactly(Of ArgumentException)(Sub() MountPointReparseData.Parse(buffer, 10))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MountPointReparseData.Parse(buffer, 20))
    End Sub

    <TestMethod>
    Public Sub MountPointReparseData_Parse_InvalidArguments_Throws()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MountPointReparseData.Parse(Nothing, 0))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() MountPointReparseData.Parse(New Byte(15) {}, 17))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() MountPointReparseData.Parse(New Byte(15) {}, -1))
    End Sub

#End Region

#Region " Directory Junctions "

    <TestMethod>
    Public Sub CreateJunction_ValidPaths_CreatesJunctionToTarget()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Me.tempDir.CreateFile("Target\Inner.txt", "inner")
        Dim junctionPath As String = Me.tempDir.Combine("My Junction")

        Directories.CreateJunction(junctionPath, targetPath)

        Assert.IsTrue(Directory.Exists(junctionPath))
        Assert.IsTrue(Directories.IsMountPoint(junctionPath))
        Assert.IsTrue(Directories.IsJunction(junctionPath))
        Assert.IsFalse(Directories.IsSymbolicLink(junctionPath))
        Assert.AreEqual(targetPath, Directories.GetMountPointTarget(junctionPath))
        Assert.AreEqual("inner", File.ReadAllText(Path.Combine(junctionPath, "Inner.txt")))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_LongPathPrefix_CreatesJunction()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = MirrorRules.AddLongPathPrefix(Me.tempDir.Combine("Junction"))

        Directories.CreateJunction(junctionPath, targetPath)

        Assert.IsTrue(Directories.IsJunction(Me.tempDir.Combine("Junction")))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_NonExistentTarget_CreatesDanglingJunction()
        Dim targetPath As String = Me.tempDir.Combine("Missing Target")
        Dim junctionPath As String = Me.tempDir.Combine("Dangling")

        Directories.CreateJunction(junctionPath, targetPath)

        Assert.IsTrue(Directories.IsJunction(junctionPath))
        Assert.AreEqual(targetPath, Directories.GetMountPointTarget(junctionPath))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_ExistingPath_ThrowsIOExceptionAndKeepsExistingDirectory()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim existingPath As String = Me.tempDir.CreateDirectory("Existing")
        Me.tempDir.CreateFile("Existing\Keep.txt")

        Assert.ThrowsExactly(Of IOException)(Sub() Directories.CreateJunction(existingPath, targetPath))
        Assert.IsTrue(File.Exists(Path.Combine(existingPath, "Keep.txt")))
        Assert.IsFalse(Directories.IsMountPoint(existingPath))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_InvalidTarget_ThrowsArgumentExceptionAndCreatesNothing()
        Dim junctionPath As String = Me.tempDir.Combine("Junction")

        Assert.ThrowsExactly(Of ArgumentException)(Sub() Directories.CreateJunction(junctionPath, "Relative\Target"))
        Assert.IsFalse(Directory.Exists(junctionPath))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_EmptyJunctionPath_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Directories.CreateJunction("", "C:\Target"))
    End Sub

    <TestMethod>
    Public Sub IsJunction_RegularDirectory_ReturnsFalse()
        Dim dirPath As String = Me.tempDir.CreateDirectory("Regular")

        Assert.IsFalse(Directories.IsJunction(dirPath))
    End Sub

    <TestMethod>
    Public Sub IsJunction_MissingDirectory_ThrowsDirectoryNotFoundException()
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() Directories.IsJunction(Me.tempDir.Combine("Missing")))
    End Sub

    <TestMethod>
    Public Sub GetMountPointTarget_RegularDirectory_ThrowsWin32Exception()
        Dim dirPath As String = Me.tempDir.CreateDirectory("Regular")

        ' ERROR_NOT_A_REPARSE_POINT.
        Dim ex As Win32Exception = Assert.ThrowsExactly(Of Win32Exception)(Sub() Directories.GetMountPointTarget(dirPath))
        Assert.AreEqual(4390, ex.NativeErrorCode)
    End Sub

    <TestMethod>
    Public Sub GetMountPointTarget_JunctionCreatedByMklink_ReturnsTarget()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        LinkFactory.CreateJunction(junctionPath, targetPath)

        Assert.IsTrue(Directories.IsJunction(junctionPath))
        Assert.AreEqual(targetPath, Directories.GetMountPointTarget(junctionPath))
    End Sub

    <TestMethod>
    Public Sub CreateJunction_ReplicatedJunction_PointsToSameTarget()
        ' Simulates the mirror operation of a directory junction:
        ' the replica must have the same name as the original junction and point to the same target.
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Me.tempDir.CreateDirectory("Source")
        Dim originalJunction As String = Me.tempDir.Combine("Source\Original Junction")
        Directories.CreateJunction(originalJunction, targetPath)

        Me.tempDir.CreateDirectory("Mirror")
        Dim replica As String = MirrorRules.BuildDestinationPath(Me.tempDir.Combine("Source"), Me.tempDir.Combine("Mirror"), originalJunction)
        Directories.CreateJunction(replica, Directories.GetMountPointTarget(originalJunction))

        Dim replicaPlainPath As String = Me.tempDir.Combine("Mirror\Original Junction")
        Assert.IsTrue(Directories.IsJunction(replicaPlainPath))
        Assert.AreEqual(targetPath, Directories.GetMountPointTarget(replicaPlainPath))
    End Sub

#End Region

#Region " Hard Links "

    <TestMethod>
    Public Sub GetLinkInfo_RegularFile_HasOneLink()
        Dim filePath As String = Me.tempDir.CreateFile("File.txt")

        Dim info As FileLinkInfo = Files.GetLinkInfo(filePath)

        Assert.AreEqual(1, info.LinkCount)
        Assert.AreNotEqual(0UL, info.FileIndex)
    End Sub

    <TestMethod>
    Public Sub CreateHardLink_ValidPaths_BothNamesAreTheSameFile()
        Dim filePath As String = Me.tempDir.CreateFile("File.txt", "shared")
        Dim linkPath As String = Me.tempDir.Combine("Other Name.txt")

        Files.CreateHardLink(linkPath, filePath)

        Dim fileInfo As FileLinkInfo = Files.GetLinkInfo(filePath)
        Dim linkInfo As FileLinkInfo = Files.GetLinkInfo(linkPath)
        Assert.AreEqual(2, fileInfo.LinkCount)
        Assert.AreEqual(2, linkInfo.LinkCount)
        Assert.AreEqual(fileInfo, linkInfo, "Both names must identify the same file.")
        Assert.AreEqual(fileInfo.GetHashCode(), linkInfo.GetHashCode())
        Assert.AreEqual("shared", File.ReadAllText(linkPath))
    End Sub

    <TestMethod>
    Public Sub CreateHardLink_LongPathPrefix_CreatesLink()
        Dim filePath As String = Me.tempDir.CreateFile("File.txt")
        Dim linkPath As String = Me.tempDir.Combine("Link.txt")

        Files.CreateHardLink(MirrorRules.AddLongPathPrefix(linkPath), MirrorRules.AddLongPathPrefix(filePath))

        Assert.AreEqual(Files.GetLinkInfo(filePath), Files.GetLinkInfo(MirrorRules.AddLongPathPrefix(linkPath)))
    End Sub

    <TestMethod>
    Public Sub GetLinkInfo_DifferentFiles_AreNotEqual()
        Dim info1 As FileLinkInfo = Files.GetLinkInfo(Me.tempDir.CreateFile("File1.txt"))
        Dim info2 As FileLinkInfo = Files.GetLinkInfo(Me.tempDir.CreateFile("File2.txt"))

        Assert.AreNotEqual(info1, info2)
    End Sub

    <TestMethod>
    Public Sub GetLinkInfo_MissingFile_ThrowsFileNotFoundException()
        Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Files.GetLinkInfo(Me.tempDir.Combine("Missing.txt")))
    End Sub

    <TestMethod>
    Public Sub CreateHardLink_ExistingLinkPath_ThrowsIOException()
        Dim filePath As String = Me.tempDir.CreateFile("File.txt")
        Dim existingPath As String = Me.tempDir.CreateFile("Existing.txt", "keep")

        Assert.ThrowsExactly(Of IOException)(Sub() Files.CreateHardLink(existingPath, filePath))
        Assert.AreEqual("keep", File.ReadAllText(existingPath))
    End Sub

    <TestMethod>
    Public Sub CreateHardLink_MissingExistingFile_ThrowsFileNotFoundException()
        Dim linkPath As String = Me.tempDir.Combine("Link.txt")

        Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Files.CreateHardLink(linkPath, Me.tempDir.Combine("Missing.txt")))
    End Sub

    <TestMethod>
    Public Sub CreateHardLink_EmptyArguments_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.CreateHardLink("", "C:\File.txt"))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.CreateHardLink("C:\Link.txt", " "))
    End Sub

    <TestMethod>
    Public Sub FileLinkInfo_Equality_IgnoresLinkCount()
        Dim a As New FileLinkInfo(1UI, 42UL, 1)
        Dim b As New FileLinkInfo(1UI, 42UL, 3)
        Dim c As New FileLinkInfo(2UI, 42UL, 1)

        Assert.IsTrue(a.Equals(b))
        Assert.IsTrue(a.Equals(CObj(b)))
        Assert.IsFalse(a.Equals(c))
        Assert.IsFalse(a.Equals(CObj("other")))
    End Sub

#End Region

End Class

#End Region
