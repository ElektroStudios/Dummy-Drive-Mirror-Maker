#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.IO
Imports System.Text

#End Region

#Region " MirrorItemKind "

''' <summary>
''' Specifies the kind of a file system item that was not mirrored.
''' </summary>
Friend Enum MirrorItemKind As Integer ' NOSONAR

    ''' <summary>A directory.</summary>
    Directory = 0

    ''' <summary>A file.</summary>
    File = 1

End Enum

#End Region

#Region " MirrorSkipReason "

''' <summary>
''' Specifies the reason why a file system item was ignored or not copied by a mirror operation.
''' <para></para>
''' The declaration order is the order in which the reasons are listed in the report.
''' </summary>
Friend Enum MirrorSkipReason As Integer ' NOSONAR

    ''' <summary>An exception occurred, and the user chose to continue the operation.</summary>
    [Error] = 0

    ''' <summary>The access was denied, and the exception was ignored by user-demand.</summary>
    AccessDenied = 1

    ''' <summary>The contents of the directory could not be listed.</summary>
    ListingFailed = 2

    ''' <summary>The destination item already exists, and it was kept (the overwrite option is disabled).</summary>
    DestinationItemKept = 3

    ''' <summary>A hidden item (the option to mirror hidden items is disabled).</summary>
    HiddenItem = 4

    ''' <summary>A symbolic link (the option to mirror symbolic links is disabled).</summary>
    SymbolicLinkNotMirrored = 5

    ''' <summary>A directory junction (the option to mirror directory junctions is disabled).</summary>
    JunctionNotMirrored = 6

    ''' <summary>A volume mount point (never mirrored).</summary>
    VolumeMountPoint = 7

    ''' <summary>A system directory, like the recycle bin (always excluded).</summary>
    SystemDirectory = 8

    ''' <summary>The directory was mirrored, but its timestamps could not be applied.</summary>
    TimestampsNotApplied = 9

End Enum

#End Region

#Region " MirrorOutcome "

''' <summary>
''' Specifies how a mirror operation ended.
''' </summary>
Friend Enum MirrorOutcome As Integer ' NOSONAR

    ''' <summary>The operation did not end yet.</summary>
    InProgress = 0

    ''' <summary>The operation processed all the directories.</summary>
    Completed = 1

    ''' <summary>The operation was aborted (by the user, or after an error the user chose not to ignore).</summary>
    Aborted = 2

    ''' <summary>The operation terminated due an unexpected error.</summary>
    Failed = 3

End Enum

#End Region

#Region " MirrorEntryCategory "

''' <summary>
''' Specifies the category of an item that was not mirrored, according to its <see cref="MirrorSkipReason"/>.
''' </summary>
Friend Enum MirrorEntryCategory As Integer ' NOSONAR

    ''' <summary>
    ''' The item was intentionally excluded by the options chosen by the user 
    ''' (e.g. a directory junction with the option to mirror directory junctions disabled), or by design 
    ''' (e.g. the recycle bin). It is only informative: it is not a warning nor an error.
    ''' </summary>
    Excluded = 0

    ''' <summary>The item could not be mirrored, but not due to an error (e.g. a volume mount point).</summary>
    Warning = 1

    ''' <summary>The item could not be mirrored due to an error.</summary>
    [Error] = 2

End Enum

#End Region

#Region " MirrorReportSeverity "

''' <summary>
''' Specifies the overall result of a mirror operation, according to its ignored items.
''' </summary>
Friend Enum MirrorReportSeverity As Integer ' NOSONAR

    ''' <summary>All the items were mirrored, except the items excluded by the options or by design.</summary>
    None = 0

    ''' <summary>Some items could not be mirrored (see <see cref="MirrorEntryCategory.Warning"/>).</summary>
    Warning = 1

    ''' <summary>Some items could not be mirrored due to errors.</summary>
    [Error] = 2

End Enum

#End Region

#Region " MirrorReportEntry "

''' <summary>
''' Represents a file system item that was ignored or not copied by a mirror operation.
''' </summary>
Friend NotInheritable Class MirrorReportEntry ' NOSONAR

    ''' <summary>Gets the kind of item.</summary>
    Friend ReadOnly Property Kind As MirrorItemKind

    ''' <summary>Gets the source path of the item (without the long path prefix).</summary>
    Friend ReadOnly Property Path As String

    ''' <summary>Gets the reason why the item was not mirrored.</summary>
    Friend ReadOnly Property Reason As MirrorSkipReason

    ''' <summary>Gets additional details (e.g. an exception message), or an empty string.</summary>
    Friend ReadOnly Property Details As String

    ''' <summary>Gets a value indicating whether the item was not mirrored due to an error.</summary>
    Friend ReadOnly Property IsError As Boolean
        Get
            Return MirrorReport.IsErrorReason(Me.Reason)
        End Get
    End Property

    ''' <summary>Gets the category of the item (excluded, warning or error).</summary>
    Friend ReadOnly Property Category As MirrorEntryCategory
        Get
            Return MirrorReport.GetReasonCategory(Me.Reason)
        End Get
    End Property

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MirrorReportEntry"/> class.
    ''' </summary>
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    '''
    ''' <param name="path">
    ''' The source path of the item.
    ''' </param>
    '''
    ''' <param name="reason">
    ''' The reason why the item was not mirrored.
    ''' </param>
    '''
    ''' <param name="details">
    ''' Additional details, or <see langword="Nothing"/>.
    ''' </param>
    Friend Sub New(kind As MirrorItemKind, path As String, reason As MirrorSkipReason, details As String)
        Me.Kind = kind
        Me.Path = path
        Me.Reason = reason
        Me.Details = If(details, String.Empty)
    End Sub

End Class

#End Region

#Region " MirroredItemEntry "

''' <summary>
''' Represents a file system item that was successfully mirrored.
''' </summary>
Friend NotInheritable Class MirroredItemEntry

    ''' <summary>Gets the kind of item.</summary>
    Friend ReadOnly Property Kind As MirrorItemKind

    ''' <summary>Gets the source path of the item (without the long path prefix).</summary>
    Friend ReadOnly Property Path As String

    ''' <summary>Gets additional details (e.g. "Directory junction"), or an empty string.</summary>
    Friend ReadOnly Property Details As String

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MirroredItemEntry"/> class.
    ''' </summary>
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    '''
    ''' <param name="path">
    ''' The source path of the item.
    ''' </param>
    '''
    ''' <param name="details">
    ''' Additional details, or <see langword="Nothing"/>.
    ''' </param>
    Friend Sub New(kind As MirrorItemKind, path As String, details As String)
        Me.Kind = kind
        Me.Path = path
        Me.Details = If(details, String.Empty)
    End Sub

End Class

#End Region

#Region " MirrorReport "

''' <summary>
''' Records in memory the files and directories that were mirrored by a mirror operation, 
''' and the files and directories that were ignored or not copied, with the reason of each one.
''' <para></para>
''' This class is thread-safe: entries are added by the background work and read by the UI thread.
''' </summary>
Friend NotInheritable Class MirrorReport

#Region " Properties "

    ''' <summary>Gets the source directory of the mirror operation.</summary>
    Friend ReadOnly Property SourcePath As String

    ''' <summary>Gets the destination directory of the mirror operation.</summary>
    Friend ReadOnly Property DestinationPath As String

    ''' <summary>Gets the date and time when the mirror operation started.</summary>
    Friend ReadOnly Property StartTime As Date

    ''' <summary>
    ''' Gets the culture used to format the numbers of the report (and of the progress bar): en-US, 
    ''' with a comma as thousands separator (e.g. <c>3,294</c>).
    ''' </summary>
    Friend Shared ReadOnly Property NumberCulture As CultureInfo = CultureInfo.GetCultureInfo("en-US")

    ''' <summary>Gets how the mirror operation ended.</summary>
    Friend ReadOnly Property Outcome As MirrorOutcome
        Get
            SyncLock Me.syncObject
                Return Me.outcomeValue
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets the number of processed subdirectories of the source directory (mirrored or ignored).</summary>
    Friend ReadOnly Property ProcessedDirectoryCount As Integer
        Get
            SyncLock Me.syncObject
                Return Me.processedDirectoryCountValue
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Gets the number of subdirectories found in the source directory (the source directory itself is not counted), 
    ''' or -1 if the source directory was not listed yet.
    ''' </summary>
    Friend ReadOnly Property SourceDirectoryCount As Integer
        Get
            SyncLock Me.syncObject
                Return Me.sourceDirectoryCountValue
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets how many of the source subdirectories are links (directory junctions, symbolic links or mount points).</summary>
    Friend ReadOnly Property SourceLinkCount As Integer
        Get
            SyncLock Me.syncObject
                Return Me.sourceLinkCountValue
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets how many of the source subdirectories are hidden.</summary>
    Friend ReadOnly Property SourceHiddenDirectoryCount As Integer
        Get
            SyncLock Me.syncObject
                Return Me.sourceHiddenCountValue
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' Gets a snapshot of the recorded entries, in the order they were added.
    ''' </summary>
    Friend ReadOnly Property Entries As IReadOnlyList(Of MirrorReportEntry)
        Get
            SyncLock Me.syncObject
                Return Me.entriesList.ToArray()
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets the number of ignored directories.</summary>
    Friend ReadOnly Property IgnoredDirectoryCount As Integer
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Kind = MirrorItemKind.Directory)
        End Get
    End Property

    ''' <summary>Gets the number of ignored files.</summary>
    Friend ReadOnly Property IgnoredFileCount As Integer
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Kind = MirrorItemKind.File)
        End Get
    End Property

    ''' <summary>Gets the number of items that were not mirrored due to errors.</summary>
    Friend ReadOnly Property ErrorCount As Integer
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Category = MirrorEntryCategory.Error)
        End Get
    End Property

    ''' <summary>Gets the number of items that could not be mirrored, but not due to errors.</summary>
    Friend ReadOnly Property WarningCount As Integer
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Category = MirrorEntryCategory.Warning)
        End Get
    End Property

    ''' <summary>Gets the number of items that were excluded by the options chosen by the user, or by design.</summary>
    Friend ReadOnly Property ExcludedCount As Integer
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Category = MirrorEntryCategory.Excluded)
        End Get
    End Property

    ''' <summary>Gets a value indicating whether any item could not be mirrored due to a warning or an error.</summary>
    Friend ReadOnly Property HasProblems As Boolean
        Get
            Return Me.CountEntries(Function(entry As MirrorReportEntry) entry.Category <> MirrorEntryCategory.Excluded) > 0
        End Get
    End Property

    ''' <summary>
    ''' Gets a snapshot of the successfully mirrored items, in the order they were mirrored.
    ''' </summary>
    Friend ReadOnly Property MirroredEntries As IReadOnlyList(Of MirroredItemEntry)
        Get
            SyncLock Me.syncObject
                Return Me.mirroredList.ToArray()
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets the number of successfully mirrored directories (the source directory itself is not counted).</summary>
    Friend ReadOnly Property MirroredDirectoryCount As Integer
        Get
            SyncLock Me.syncObject
                Return Enumerable.Count(Me.mirroredList, Function(entry As MirroredItemEntry) entry.Kind = MirrorItemKind.Directory)
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets the number of successfully mirrored files.</summary>
    Friend ReadOnly Property MirroredFileCount As Integer
        Get
            SyncLock Me.syncObject
                Return Enumerable.Count(Me.mirroredList, Function(entry As MirroredItemEntry) entry.Kind = MirrorItemKind.File)
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets a value indicating whether any item was ignored or not copied.</summary>
    Friend ReadOnly Property HasEntries As Boolean
        Get
            SyncLock Me.syncObject
                Return Me.entriesList.Count > 0
            End SyncLock
        End Get
    End Property

    ''' <summary>Gets the overall result of the mirror operation.</summary>
    Friend ReadOnly Property Severity As MirrorReportSeverity
        Get
            If Me.ErrorCount > 0 Then
                Return MirrorReportSeverity.Error
            End If
            ' The items excluded by the options (or by design) are only informative.
            Return If(Me.WarningCount > 0, MirrorReportSeverity.Warning, MirrorReportSeverity.None)
        End Get
    End Property

#End Region

#Region " Fields "

    ''' <summary>The recorded entries.</summary>
    Private ReadOnly entriesList As New List(Of MirrorReportEntry)

    ''' <summary>The successfully mirrored items.</summary>
    Private ReadOnly mirroredList As New List(Of MirroredItemEntry)

    ''' <summary>The object used to synchronize the access to the entries.</summary>
    Private ReadOnly syncObject As New Object()

    ''' <summary>The value of <see cref="MirrorReport.Outcome"/>.</summary>
    Private outcomeValue As MirrorOutcome = MirrorOutcome.InProgress

    ''' <summary>The value of <see cref="MirrorReport.ProcessedDirectoryCount"/>.</summary>
    Private processedDirectoryCountValue As Integer

    ''' <summary>The value of <see cref="MirrorReport.SourceDirectoryCount"/>.</summary>
    Private sourceDirectoryCountValue As Integer = -1

    ''' <summary>The value of <see cref="MirrorReport.SourceLinkCount"/>.</summary>
    Private sourceLinkCountValue As Integer

    ''' <summary>The value of <see cref="MirrorReport.SourceHiddenDirectoryCount"/>.</summary>
    Private sourceHiddenCountValue As Integer

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MirrorReport"/> class.
    ''' </summary>
    ''' <param name="sourcePath">
    ''' The source directory of the mirror operation.
    ''' </param>
    '''
    ''' <param name="destinationPath">
    ''' The destination directory of the mirror operation.
    ''' </param>
    '''
    ''' <param name="startTime">
    ''' The date and time when the mirror operation started.
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="sourcePath"/> or <paramref name="destinationPath"/> is null, empty or white-spaced.
    ''' </exception>
    Friend Sub New(sourcePath As String, destinationPath As String, startTime As Date)

        If String.IsNullOrWhiteSpace(sourcePath) Then
            Throw New ArgumentNullException(NameOf(sourcePath), "Value is null, empty, or white-spaced.")
        End If

        If String.IsNullOrWhiteSpace(destinationPath) Then
            Throw New ArgumentNullException(NameOf(destinationPath), "Value is null, empty, or white-spaced.")
        End If

        Me.SourcePath = sourcePath
        Me.DestinationPath = destinationPath
        Me.StartTime = startTime

    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Records an item that was ignored or not copied.
    ''' </summary>
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    '''
    ''' <param name="path">
    ''' The source path of the item. The long path prefix is removed.
    ''' </param>
    '''
    ''' <param name="reason">
    ''' The reason why the item was not mirrored.
    ''' </param>
    '''
    ''' <param name="details">
    ''' Additional details (e.g. an exception message).
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="path"/> is null, empty or white-spaced.
    ''' </exception>
    Friend Sub Add(kind As MirrorItemKind, path As String, reason As MirrorSkipReason, Optional details As String = Nothing)

        If String.IsNullOrWhiteSpace(path) Then
            Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
        End If

        Dim entry As New MirrorReportEntry(kind, MirrorRules.RemoveLongPathPrefix(path), reason, details)
        SyncLock Me.syncObject
            Me.entriesList.Add(entry)
        End SyncLock

    End Sub

    ''' <summary>
    ''' Records an item that was successfully mirrored.
    ''' </summary>
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    ''' 
    ''' <param name="path">
    ''' The source path of the item. The long path prefix is removed.
    ''' </param>
    ''' 
    ''' <param name="details">
    ''' Optional details (e.g. "Directory junction").
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="path"/> is <see langword="Nothing"/>, empty or white-spaced.
    ''' </exception>
    Friend Sub AddMirrored(kind As MirrorItemKind, path As String, Optional details As String = Nothing)

        If String.IsNullOrWhiteSpace(path) Then
            Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
        End If

        Dim entry As New MirroredItemEntry(kind, MirrorRules.RemoveLongPathPrefix(path), details)
        SyncLock Me.syncObject
            Me.mirroredList.Add(entry)
        End SyncLock

    End Sub

    ''' <summary>
    ''' Records the statistics of the listed subdirectories of the source directory.
    ''' </summary>
    ''' <param name="subdirectories">
    ''' The listed subdirectories (without the source directory itself).
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="subdirectories"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub SetSourceStatistics(subdirectories As IEnumerable(Of DirectoryInfo))

        If subdirectories Is Nothing Then
            Throw New ArgumentNullException(NameOf(subdirectories))
        End If

        Dim total As Integer
        Dim links As Integer
        Dim hidden As Integer
        For Each directory As DirectoryInfo In subdirectories
            total += 1
            Dim attributes As FileAttributes = directory.Attributes
            If attributes.HasFlag(FileAttributes.ReparsePoint) Then
                links += 1
            End If
            If attributes.HasFlag(FileAttributes.Hidden) Then
                hidden += 1
            End If
        Next directory

        SyncLock Me.syncObject
            Me.sourceDirectoryCountValue = total
            Me.sourceLinkCountValue = links
            Me.sourceHiddenCountValue = hidden
        End SyncLock

    End Sub

    ''' <summary>
    ''' Records how the mirror operation ended.
    ''' </summary>
    ''' <param name="outcome">
    ''' How the operation ended.
    ''' </param>
    ''' 
    ''' <param name="processedDirectoryCount">
    ''' The number of processed subdirectories of the source directory.
    ''' </param>
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="outcome"/> is <see cref="MirrorOutcome.InProgress"/> or not a valid value.
    ''' </exception>
    ''' 
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when <paramref name="processedDirectoryCount"/> is negative.
    ''' </exception>
    Friend Sub Complete(outcome As MirrorOutcome, processedDirectoryCount As Integer)

        If (outcome <> MirrorOutcome.Completed) AndAlso (outcome <> MirrorOutcome.Aborted) AndAlso (outcome <> MirrorOutcome.Failed) Then
            Throw New ArgumentException($"Invalid outcome: '{outcome}'.", NameOf(outcome))
        End If

        If processedDirectoryCount < 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(processedDirectoryCount), processedDirectoryCount, "Value must be zero or greater.")
        End If

        SyncLock Me.syncObject
            Me.outcomeValue = outcome
            Me.processedDirectoryCountValue = processedDirectoryCount
        End SyncLock

    End Sub

    ''' <summary>
    ''' Builds the short summary of the items that could not be mirrored due to warnings or errors 
    ''' (e.g. "3 directories and 12 files could not be mirrored"). 
    ''' The items excluded by the options (or by design) are not included.
    ''' </summary>
    ''' <returns>
    ''' The summary, or an empty string if there is no warning nor error.
    ''' </returns>
    Friend Function GetSummary() As String

        Dim problems As MirrorReportEntry() = Me.Entries.Where(Function(entry As MirrorReportEntry) entry.Category <> MirrorEntryCategory.Excluded).ToArray()
        Dim directories As Integer = problems.Count(Function(entry As MirrorReportEntry) entry.Kind = MirrorItemKind.Directory)
        Dim files As Integer = problems.Length - directories
        If problems.Length = 0 Then
            Return String.Empty
        End If

        Dim parts As New List(Of String)(2)
        If directories > 0 Then
            parts.Add($"{MirrorReport.FormatNumber(directories)} {If(directories = 1, "directory", "directories")}")
        End If
        If files > 0 Then
            parts.Add($"{MirrorReport.FormatNumber(files)} {If(files = 1, "file", "files")}")
        End If

        Return $"{String.Join(" and ", parts)} could not be mirrored"

    End Function

    ''' <summary>
    ''' Builds the full text of the report, with the entries grouped by reason.
    ''' </summary>
    ''' <returns>
    ''' The text of the report.
    ''' </returns>
    Friend Function ToText() As String

        Dim entries As IReadOnlyList(Of MirrorReportEntry) = Me.Entries
        Dim mirrored As IReadOnlyList(Of MirroredItemEntry) = Me.MirroredEntries
        Dim mirroredDirectories As Integer = Enumerable.Count(mirrored, Function(item As MirroredItemEntry) item.Kind = MirrorItemKind.Directory)
        Dim mirroredFiles As Integer = mirrored.Count - mirroredDirectories

        Dim sb As New StringBuilder()
        sb.AppendLine("Dummy Drive Mirror Maker - Mirror report")
        sb.AppendLine()
        sb.AppendLine($"Date        : {Me.StartTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}")
        sb.AppendLine($"Source      : {Me.SourcePath}")
        sb.AppendLine($"Destination : {Me.DestinationPath}")
        sb.AppendLine($"Result      : {MirrorReport.GetOutcomeDescription(Me.Outcome)}")
        sb.AppendLine()

        Dim sourceDirectories As Integer = Me.SourceDirectoryCount
        If sourceDirectories >= 0 Then
            sb.AppendLine($"Source subdirectories : {MirrorReport.FormatNumber(sourceDirectories)} (the source directory itself is not counted)")
            sb.AppendLine($"  Links (directory junctions, symbolic links and mount points) : {MirrorReport.FormatNumber(Me.SourceLinkCount)}")
            sb.AppendLine($"  Hidden directories                                          : {MirrorReport.FormatNumber(Me.SourceHiddenDirectoryCount)}")
            sb.AppendLine($"Processed subdirectories : {MirrorReport.FormatNumber(Me.ProcessedDirectoryCount)} (excluded subdirectories are not processed)")
            sb.AppendLine()
        End If

        sb.AppendLine($"Mirrored directories : {MirrorReport.FormatNumber(mirroredDirectories)}")
        sb.AppendLine($"Mirrored files       : {MirrorReport.FormatNumber(mirroredFiles)}")
        sb.AppendLine($"Ignored directories  : {MirrorReport.FormatNumber(Me.IgnoredDirectoryCount)}")
        sb.AppendLine($"Ignored files        : {MirrorReport.FormatNumber(Me.IgnoredFileCount)}")
        sb.AppendLine($"  Excluded by the options or by design : {MirrorReport.FormatNumber(Me.ExcludedCount)}")
        sb.AppendLine($"  Warnings                             : {MirrorReport.FormatNumber(Me.WarningCount)}")
        sb.AppendLine($"  Errors                               : {MirrorReport.FormatNumber(Me.ErrorCount)}")

        sb.AppendLine()
        If Not Me.HasProblems Then
            Select Case Me.Outcome
                Case MirrorOutcome.Completed
                    sb.AppendLine("All the files and directories were mirrored (except the items excluded by the options or by design).")
                Case MirrorOutcome.InProgress
                    sb.AppendLine("No warnings nor errors yet.")
                Case Else
                    sb.AppendLine("No warnings nor errors before the operation ended.")
            End Select
        End If

        ' Ignored items, grouped by reason: errors first, then warnings, then the excluded items.
        For Each group As IGrouping(Of MirrorSkipReason, MirrorReportEntry) In
            entries.GroupBy(Function(entry As MirrorReportEntry) entry.Reason).
                    OrderByDescending(Function(g As IGrouping(Of MirrorSkipReason, MirrorReportEntry)) MirrorReport.GetReasonCategory(g.Key)).
                    ThenBy(Function(g As IGrouping(Of MirrorSkipReason, MirrorReportEntry)) g.Key)

            Dim categoryLabel As String = MirrorReport.GetCategoryLabel(MirrorReport.GetReasonCategory(group.Key))
            sb.AppendLine()
            sb.AppendLine($"=== [{categoryLabel}] {MirrorReport.GetReasonDescription(group.Key)} ({MirrorReport.FormatNumber(group.Count())}) ===")
            For Each entry As MirrorReportEntry In group
                sb.AppendLine($"{MirrorReport.GetKindLabel(entry.Kind)} {entry.Path}")
                If Not String.IsNullOrEmpty(entry.Details) Then
                    sb.AppendLine($"            {entry.Details}")
                End If
            Next entry
        Next group

        ' Successfully mirrored items.
        MirrorReport.AppendMirroredSection(sb, "Mirrored directories", mirrored.Where(Function(item As MirroredItemEntry) item.Kind = MirrorItemKind.Directory), mirroredDirectories)
        MirrorReport.AppendMirroredSection(sb, "Mirrored files", mirrored.Where(Function(item As MirroredItemEntry) item.Kind = MirrorItemKind.File), mirroredFiles)

        Return sb.ToString()

    End Function

    ''' <summary>
    ''' Appends a section with a list of mirrored items to the report text.
    ''' </summary>
    ''' <param name="sb">
    ''' The report text.
    ''' </param>
    ''' 
    ''' <param name="title">
    ''' The title of the section.
    ''' </param>
    ''' 
    ''' <param name="items">
    ''' The mirrored items.
    ''' </param>
    ''' 
    ''' <param name="count">
    ''' The number of items.
    ''' </param>
    Private Shared Sub AppendMirroredSection(sb As StringBuilder, title As String, items As IEnumerable(Of MirroredItemEntry), count As Integer)

        If count = 0 Then
            Exit Sub
        End If

        sb.AppendLine()
        sb.AppendLine($"=== [Mirrored] {title} ({MirrorReport.FormatNumber(count)}) ===")
        For Each item As MirroredItemEntry In items
            If String.IsNullOrEmpty(item.Details) Then
                sb.AppendLine(item.Path)
            Else
                sb.AppendLine($"{item.Path}  ({item.Details})")
            End If
        Next item

    End Sub

    ''' <summary>
    ''' Gets the label of a kind of item in the report text.
    ''' </summary>
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    ''' <returns>
    ''' The label (padded to the same width for every kind).
    ''' </returns>
    Private Shared Function GetKindLabel(kind As MirrorItemKind) As String
        Return If(kind = MirrorItemKind.Directory, "[Directory]", "[File]     ")
    End Function

    ''' <summary>
    ''' Formats a number with thousands separators, using the <see cref="MirrorReport.NumberCulture"/> (e.g. <c>3,294</c>).
    ''' </summary>
    ''' <param name="value">
    ''' The number.
    ''' </param>
    ''' <returns>
    ''' The formatted number.
    ''' </returns>
    Friend Shared Function FormatNumber(value As Integer) As String
        Return value.ToString("N0", MirrorReport.NumberCulture)
    End Function

    ''' <summary>
    ''' Gets the description of how a mirror operation ended.
    ''' </summary>
    ''' <param name="outcome">
    ''' How the operation ended.
    ''' </param>
    ''' <returns>
    ''' The description.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="outcome"/> is not a valid value.
    ''' </exception>
    Friend Shared Function GetOutcomeDescription(outcome As MirrorOutcome) As String

        Select Case outcome
            Case MirrorOutcome.InProgress
                Return "In progress"
            Case MirrorOutcome.Completed
                Return "Completed"
            Case MirrorOutcome.Aborted
                Return "Aborted (the remaining directories were not processed)"
            Case MirrorOutcome.Failed
                Return "Failed due an unexpected error (the remaining directories were not processed)"
            Case Else
                Throw New ArgumentException($"Invalid outcome: '{outcome}'.", NameOf(outcome))
        End Select

    End Function

    ''' <summary>
    ''' Gets the user-friendly description of the specified reason.
    ''' </summary>
    ''' <param name="reason">
    ''' The reason.
    ''' </param>
    ''' <returns>
    ''' The description.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' <paramref name="reason"/> is not a valid <see cref="MirrorSkipReason"/> value.
    ''' </exception>
    Friend Shared Function GetReasonDescription(reason As MirrorSkipReason) As String

        Select Case reason
            Case MirrorSkipReason.Error
                Return "Errors"
            Case MirrorSkipReason.AccessDenied
                Return "Access denied (ignored by the 'Ignore security access exceptions' option)"
            Case MirrorSkipReason.ListingFailed
                Return "Directories whose contents could not be listed"
            Case MirrorSkipReason.DestinationItemKept
                Return "Already existing in the destination, kept untouched ('Overwrite destination files' is disabled)"
            Case MirrorSkipReason.HiddenItem
                Return "Hidden items, including their contents ('Mirror hidden files and folders' is disabled)"
            Case MirrorSkipReason.SymbolicLinkNotMirrored
                Return "Symbolic links ('Mirror symbolic links' is disabled)"
            Case MirrorSkipReason.JunctionNotMirrored
                Return "Directory junctions ('Mirror directory junctions' is disabled)"
            Case MirrorSkipReason.VolumeMountPoint
                Return "Volume mount points (never mirrored)"
            Case MirrorSkipReason.SystemDirectory
                Return "System directories, including their contents (always excluded)"
            Case MirrorSkipReason.TimestampsNotApplied
                Return "Directories mirrored without their original timestamps"
            Case Else
                Throw New ArgumentException($"Invalid reason: '{reason}'.", NameOf(reason))
        End Select

    End Function

    ''' <summary>
    ''' Determines whether the specified reason represents an error (instead of an intentional exclusion).
    ''' </summary>
    ''' <param name="reason">
    ''' The reason.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the reason represents an error; otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function IsErrorReason(reason As MirrorSkipReason) As Boolean
        Return MirrorReport.GetReasonCategory(reason) = MirrorEntryCategory.Error
    End Function

    ''' <summary>
    ''' Gets the category of a reason: excluded (by the options chosen by the user, or by design), warning or error.
    ''' </summary>
    ''' <param name="reason">
    ''' The reason.
    ''' </param>
    ''' <returns>
    ''' The category.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="reason"/> is not a valid value.
    ''' </exception>
    Friend Shared Function GetReasonCategory(reason As MirrorSkipReason) As MirrorEntryCategory

        Select Case reason
            Case MirrorSkipReason.HiddenItem,
                 MirrorSkipReason.SymbolicLinkNotMirrored,
                 MirrorSkipReason.JunctionNotMirrored,
                 MirrorSkipReason.DestinationItemKept,
                 MirrorSkipReason.SystemDirectory
                Return MirrorEntryCategory.Excluded

            Case MirrorSkipReason.VolumeMountPoint
                Return MirrorEntryCategory.Warning

            Case MirrorSkipReason.Error,
                 MirrorSkipReason.AccessDenied,
                 MirrorSkipReason.ListingFailed,
                 MirrorSkipReason.TimestampsNotApplied
                Return MirrorEntryCategory.Error

            Case Else
                Throw New ArgumentException($"Invalid reason: '{reason}'.", NameOf(reason))
        End Select

    End Function

    ''' <summary>
    ''' Gets the label of a category in the report text.
    ''' </summary>
    ''' <param name="category">
    ''' The category.
    ''' </param>
    ''' <returns>
    ''' The label.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="category"/> is not a valid value.
    ''' </exception>
    Friend Shared Function GetCategoryLabel(category As MirrorEntryCategory) As String

        Select Case category
            Case MirrorEntryCategory.Excluded
                Return "Excluded"
            Case MirrorEntryCategory.Warning
                Return "Warning"
            Case MirrorEntryCategory.Error
                Return "Error"
            Case Else
                Throw New ArgumentException($"Invalid category: '{category}'.", NameOf(category))
        End Select

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Counts the entries that satisfy the specified condition.
    ''' </summary>
    ''' <param name="predicate">
    ''' The condition.
    ''' </param>
    ''' <returns>
    ''' The number of entries.
    ''' </returns>
    Private Function CountEntries(predicate As Func(Of MirrorReportEntry, Boolean)) As Integer
        SyncLock Me.syncObject
            Return Enumerable.Count(Me.entriesList, predicate)
        End SyncLock
    End Function

#End Region

End Class

#End Region
