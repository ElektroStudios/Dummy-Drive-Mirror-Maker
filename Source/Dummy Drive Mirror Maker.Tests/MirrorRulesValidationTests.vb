#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " MirrorRulesValidationTests "

''' <summary>
''' Unit tests for the destination validation and the system directory exclusion rules of <see cref="MirrorRules"/>.
''' </summary>
<TestClass>
Public NotInheritable Class MirrorRulesValidationTests

#Region " ValidateDestination "

    <TestMethod>
    <DataRow("C:\Data", "C:\Data")>
    <DataRow("C:\Data", "c:\data\")>
    <DataRow("C:\Data", "\\?\C:\Data")>
    <DataRow("D:\", "d:\")>
    Public Sub ValidateDestination_SameDirectory_ReturnsSameAsSource(sourcePath As String, destinationPath As String)
        Assert.AreEqual(DestinationValidationResult.SameAsSource, MirrorRules.ValidateDestination(sourcePath, destinationPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data", "C:\Data\Mirror")>
    <DataRow("C:\Data", "c:\DATA\Sub\Deep")>
    <DataRow("D:\", "D:\Mirror")>
    Public Sub ValidateDestination_SubdirectoryOfSource_ReturnsInsideSource(sourcePath As String, destinationPath As String)
        Assert.AreEqual(DestinationValidationResult.InsideSource, MirrorRules.ValidateDestination(sourcePath, destinationPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data\Music", "C:\Data")>
    <DataRow("C:\Data\Music", "c:\data\")>
    <DataRow("C:\Data", "C:\")>
    Public Sub ValidateDestination_ParentOfSource_ReturnsParentOfSource(sourcePath As String, destinationPath As String)
        Assert.AreEqual(DestinationValidationResult.ParentOfSource, MirrorRules.ValidateDestination(sourcePath, destinationPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data", "D:\Mirror")>
    <DataRow("C:\Data\Music", "C:\Other")>
    <DataRow("D:\", "E:\")>
    <DataRow("D:\", "E:\Mirror")>
    <DataRow("C:\A\B", "C:\")>
    <DataRow("C:\Data", "\\Server\Share\Mirror")>
    Public Sub ValidateDestination_UnrelatedDirectory_ReturnsValid(sourcePath As String, destinationPath As String)
        Assert.AreEqual(DestinationValidationResult.Valid, MirrorRules.ValidateDestination(sourcePath, destinationPath))
    End Sub

    <TestMethod>
    <DataRow("C:\Data", "C:\Data2")>
    <DataRow("C:\Data", "C:\Database")>
    <DataRow("C:\Data", "C:\Data - Mirror")>
    Public Sub ValidateDestination_SiblingSharingNamePrefix_ReturnsValid(sourcePath As String, destinationPath As String)
        ' Regression test: the previous implementation used String.StartsWith without a trailing separator,
        ' so a sibling directory whose name starts with the source directory name was rejected as a subfolder.
        Assert.AreEqual(DestinationValidationResult.Valid, MirrorRules.ValidateDestination(sourcePath, destinationPath))
    End Sub

    <TestMethod>
    Public Sub ValidateDestination_DifferentCaseOfSameDirectory_ReturnsSameAsSource()
        ' Regression test: the previous implementation used case-sensitive comparisons,
        ' but Windows file system paths are case-insensitive.
        Assert.AreEqual(DestinationValidationResult.SameAsSource, MirrorRules.ValidateDestination("C:\Data", "C:\DATA"))
    End Sub

#End Region

#Region " IsExcludedSystemDirectory "

    <TestMethod>
    <DataRow("D:\$Recycle.Bin")>
    <DataRow("D:\$RECYCLE.BIN\S-1-5-21-1000")>
    <DataRow("C:\RECYCLER")>
    <DataRow("C:\recycler\S-1-5-21-1000")>
    <DataRow("C:\RECYCLED")>
    <DataRow("C:\Recycled\Sub")>
    <DataRow("E:\System Volume Information")>
    <DataRow("E:\system volume information\SPP")>
    <DataRow("\\?\E:\System Volume Information")>
    <DataRow("D:\$Recycle.Bin\")>
    <DataRow("\\Server\Share\$Recycle.Bin")>
    Public Sub IsExcludedSystemDirectory_SystemDirectoryAtVolumeRoot_ReturnsTrue(dirPath As String)
        Assert.IsTrue(MirrorRules.IsExcludedSystemDirectory(dirPath))
    End Sub

    <TestMethod>
    <DataRow("D:\")>
    <DataRow("D:\Music")>
    <DataRow("D:\Data\$Recycle.Bin")>
    <DataRow("D:\Data\System Volume Information")>
    Public Sub IsExcludedSystemDirectory_RegularDirectory_ReturnsFalse(dirPath As String)
        Assert.IsFalse(MirrorRules.IsExcludedSystemDirectory(dirPath))
    End Sub

    <TestMethod>
    <DataRow("D:\Recycled Photos")>
    <DataRow("D:\Recyclers")>
    <DataRow("D:\RECYCLER Backup")>
    <DataRow("D:\$Recycle.Bin.Old")>
    <DataRow("D:\System Volume Information Backup")>
    Public Sub IsExcludedSystemDirectory_UserDirectoryWithSimilarName_ReturnsFalse(dirPath As String)
        ' Regression test: the previous implementation used patterns like "?:\recycled*",
        ' so any user directory at the root of a drive whose name started with these words was silently ignored.
        Assert.IsFalse(MirrorRules.IsExcludedSystemDirectory(dirPath))
    End Sub

    <TestMethod>
    Public Sub ExcludedSystemDirectoryNames_ContainsExpectedNames()
        CollectionAssert.AreEquivalent({"$Recycle.Bin", "RECYCLER", "RECYCLED", "System Volume Information"},
                                       MirrorRules.ExcludedSystemDirectoryNames.ToArray())
    End Sub

    <TestMethod>
    Public Sub ExcludedSystemDirectoryNames_IsReadOnly()
        Dim list As IList(Of String) = DirectCast(MirrorRules.ExcludedSystemDirectoryNames, IList(Of String))
        Assert.ThrowsExactly(Of NotSupportedException)(Sub() list.Add("Other"))
    End Sub

#End Region

#Region " IsVolumeRoot "

    <TestMethod>
    <DataRow("D:\")>
    <DataRow("d:\")>
    <DataRow("D:")>
    <DataRow("\\?\D:\")>
    <DataRow("\\Server\Share")>
    <DataRow("\\Server\Share\")>
    Public Sub IsVolumeRoot_VolumeRoot_ReturnsTrue(dirPath As String)
        Assert.IsTrue(MirrorRules.IsVolumeRoot(dirPath))
    End Sub

    <TestMethod>
    <DataRow("D:\Music")>
    <DataRow("D:\Music\")>
    <DataRow("\\?\D:\Music")>
    <DataRow("\\Server\Share\Dir")>
    Public Sub IsVolumeRoot_RegularDirectory_ReturnsFalse(dirPath As String)
        Assert.IsFalse(MirrorRules.IsVolumeRoot(dirPath))
    End Sub

#End Region

End Class

#End Region
