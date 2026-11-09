#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " MirrorRulesPathTests "

''' <summary>
''' Unit tests for the path manipulation rules of <see cref="MirrorRules"/>.
''' </summary>
<TestClass>
Public NotInheritable Class MirrorRulesPathTests

#Region " RemoveLongPathPrefix "

    <TestMethod>
    <DataRow("\\?\C:\Folder", "C:\Folder")>
    <DataRow("\\?\C:\", "C:\")>
    <DataRow("\\?\UNC\Server\Share\Dir", "\\Server\Share\Dir")>
    <DataRow("\\?\unc\Server\Share", "\\Server\Share")>
    <DataRow("C:\Folder", "C:\Folder")>
    <DataRow("\\Server\Share", "\\Server\Share")>
    <DataRow("", "")>
    Public Sub RemoveLongPathPrefix_VariousPaths_ReturnsPathWithoutPrefix(input As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.RemoveLongPathPrefix(input))
    End Sub

    <TestMethod>
    Public Sub RemoveLongPathPrefix_Nothing_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.RemoveLongPathPrefix(Nothing))
    End Sub

#End Region

#Region " AddLongPathPrefix "

    <TestMethod>
    <DataRow("C:\Folder", "\\?\C:\Folder")>
    <DataRow("C:\", "\\?\C:\")>
    <DataRow("c:/Folder/Sub", "\\?\c:\Folder\Sub")>
    <DataRow("//Server/Share", "\\?\UNC\Server\Share")>
    <DataRow("\\Server\Share\Dir", "\\?\UNC\Server\Share\Dir")>
    <DataRow("\\?\C:\Folder", "\\?\C:\Folder")>
    <DataRow("\\?\UNC\Server\Share", "\\?\UNC\Server\Share")>
    Public Sub AddLongPathPrefix_FullyQualifiedPaths_ReturnsPrefixedPath(input As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.AddLongPathPrefix(input))
    End Sub

    <TestMethod>
    <DataRow("Folder")>
    <DataRow("Folder\Sub")>
    <DataRow("\Folder")>
    <DataRow("C:Folder")>
    <DataRow("C:")>
    Public Sub AddLongPathPrefix_NotFullyQualifiedPath_ThrowsArgumentException(input As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MirrorRules.AddLongPathPrefix(input))
    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("   ")>
    Public Sub AddLongPathPrefix_EmptyOrWhiteSpace_ThrowsArgumentNullException(input As String)
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.AddLongPathPrefix(input))
    End Sub

    <TestMethod>
    Public Sub AddLongPathPrefix_Nothing_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.AddLongPathPrefix(Nothing))
    End Sub

    <TestMethod>
    <DataRow("C:\Folder\Sub\File.txt")>
    <DataRow("D:\")>
    <DataRow("\\Server\Share\Folder")>
    Public Sub AddLongPathPrefix_ThenRemoveLongPathPrefix_ReturnsOriginalPath(input As String)
        Assert.AreEqual(input, MirrorRules.RemoveLongPathPrefix(MirrorRules.AddLongPathPrefix(input)))
    End Sub

#End Region

#Region " AddLongPathPrefixIfNeeded "

    <TestMethod>
    <DataRow("C:\Folder", "C:\Folder")>
    <DataRow("\\?\C:\Folder", "C:\Folder")>
    <DataRow("\\Server\Share\Dir", "\\Server\Share\Dir")>
    <DataRow("\\?\UNC\Server\Share\Dir", "\\Server\Share\Dir")>
    Public Sub AddLongPathPrefixIfNeeded_ShortPath_ReturnsPathWithoutPrefix(input As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.AddLongPathPrefixIfNeeded(input))
    End Sub

    <TestMethod>
    Public Sub AddLongPathPrefixIfNeeded_PathBelowThreshold_ReturnsPathWithoutPrefix()
        Dim path As String = $"C:\{New String("a"c, MirrorRules.LongPathThreshold - 4)}"
        Assert.AreEqual(MirrorRules.LongPathThreshold - 1, path.Length)

        Assert.AreEqual(path, MirrorRules.AddLongPathPrefixIfNeeded(path))
    End Sub

    <TestMethod>
    Public Sub AddLongPathPrefixIfNeeded_PathAtThreshold_ReturnsPrefixedPath()
        Dim path As String = $"C:\{New String("a"c, MirrorRules.LongPathThreshold - 3)}"
        Assert.AreEqual(MirrorRules.LongPathThreshold, path.Length)

        Assert.AreEqual($"\\?\{path}", MirrorRules.AddLongPathPrefixIfNeeded(path))
    End Sub

    <TestMethod>
    Public Sub AddLongPathPrefixIfNeeded_LongUncPath_ReturnsUncPrefixedPath()
        Dim path As String = $"\\Server\Share\{New String("a"c, 300)}"

        Assert.AreEqual($"\\?\UNC\Server\Share\{New String("a"c, 300)}", MirrorRules.AddLongPathPrefixIfNeeded(path))
    End Sub

    <TestMethod>
    <DataRow("Folder")>
    <DataRow("\Folder")>
    Public Sub AddLongPathPrefixIfNeeded_NotFullyQualifiedPath_ThrowsArgumentException(input As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MirrorRules.AddLongPathPrefixIfNeeded(input))
    End Sub

    <TestMethod>
    Public Sub AddLongPathPrefixIfNeeded_Nothing_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.AddLongPathPrefixIfNeeded(Nothing))
    End Sub

#End Region

#Region " NormalizeDirectoryPath "

    <TestMethod>
    <DataRow("C:\Folder", "C:\Folder")>
    <DataRow("C:\Folder\", "C:\Folder")>
    <DataRow("C:\Folder\\\", "C:\Folder")>
    <DataRow("C:\", "C:\")>
    <DataRow("C:\\", "C:\")>
    <DataRow("c:/Folder/Sub/", "c:\Folder\Sub")>
    <DataRow("\\?\C:\Folder\", "C:\Folder")>
    <DataRow("\\?\C:\", "C:\")>
    <DataRow("\\Server\Share\", "\\Server\Share")>
    <DataRow("\\?\UNC\Server\Share\Dir\", "\\Server\Share\Dir")>
    Public Sub NormalizeDirectoryPath_VariousPaths_ReturnsNormalizedPath(input As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.NormalizeDirectoryPath(input))
    End Sub

    <TestMethod>
    <DataRow("Folder")>
    <DataRow("Folder\Sub\")>
    Public Sub NormalizeDirectoryPath_RelativePath_ThrowsArgumentException(input As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MirrorRules.NormalizeDirectoryPath(input))
    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow(" ")>
    Public Sub NormalizeDirectoryPath_EmptyOrWhiteSpace_ThrowsArgumentNullException(input As String)
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.NormalizeDirectoryPath(input))
    End Sub

    <TestMethod>
    Public Sub NormalizeDirectoryPath_Nothing_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MirrorRules.NormalizeDirectoryPath(Nothing))
    End Sub

#End Region

#Region " ArePathsEqual "

    <TestMethod>
    <DataRow("C:\Data", "C:\Data")>
    <DataRow("C:\Data", "c:\DATA")>
    <DataRow("C:\Data", "C:\Data\")>
    <DataRow("\\?\C:\Data", "C:\Data")>
    <DataRow("C:\", "c:\")>
    <DataRow("C:/Data/Sub", "C:\Data\Sub\")>
    <DataRow("\\Server\Share\Dir", "\\?\UNC\Server\Share\Dir\")>
    Public Sub ArePathsEqual_SameLocation_ReturnsTrue(path1 As String, path2 As String)
        Assert.IsTrue(MirrorRules.ArePathsEqual(path1, path2))
    End Sub

    <TestMethod>
    <DataRow("C:\Data", "C:\Data2")>
    <DataRow("C:\Data", "D:\Data")>
    <DataRow("C:\Data", "C:\Data\Sub")>
    <DataRow("C:\", "C:\Data")>
    Public Sub ArePathsEqual_DifferentLocation_ReturnsFalse(path1 As String, path2 As String)
        Assert.IsFalse(MirrorRules.ArePathsEqual(path1, path2))
    End Sub

#End Region

#Region " IsDescendantOf "

    <TestMethod>
    <DataRow("C:\Data\Sub", "C:\Data")>
    <DataRow("C:\Data\Sub\Deep\Deeper", "C:\Data")>
    <DataRow("c:\data\sub", "C:\DATA\")>
    <DataRow("D:\Music", "D:\")>
    <DataRow("\\?\C:\Data\Sub", "C:\Data")>
    <DataRow("C:\Data\Sub", "\\?\C:\Data")>
    <DataRow("\\Server\Share\Dir", "\\Server\Share")>
    Public Sub IsDescendantOf_PathInsideAncestor_ReturnsTrue(path As String, ancestorPath As String)
        Assert.IsTrue(MirrorRules.IsDescendantOf(path, ancestorPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data2", "C:\Data")>
    <DataRow("C:\Database\Sub", "C:\Data")>
    <DataRow("C:\Data", "C:\Data")>
    <DataRow("C:\Data\", "C:\Data")>
    <DataRow("C:\", "C:\Data")>
    <DataRow("D:\Data\Sub", "C:\Data")>
    <DataRow("D:\", "C:\")>
    Public Sub IsDescendantOf_PathNotInsideAncestor_ReturnsFalse(path As String, ancestorPath As String)
        Assert.IsFalse(MirrorRules.IsDescendantOf(path, ancestorPath))
    End Sub

#End Region

#Region " GetRelativePath "

    <TestMethod>
    <DataRow("C:\Data", "C:\Data\Sub\File.txt", "Sub\File.txt")>
    <DataRow("C:\Data\", "C:\Data\File.txt", "File.txt")>
    <DataRow("D:\", "D:\Music\Song.mp3", "Music\Song.mp3")>
    <DataRow("d:\data", "D:\Data\Sub", "Sub")>
    <DataRow("C:\Data", "\\?\C:\Data\Sub\File.txt", "Sub\File.txt")>
    <DataRow("C:\Data", "C:\Data", "")>
    <DataRow("C:\Data", "c:\data\", "")>
    Public Sub GetRelativePath_PathInsideRoot_ReturnsRelativePath(rootPath As String, fullPath As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.GetRelativePath(rootPath, fullPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data", "C:\Data2\File.txt")>
    <DataRow("C:\Data", "D:\Data\File.txt")>
    <DataRow("C:\Data\Sub", "C:\Data")>
    Public Sub GetRelativePath_PathNotInsideRoot_ThrowsArgumentException(rootPath As String, fullPath As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MirrorRules.GetRelativePath(rootPath, fullPath))
    End Sub

#End Region

#Region " BuildDestinationPath "

    <TestMethod>
    <DataRow("D:\", "E:\Mirror", "D:\Music\Song.mp3", "E:\Mirror\Music\Song.mp3")>
    <DataRow("D:\", "E:\", "D:\Music", "E:\Music")>
    <DataRow("D:\", "E:\Mirror\", "D:\", "E:\Mirror")>
    <DataRow("C:\Data", "E:\", "C:\Data", "E:\")>
    <DataRow("C:\Data", "E:\Mirror", "C:\Data\Sub\Deep", "E:\Mirror\Sub\Deep")>
    <DataRow("C:\Data", "E:\Mirror", "\\?\C:\Data\File.txt", "E:\Mirror\File.txt")>
    <DataRow("C:\Data", "\\?\E:\Mirror", "C:\Data\File.txt", "E:\Mirror\File.txt")>
    <DataRow("c:\data", "E:\Mirror", "C:\Data\Sub\File.txt", "E:\Mirror\Sub\File.txt")>
    <DataRow("C:\Data", "\\Server\Share\Mirror", "C:\Data\Sub", "\\Server\Share\Mirror\Sub")>
    <DataRow("\\Server\Share\Data", "E:\Mirror", "\\Server\Share\Data\Sub", "E:\Mirror\Sub")>
    Public Sub BuildDestinationPath_PathInsideSource_ReturnsMirroredLongPath(sourceRoot As String, destinationRoot As String,
                                                                             sourcePath As String, expected As String)
        Assert.AreEqual(expected, MirrorRules.BuildDestinationPath(sourceRoot, destinationRoot, sourcePath))
    End Sub

    <TestMethod>
    Public Sub BuildDestinationPath_ShortPath_NeverHasLongPathPrefix()
        ' Regression test: the long path prefix was always added, 
        ' so it appeared in exception messages even for short paths.
        Dim result As String = MirrorRules.BuildDestinationPath("C:\Data", "E:\Mirror", "C:\Data\Sub\File.txt")

        Assert.IsFalse(result.StartsWith(MirrorRules.LongPathPrefix, StringComparison.Ordinal))
    End Sub

    <TestMethod>
    Public Sub BuildDestinationPath_LongPath_HasLongPathPrefix()
        Dim longName As New String("a"c, 250)
        Dim result As String = MirrorRules.BuildDestinationPath("C:\Data", "E:\Mirror", $"C:\Data\{longName}")

        Assert.AreEqual($"\\?\E:\Mirror\{longName}", result)
    End Sub

    <TestMethod>
    Public Sub BuildDestinationPath_PathOutsideSource_ThrowsArgumentException()
        Assert.ThrowsExactly(Of ArgumentException)(
            Sub() MirrorRules.BuildDestinationPath("C:\Data", "E:\Mirror", "C:\Data2\File.txt"))
    End Sub

#End Region

End Class

#End Region
