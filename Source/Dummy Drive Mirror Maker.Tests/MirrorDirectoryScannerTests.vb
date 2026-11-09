#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " MirrorDirectoryScannerTests "

''' <summary>
''' Unit tests for <see cref="MirrorDirectoryScanner"/>, which lists the directories to mirror
''' without descending into links (the cause of the directories reported as ignored when mirroring a mirror).
''' </summary>
<TestClass>
Public NotInheritable Class MirrorDirectoryScannerTests

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

#Region " Helpers "

    ''' <summary>
    ''' Lists the directories of the specified root with the same rules of the mirror operation.
    ''' </summary>
    ''' <param name="root">
    ''' The root directory.
    ''' </param>
    '''
    ''' <param name="mirrorHiddenItems">
    ''' A value indicating whether hidden items are mirrored.
    ''' </param>
    ''' <returns>
    ''' The full paths of the listed directories.
    ''' </returns>
    Private Shared Function List(root As String, mirrorHiddenItems As Boolean) As List(Of String)
        Return MirrorDirectoryScanner.ListDirectories(
            New DirectoryInfo(root),
            Function(di As DirectoryInfo) MirrorDirectoryScanner.ShouldDescendInto(di, mirrorHiddenItems),
            Sub(di As DirectoryInfo, ex As Exception) Assert.Fail($"Unexpected listing error in '{di.FullName}': {ex.Message}")).
            Select(Function(di As DirectoryInfo) di.FullName.TrimEnd(Path.DirectorySeparatorChar)).
            ToList()
    End Function

#End Region

#Region " Tests "

    <TestMethod>
    Public Sub ListDirectories_ReturnsRootFirstAndAllSubdirectoriesSorted()
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim b As String = Me.tempDir.CreateDirectory("Root\b")
        Dim a As String = Me.tempDir.CreateDirectory("Root\A")
        Dim aChild As String = Me.tempDir.CreateDirectory("Root\A\Child")

        Dim result As List(Of String) = MirrorDirectoryScannerTests.List(root, mirrorHiddenItems:=True)

        CollectionAssert.AreEqual({root.TrimEnd("\"c), a, aChild, b}, result)
    End Sub

    <TestMethod>
    Public Sub ListDirectories_DoesNotDescendIntoJunctions()
        ' The junction itself is listed (it is mirrored as a link or reported as ignored),
        ' but the directories of its target are not listed through it.
        Dim target As String = Me.tempDir.CreateDirectory("Target")
        Me.tempDir.CreateDirectory("Target\Inner")
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim junction As String = Path.Combine(root, "Junction")
        LinkFactory.CreateJunction(junction, target)

        Dim result As List(Of String) = MirrorDirectoryScannerTests.List(root, mirrorHiddenItems:=True)

        CollectionAssert.Contains(result, junction)
        CollectionAssert.DoesNotContain(result, Path.Combine(junction, "Inner"))
        Assert.AreEqual(2, result.Count)
    End Sub

    <TestMethod>
    Public Sub ListDirectories_HiddenDirectories_AreListedButOnlyEnteredWhenMirroringHiddenItems()
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim hidden As String = Me.tempDir.CreateDirectory("Root\Hidden")
        Dim hiddenChild As String = Me.tempDir.CreateDirectory("Root\Hidden\Child")
        File.SetAttributes(hidden, File.GetAttributes(hidden) Or FileAttributes.Hidden)

        Dim withoutHidden As List(Of String) = MirrorDirectoryScannerTests.List(root, mirrorHiddenItems:=False)
        CollectionAssert.Contains(withoutHidden, hidden)
        CollectionAssert.DoesNotContain(withoutHidden, hiddenChild, "The contents of a hidden directory are ignored together with it.")

        Dim withHidden As List(Of String) = MirrorDirectoryScannerTests.List(root, mirrorHiddenItems:=True)
        CollectionAssert.Contains(withHidden, hiddenChild)
    End Sub

    <TestMethod>
    Public Sub ShouldDescendInto_JunctionOrExcludedDirectory_ReturnsFalse()
        Dim target As String = Me.tempDir.CreateDirectory("Target")
        Dim junction As String = Me.tempDir.Combine("Junction")
        LinkFactory.CreateJunction(junction, target)
        Dim recycleBin As String = Me.tempDir.CreateDirectory("$RECYCLE.BIN")

        Assert.IsFalse(MirrorDirectoryScanner.ShouldDescendInto(New DirectoryInfo(junction), mirrorHiddenItems:=True))
        Assert.AreEqual(Not MirrorRules.IsExcludedSystemDirectory(recycleBin),
                        MirrorDirectoryScanner.ShouldDescendInto(New DirectoryInfo(recycleBin), mirrorHiddenItems:=True))
        Assert.IsTrue(MirrorDirectoryScanner.ShouldDescendInto(New DirectoryInfo(target), mirrorHiddenItems:=False))
    End Sub

    ''' <summary>
    ''' Lists and selects the directories to mirror with the specified options, 
    ''' collecting the excluded directories and their reasons.
    ''' </summary>
    Private Shared Function SelectToMirror(root As String, mirrorHidden As Boolean, mirrorJunctions As Boolean, mirrorSymbolicLinks As Boolean,
                                           excluded As Dictionary(Of String, MirrorSkipReason)) As List(Of String)

        Dim listed As List(Of DirectoryInfo) = MirrorDirectoryScanner.ListDirectories(
            New DirectoryInfo(root),
            Function(di As DirectoryInfo) MirrorDirectoryScanner.ShouldDescendInto(di, mirrorHidden),
            Sub(di As DirectoryInfo, ex As Exception) Assert.Fail(ex.Message))

        Return MirrorDirectoryScanner.SelectDirectoriesToMirror(
            listed, mirrorHidden, mirrorJunctions, mirrorSymbolicLinks,
            Sub(di As DirectoryInfo, reason As MirrorSkipReason) excluded.Add(di.FullName.TrimEnd(Path.DirectorySeparatorChar), reason)).
            Select(Function(di As DirectoryInfo) di.FullName.TrimEnd(Path.DirectorySeparatorChar)).
            ToList()
    End Function

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub SelectDirectoriesToMirror_Junctions_AreOnlySelectedWhenTheOptionIsEnabled(mirrorJunctions As Boolean)
        Dim target As String = Me.tempDir.CreateDirectory("Target")
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim normal As String = Me.tempDir.CreateDirectory("Root\Normal")
        Dim junction As String = Path.Combine(root, "Junction")
        LinkFactory.CreateJunction(junction, target)

        Dim excluded As New Dictionary(Of String, MirrorSkipReason)
        Dim selected As List(Of String) = MirrorDirectoryScannerTests.SelectToMirror(root, True, mirrorJunctions, True, excluded)

        CollectionAssert.Contains(selected, normal)
        If mirrorJunctions Then
            CollectionAssert.Contains(selected, junction)
            Assert.AreEqual(0, excluded.Count)
        Else
            ' Not processed nor counted, but registered (with its reason) for the report.
            CollectionAssert.DoesNotContain(selected, junction)
            Assert.AreEqual(MirrorSkipReason.JunctionNotMirrored, excluded(junction))
        End If
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub SelectDirectoriesToMirror_SymbolicLinks_AreOnlySelectedWhenTheOptionIsEnabled(mirrorSymbolicLinks As Boolean)
        Dim target As String = Me.tempDir.CreateDirectory("Target")
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim link As String = Path.Combine(root, "SymLink")
        LinkFactory.CreateSymbolicLinkOrInconclusive(link, target, isDirectory:=True)

        Dim excluded As New Dictionary(Of String, MirrorSkipReason)
        Dim selected As List(Of String) = MirrorDirectoryScannerTests.SelectToMirror(root, True, True, mirrorSymbolicLinks, excluded)

        If mirrorSymbolicLinks Then
            CollectionAssert.Contains(selected, link)
        Else
            CollectionAssert.DoesNotContain(selected, link)
            Assert.AreEqual(MirrorSkipReason.SymbolicLinkNotMirrored, excluded(link))
        End If
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub SelectDirectoriesToMirror_HiddenDirectories_AreOnlySelectedWhenTheOptionIsEnabled(mirrorHidden As Boolean)
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        Dim hidden As String = Me.tempDir.CreateDirectory("Root\Hidden")
        Dim hiddenChild As String = Me.tempDir.CreateDirectory("Root\Hidden\Child")
        File.SetAttributes(hidden, File.GetAttributes(hidden) Or FileAttributes.Hidden)

        Dim excluded As New Dictionary(Of String, MirrorSkipReason)
        Dim selected As List(Of String) = MirrorDirectoryScannerTests.SelectToMirror(root, mirrorHidden, True, True, excluded)

        If mirrorHidden Then
            CollectionAssert.Contains(selected, hidden)
            CollectionAssert.Contains(selected, hiddenChild)
        Else
            CollectionAssert.DoesNotContain(selected, hidden)
            CollectionAssert.DoesNotContain(selected, hiddenChild)
            Assert.AreEqual(MirrorSkipReason.HiddenItem, excluded(hidden))
            Assert.AreEqual(1, excluded.Count, "The contents of an excluded directory are not listed.")
        End If
    End Sub

    <TestMethod>
    Public Sub SelectDirectoriesToMirror_SourceDirectoryIsAlwaysSelectedFirst()
        ' Even a hidden source directory, with hidden items not mirrored.
        Dim root As String = Me.tempDir.CreateDirectory("Root")
        File.SetAttributes(root, File.GetAttributes(root) Or FileAttributes.Hidden)

        Dim excluded As New Dictionary(Of String, MirrorSkipReason)
        Dim selected As List(Of String) = MirrorDirectoryScannerTests.SelectToMirror(root, False, False, False, excluded)

        Assert.AreEqual(1, selected.Count)
        Assert.AreEqual(root.TrimEnd("\"c), selected(0))
        Assert.AreEqual(0, excluded.Count)
    End Sub

    <TestMethod>
    Public Sub SelectDirectoriesToMirror_InvalidArguments_Throw()
        Dim onExcluded As Action(Of DirectoryInfo, MirrorSkipReason) = Sub(di As DirectoryInfo, reason As MirrorSkipReason)
                                                                       End Sub

        Assert.ThrowsExactly(Of ArgumentNullException)(Function() MirrorDirectoryScanner.SelectDirectoriesToMirror(Nothing, True, True, True, onExcluded))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() MirrorDirectoryScanner.SelectDirectoriesToMirror(New List(Of DirectoryInfo) From {New DirectoryInfo(Me.tempDir.FullPath)}, True, True, True, Nothing))
        Assert.ThrowsExactly(Of ArgumentException)(Function() MirrorDirectoryScanner.SelectDirectoriesToMirror(New List(Of DirectoryInfo), True, True, True, onExcluded))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() MirrorDirectoryScanner.GetExclusionReason(Nothing, True, True, True))
    End Sub

    <TestMethod>
    Public Sub ListDirectories_InvalidArguments_Throw()
        Dim root As New DirectoryInfo(Me.tempDir.FullPath)
        Dim descend As Func(Of DirectoryInfo, Boolean) = Function(di As DirectoryInfo) True
        Dim onError As Action(Of DirectoryInfo, Exception) = Sub(di As DirectoryInfo, ex As Exception)
                                                             End Sub

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorDirectoryScanner.ListDirectories(Nothing, descend, onError))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorDirectoryScanner.ListDirectories(root, Nothing, onError))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorDirectoryScanner.ListDirectories(root, descend, Nothing))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(
            Sub() MirrorDirectoryScanner.ListDirectories(New DirectoryInfo(Me.tempDir.Combine("Missing")), descend, onError))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorDirectoryScanner.ShouldDescendInto(Nothing, True))
    End Sub

#End Region

End Class

#End Region
