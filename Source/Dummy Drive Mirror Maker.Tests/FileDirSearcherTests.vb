#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports DevCase.Core.IO.Tools

Imports DummyDriveMirrorMaker.Tests.Infrastructure

#End Region

#Region " FileDirSearcherTests "

''' <summary>
''' Unit tests for <see cref="FileDirSearcher"/>, <see cref="Files.GetFiles"/> and <see cref="Directories.GetDirs"/>.
''' </summary>
''' <remarks>
''' The tests use the following directory tree:
''' <code>
''' (root)
''' ├── a.txt
''' ├── b.log
''' ├── Upper.TXT
''' ├── Empty\
''' ├── Sub1\
''' │   ├── c.txt
''' │   └── Deep\
''' │       └── d.txt
''' └── Sub2\
'''     └── e.LOG
''' </code>
''' </remarks>
<TestClass>
Public NotInheritable Class FileDirSearcherTests

#Region " Fields "

    ''' <summary>
    ''' The temporary directory tree used by the current test.
    ''' </summary>
    Private tempDir As TemporaryDirectory

#End Region

#Region " Initialization / Cleanup "

    <TestInitialize>
    Public Sub TestInitialize()
        Me.tempDir = New TemporaryDirectory()
        Me.tempDir.CreateFile("a.txt")
        Me.tempDir.CreateFile("b.log")
        Me.tempDir.CreateFile("Upper.TXT")
        Me.tempDir.CreateDirectory("Empty")
        Me.tempDir.CreateFile("Sub1\c.txt")
        Me.tempDir.CreateFile("Sub1\Deep\d.txt")
        Me.tempDir.CreateFile("Sub2\e.LOG")
    End Sub

    <TestCleanup>
    Public Sub TestCleanup()
        Me.tempDir.Dispose()
    End Sub

#End Region

#Region " GetFiles "

    <TestMethod>
    Public Sub GetFiles_TopDirectoryOnly_ReturnsOnlyTopLevelFiles()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.TopDirectoryOnly).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"a.txt", "b.log", "Upper.TXT"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_AllDirectories_ReturnsAllFiles()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"a.txt", "b.log", "Upper.TXT", "c.txt", "d.txt", "e.LOG"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_AllDirectories_ReturnsFullPathsInsideRoot()
        Dim files As FileInfo() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories).ToArray()

        Assert.IsTrue(files.All(Function(fi As FileInfo) fi.FullName.StartsWith(Me.tempDir.FullPath, StringComparison.OrdinalIgnoreCase)))
        Assert.IsTrue(files.All(Function(fi As FileInfo) fi.Exists))
    End Sub

    <TestMethod>
    Public Sub GetFiles_ExtensionPatternIgnoringCase_ReturnsMatchingFiles()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                         fileExtPatterns:={".txt"}, ignoreCase:=True).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"a.txt", "Upper.TXT", "c.txt", "d.txt"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_ExtensionPatternMatchingCase_ReturnsExactCaseMatchesOnly()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                         fileExtPatterns:={".txt"}, ignoreCase:=False).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"a.txt", "c.txt", "d.txt"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_MultipleExtensionPatterns_ReturnsFilesMatchingAnyPattern()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.TopDirectoryOnly,
                                                         fileExtPatterns:={".log", ".TXT"}, ignoreCase:=False).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"b.log", "Upper.TXT"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_FileNamePattern_ReturnsMatchingFiles()
        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                         fileNamePatterns:={"?.txt"}).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.AreEquivalent({"a.txt", "c.txt", "d.txt"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFiles_AsteriskPatterns_ReturnsAllFiles()
        Dim count As Integer = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories, {"*"}, {"*"}, True).Count()

        Assert.AreEqual(6, count)
    End Sub

    <TestMethod>
    Public Sub GetFiles_NamePatternWithoutMatches_ReturnsEmpty()
        Dim count As Integer = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                        fileNamePatterns:={"nothing-matches-this*"}).Count()

        Assert.AreEqual(0, count)
    End Sub

    <TestMethod>
    Public Sub GetFiles_EmptyDirectory_ReturnsEmpty()
        Dim count As Integer = FileDirSearcher.GetFiles(Me.tempDir.Combine("Empty"), SearchOption.AllDirectories).Count()

        Assert.AreEqual(0, count)
    End Sub

    <TestMethod>
    Public Sub GetFiles_HiddenFile_IsIncluded()
        Dim hiddenFile As String = Me.tempDir.CreateFile("Hidden.txt")
        File.SetAttributes(hiddenFile, FileAttributes.Hidden)

        Dim names As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.TopDirectoryOnly).
                                                Select(Function(fi As FileInfo) fi.Name).ToArray()

        CollectionAssert.Contains(names, "Hidden.txt")
    End Sub

    <TestMethod>
    Public Sub GetFiles_PathWithTrailingSeparatorAndSpaces_ReturnsSameFiles()
        Dim expected As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories).
                                                   Select(Function(fi As FileInfo) fi.FullName).OrderBy(Function(x As String) x).ToArray()

        Dim actual As String() = FileDirSearcher.GetFiles($"  {Me.tempDir.FullPath}\  ", SearchOption.AllDirectories).
                                                 Select(Function(fi As FileInfo) fi.FullName).OrderBy(Function(x As String) x).ToArray()

        CollectionAssert.AreEqual(expected, actual)
    End Sub

#End Region

#Region " GetFilePaths "

    <TestMethod>
    Public Sub GetFilePaths_AllDirectories_ReturnsAllFilePaths()
        Dim paths As String() = FileDirSearcher.GetFilePaths(Me.tempDir.FullPath, SearchOption.AllDirectories).ToArray()

        Assert.AreEqual(6, paths.Length)
        Assert.IsTrue(paths.All(Function(p As String) File.Exists(p)))
    End Sub

    <TestMethod>
    Public Sub GetFilePaths_ExtensionPattern_ReturnsMatchingFilePaths()
        Dim names As String() = FileDirSearcher.GetFilePaths(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                             fileExtPatterns:={".log"}, ignoreCase:=True).
                                                Select(Function(p As String) Path.GetFileName(p)).ToArray()

        CollectionAssert.AreEquivalent({"b.log", "e.LOG"}, names)
    End Sub

    <TestMethod>
    Public Sub GetFilePaths_FileNamePattern_IsMatchedAgainstNameWithoutExtension()
        ' Documents the current behavior: unlike GetFiles, GetFilePaths matches the name patterns
        ' against the file name without its extension.
        Dim names As String() = FileDirSearcher.GetFilePaths(Me.tempDir.FullPath, SearchOption.TopDirectoryOnly,
                                                             fileNamePatterns:={"a"}).
                                                Select(Function(p As String) Path.GetFileName(p)).ToArray()

        CollectionAssert.AreEquivalent({"a.txt"}, names)
    End Sub

#End Region

#Region " GetDirs / GetDirPaths "

    <TestMethod>
    Public Sub GetDirs_TopDirectoryOnly_ReturnsOnlyTopLevelDirectories()
        Dim names As String() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.TopDirectoryOnly).
                                                Select(Function(di As DirectoryInfo) di.Name).ToArray()

        CollectionAssert.AreEquivalent({"Empty", "Sub1", "Sub2"}, names)
    End Sub

    <TestMethod>
    Public Sub GetDirs_AllDirectories_ReturnsAllDirectories()
        Dim names As String() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories).
                                                Select(Function(di As DirectoryInfo) di.Name).ToArray()

        CollectionAssert.AreEquivalent({"Empty", "Sub1", "Deep", "Sub2"}, names)
    End Sub

    <TestMethod>
    Public Sub GetDirs_AllDirectories_DoesNotIncludeRootDirectory()
        Dim dirs As DirectoryInfo() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories).ToArray()

        Assert.IsFalse(dirs.Any(Function(di As DirectoryInfo) MirrorRules.ArePathsEqual(di.FullName, Me.tempDir.FullPath)))
    End Sub

    <TestMethod>
    Public Sub GetDirs_NamePattern_ReturnsMatchingDirectories()
        Dim names As String() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                        dirNamePatterns:={"sub*"}, ignoreCase:=True).
                                                Select(Function(di As DirectoryInfo) di.Name).ToArray()

        CollectionAssert.AreEquivalent({"Sub1", "Sub2"}, names)
    End Sub

    <TestMethod>
    Public Sub GetDirs_NamePatternMatchingCase_ReturnsExactCaseMatchesOnly()
        Dim count As Integer = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                       dirNamePatterns:={"sub*"}, ignoreCase:=False).Count()

        Assert.AreEqual(0, count)
    End Sub

    <TestMethod>
    Public Sub GetDirs_PathPattern_ReturnsMatchingDirectories()
        Dim names As String() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories,
                                                        dirPathPatterns:={"*\Sub1\*"}).
                                                Select(Function(di As DirectoryInfo) di.Name).ToArray()

        CollectionAssert.AreEquivalent({"Deep"}, names)
    End Sub

    <TestMethod>
    Public Sub GetDirPaths_AllDirectories_ReturnsAllDirectoryPaths()
        Dim paths As String() = FileDirSearcher.GetDirPaths(Me.tempDir.FullPath, SearchOption.AllDirectories).ToArray()

        Assert.AreEqual(4, paths.Length)
        Assert.IsTrue(paths.All(Function(p As String) Directory.Exists(p)))
    End Sub

    <TestMethod>
    Public Sub GetDirs_EmptyDirectory_ReturnsEmpty()
        Dim count As Integer = FileDirSearcher.GetDirs(Me.tempDir.Combine("Empty"), SearchOption.AllDirectories).Count()

        Assert.AreEqual(0, count)
    End Sub

#End Region

#Region " Argument Validation "

    <TestMethod>
    Public Sub GetFiles_NonExistentDirectory_ThrowsArgumentException()
        Dim missingDir As String = Me.tempDir.Combine("DoesNotExist")

        Assert.ThrowsExactly(Of ArgumentException)(Sub() FileDirSearcher.GetFiles(missingDir, SearchOption.AllDirectories))
    End Sub

    <TestMethod>
    Public Sub GetDirs_NonExistentDirectory_ThrowsArgumentException()
        Dim missingDir As String = Me.tempDir.Combine("DoesNotExist")

        Assert.ThrowsExactly(Of ArgumentException)(Sub() FileDirSearcher.GetDirs(missingDir, SearchOption.AllDirectories))
    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("   ")>
    Public Sub GetFiles_EmptyOrWhiteSpacePath_ThrowsArgumentNullException(dirPath As String)
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() FileDirSearcher.GetFiles(dirPath, SearchOption.AllDirectories))
    End Sub

    <TestMethod>
    Public Sub GetDirs_NullPath_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() FileDirSearcher.GetDirs(Nothing, SearchOption.AllDirectories))
    End Sub

    <TestMethod>
    Public Sub GetFiles_InvalidSearchOption_ThrowsArgumentException()
        Dim invalidOption As SearchOption = CType(99, SearchOption)

        Assert.ThrowsExactly(Of ArgumentException)(Sub() FileDirSearcher.GetFiles(Me.tempDir.FullPath, invalidOption))
    End Sub

#End Region

#Region " Wrappers "

    <TestMethod>
    Public Sub FilesGetFiles_ReturnsSameResultsAsFileDirSearcher()
        Dim expected As String() = FileDirSearcher.GetFiles(Me.tempDir.FullPath, SearchOption.AllDirectories).
                                                   Select(Function(fi As FileInfo) fi.FullName).OrderBy(Function(x As String) x).ToArray()

        Dim actual As String() = Files.GetFiles(Me.tempDir.Info, SearchOption.AllDirectories, {"*"}, {"*"}, True, throwOnError:=False).
                                       Select(Function(fi As FileInfo) fi.FullName).OrderBy(Function(x As String) x).ToArray()

        CollectionAssert.AreEqual(expected, actual)
    End Sub

    <TestMethod>
    Public Sub DirectoriesGetDirs_ReturnsSameResultsAsFileDirSearcher()
        Dim expected As String() = FileDirSearcher.GetDirs(Me.tempDir.FullPath, SearchOption.AllDirectories).
                                                   Select(Function(di As DirectoryInfo) di.FullName).OrderBy(Function(x As String) x).ToArray()

        Dim actual As String() = Directories.GetDirs(Me.tempDir.Info, SearchOption.AllDirectories, {"*"}, {"*"}, True, throwOnError:=False).
                                             Select(Function(di As DirectoryInfo) di.FullName).OrderBy(Function(x As String) x).ToArray()

        CollectionAssert.AreEqual(expected, actual)
    End Sub

#End Region

End Class

#End Region
