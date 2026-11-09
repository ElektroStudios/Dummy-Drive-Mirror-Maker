#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Runtime.InteropServices

Imports Microsoft.Win32.SafeHandles

Imports DevCase.Core.IO.Tools
Imports DevCase.Interop.Win32

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " TimestampTests "

''' <summary>
''' Unit tests for <see cref="Files.SetTimestampsUtc"/> and <see cref="Main.ApplyDirectoryTimestamps"/>.
''' </summary>
<TestClass>
Public NotInheritable Class TimestampTests

#Region " Fields "

    ''' <summary>
    ''' The temporary directory used by the current test.
    ''' </summary>
    Private tempDir As TemporaryDirectory

    ''' <summary>The creation time used by the tests.</summary>
    Private Shared ReadOnly CreationUtc As New Date(2001, 2, 3, 4, 5, 6, DateTimeKind.Utc)

    ''' <summary>The last access time used by the tests.</summary>
    Private Shared ReadOnly LastAccessUtc As New Date(2002, 3, 4, 5, 6, 7, DateTimeKind.Utc)

    ''' <summary>The last write time used by the tests.</summary>
    Private Shared ReadOnly LastWriteUtc As New Date(2003, 4, 5, 6, 7, 8, DateTimeKind.Utc)

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

#Region " Helpers "

    ''' <summary>
    ''' Opens a directory the same way Windows Explorer does to watch its changes:
    ''' with <c>FILE_LIST_DIRECTORY</c> (read) access, sharing read, write and delete access.
    ''' </summary>
    ''' <param name="directoryPath">
    ''' The directory path.
    ''' </param>
    ''' <returns>
    ''' The directory handle.
    ''' </returns>
    Private Shared Function OpenDirectoryLikeExplorer(directoryPath As String) As SafeFileHandle
        Const FileListDirectory As UInteger = &H1UI
        Const ShareAll As UInteger = &H7UI
        Const OpenExisting As UInteger = 3UI
        Const BackupSemantics As UInteger = &H2000000UI

        Dim handle As SafeFileHandle = NativeMethods.CreateFileW(directoryPath, FileListDirectory, ShareAll, IntPtr.Zero,
                                                                 OpenExisting, BackupSemantics, IntPtr.Zero)
        If handle.IsInvalid Then
            Dim win32Error As Integer = Marshal.GetLastWin32Error()
            handle.Dispose()
            Assert.Inconclusive($"The directory could not be opened (Win32 error {win32Error}).")
        End If
        Return handle
    End Function

#End Region

#Region " SetTimestampsUtc "

    <TestMethod>
    Public Sub SetTimestampsUtc_File_SetsTheThreeTimestamps()
        Dim filePath As String = Me.tempDir.CreateFile("File.txt")

        Files.SetTimestampsUtc(filePath, CreationUtc, LastAccessUtc, LastWriteUtc)

        Assert.AreEqual(CreationUtc, File.GetCreationTimeUtc(filePath))
        Assert.AreEqual(LastAccessUtc, File.GetLastAccessTimeUtc(filePath))
        Assert.AreEqual(LastWriteUtc, File.GetLastWriteTimeUtc(filePath))
    End Sub

    <TestMethod>
    Public Sub SetTimestampsUtc_Directory_SetsTheThreeTimestamps()
        Dim directoryPath As String = Me.tempDir.CreateDirectory("Directory")

        Files.SetTimestampsUtc(directoryPath, CreationUtc, LastAccessUtc, LastWriteUtc)

        Assert.AreEqual(CreationUtc, Directory.GetCreationTimeUtc(directoryPath))
        Assert.AreEqual(LastWriteUtc, Directory.GetLastWriteTimeUtc(directoryPath))
    End Sub

    <TestMethod>
    Public Sub SetTimestampsUtc_DirectoryOpenedByAnotherProcessLikeExplorer_DoesNotThrow()
        ' Regression test: with Windows Explorer showing the destination directory, the mirror operation failed with
        ' "The process cannot access the file because it is being used by another process".
        Dim directoryPath As String = Me.tempDir.CreateDirectory("Watched")

        Using explorerHandle As SafeFileHandle = TimestampTests.OpenDirectoryLikeExplorer(directoryPath)
            Files.SetTimestampsUtc(directoryPath, CreationUtc, LastAccessUtc, LastWriteUtc)
        End Using

        Assert.AreEqual(LastWriteUtc, Directory.GetLastWriteTimeUtc(directoryPath))
    End Sub

    <TestMethod>
    Public Sub NetFrameworkSetCreationTime_DirectoryOpenedLikeExplorer_FailsWithSharingViolation()
        ' Documents the root cause of the bug: .NET Framework opens the directory with write access
        ' but without sharing read access, which conflicts with the handle opened by Windows Explorer.
        Dim directoryPath As String = Me.tempDir.CreateDirectory("Watched")

        Using explorerHandle As SafeFileHandle = TimestampTests.OpenDirectoryLikeExplorer(directoryPath)
            Assert.ThrowsExactly(Of IOException)(Sub() Directory.SetCreationTimeUtc(directoryPath, CreationUtc))
        End Using
    End Sub

    <TestMethod>
    Public Sub SetTimestampsUtc_InvalidArguments_Throw()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.SetTimestampsUtc(Nothing, CreationUtc, LastAccessUtc, LastWriteUtc))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Files.SetTimestampsUtc(" ", CreationUtc, LastAccessUtc, LastWriteUtc))
        Assert.ThrowsExactly(Of FileNotFoundException)(
            Sub() Files.SetTimestampsUtc(Me.tempDir.Combine("Missing.txt"), CreationUtc, LastAccessUtc, LastWriteUtc))
    End Sub

#End Region

#Region " ApplyDirectoryTimestamps "

    <TestMethod>
    Public Sub ApplyDirectoryTimestamps_ParentKeepsItsTimestampsAfterItsChildrenAreCreated()
        ' Creating a subdirectory or a file inside a directory modifies its last write time,
        ' so the timestamps are applied at the end, deepest directories first.
        Dim source As String = Me.tempDir.CreateDirectory("Source")
        Dim sourceChild As String = Me.tempDir.CreateDirectory("Source\Child")
        Directory.SetLastWriteTimeUtc(sourceChild, LastAccessUtc)
        Directory.SetLastWriteTimeUtc(source, LastWriteUtc)

        Dim destination As String = Me.tempDir.CreateDirectory("Destination")
        Dim destinationChild As String = Me.tempDir.CreateDirectory("Destination\Child")
        Me.tempDir.CreateFile("Destination\Child\File.txt")

        Dim pending As New List(Of KeyValuePair(Of String, DirectoryInfo)) From {
            New KeyValuePair(Of String, DirectoryInfo)(destination, New DirectoryInfo(source)),
            New KeyValuePair(Of String, DirectoryInfo)(destinationChild, New DirectoryInfo(sourceChild))
        }
        Dim report As New MirrorReport(source, destination, Date.Now)

        Main.ApplyDirectoryTimestamps(pending, report)

        Assert.AreEqual(LastWriteUtc, Directory.GetLastWriteTimeUtc(destination))
        Assert.AreEqual(LastAccessUtc, Directory.GetLastWriteTimeUtc(destinationChild))
        Assert.IsFalse(report.HasEntries)
    End Sub

    <TestMethod>
    Public Sub ApplyDirectoryTimestamps_MissingDestination_IsReportedAsError()
        Dim source As String = Me.tempDir.CreateDirectory("Source")
        Dim pending As New List(Of KeyValuePair(Of String, DirectoryInfo)) From {
            New KeyValuePair(Of String, DirectoryInfo)(Me.tempDir.Combine("Missing"), New DirectoryInfo(source))
        }
        Dim report As New MirrorReport(source, Me.tempDir.FullPath, Date.Now)

        Main.ApplyDirectoryTimestamps(pending, report)

        Assert.AreEqual(1, report.ErrorCount)
        Assert.AreEqual(MirrorSkipReason.TimestampsNotApplied, report.Entries(0).Reason)
    End Sub

    <TestMethod>
    Public Sub ApplyDirectoryTimestamps_NullArguments_Throw()
        Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Main.ApplyDirectoryTimestamps(Nothing, report))
        Assert.ThrowsExactly(Of ArgumentNullException)(
            Sub() Main.ApplyDirectoryTimestamps(New List(Of KeyValuePair(Of String, DirectoryInfo)), Nothing))
    End Sub

#End Region

End Class

#End Region
