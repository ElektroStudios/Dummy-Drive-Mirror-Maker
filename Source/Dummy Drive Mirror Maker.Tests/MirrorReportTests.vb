#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Threading.Tasks

#End Region

#Region " MirrorReportTests "

''' <summary>
''' Unit tests for <see cref="MirrorReport"/> and <see cref="MirrorReportForm"/>,
''' which register and show the files and directories ignored (not copied) by a mirror operation.
''' </summary>
<TestClass>
Public NotInheritable Class MirrorReportTests

#Region " Helpers "

    ''' <summary>
    ''' Creates an empty report.
    ''' </summary>
    ''' <returns>
    ''' The report.
    ''' </returns>
    Private Shared Function CreateReport() As MirrorReport
        Return New MirrorReport("C:\Source", "D:\Mirror", New Date(2026, 11, 8, 18, 30, 5))
    End Function

#End Region

#Region " Constructor "

    <TestMethod>
    Public Sub Constructor_InvalidArguments_ThrowArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New MirrorReport(Nothing, "D:\Mirror", Date.Now))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New MirrorReport(" ", "D:\Mirror", Date.Now))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New MirrorReport("C:\Source", "", Date.Now))
    End Sub

    <TestMethod>
    Public Sub NewReport_IsEmpty()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()

        Assert.IsFalse(report.HasEntries)
        Assert.AreEqual(0, report.Entries.Count)
        Assert.AreEqual(0, report.IgnoredDirectoryCount)
        Assert.AreEqual(0, report.IgnoredFileCount)
        Assert.AreEqual(0, report.ErrorCount)
        Assert.AreEqual(MirrorReportSeverity.None, report.Severity)
        Assert.AreEqual(String.Empty, report.GetSummary())
        Assert.AreEqual(MirrorOutcome.InProgress, report.Outcome)
        StringAssert.Contains(report.ToText(), "No warnings nor errors yet.")
    End Sub

    <TestMethod>
    Public Sub CompletedReportWithoutProblems_SaysThatEverythingWasMirrored()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\Junction", MirrorSkipReason.JunctionNotMirrored)
        report.Complete(MirrorOutcome.Completed, 1)

        StringAssert.Contains(report.ToText(), "All the files and directories were mirrored (except the items excluded by the options or by design).")
    End Sub

#End Region

#Region " Add "

    <TestMethod>
    Public Sub Add_CountsDirectoriesAndFilesSeparately()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\Hidden", MirrorSkipReason.HiddenItem)
        report.Add(MirrorItemKind.Directory, "C:\Source\Junction", MirrorSkipReason.JunctionNotMirrored)
        report.Add(MirrorItemKind.File, "C:\Source\File.txt", MirrorSkipReason.DestinationItemKept)

        Assert.IsTrue(report.HasEntries)
        Assert.AreEqual(3, report.Entries.Count)
        Assert.AreEqual(2, report.IgnoredDirectoryCount)
        Assert.AreEqual(1, report.IgnoredFileCount)
        Assert.AreEqual(0, report.ErrorCount)
    End Sub

    <TestMethod>
    Public Sub Add_PathWithLongPathPrefix_StoresPathWithoutPrefix()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.File, "\\?\C:\Source\File.txt", MirrorSkipReason.Error, "Some error.")

        Dim entry As MirrorReportEntry = report.Entries(0)
        Assert.AreEqual("C:\Source\File.txt", entry.Path)
        Assert.AreEqual("Some error.", entry.Details)
        Assert.IsTrue(entry.IsError)
    End Sub

    <TestMethod>
    Public Sub Add_InvalidPath_ThrowsArgumentNullException()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() report.Add(MirrorItemKind.File, Nothing, MirrorSkipReason.Error))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() report.Add(MirrorItemKind.File, "  ", MirrorSkipReason.Error))
    End Sub

    <TestMethod>
    Public Sub Add_FromSeveralThreads_KeepsAllTheEntries()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()

        Parallel.For(0, 1000, Sub(i As Integer) report.Add(MirrorItemKind.File, $"C:\Source\File{i}.txt", MirrorSkipReason.HiddenItem))

        Assert.AreEqual(1000, report.IgnoredFileCount)
    End Sub

    <TestMethod>
    Public Sub Entries_IsASnapshot()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.File, "C:\Source\A.txt", MirrorSkipReason.HiddenItem)

        Dim snapshot As IReadOnlyList(Of MirrorReportEntry) = report.Entries
        report.Add(MirrorItemKind.File, "C:\Source\B.txt", MirrorSkipReason.HiddenItem)

        Assert.AreEqual(1, snapshot.Count)
        Assert.AreEqual(2, report.Entries.Count)
    End Sub

#End Region

#Region " Severity "

    <TestMethod>
    <DataRow(CInt(MirrorSkipReason.DestinationItemKept), CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorSkipReason.HiddenItem), CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorSkipReason.SymbolicLinkNotMirrored), CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorSkipReason.JunctionNotMirrored), CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorSkipReason.VolumeMountPoint), CInt(MirrorReportSeverity.Warning))>
    <DataRow(CInt(MirrorSkipReason.SystemDirectory), CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorSkipReason.Error), CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorSkipReason.AccessDenied), CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorSkipReason.ListingFailed), CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorSkipReason.TimestampsNotApplied), CInt(MirrorReportSeverity.Error))>
    Public Sub Severity_DependsOnTheReason(reasonValue As Integer, expected As Integer)
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\Item", CType(reasonValue, MirrorSkipReason))

        Assert.AreEqual(CType(expected, MirrorReportSeverity), report.Severity)
        Assert.AreEqual(expected = CInt(MirrorReportSeverity.Error), MirrorReport.IsErrorReason(CType(reasonValue, MirrorSkipReason)))
    End Sub

    <TestMethod>
    Public Sub Severity_ErrorWinsOverWarning()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\MountPoint", MirrorSkipReason.VolumeMountPoint)
        report.Add(MirrorItemKind.File, "C:\Source\Locked.txt", MirrorSkipReason.Error)

        Assert.AreEqual(MirrorReportSeverity.Error, report.Severity)
        Assert.AreEqual(1, report.ErrorCount)
    End Sub

#End Region

#Region " Text "

    <TestMethod>
    <DataRow(1, 1, "1 directory and 1 file could not be mirrored")>
    <DataRow(1, 0, "1 directory could not be mirrored")>
    <DataRow(0, 1, "1 file could not be mirrored")>
    <DataRow(2, 0, "2 directories could not be mirrored")>
    <DataRow(0, 3, "3 files could not be mirrored")>
    <DataRow(1200, 3, "1,200 directories and 3 files could not be mirrored")>
    Public Sub GetSummary_UsesSingularAndPluralFormsAndOmitsZeros(directories As Integer, files As Integer, expected As String)
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        For i As Integer = 1 To directories
            report.Add(MirrorItemKind.Directory, $"C:\Source\Dir{i}", MirrorSkipReason.VolumeMountPoint)
        Next i
        For i As Integer = 1 To files
            report.Add(MirrorItemKind.File, $"C:\Source\File{i}.txt", MirrorSkipReason.Error)
        Next i

        Assert.AreEqual(expected, report.GetSummary())
    End Sub

    <TestMethod>
    Public Sub ExcludedItems_AreOnlyInformative()
        ' Items excluded by the options chosen by the user are not warnings: 
        ' no yellow progress bar, no summary in the final message, but they are listed in the report.
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\Junction", MirrorSkipReason.JunctionNotMirrored)
        report.Add(MirrorItemKind.Directory, "C:\Source\SymLink", MirrorSkipReason.SymbolicLinkNotMirrored)
        report.Add(MirrorItemKind.File, "C:\Source\Hidden.txt", MirrorSkipReason.HiddenItem)
        report.Add(MirrorItemKind.File, "C:\Source\Kept.txt", MirrorSkipReason.DestinationItemKept)

        Assert.AreEqual(MirrorReportSeverity.None, report.Severity)
        Assert.IsFalse(report.HasProblems)
        Assert.IsTrue(report.HasEntries)
        Assert.AreEqual(4, report.ExcludedCount)
        Assert.AreEqual(String.Empty, report.GetSummary())

        Dim text As String = report.ToText()
        StringAssert.Contains(text, "[Excluded] Directory junctions ('Mirror directory junctions' is disabled) (1)")
        StringAssert.Contains(text, "[Directory] C:\Source\Junction")
    End Sub

    <TestMethod>
    <DataRow(CInt(MirrorSkipReason.HiddenItem), CInt(MirrorEntryCategory.Excluded))>
    <DataRow(CInt(MirrorSkipReason.SymbolicLinkNotMirrored), CInt(MirrorEntryCategory.Excluded))>
    <DataRow(CInt(MirrorSkipReason.JunctionNotMirrored), CInt(MirrorEntryCategory.Excluded))>
    <DataRow(CInt(MirrorSkipReason.DestinationItemKept), CInt(MirrorEntryCategory.Excluded))>
    <DataRow(CInt(MirrorSkipReason.SystemDirectory), CInt(MirrorEntryCategory.Excluded))>
    <DataRow(CInt(MirrorSkipReason.VolumeMountPoint), CInt(MirrorEntryCategory.Warning))>
    <DataRow(CInt(MirrorSkipReason.Error), CInt(MirrorEntryCategory.Error))>
    <DataRow(CInt(MirrorSkipReason.AccessDenied), CInt(MirrorEntryCategory.Error))>
    <DataRow(CInt(MirrorSkipReason.ListingFailed), CInt(MirrorEntryCategory.Error))>
    <DataRow(CInt(MirrorSkipReason.TimestampsNotApplied), CInt(MirrorEntryCategory.Error))>
    Public Sub GetReasonCategory_ReturnsTheExpectedCategory(reason As Integer, expected As Integer)
        Assert.AreEqual(CType(expected, MirrorEntryCategory), MirrorReport.GetReasonCategory(CType(reason, MirrorSkipReason)))
    End Sub

    <TestMethod>
    Public Sub GetReasonCategory_AllReasonsHaveACategory()
        For Each reason As MirrorSkipReason In [Enum].GetValues(GetType(MirrorSkipReason))
            Dim category As MirrorEntryCategory = MirrorReport.GetReasonCategory(reason)
            Assert.IsFalse(String.IsNullOrEmpty(MirrorReport.GetCategoryLabel(category)))
        Next reason
        Assert.ThrowsExactly(Of ArgumentException)(Function() MirrorReport.GetReasonCategory(CType(999, MirrorSkipReason)))
    End Sub

    <TestMethod>
    Public Sub AddMirrored_RecordsTheMirroredItemsAndTheyAreListedInTheText()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.AddMirrored(MirrorItemKind.Directory, "\\?\C:\Source\Dir")
        report.AddMirrored(MirrorItemKind.File, "C:\Source\Dir\File.txt")
        report.AddMirrored(MirrorItemKind.Directory, "C:\Source\Junction", "Directory junction to 'C:\Target'")

        Assert.AreEqual(2, report.MirroredDirectoryCount)
        Assert.AreEqual(1, report.MirroredFileCount)
        Assert.AreEqual("C:\Source\Dir", report.MirroredEntries(0).Path)
        Assert.IsFalse(report.HasEntries, "Mirrored items are not ignored items.")

        Dim text As String = report.ToText()
        StringAssert.Contains(text, "Mirrored directories : 2")
        StringAssert.Contains(text, "Mirrored files       : 1")
        StringAssert.Contains(text, "=== [Mirrored] Mirrored directories (2) ===")
        StringAssert.Contains(text, "=== [Mirrored] Mirrored files (1) ===")
        StringAssert.Contains(text, "C:\Source\Dir\File.txt")
        StringAssert.Contains(text, "C:\Source\Junction  (Directory junction to 'C:\Target')")
    End Sub

    <TestMethod>
    Public Sub AddMirrored_InvalidPath_ThrowsArgumentNullException()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() report.AddMirrored(MirrorItemKind.File, Nothing))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() report.AddMirrored(MirrorItemKind.File, " "))
    End Sub

    <TestMethod>
    Public Sub ToText_ListsErrorsBeforeWarningsAndExcludedItems()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.File, "C:\Source\Hidden.txt", MirrorSkipReason.HiddenItem)
        report.Add(MirrorItemKind.Directory, "C:\Source\MountPoint", MirrorSkipReason.VolumeMountPoint)
        report.Add(MirrorItemKind.File, "C:\Source\Locked.txt", MirrorSkipReason.Error)

        Dim text As String = report.ToText()
        Dim errorIndex As Integer = text.IndexOf("[Error]", StringComparison.Ordinal)
        Dim warningIndex As Integer = text.IndexOf("[Warning]", StringComparison.Ordinal)
        Dim excludedIndex As Integer = text.IndexOf("[Excluded]", StringComparison.Ordinal)

        Assert.IsTrue(errorIndex >= 0 AndAlso errorIndex < warningIndex AndAlso warningIndex < excludedIndex)
    End Sub

    <TestMethod>
    Public Sub ToText_ContainsHeaderAndEntriesGroupedByReason()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.Directory, "C:\Source\Hidden1", MirrorSkipReason.HiddenItem)
        report.Add(MirrorItemKind.File, "C:\Source\Locked.txt", MirrorSkipReason.Error, "The file is in use.")
        report.Add(MirrorItemKind.Directory, "C:\Source\Hidden2", MirrorSkipReason.HiddenItem)

        Dim text As String = report.ToText()

        StringAssert.Contains(text, "Source      : C:\Source")
        StringAssert.Contains(text, "Destination : D:\Mirror")
        StringAssert.Contains(text, "2026-11-08 18:30:05")
        StringAssert.Contains(text, $"=== [Excluded] {MirrorReport.GetReasonDescription(MirrorSkipReason.HiddenItem)} (2) ===")
        StringAssert.Contains(text, $"=== [Error] {MirrorReport.GetReasonDescription(MirrorSkipReason.Error)} (1) ===")
        StringAssert.Contains(text, "[Directory] C:\Source\Hidden1")
        StringAssert.Contains(text, "C:\Source\Locked.txt")
        StringAssert.Contains(text, "The file is in use.")

        ' Both hidden directories are listed together, under their group.
        Dim groupIndex As Integer = text.IndexOf(MirrorReport.GetReasonDescription(MirrorSkipReason.HiddenItem), StringComparison.Ordinal)
        Assert.IsTrue(text.IndexOf("Hidden1", StringComparison.Ordinal) > groupIndex)
        Assert.IsTrue(text.IndexOf("Hidden2", StringComparison.Ordinal) > groupIndex)
    End Sub

    <TestMethod>
    Public Sub GetReasonDescription_AllReasonsHaveDistinctDescriptions()
        Dim descriptions As New HashSet(Of String)(StringComparer.Ordinal)

        For Each reason As MirrorSkipReason In [Enum].GetValues(GetType(MirrorSkipReason))
            Dim description As String = MirrorReport.GetReasonDescription(reason)
            Assert.IsFalse(String.IsNullOrWhiteSpace(description))
            Assert.IsTrue(descriptions.Add(description), $"Duplicated description: {description}")
        Next reason
    End Sub

    <TestMethod>
    Public Sub GetReasonDescription_InvalidReason_ThrowsArgumentException()
        Assert.ThrowsExactly(Of ArgumentException)(Sub() MirrorReport.GetReasonDescription(CType(999, MirrorSkipReason)))
    End Sub

#End Region

#Region " Statistics and Outcome "

    <TestMethod>
    Public Sub NewReport_HasNoStatisticsAndIsInProgress()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()

        Assert.AreEqual(-1, report.SourceDirectoryCount)
        Assert.AreEqual(MirrorOutcome.InProgress, report.Outcome)
        Assert.IsFalse(report.ToText().Contains("Source subdirectories"))
    End Sub

    <TestMethod>
    Public Sub SetSourceStatistics_CountsLinksAndHiddenDirectories()
        Using tempDir As New Infrastructure.TemporaryDirectory()
            Dim normal As String = tempDir.CreateDirectory("Normal")
            Dim hidden As String = tempDir.CreateDirectory("Hidden")
            IO.File.SetAttributes(hidden, IO.File.GetAttributes(hidden) Or IO.FileAttributes.Hidden)
            Dim junction As String = tempDir.Combine("Junction")
            Infrastructure.LinkFactory.CreateJunction(junction, normal)

            Dim report As MirrorReport = MirrorReportTests.CreateReport()
            report.SetSourceStatistics({New IO.DirectoryInfo(normal), New IO.DirectoryInfo(hidden), New IO.DirectoryInfo(junction)})

            Assert.AreEqual(3, report.SourceDirectoryCount)
            Assert.AreEqual(1, report.SourceLinkCount)
            Assert.AreEqual(1, report.SourceHiddenDirectoryCount)
        End Using
    End Sub

    <TestMethod>
    Public Sub Complete_RecordsTheOutcomeAndItIsShownInTheText()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.SetSourceStatistics(Enumerable.Empty(Of IO.DirectoryInfo)())
        report.Complete(MirrorOutcome.Aborted, 0)

        Assert.AreEqual(MirrorOutcome.Aborted, report.Outcome)
        Dim text As String = report.ToText()
        StringAssert.Contains(text, MirrorReport.GetOutcomeDescription(MirrorOutcome.Aborted))
        StringAssert.Contains(text, "Processed subdirectories : 0")
        StringAssert.Contains(text, "No warnings nor errors before the operation ended.")
    End Sub

    <TestMethod>
    Public Sub Complete_InvalidArguments_Throw()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() report.Complete(MirrorOutcome.InProgress, 0))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() report.Complete(MirrorOutcome.Completed, -1))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() report.SetSourceStatistics(Nothing))
    End Sub

    <TestMethod>
    Public Sub FormatNumber_UsesTheUnitedStatesCultureRegardlessOfTheCurrentCulture()
        Dim previousCulture As Globalization.CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
        Try
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.GetCultureInfo("es-ES")
            Assert.AreEqual("3,294", MirrorReport.FormatNumber(3294))
            Assert.AreEqual("1,234,567", MirrorReport.FormatNumber(1234567))
            Assert.AreEqual("12", MirrorReport.FormatNumber(12))
        Finally
            Threading.Thread.CurrentThread.CurrentCulture = previousCulture
        End Try
    End Sub

    <TestMethod>
    Public Sub ToText_FormatsTheCountsWithThousandsSeparators()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        For i As Integer = 1 To 1234
            report.Add(MirrorItemKind.File, $"C:\Source\File{i}.txt", MirrorSkipReason.HiddenItem)
        Next i

        Dim text As String = report.ToText()
        StringAssert.Contains(text, "Ignored files        : 1,234")
        StringAssert.Contains(text, "(1,234) ===")
    End Sub

#End Region

#Region " MirrorReportForm "

    <TestMethod>
    Public Sub MirrorReportForm_ShowsTheReport()
        Dim report As MirrorReport = MirrorReportTests.CreateReport()
        report.Add(MirrorItemKind.File, "C:\Source\File.txt", MirrorSkipReason.DestinationItemKept)

        Using form As New MirrorReportForm(report, ThemePalette.Dark, useDarkTitleBar:=True)
            Assert.AreEqual(report.ToText(), form.TextBoxReport.Text)
            Assert.IsTrue(form.TextBoxReport.ReadOnly)
            StringAssert.Contains(form.LabelSummary.Text, report.GetSummary())
            Assert.AreSame(form.ButtonClose, form.CancelButton)
            Assert.AreEqual(ThemePalette.Dark.WindowBackColor, form.BackColor)
        End Using
    End Sub

    <TestMethod>
    Public Sub MirrorReportForm_NullArguments_ThrowArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New MirrorReportForm(Nothing, ThemePalette.Light, False))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New MirrorReportForm(MirrorReportTests.CreateReport(), Nothing, False))
    End Sub

    <TestMethod>
    Public Sub GetDefaultFileName_ContainsTheDateAndHasNoInvalidCharacters()
        Dim fileName As String = MirrorReportForm.GetDefaultFileName(New Date(2026, 11, 8, 18, 30, 5))

        Assert.AreEqual("Mirror report 2026-11-08 18.30.05.txt", fileName)
        Assert.AreEqual(-1, fileName.IndexOfAny(IO.Path.GetInvalidFileNameChars()))
    End Sub

#End Region

End Class

#End Region
