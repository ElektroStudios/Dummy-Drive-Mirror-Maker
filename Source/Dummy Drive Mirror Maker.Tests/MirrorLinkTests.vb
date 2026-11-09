#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports DevCase.Core.IO.Tools

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " MirrorLinkTests "

''' <summary>
''' Unit tests for <see cref="MirrorRules.DecideLinkAction"/> and <see cref="Main.MirrorLink"/>,
''' which mirror links (junctions and symbolic links) in an idempotent way.
''' </summary>
<TestClass>
Public NotInheritable Class MirrorLinkTests

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

#Region " DecideLinkAction "

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub DecideLinkAction_NothingExists_ReturnsCreate(overwrite As Boolean)
        Assert.AreEqual(LinkAction.Create, MirrorRules.DecideLinkAction(False, False, False, False, overwrite))
    End Sub

    <TestMethod>
    <DataRow(False, False)>
    <DataRow(False, True)>
    <DataRow(True, False)>
    <DataRow(True, True)>
    Public Sub DecideLinkAction_IdenticalLinkExists_ReturnsSkip(isDirectory As Boolean, overwrite As Boolean)
        ' Regression test: mirroring again into the same destination failed with "Destination path already exists".
        Assert.AreEqual(LinkAction.Skip, MirrorRules.DecideLinkAction(True, True, isDirectory, True, overwrite))
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub DecideLinkAction_DifferentLinkExists_ReplacesOnlyWhenOverwriting(isDirectory As Boolean)
        Assert.AreEqual(LinkAction.Replace, MirrorRules.DecideLinkAction(True, True, isDirectory, False, overwrite:=True))
        Assert.AreEqual(LinkAction.Keep, MirrorRules.DecideLinkAction(True, True, isDirectory, False, overwrite:=False))
    End Sub

    <TestMethod>
    Public Sub DecideLinkAction_RegularFileExists_ReplacesOnlyWhenOverwriting()
        Assert.AreEqual(LinkAction.Replace, MirrorRules.DecideLinkAction(True, False, False, False, overwrite:=True))
        Assert.AreEqual(LinkAction.Keep, MirrorRules.DecideLinkAction(True, False, False, False, overwrite:=False))
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub DecideLinkAction_RegularDirectoryExists_ReturnsConflict(overwrite As Boolean)
        ' A directory tree is never deleted to place a link.
        Assert.AreEqual(LinkAction.Conflict, MirrorRules.DecideLinkAction(True, False, True, False, overwrite))
    End Sub

#End Region

#Region " MirrorLink (Junctions) "

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub MirrorLink_JunctionTwice_SecondTimeDoesNothing(overwrite As Boolean)
        ' Regression test: building the mirror a second time failed for every directory junction.
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")

        Assert.AreEqual(LinkAction.Create, Main.MirrorLink(junctionPath, targetPath, Main.LinkKind.DirectoryJunction, overwrite))
        Assert.AreEqual(LinkAction.Skip, Main.MirrorLink(junctionPath, targetPath, Main.LinkKind.DirectoryJunction, overwrite))

        Assert.IsTrue(Directories.IsJunction(junctionPath))
        Assert.AreEqual(targetPath, Directories.GetMountPointTarget(junctionPath))
    End Sub

    <TestMethod>
    Public Sub MirrorLink_JunctionWithDifferentTarget_WithoutOverwrite_KeepsExistingJunction()
        Dim oldTarget As String = Me.tempDir.CreateDirectory("Old Target")
        Dim newTarget As String = Me.tempDir.CreateDirectory("New Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        Directories.CreateJunction(junctionPath, oldTarget)

        Assert.AreEqual(LinkAction.Keep, Main.MirrorLink(junctionPath, newTarget, Main.LinkKind.DirectoryJunction, overwrite:=False))

        Assert.AreEqual(oldTarget, Directories.GetMountPointTarget(junctionPath))
    End Sub

    <TestMethod>
    Public Sub MirrorLink_JunctionWithDifferentTarget_WithOverwrite_ReplacesJunctionAndKeepsOldTargetContents()
        Dim oldTarget As String = Me.tempDir.CreateDirectory("Old Target")
        Me.tempDir.CreateFile("Old Target\Keep.txt", "keep")
        Dim newTarget As String = Me.tempDir.CreateDirectory("New Target")
        Dim junctionPath As String = Me.tempDir.Combine("Junction")
        Directories.CreateJunction(junctionPath, oldTarget)

        Assert.AreEqual(LinkAction.Replace, Main.MirrorLink(junctionPath, newTarget, Main.LinkKind.DirectoryJunction, overwrite:=True))

        Assert.AreEqual(newTarget, Directories.GetMountPointTarget(junctionPath))
        Assert.AreEqual("keep", File.ReadAllText(Path.Combine(oldTarget, "Keep.txt")), "Replacing a link must never delete the contents of its target.")
    End Sub

    <TestMethod>
    Public Sub MirrorLink_RegularDirectoryAtDestination_ThrowsIOExceptionWithoutLongPathPrefix()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim existingDir As String = Me.tempDir.CreateDirectory("Existing")
        Me.tempDir.CreateFile("Existing\Keep.txt")

        Dim ex As IOException = Assert.ThrowsExactly(Of IOException)(
            Sub() Main.MirrorLink(MirrorRules.AddLongPathPrefix(existingDir), targetPath, Main.LinkKind.DirectoryJunction, overwrite:=True))

        Assert.IsFalse(ex.Message.Contains(MirrorRules.LongPathPrefix), "The message must not show the long path prefix.")
        Assert.IsTrue(File.Exists(Path.Combine(existingDir, "Keep.txt")), "A regular directory must never be deleted.")
    End Sub

    <TestMethod>
    Public Sub MirrorLink_RegularFileAtDestination_ReplacedOnlyWhenOverwriting()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim filePath As String = Me.tempDir.CreateFile("Item")

        Assert.AreEqual(LinkAction.Keep, Main.MirrorLink(filePath, targetPath, Main.LinkKind.DirectoryJunction, overwrite:=False))
        Assert.IsTrue(File.Exists(filePath), "Without overwrite, the existing file must be kept.")

        Assert.AreEqual(LinkAction.Replace, Main.MirrorLink(filePath, targetPath, Main.LinkKind.DirectoryJunction, overwrite:=True))
        Assert.IsTrue(Directories.IsJunction(filePath))
    End Sub

#End Region

#Region " MirrorLink (Symbolic Links) "

    <TestMethod>
    Public Sub MirrorLink_FileSymbolicLinkTwice_SecondTimeDoesNothing()
        Dim targetPath As String = Me.tempDir.CreateFile("Target.txt")
        Dim linkPath As String = Me.tempDir.Combine("Link.txt")
        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=False)

        Assert.AreEqual(LinkAction.Skip, Main.MirrorLink(linkPath, targetPath, Main.LinkKind.FileSymbolicLink, overwrite:=False))
        Assert.AreEqual(LinkAction.Skip, Main.MirrorLink(linkPath, targetPath, Main.LinkKind.FileSymbolicLink, overwrite:=True))

        Assert.IsTrue(Files.IsSymbolicLink(linkPath))
        Assert.AreEqual(targetPath, Files.GetSymbolicLinkDestination(linkPath))
    End Sub

    <TestMethod>
    Public Sub MirrorLink_DirectorySymbolicLinkTwice_SecondTimeDoesNothing()
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim linkPath As String = Me.tempDir.Combine("Link")
        LinkFactory.CreateSymbolicLinkOrInconclusive(linkPath, targetPath, isDirectory:=True)

        Assert.AreEqual(LinkAction.Skip, Main.MirrorLink(linkPath, targetPath, Main.LinkKind.DirectorySymbolicLink, overwrite:=True))

        Assert.IsTrue(Directories.IsSymbolicLink(linkPath))
        Assert.AreEqual(targetPath, Directories.GetSymbolicLinkDestination(linkPath))
    End Sub

    <TestMethod>
    Public Sub MirrorLink_JunctionWhereSymbolicLinkIsExpected_IsNotConsideredIdentical()
        ' A junction and a symbolic link to the same target are different kinds of link.
        Dim targetPath As String = Me.tempDir.CreateDirectory("Target")
        Dim linkPath As String = Me.tempDir.Combine("Link")
        Directories.CreateJunction(linkPath, targetPath)

        Assert.AreEqual(LinkAction.Keep, Main.MirrorLink(linkPath, targetPath, Main.LinkKind.DirectorySymbolicLink, overwrite:=False))

        Assert.IsTrue(Directories.IsJunction(linkPath), "Without overwrite, the existing junction must be kept.")
    End Sub

#End Region

End Class

#End Region
