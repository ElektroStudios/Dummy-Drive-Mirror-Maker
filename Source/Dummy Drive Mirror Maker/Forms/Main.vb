#Region " Imports "

Imports DevCase.Core.Application.UserInterface.Types
Imports DevCase.Core.IO.Tools
Imports DevCase.Core.Threading.Enums
Imports DevCase.Core.Threading.Types
Imports DevCase.Interop.Win32

Imports Ookii.Dialogs
Imports Ookii.Dialogs.WinForms

Imports System.ComponentModel
Imports System.IO
Imports System.Threading

#End Region

#Region " Main Form "

''' <summary>
''' The main application Form.
''' </summary>
Public NotInheritable Class Main : Inherits Form

#Region " Fields "

    ''' <summary>
    ''' Keeps track of the current source filepath to be mirrored.
    ''' </summary>
    Private currentFilePath As String

    ''' <summary>
    ''' Flag to determine whether the initial values of the progress bar has been set.
    ''' </summary>
    Private isProgressBarInitialValuesSet As Boolean

    ''' <summary>
    ''' Flag to determine whether the background work has finished listing the source directories 
    ''' (so <see cref="Main.progressMaximum"/> contains the real number of directories).
    ''' <para></para>
    ''' It is written by the background thread and read by the UI thread.
    ''' </summary>
    Private isListingCompleted As Boolean

    ' Progress indicator values.
    Private progressCurrent As Integer ' of progressMaximum.
    Private progressMaximum As Integer

    ''' <summary>
    ''' The report of the ignored (not copied) files and directories of the last mirror operation, 
    ''' or <see langword="Nothing"/> if no mirror operation was started yet.
    ''' </summary>
    Private currentReport As MirrorReport

    ''' <summary>
    ''' The visual theme currently applied to the form.
    ''' </summary>
    Private currentTheme As AppTheme

    ''' <summary>
    ''' Flag to determine whether the theme toggle switch is being updated by code (not by the user).
    ''' </summary>
    Private isUpdatingThemeSwitch As Boolean

#End Region

#Region " Constants "

    ''' <summary>
    ''' The URL of the GitHub repository of the application.
    ''' </summary>
    Friend Const RepositoryUrl As String = "https://github.com/ElektroStudios/Dummy-Drive-Mirror-Maker"

#End Region

#Region " Nested Types "

    ''' <summary>
    ''' Specifies the kind of link to mirror.
    ''' </summary>
    Friend Enum LinkKind As Integer

        ''' <summary>A directory junction.</summary>
        DirectoryJunction = 0

        ''' <summary>A directory symbolic link.</summary>
        DirectorySymbolicLink = 1

        ''' <summary>A file symbolic link.</summary>
        FileSymbolicLink = 2

    End Enum

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Main"/> class.
    ''' </summary>
    Public Sub New()

        Me.InitializeComponent()

        ' Numbers with thousands separators (e.g. "3,294 of 5,000 directories").
        Me.ElektroProgressBar1.NumberFormat = "N0"
        Me.ElektroProgressBar1.FormatCulture = MirrorReport.NumberCulture

        ' Window title: "{name} {version}".
        Me.Text = Main.BuildWindowTitle(My.Application.Info.Title, My.Application.Info.Version)

        ' Load the theme saved in the user settings ("System" follows the Windows apps theme).
        Me.currentTheme = ThemeManager.ResolveTheme(My.Settings.Theme, ThemeManager.IsSystemLightThemeEnabled())

        Me.isUpdatingThemeSwitch = True
        Try
            Me.ToggleSwitchTheme.Checked = (Me.currentTheme = AppTheme.Dark)
        Finally
            Me.isUpdatingThemeSwitch = False
        End Try

        ThemeManager.ApplyTheme(Me, ThemeManager.GetPalette(Me.currentTheme))

        ' Distribute the option columns across the full width, according to the real (measured) text widths.
        Me.LayoutOptionColumns()

    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Builds the window title, in the format "{name} {major}.{minor}" 
    ''' (the build number is appended only when it is greater than zero).
    ''' </summary>
    ''' <param name="name">
    ''' The application name.
    ''' </param>
    ''' 
    ''' <param name="version">
    ''' The application version.
    ''' </param>
    ''' <returns>
    ''' The window title.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="name"/> is null, empty or white-spaced, or <paramref name="version"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Shared Function BuildWindowTitle(name As String, version As Version) As String

        If String.IsNullOrWhiteSpace(name) Then
            Throw New ArgumentNullException(NameOf(name), "Value is null, empty, or white-spaced.")
        End If

        If version Is Nothing Then
            Throw New ArgumentNullException(NameOf(version))
        End If

        Dim versionText As String = If(version.Build > 0, version.ToString(3), version.ToString(2))
        Return $"{name} {versionText}"

    End Function

    ''' <summary>
    ''' Gets the severity used to color the progress bar at the end of a mirror operation.
    ''' </summary>
    ''' <param name="reportSeverity">
    ''' The severity of the report of ignored items.
    ''' </param>
    ''' 
    ''' <param name="isCancelled">
    ''' A value indicating whether the operation was aborted.
    ''' </param>
    ''' 
    ''' <param name="hasFailed">
    ''' A value indicating whether the operation terminated due an error.
    ''' </param>
    ''' <returns>
    ''' <see cref="MirrorReportSeverity.Error"/> if the operation failed or some item failed; 
    ''' <see cref="MirrorReportSeverity.Warning"/> if the operation was aborted or some item was ignored; 
    ''' otherwise, <see cref="MirrorReportSeverity.None"/>.
    ''' </returns>
    Friend Shared Function GetResultSeverity(reportSeverity As MirrorReportSeverity, isCancelled As Boolean, hasFailed As Boolean) As MirrorReportSeverity

        If hasFailed OrElse (reportSeverity = MirrorReportSeverity.Error) Then
            Return MirrorReportSeverity.Error
        End If

        If isCancelled OrElse (reportSeverity = MirrorReportSeverity.Warning) Then
            Return MirrorReportSeverity.Warning
        End If

        Return MirrorReportSeverity.None

    End Function

    ''' <summary>
    ''' Builds the text of the progress bar that shows the result of a mirror operation 
    ''' (e.g. <c>3,285 of 3,285 directories (100%) - Copied with warnings</c>).
    ''' <para></para>
    ''' The text is kept short so it fits inside the progress bar: 
    ''' the details of the ignored items are shown in the report (<c>Details...</c> button).
    ''' </summary>
    ''' <param name="processedDirectoryCount">
    ''' The number of processed subdirectories.
    ''' </param>
    ''' 
    ''' <param name="sourceDirectoryCount">
    ''' The number of subdirectories of the source directory, or -1 if the source directory was not listed.
    ''' </param>
    ''' 
    ''' <param name="outcome">
    ''' How the operation ended.
    ''' </param>
    ''' 
    ''' <param name="reportSeverity">
    ''' The severity of the report of ignored items.
    ''' </param>
    ''' <returns>
    ''' The text, with the numbers formatted in the <see cref="MirrorReport.NumberCulture"/>.
    ''' </returns>
    Friend Shared Function BuildResultText(processedDirectoryCount As Integer, sourceDirectoryCount As Integer,
                                           outcome As MirrorOutcome, reportSeverity As MirrorReportSeverity) As String

        Dim suffix As String
        Select Case outcome
            Case MirrorOutcome.Aborted
                suffix = " - Aborted"
            Case MirrorOutcome.Failed
                suffix = " - Failed"
            Case MirrorOutcome.Completed
                Select Case reportSeverity
                    Case MirrorReportSeverity.None
                        suffix = String.Empty
                    Case MirrorReportSeverity.Warning
                        suffix = " - Copied with warnings"
                    Case MirrorReportSeverity.Error
                        suffix = " - Copied with errors"
                    Case Else
                        Throw New ArgumentException($"Invalid severity: '{reportSeverity}'.", NameOf(reportSeverity))
                End Select
            Case Else
                Throw New ArgumentException($"Invalid outcome: '{outcome}'.", NameOf(outcome))
        End Select

        ' The operation ended before the source directory was listed.
        If sourceDirectoryCount < 0 Then
            Return $"Directories not listed{suffix}"
        End If

        If (processedDirectoryCount < 0) OrElse (processedDirectoryCount > sourceDirectoryCount) Then
            Throw New ArgumentOutOfRangeException(NameOf(processedDirectoryCount), processedDirectoryCount,
                                                  $"Value must be in the range 0 to {sourceDirectoryCount}.")
        End If

        ' A source directory without subdirectories is fully processed when the operation completes.
        Dim percent As Integer
        If sourceDirectoryCount = 0 Then
            percent = If(outcome = MirrorOutcome.Completed, 100, 0)
        Else
            percent = MirrorRules.CalculateProgressPercent(processedDirectoryCount, sourceDirectoryCount)
        End If

        Return $"{MirrorReport.FormatNumber(processedDirectoryCount)} of {MirrorReport.FormatNumber(sourceDirectoryCount)} directories ({percent}%){suffix}"

    End Function

    ''' <summary>
    ''' Computes the horizontal positions of a set of columns, justified between two bounds: 
    ''' the first column starts at <paramref name="left"/>, the last column ends at <paramref name="right"/>,
    ''' and the free space is distributed equally between the columns.
    ''' </summary>
    ''' <param name="left">
    ''' The left bound.
    ''' </param>
    ''' 
    ''' <param name="right">
    ''' The right bound.
    ''' </param>
    ''' 
    ''' <param name="columnWidths">
    ''' The width of each column (at least two columns).
    ''' </param>
    ''' <returns>
    ''' The left position of each column.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="columnWidths"/> is <see langword="Nothing"/>.
    ''' </exception>
    ''' 
    ''' <exception cref="ArgumentException">
    ''' There are less than two columns, or any width is negative.
    ''' </exception>
    ''' 
    ''' <exception cref="InvalidOperationException">
    ''' The columns do not fit between the bounds.
    ''' </exception>
    Friend Shared Function ComputeJustifiedColumnPositions(left As Integer, right As Integer, columnWidths As Integer()) As Integer()

        If columnWidths Is Nothing Then
            Throw New ArgumentNullException(NameOf(columnWidths))
        End If

        If columnWidths.Length < 2 Then
            Throw New ArgumentException("At least two columns are required.", NameOf(columnWidths))
        End If

        For Each width As Integer In columnWidths
            If width < 0 Then
                Throw New ArgumentException("The column widths cannot be negative.", NameOf(columnWidths))
            End If
        Next width

        Dim freeSpace As Integer = (right - left) - columnWidths.Sum()
        If freeSpace < 0 Then
            Throw New InvalidOperationException($"The columns ({columnWidths.Sum()} px) do not fit in the available width ({right - left} px).")
        End If

        Dim gapCount As Integer = columnWidths.Length - 1
        Dim positions(columnWidths.Length - 1) As Integer
        Dim x As Integer = left
        For index As Integer = 0 To columnWidths.Length - 1
            ' Distribute the remainder of the integer division between the first gaps.
            positions(index) = x
            If index < gapCount Then
                Dim gap As Integer = (freeSpace \ gapCount) + If(index < (freeSpace Mod gapCount), 1, 0)
                x += columnWidths(index) + gap
            End If
        Next index

        Return positions

    End Function

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event.
    ''' <para></para>
    ''' The dark title bar must be applied here: the window handle does not exist yet in the constructor, 
    ''' and applying it in the <see cref="Form.Load"/> event has no effect on the already drawn title bar.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnHandleCreated(e As EventArgs)

        MyBase.OnHandleCreated(e)
        ThemeManager.ApplyTitleBarTheme(Me, useDarkMode:=(Me.currentTheme = AppTheme.Dark))

    End Sub

#End Region

#Region " Event Handlers "

#Region " Form "

    ''' <summary>
    ''' Handles the <see cref="Form.Load"/> event of the <see cref="Main"/> Form.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Form.FormClosing"/> event of the <see cref="Main"/> Form.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If e.CloseReason = CloseReason.UserClosing Then

            If Me.ElektroBackgroundWorker1.State = ElektroBackgroundWorkerState.Running Then

                Dim dlgResult As DialogResult =
                    MessageBox.Show(UIMessages.FormClosingWarning,
                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                e.Cancel = (dlgResult = DialogResult.No)
            End If
        End If

    End Sub

#End Region

#Region " TextBoxes "

    ''' <summary>
    ''' Handles the <see cref="TextBox.DragEnter"/> event of the <see cref="Main.TextBoxSource"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub TextBoxSource_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxSource.DragEnter

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then

            Dim items As IEnumerable(Of String) = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String))
            If (items.Count <> 1) Then
                e.Effect = DragDropEffects.None

            ElseIf File.GetAttributes(items.Single()).HasFlag(FileAttributes.Directory) Then
                e.Effect = DragDropEffects.All

            End If

        Else
            e.Effect = DragDropEffects.None

        End If

    End Sub

    ''' <summary>
    ''' Handles the <see cref="TextBox.DragDrop"/> event of the <see cref="Main.TextBoxSource"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub TextBoxSource_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxSource.DragDrop

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            Dim dirpath As String = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String)).Single()
            Options.SourceDir = New DirectoryInfo(dirpath)

            Me.TextBoxSource.Text = Options.SourceDir.FullName
            If (Options.DestinationDir IsNot Nothing) Then
                Me.ButtonMirror.Enabled = False
                Me.ValidateDestinationDirectory()

            Else
                Me.ButtonDestination.Enabled = True
                Me.TextBoxDestination.Enabled = True

            End If

        End If

    End Sub

    ''' <summary>
    ''' Handles the <see cref="TextBox.DragEnter"/> event of the <see cref="Main.TextBoxDestination"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub TextBoxDestination_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxDestination.DragEnter

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then

            Dim items As IEnumerable(Of String) = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String))
            If (items.Count <> 1) Then
                e.Effect = DragDropEffects.None

            ElseIf File.GetAttributes(items.Single()).HasFlag(FileAttributes.Directory) Then
                e.Effect = DragDropEffects.All

            End If

        Else
            e.Effect = DragDropEffects.None

        End If

    End Sub

    ''' <summary>
    ''' Handles the <see cref="TextBox.DragDrop"/> event of the <see cref="Main.TextBoxDestination"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub TextBoxDestination_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxDestination.DragDrop

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            Dim dirpath As String = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String)).Single()

            Options.DestinationDir = New DirectoryInfo(dirpath)
            Me.ButtonMirror.Enabled = False
            Me.ValidateDestinationDirectory()
        End If

    End Sub

#End Region

#Region " CheckBoxes "

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxAttribs"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxAttribs_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAttribs.CheckedChanged
        Options.PreserveAttribs = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxDatestamps"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxDatestamps_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxDatestamps.CheckedChanged
        Options.PreserveTimestamps = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxHiddenFiles"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxHiddenFiles_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHiddenFiles.CheckedChanged
        Options.MirrorHiddenFiles = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxSymLinks"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxSymLinks_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSymLinks.CheckedChanged
        Options.MirrorSymbolicLinks = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxJunctions"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxJunctions_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxJunctions.CheckedChanged
        Options.MirrorJunctions = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxHardLinks"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxHardLinks_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHardLinks.CheckedChanged
        Options.MirrorHardLinks = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxIgnoreSecurityExceptions"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxIgnoreSecurityExceptions_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIgnoreSecurityExceptions.CheckedChanged
        Options.IgnoreSecurityExceptions = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxOverwrite"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub CheckBoxOverwrite_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxOverwrite.CheckedChanged
        Options.OverwriteDestinationFiles = DirectCast(sender, CheckBox).Checked
    End Sub

#End Region

#Region " Buttons "

    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonSource"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonSource_Click(sender As Object, e As EventArgs) Handles ButtonSource.Click

        Using vfbd As New VistaFolderBrowserDialog
            vfbd.Description = "Select a source directory."
            vfbd.UseDescriptionForTitle = True
            vfbd.ShowNewFolderButton = False
            vfbd.SelectedPath = If(Options.SourceDir IsNot Nothing, Options.SourceDir.FullName, vfbd.SelectedPath)

            Dim dlgResult As DialogResult = vfbd.ShowDialog()
            If (dlgResult = DialogResult.OK) Then
                Options.SourceDir = New DirectoryInfo(vfbd.SelectedPath)

                Me.TextBoxSource.Text = Options.SourceDir.FullName
                If (Options.DestinationDir IsNot Nothing) Then
                    Me.ButtonMirror.Enabled = False
                    Me.ValidateDestinationDirectory()

                Else
                    Me.ButtonDestination.Enabled = True
                    Me.TextBoxDestination.Enabled = True

                End If

            End If
        End Using

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonDestination"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonDestination_Click(sender As Object, e As EventArgs) Handles ButtonDestination.Click

        Using vfbd As New VistaFolderBrowserDialog
            vfbd.Description = "Select a destination directory."
            vfbd.UseDescriptionForTitle = True
            vfbd.ShowNewFolderButton = True
            vfbd.SelectedPath = If(Options.DestinationDir IsNot Nothing, Options.DestinationDir.FullName, vfbd.SelectedPath)

            Dim dlgResult As DialogResult = vfbd.ShowDialog()
            If (dlgResult = DialogResult.OK) Then
                Options.DestinationDir = New DirectoryInfo(vfbd.SelectedPath)
                Me.ValidateDestinationDirectory()
            End If

        End Using

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonMirror"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonMirror_Click(sender As Object, e As EventArgs) Handles ButtonMirror.Click

        ' Abort a pending mirror operation by user demand.
        If (Me.ElektroBackgroundWorker1.State = ElektroBackgroundWorkerState.Running) Then
            Me.ElektroBackgroundWorker1.Cancel()
            Exit Sub
        End If

        ' Disable UI controls to prevent bad things (such as button double-click).
        Me.ButtonMirror.Enabled = False
        Me.SetInputControlsEnabled(False)

        ' Set pre-mirror information on controls, and reset progress values.
        Me.SetPreMirrorValues()

        ' A new report for the ignored files and directories of this mirror operation.
        Me.currentReport = New MirrorReport(Options.SourceDir.FullName, Options.DestinationDir.FullName, Date.Now)
        Me.ButtonReport.Enabled = False

        ' Run the mirror operation in background.
        Me.ElektroBackgroundWorker1 = New ElektroBackgroundWorker()
        Me.ElektroBackgroundWorker1.RunAsync()
        Me.TimerProgress.Start()

        ' Set the 'Abort' visuals.
        Me.SetMirrorButtonState(isRunning:=True)
        Me.ButtonMirror.Enabled = True

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonGitHub"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonGitHub_Click(sender As Object, e As EventArgs) Handles ButtonGitHub.Click

        ' Opens the URL in the default web browser of the user.
        Using browserProcess As Process = Process.Start(New ProcessStartInfo(Main.RepositoryUrl) With {.UseShellExecute = True})
        End Using

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonReport"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonReport_Click(sender As Object, e As EventArgs) Handles ButtonReport.Click
        Me.ShowReportWindow()
    End Sub

#End Region

#Region " Toggle Switches "

    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.ToggleSwitchTheme"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ToggleSwitchTheme_CheckedChanged(sender As Object, e As EventArgs) Handles ToggleSwitchTheme.CheckedChanged

        If Me.isUpdatingThemeSwitch Then
            Exit Sub
        End If

        Me.currentTheme = If(Me.ToggleSwitchTheme.Checked, AppTheme.Dark, AppTheme.Light)

        ' Save the user choice.
        My.Settings.Theme = ThemeManager.ToSettingValue(Me.currentTheme)
        My.Settings.Save()

        ' Apply the theme to the controls and to the title bar.
        ThemeManager.ApplyTheme(Me, ThemeManager.GetPalette(Me.currentTheme))
        ThemeManager.ApplyTitleBarTheme(Me, useDarkMode:=(Me.currentTheme = AppTheme.Dark))

    End Sub

#End Region

#End Region

#Region " Background Worker "

    ''' <summary>
    ''' Handles the <see cref="ElektroBackgroundWorker.DoWork"/> event 
    ''' of the <see cref="Main.ElektroBackgroundWorker1"/> component.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DoWorkEventArgs"/> instance containing the event data.
    ''' </param>
    <DebuggerStepperBoundary>
    Private Sub ElektroBackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles ElektroBackgroundWorker1.DoWork

        ' Short reference for this 'ElektroBackgroundWorker' instance.
        Dim worker As ElektroBackgroundWorker = DirectCast(sender, ElektroBackgroundWorker)

        ' Short reference for the report of the current mirror operation.
        Dim report As MirrorReport = Me.currentReport

        ' Flag to determine whether folder-exists conflicts must be ignored.
        Dim ignoreFolderExistsConflict As Boolean

        ' Flag to determine whether the user answered to folder-exists conflicts question.
        Dim ignoreFolderExistsConflictAnswered As Boolean

        ' Get the root source directory and all its sub-directories.
        ' The scanner never descends into links, mount points and excluded directories,
        ' because their contents are not part of the mirrored tree.
        Dim mirrorHiddenItems As Boolean = Options.MirrorHiddenFiles
        Dim srcSubDirs As List(Of DirectoryInfo) =
            MirrorDirectoryScanner.ListDirectories(Options.SourceDir,
                                                   Function(di As DirectoryInfo) MirrorDirectoryScanner.ShouldDescendInto(di, mirrorHiddenItems),
                                                   Sub(di As DirectoryInfo, ex As Exception) report.Add(MirrorItemKind.Directory, di.FullName, MirrorSkipReason.ListingFailed, ex.Message))

        report.SetSourceStatistics(srcSubDirs.Skip(1))

        ' Exclude the directories that the user chose not to mirror (hidden directories, directory junctions, symbolic links),
        ' and the directories excluded by design (system directories, volume mount points).
        ' They are not processed nor counted in the progress, but they are registered in the report.
        srcSubDirs = MirrorDirectoryScanner.SelectDirectoriesToMirror(
            srcSubDirs, mirrorHiddenItems, Options.MirrorJunctions, Options.MirrorSymbolicLinks,
            Sub(di As DirectoryInfo, reason As MirrorSkipReason) report.Add(MirrorItemKind.Directory, di.FullName, reason))

        ' Set progressbar max. value.
        ' The root source directory (the first directory) is processed but not counted.
        Me.progressMaximum = srcSubDirs.Count - 1

        ' The timestamps of the destination directories are applied when the mirror operation ends,
        ' in reverse order (deepest directories first), because creating a file or a subdirectory 
        ' inside a directory modifies its last write time.
        Dim pendingDirectoryTimestamps As New List(Of KeyValuePair(Of String, DirectoryInfo))

        ' Keeps track of the source files with multiple hard links that were already mirrored,
        ' and the destination path of their first mirrored name.
        Dim mirroredHardLinks As New Dictionary(Of FileLinkInfo, String)

        ' The progress bar can be initialized from now on.
        Volatile.Write(Me.isListingCompleted, True)

        For Each srcSubDir As DirectoryInfo In srcSubDirs
            ' Cancel mirror operation by user demand.
            If (worker.CancellationPending) Then ' Cancel the background operation.
                e.Cancel = True
                Exit For
            End If

            ' The root source directory was explicitly selected by the user,
            ' so it is always mirrored regardless of its attributes (e.g. the root of a drive is usually hidden).
            Dim isSourceRoot As Boolean = (srcSubDir Is srcSubDirs(0))

            ' Every subdirectory counts as processed, whether it is mirrored or not (e.g. due to an error), 
            ' so the progress bar always reaches its maximum value when the operation completes.
            If Not isSourceRoot Then
                Interlocked.Increment(Me.progressCurrent)
            End If

            ' Build destination directory full path.
            Dim dstDir As New DirectoryInfo(MirrorRules.BuildDestinationPath(Options.SourceDir.FullName, Options.DestinationDir.FullName, srcSubDir.FullName))

#If DEBUG Then
            Debug.WriteLine($"Attempting to create dir.: '{dstDir.FullName}'")
#End If

            Try
                ' Note: the hidden directories (when they are not mirrored), the system directories, the volume mount points,
                ' and the directory junctions and symbolic links that are not mirrored, were already excluded
                ' (see 'MirrorDirectoryScanner.SelectDirectoriesToMirror').

                ' Directory junction: create a copy of the junction (with the same name) pointing to the same original target directory.
                ' Its contents are never mirrored as real directories and files.
                If Not (isSourceRoot) AndAlso Directories.IsJunction(srcSubDir.FullName) Then
                    Dim junctionTarget As String = Directories.GetMountPointTarget(srcSubDir.FullName)
                    dstDir.Parent.Create()
                    Main.RegisterLinkResult(report, MirrorItemKind.Directory, srcSubDir.FullName,
                                            Main.MirrorLink(dstDir.FullName, junctionTarget, LinkKind.DirectoryJunction, Options.OverwriteDestinationFiles),
                                            $"Directory junction to '{junctionTarget}'")
                    Continue For
                End If

                ' Symbolic link: create a copy of the symbolic link (with the same name) pointing to the same original target path.
                ' Its contents are never mirrored as real directories and files.
                If Not (isSourceRoot) AndAlso Directories.IsSymbolicLink(srcSubDir) Then
                    Dim linkTarget As String = Directories.GetSymbolicLinkDestination(srcSubDir.FullName)
                    dstDir.Parent.Create()
                    Main.RegisterLinkResult(report, MirrorItemKind.Directory, srcSubDir.FullName,
                                            Main.MirrorLink(dstDir.FullName, linkTarget, LinkKind.DirectorySymbolicLink, Options.OverwriteDestinationFiles),
                                            $"Symbolic link to '{linkTarget}'")
                    Continue For
                End If

                ' Ask the user when a destination directory already exists before creating dummy files inside, 
                ' because the destination directory could contain user files
                ' and those files will be end messed up with dummy files.
                ' The question is not made when the user explicitly allowed to overwrite the destination.
                If Not (Options.OverwriteDestinationFiles) AndAlso
                   Not (ignoreFolderExistsConflict) AndAlso
                       dstDir.Exists() AndAlso
                   Not MirrorRules.ArePathsEqual(dstDir.FullName, Options.DestinationDir.FullName) Then

                    Dim dlgResult As DialogResult =
                        MessageBox.Show(String.Format(UIMessages.DirectoryExistQuestion1, MirrorRules.RemoveLongPathPrefix(dstDir.FullName)),
                                        Me.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

                    If (dlgResult = DialogResult.Cancel) Then
                        e.Cancel = True
                        Exit For
                    End If

                    ' In case of the user is really aware of what he is doing, 
                    ' we simplify things by brinding to him the chance to discard more folder conflict questions like this.
                    If (dlgResult = DialogResult.Yes) AndAlso Not (ignoreFolderExistsConflictAnswered) Then
                        dlgResult = MessageBox.Show(String.Format(UIMessages.DirectoryExistQuestion2, MirrorRules.RemoveLongPathPrefix(dstDir.FullName)),
                                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                        Select Case dlgResult
                            Case DialogResult.Yes
                                ignoreFolderExistsConflict = True
                            Case Else
                                ignoreFolderExistsConflict = False
                        End Select
                        ignoreFolderExistsConflictAnswered = True

                    ElseIf (dlgResult = DialogResult.No) Then
                        e.Cancel = True
                        Exit For

                    End If
                End If

                ' Create the destination directory.
                dstDir.Create()

                ' The timestamps and attributes of a volume root directory (e.g. 'D:\') are not meaningful
                ' (a volume root is usually marked as hidden and system), so they are never applied to the destination directory.
                Dim isVolumeRoot As Boolean = MirrorRules.IsVolumeRoot(srcSubDir.FullName)

                ' Set directory timestamps (deferred, see 'pendingDirectoryTimestamps').
                If (Options.PreserveTimestamps) AndAlso Not (isVolumeRoot) Then
                    pendingDirectoryTimestamps.Add(New KeyValuePair(Of String, DirectoryInfo)(dstDir.FullName, srcSubDir))
                End If

                ' Set directory attributes.
                If (Options.PreserveAttribs) AndAlso Not (isVolumeRoot) Then
                    dstDir.Attributes = srcSubDir.Attributes
                End If

                ' The source directory itself is not counted (it is the root of the mirror).
                If Not (isSourceRoot) Then
                    report.AddMirrored(MirrorItemKind.Directory, srcSubDir.FullName)
                End If

                ' Ignore security access exceptions by user-demand.
            Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) AndAlso
                                       (Options.IgnoreSecurityExceptions)
                report.Add(MirrorItemKind.Directory, srcSubDir.FullName, MirrorSkipReason.AccessDenied, ex.Message)
                Continue For

            Catch ex As Exception
                report.Add(MirrorItemKind.Directory, srcSubDir.FullName, MirrorSkipReason.Error, ex.Message)
                Dim dlgResult As DialogResult =
                    MessageBox.Show(String.Format(UIMessages.DirectoryException, ex.Message),
                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                If (dlgResult = DialogResult.No) Then
                    e.Cancel = True
                    Exit For
                End If

            End Try

            Dim topFiles As IEnumerable(Of FileInfo) =
                Files.GetFiles(srcSubDir, SearchOption.TopDirectoryOnly, {"*"}, {"*"}, True,
                               throwOnError:=False).
                               Select(Function(fi As FileInfo) New FileInfo(MirrorRules.AddLongPathPrefixIfNeeded(fi.FullName))).
                               OrderBy(Function(fi As FileInfo) fi.FullName)

            For Each topFile As FileInfo In topFiles

                If (worker.CancellationPending) Then ' Cancel the background operation.
                    e.Cancel = True
                    Exit For
                End If

                Volatile.Write(Me.currentFilePath, MirrorRules.RemoveLongPathPrefix(topFile.FullName))

                ' Build destination file full path.
                Dim dstFile As New FileInfo(MirrorRules.BuildDestinationPath(Options.SourceDir.FullName, Options.DestinationDir.FullName, topFile.FullName))

#If DEBUG Then
                Debug.WriteLine($"Attempting to create file: '{dstFile.FullName}'")
#End If

                Try
                    ' Ignore hidden file by user-demand.
                    If Not (Options.MirrorHiddenFiles) AndAlso
                           (topFile.Attributes.HasFlag(FileAttributes.Hidden)) Then
                        report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.HiddenItem)
                        Continue For
                    End If

                    Dim isSymbolicLink As Boolean = Files.IsSymbolicLink(topFile)
                    If (isSymbolicLink) Then
                        If (Options.MirrorSymbolicLinks) Then
                            ' Create a copy of the symbolic link (with the same name) pointing to the same original target path.
                            Dim linkTarget As String = Files.GetSymbolicLinkDestination(topFile.FullName)
                            Main.RegisterLinkResult(report, MirrorItemKind.File, topFile.FullName,
                                                    Main.MirrorLink(dstFile.FullName, linkTarget, LinkKind.FileSymbolicLink, Options.OverwriteDestinationFiles),
                                                    $"Symbolic link to '{linkTarget}'")

                        Else ' Ignore symbolic links by user-demand.
                            report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.SymbolicLinkNotMirrored)

                        End If

                        Continue For
                    End If

                    ' Hard links: mirror every name of the same source file as a hard link of a single dummy file.
                    Dim hardLinkInfo As FileLinkInfo? = Nothing
                    If (Options.MirrorHardLinks) Then
                        Dim linkInfo As FileLinkInfo = Files.GetLinkInfo(topFile.FullName)
                        If linkInfo.LinkCount > 1 Then
                            Dim firstMirroredPath As String = Nothing
                            If mirroredHardLinks.TryGetValue(linkInfo, firstMirroredPath) Then
                                ' Another name of this file was already mirrored, so link to it.
                                ' The timestamps and attributes are shared by all the names of a file.
                                If dstFile.Exists Then
                                    ' Already linked (e.g. by a previous mirror operation): nothing to do.
                                    If Files.GetLinkInfo(dstFile.FullName).Equals(Files.GetLinkInfo(firstMirroredPath)) Then
                                        report.AddMirrored(MirrorItemKind.File, topFile.FullName, "Hard link, already up to date")
                                        Continue For
                                    End If

                                    ' Keep the existing file, unless the user allowed to overwrite it.
                                    If Not (Options.OverwriteDestinationFiles) Then
                                        report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.DestinationItemKept)
                                        Continue For
                                    End If
                                    dstFile.Attributes = FileAttributes.Normal
                                    dstFile.Delete()
                                End If
                                Files.CreateHardLink(dstFile.FullName, firstMirroredPath)
                                report.AddMirrored(MirrorItemKind.File, topFile.FullName, $"Hard link of '{MirrorRules.RemoveLongPathPrefix(firstMirroredPath)}'")
                                Continue For
                            End If
                            hardLinkInfo = linkInfo
                        End If
                    End If

                    ' Create the (dummy) destination file.
                    ' An existing destination file is replaced only if the user allowed to overwrite it; otherwise, it is kept.
                    Dim isDummyFileCreated As Boolean
                    If Not dstFile.Exists Then
                        Using fs As FileStream = dstFile.Create()
                        End Using
                        isDummyFileCreated = True

                    ElseIf (Options.OverwriteDestinationFiles) Then
                        ' A read-only, hidden or system file cannot be truncated, so reset its attributes first
                        ' (this happens when the destination file was created by a previous mirror operation that preserved the attributes).
                        dstFile.Attributes = FileAttributes.Normal
                        Using fs As FileStream = dstFile.Create()
                        End Using
                        isDummyFileCreated = True

                    End If

                    ' Register the first mirrored name of a file with multiple hard links.
                    If hardLinkInfo.HasValue Then
                        mirroredHardLinks.Add(hardLinkInfo.Value, dstFile.FullName)
                    End If

                    ' An existing destination file that was not overwritten is kept untouched (timestamps and attributes included).
                    If Not isDummyFileCreated Then
                        report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.DestinationItemKept)
                        Continue For
                    End If

                    ' Set file timestamps.
                    If (Options.PreserveTimestamps) Then
                        Files.SetTimestampsUtc(dstFile.FullName, topFile.CreationTimeUtc, topFile.LastAccessTimeUtc, topFile.LastWriteTimeUtc)
                    End If

                    ' Set file attributes.
                    If (Options.PreserveAttribs) Then
                        dstFile.Attributes = topFile.Attributes
                    End If

                    report.AddMirrored(MirrorItemKind.File, topFile.FullName)

                    ' Ignore security access exceptions by user-demand.
                Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) AndAlso
                                           (Options.IgnoreSecurityExceptions)
                    report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.AccessDenied, ex.Message)
                    Continue For

                Catch ex As Exception
                    report.Add(MirrorItemKind.File, topFile.FullName, MirrorSkipReason.Error, ex.Message)
                    Dim dlgResult As DialogResult =
                        MessageBox.Show(String.Format(UIMessages.FileException, ex.Message),
                                        Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                    If (dlgResult = DialogResult.No) Then
                        e.Cancel = True
                        Exit For

                    End If

                End Try

            Next topFile

            ' The inner loop was exited due to a cancellation request.
            If (e.Cancel) Then
                Exit For
            End If

        Next srcSubDir

        ' Apply the timestamps of the destination directories (also when the operation was aborted, 
        ' for the directories already created).
        Main.ApplyDirectoryTimestamps(pendingDirectoryTimestamps, report)

    End Sub

    ''' <summary>
    ''' Registers in the report the result of mirroring a link (see <see cref="Main.MirrorLink"/>).
    ''' </summary>
    ''' <param name="report">
    ''' The report of the mirror operation.
    ''' </param>
    ''' 
    ''' <param name="kind">
    ''' The kind of item.
    ''' </param>
    ''' 
    ''' <param name="sourcePath">
    ''' The source path of the link.
    ''' </param>
    ''' 
    ''' <param name="action">
    ''' The action performed by <see cref="Main.MirrorLink"/>.
    ''' </param>
    ''' 
    ''' <param name="description">
    ''' The description of the link (e.g. "Directory junction to 'C:\Target'").
    ''' </param>
    Friend Shared Sub RegisterLinkResult(report As MirrorReport, kind As MirrorItemKind, sourcePath As String,
                                         action As LinkAction, description As String)

        If report Is Nothing Then
            Throw New ArgumentNullException(NameOf(report))
        End If

        Select Case action
            Case LinkAction.Create, LinkAction.Replace
                report.AddMirrored(kind, sourcePath, description)
            Case LinkAction.Skip
                report.AddMirrored(kind, sourcePath, $"{description}, already up to date")
            Case LinkAction.Keep
                report.Add(kind, sourcePath, MirrorSkipReason.DestinationItemKept)
            Case Else
                Throw New ArgumentException($"Unexpected link action: '{action}'.", NameOf(action))
        End Select

    End Sub

    ''' <summary>
    ''' Applies the timestamps of the source directories to their destination directories, 
    ''' in reverse order (deepest directories first).
    ''' <para></para>
    ''' A directory whose timestamps cannot be applied is registered in the report as an error.
    ''' </summary>
    ''' <param name="pendingDirectoryTimestamps">
    ''' The destination directory paths, and their source directories, in the order they were created.
    ''' </param>
    ''' 
    ''' <param name="report">
    ''' The report of the mirror operation.
    ''' </param>
    Friend Shared Sub ApplyDirectoryTimestamps(pendingDirectoryTimestamps As IList(Of KeyValuePair(Of String, DirectoryInfo)), report As MirrorReport)

        If pendingDirectoryTimestamps Is Nothing Then
            Throw New ArgumentNullException(NameOf(pendingDirectoryTimestamps))
        End If
        If report Is Nothing Then
            Throw New ArgumentNullException(NameOf(report))
        End If

        For index As Integer = pendingDirectoryTimestamps.Count - 1 To 0 Step -1
            Dim destinationPath As String = pendingDirectoryTimestamps(index).Key
            Dim source As DirectoryInfo = pendingDirectoryTimestamps(index).Value
            Try
                Files.SetTimestampsUtc(destinationPath, source.CreationTimeUtc, source.LastAccessTimeUtc, source.LastWriteTimeUtc)

            Catch ex As Exception When (TypeOf ex Is IOException) OrElse (TypeOf ex Is UnauthorizedAccessException)
                report.Add(MirrorItemKind.Directory, source.FullName, MirrorSkipReason.TimestampsNotApplied, ex.Message)

            End Try
        Next index

    End Sub

    ''' <summary>
    ''' Handles the <see cref="System.Windows.Forms.Timer.Tick"/> event of the <see cref="Main.TimerProgress"/> component.
    ''' <para></para>
    ''' Updates the progress information in the UI at a fixed rate, reading the values written by the background work.
    ''' </summary>
    ''' <remarks>
    ''' The progress is not reported from the background work (with <see cref="BackgroundWorker.ReportProgress(Integer)"/>) 
    ''' because the mirror operation processes thousands of files per second: 
    ''' reporting every file floods the message queue of the UI thread and freezes the window.
    ''' </remarks>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub TimerProgress_Tick(sender As Object, e As EventArgs) Handles TimerProgress.Tick
        Me.UpdateProgressInfo()
    End Sub

    ''' <summary>
    ''' Handles the <see cref="ElektroBackgroundWorker.RunWorkerCompleted"/> event 
    ''' of the <see cref="Main.ElektroBackgroundWorker1"/> component.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.
    ''' </param>
    <DebuggerStepperBoundary>
    Private Sub ElektroBackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) _
    Handles ElektroBackgroundWorker1.RunWorkerCompleted

        ' The report is always available when the operation ends (completed, aborted or failed).
        ' It is enabled first, so nothing below can prevent it.
        Me.ButtonReport.Enabled = True

        ' Stop the periodic UI updates, and show the final progress.
        Me.TimerProgress.Stop()
        Me.UpdateProgressInfo()

        ' Restore button style.
        Me.SetMirrorButtonState(isRunning:=False)

        ' Update UI progress information.
        ' Note: the progress bar could be not initialized yet if no progress was reported during the operation
        ' (e.g. the source directories contains no files, or the operation was aborted before processing any file).
        Me.EnsureProgressBarInitialized()
        Me.ElektroProgressBar1.Value = Math.Min(Me.progressCurrent, Me.ElektroProgressBar1.Maximum)
        Me.ShowMirrorResult(Me.currentReport, e.Cancelled, hasFailed:=(e.Error IsNot Nothing))
        Me.SetStatusText("Ready")

        ' Show termination message.
        If (e.Cancelled) Then
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(UIMessages.MirrorCompletedAborted, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using

        ElseIf (e.Error IsNot Nothing) Then
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(String.Format(UIMessages.MirrorCompletedError, e.Error.Message), Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using

        ElseIf (Me.currentReport.HasProblems) Then
            ' Offer to show the details of the items that could not be mirrored (they are also available later with the 'Details...' button).
            ' The items excluded by the options chosen by the user (or by design) are not a problem, so they are not mentioned here.
            Dim dlgResult As DialogResult
            Using msg As New CenteredMessageBox(owner:=Me)
                dlgResult = msg.Show(String.Format(UIMessages.MirrorCompletedWithIgnoredItems, Me.currentReport.GetSummary()),
                                     Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            End Using
            If (dlgResult = DialogResult.Yes) Then
                Me.ShowReportWindow()
            End If

        Else
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(UIMessages.MirrorCompletedSuccessful, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        End If

        ' Re-enable UI controls.
        Me.SetInputControlsEnabled(True)
        Me.ButtonMirror.Enabled = True

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Shows the result of a mirror operation in the progress bar: 
    ''' the final text (with the number of ignored directories and files) and the color 
    ''' (green: everything was mirrored; yellow: some items were ignored; red: some items failed).
    ''' </summary>
    ''' <param name="report">
    ''' The report of the mirror operation.
    ''' </param>
    ''' 
    ''' <param name="isCancelled">
    ''' A value indicating whether the operation was aborted (the bar is shown at least in yellow).
    ''' </param>
    ''' 
    ''' <param name="hasFailed">
    ''' A value indicating whether the operation terminated due an error (the bar is shown in red).
    ''' </param>
    Friend Sub ShowMirrorResult(report As MirrorReport, isCancelled As Boolean, hasFailed As Boolean)

        If report Is Nothing Then
            Throw New ArgumentNullException(NameOf(report))
        End If

        Dim outcome As MirrorOutcome = If(hasFailed, MirrorOutcome.Failed, If(isCancelled, MirrorOutcome.Aborted, MirrorOutcome.Completed))
        Dim processed As Integer = Math.Max(0, Volatile.Read(Me.progressCurrent))
        Dim total As Integer = report.SourceDirectoryCount
        If total >= 0 Then
            processed = Math.Min(processed, total)
        End If
        report.Complete(outcome, processed)

        ' The bar is filled when the operation completes (also for a source directory without subdirectories).
        If (outcome = MirrorOutcome.Completed) Then
            Me.ElektroProgressBar1.Value = Me.ElektroProgressBar1.Maximum
        End If

        Me.ElektroProgressBar1.FormatString = Main.BuildResultText(processed, total, outcome, report.Severity)
        Me.SetProgressBarResultState(Main.GetResultSeverity(report.Severity, isCancelled, hasFailed))

        ' The report is always available when the operation ends.
        Me.ButtonReport.Enabled = True
        Me.ToolTipMain.SetToolTip(Me.ButtonReport,
                                  If(report.HasProblems,
                                     $"{report.GetSummary()}. Click to see the details.",
                                     "Click to see the details of the last mirror operation."))

    End Sub

    ''' <summary>
    ''' Sets the color state of the progress bar according to the severity of the mirror result.
    ''' </summary>
    ''' <param name="severity">
    ''' The severity.
    ''' </param>
    Private Sub SetProgressBarResultState(severity As MirrorReportSeverity)

        Dim state As DevCase.UserControls.Enums.ProgressBarState
        Select Case severity
            Case MirrorReportSeverity.None
                state = DevCase.UserControls.Enums.ProgressBarState.Normal
            Case MirrorReportSeverity.Warning
                state = DevCase.UserControls.Enums.ProgressBarState.Paused  ' Yellow.
            Case MirrorReportSeverity.Error
                state = DevCase.UserControls.Enums.ProgressBarState.Error   ' Red.
            Case Else
                Throw New InvalidOperationException($"Unexpected severity: {severity}")
        End Select

        ' The same text is displayed in every state.
        Me.ElektroProgressBar1.FormatStringPaused = Me.ElektroProgressBar1.FormatString
        Me.ElektroProgressBar1.FormatStringError = Me.ElektroProgressBar1.FormatString
        Me.ElektroProgressBar1.State = state

        ' The bar color of the dark theme (classic style) depends on the state.
        ThemeManager.ApplyTheme(Me.ElektroProgressBar1, ThemeManager.GetPalette(Me.currentTheme))

    End Sub

    ''' <summary>
    ''' Shows the window with the report of the ignored files and directories of the last mirror operation.
    ''' </summary>
    Private Sub ShowReportWindow()

        If Me.currentReport Is Nothing Then
            Throw New InvalidOperationException("No mirror operation was started yet.")
        End If

        Using reportForm As New MirrorReportForm(Me.currentReport, ThemeManager.GetPalette(Me.currentTheme), Me.currentTheme = AppTheme.Dark)
            reportForm.ShowDialog(Me)
        End Using

    End Sub

    ''' <summary>
    ''' Sets the text of the status bar. 
    ''' A path is compacted with an ellipsis to fit in the available width (the full path is shown in the tool tip), 
    ''' so the text never pushes the icon of the status bar out of the visible area.
    ''' </summary>
    ''' <param name="text">
    ''' The text.
    ''' </param>
    ''' 
    ''' <param name="isPath">
    ''' <see langword="True"/> if the text is a file system path.
    ''' </param>
    Private Sub SetStatusText(text As String, Optional isPath As Boolean = False)

        Dim label As ToolStripStatusLabel = Me.ToolStripStatusLabelFilepaths
        Dim availableWidth As Integer = Me.StatusStripFilepaths.DisplayRectangle.Width -
                                        label.Margin.Horizontal - label.Padding.Horizontal -
                                        Me.StatusStripFilepaths.ImageScalingSize.Width - 8

        Dim displayText As String = If(isPath, MirrorRules.RemoveLongPathPrefix(text), text)
        label.Text = GraphicsHelper.CompactPath(displayText, label.Font, Math.Max(1, availableWidth))
        label.ToolTipText = If(isPath, displayText, Nothing)

    End Sub

    ''' <summary>
    ''' Places the three columns of option check boxes, justified across the width of the 'Options' section: 
    ''' the first column is aligned with the left margin and the last column ends at the right margin.
    ''' </summary>
    Private Sub LayoutOptionColumns()

        Dim columns As CheckBox()() = {
            New CheckBox() {Me.CheckBoxAttribs, Me.CheckBoxDatestamps, Me.CheckBoxHiddenFiles},
            New CheckBox() {Me.CheckBoxSymLinks, Me.CheckBoxJunctions, Me.CheckBoxHardLinks},
            New CheckBox() {Me.CheckBoxIgnoreSecurityExceptions, Me.CheckBoxOverwrite}
        }

        Dim widths As Integer() = columns.Select(Function(column As CheckBox()) column.Max(Function(cb As CheckBox) cb.PreferredSize.Width)).ToArray()
        Dim positions As Integer() = Main.ComputeJustifiedColumnPositions(Me.CheckBoxAttribs.Left, Me.SectionHeaderOptions.Right, widths)

        For index As Integer = 0 To columns.Length - 1
            For Each checkBox As CheckBox In columns(index)
                checkBox.Left = positions(index)
            Next checkBox
        Next index

    End Sub

    ''' <summary>
    ''' Enables or disables the controls that allow the user to modify the directories and options 
    ''' (the labels and section headers are never disabled, so they keep a readable look in every theme).
    ''' <para></para>
    ''' This member is declared as <see langword="Friend"/> to allow unit testing.
    ''' </summary>
    ''' <param name="enabled">
    ''' <see langword="True"/> to enable the controls; <see langword="False"/> to disable them.
    ''' </param>
    Friend Sub SetInputControlsEnabled(enabled As Boolean)

        Me.TextBoxSource.Enabled = enabled
        Me.ButtonSource.Enabled = enabled
        Me.TextBoxDestination.Enabled = enabled
        Me.ButtonDestination.Enabled = enabled

        ' All the option check boxes (so a new option can never be forgotten here).
        For Each optionCheckBox As ThemedCheckBox In Me.Controls.OfType(Of ThemedCheckBox)()
            optionCheckBox.Enabled = enabled
        Next optionCheckBox

    End Sub

    ''' <summary>
    ''' Mirrors a link (directory junction or symbolic link) at the specified destination path.
    ''' <para></para>
    ''' The operation is idempotent: if an identical link already exists (e.g. created by a previous mirror operation), 
    ''' nothing is done. See <see cref="MirrorRules.DecideLinkAction"/> for the complete rules.
    ''' <para></para>
    ''' This member is declared as <see langword="Friend"/> to allow unit testing.
    ''' </summary>
    ''' <param name="destinationPath">
    ''' The full path of the link to create.
    ''' </param>
    ''' 
    ''' <param name="targetPath">
    ''' The target that the link must point to.
    ''' </param>
    ''' 
    ''' <param name="kind">
    ''' The kind of link.
    ''' </param>
    ''' 
    ''' <param name="overwrite">
    ''' A value indicating whether an existing different link or file at the destination path can be replaced.
    ''' </param>
    ''' <returns>
    ''' The <see cref="LinkAction"/> that was performed: 
    ''' <see cref="LinkAction.Create"/> or <see cref="LinkAction.Replace"/> (the link was created), 
    ''' <see cref="LinkAction.Skip"/> (an identical link already existed) 
    ''' or <see cref="LinkAction.Keep"/> (the existing destination item was kept, and the link was not created).
    ''' </returns>
    ''' <exception cref="IOException">
    ''' A regular directory exists at the destination path.
    ''' </exception>
    Friend Shared Function MirrorLink(destinationPath As String, targetPath As String, kind As LinkKind, overwrite As Boolean) As LinkAction

        Dim exists As Boolean = File.Exists(destinationPath) OrElse Directory.Exists(destinationPath)
        Dim attributes As FileAttributes
        Dim isLink As Boolean
        Dim isDirectory As Boolean
        Dim isIdentical As Boolean

        If exists Then
            attributes = File.GetAttributes(destinationPath)
            isLink = attributes.HasFlag(FileAttributes.ReparsePoint)
            isDirectory = attributes.HasFlag(FileAttributes.Directory)
            If isLink Then
                Dim existingTarget As String = Main.GetExistingLinkTarget(destinationPath, kind, isDirectory)
                isIdentical = (existingTarget IsNot Nothing) AndAlso
                              String.Equals(existingTarget, targetPath, StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Dim action As LinkAction = MirrorRules.DecideLinkAction(exists, isLink, isDirectory, isIdentical, overwrite)
        Select Case action

            Case LinkAction.Skip, LinkAction.Keep
                Return action

            Case LinkAction.Conflict
                Throw New IOException($"A regular directory already exists at '{MirrorRules.RemoveLongPathPrefix(destinationPath)}', so the link cannot be created there.")

            Case LinkAction.Replace
                ' Deleting a link removes only the link itself, never its target.
                File.SetAttributes(destinationPath, attributes And Not FileAttributes.ReadOnly)
                If isDirectory Then
                    Directory.Delete(destinationPath, recursive:=False)
                Else
                    File.Delete(destinationPath)
                End If

            Case LinkAction.Create
                ' Nothing to prepare.

            Case Else
                Throw New InvalidOperationException("Unexpected link action.")

        End Select

        Select Case kind
            Case LinkKind.DirectoryJunction
                Directories.CreateJunction(destinationPath, targetPath)

            Case LinkKind.DirectorySymbolicLink
                Directories.CreateSymbolicLinkAt(destinationPath, targetPath)

            Case LinkKind.FileSymbolicLink
                Files.CreateSymbolicLinkAt(destinationPath, targetPath)

            Case Else
                Throw New InvalidOperationException($"Unexpected link kind: {kind}")

        End Select

        Return action

    End Function

    ''' <summary>
    ''' Gets the target of an existing link, if it is a link of the specified kind.
    ''' </summary>
    ''' <param name="linkPath">
    ''' The path of the existing link (a reparse point).
    ''' </param>
    ''' 
    ''' <param name="kind">
    ''' The expected kind of link.
    ''' </param>
    ''' 
    ''' <param name="isDirectory">
    ''' A value indicating whether the existing link is a directory link.
    ''' </param>
    ''' <returns>
    ''' The target of the link, or <see langword="Nothing"/> if the existing link is of a different kind 
    ''' (so it is not identical to the link to create).
    ''' </returns>
    Private Shared Function GetExistingLinkTarget(linkPath As String, kind As LinkKind, isDirectory As Boolean) As String

        Select Case kind
            Case LinkKind.DirectoryJunction
                Return If(isDirectory AndAlso Directories.IsJunction(linkPath), Directories.GetMountPointTarget(linkPath), Nothing)

            Case LinkKind.DirectorySymbolicLink
                Return If(isDirectory AndAlso Directories.IsSymbolicLink(linkPath), Directories.GetSymbolicLinkDestination(linkPath), Nothing)

            Case LinkKind.FileSymbolicLink
                Return If(Not isDirectory AndAlso Files.IsSymbolicLink(linkPath), Files.GetSymbolicLinkDestination(linkPath), Nothing)

            Case Else
                Throw New InvalidOperationException($"Unexpected link kind: {kind}")

        End Select

    End Function

    ''' <summary>
    ''' Sets the visual state of the <see cref="Main.ButtonMirror"/> control.
    ''' </summary>
    ''' <param name="isRunning">
    ''' <see langword="True"/> to show the 'Abort' state; <see langword="False"/> to show the 'Build Mirror' state.
    ''' </param>
    Private Sub SetMirrorButtonState(isRunning As Boolean)

        If isRunning Then
            Me.ButtonMirror.Kind = ActionButtonKind.Danger
            Me.ButtonMirror.Glyph = ActionButtonGlyph.Stop
            Me.ButtonMirror.Text = "Abort"
        Else
            Me.ButtonMirror.Kind = ActionButtonKind.Primary
            Me.ButtonMirror.Glyph = ActionButtonGlyph.Play
            Me.ButtonMirror.Text = "Build Mirror"
        End If

    End Sub

    ''' <summary>
    ''' Validates the destination directory (<see cref="Options.DestinationDir"/>).
    ''' </summary>
    Private Sub ValidateDestinationDirectory()

        Dim conflictMessage As String
        Select Case MirrorRules.ValidateDestination(Options.SourceDir.FullName, Options.DestinationDir.FullName)

            Case DestinationValidationResult.Valid
                Me.TextBoxDestination.Text = Options.DestinationDir.FullName
                Me.ButtonMirror.Enabled = True
                Exit Sub

            Case DestinationValidationResult.SameAsSource
                conflictMessage = UIMessages.ButtonDestinationConflict1

            Case DestinationValidationResult.InsideSource
                conflictMessage = UIMessages.ButtonDestinationConflict2

            Case DestinationValidationResult.ParentOfSource
                conflictMessage = UIMessages.ButtonDestinationConflict3

            Case Else
                Throw New InvalidOperationException("Unexpected destination validation result.")

        End Select

        Options.DestinationDir = Nothing
        Me.TextBoxDestination.Text = ""
        Using msg As New CenteredMessageBox(owner:=Me, timeOut:=Timeout.Infinite)
            msg.Show(conflictMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Using

    End Sub

    ''' <summary>
    ''' Updates the progress bar and the status bar with the current progress of the mirror operation.
    ''' <para></para>
    ''' It does nothing while the source directories are being listed.
    ''' </summary>
    Private Sub UpdateProgressInfo()

        If Not Volatile.Read(Me.isListingCompleted) Then
            Exit Sub
        End If

        Me.EnsureProgressBarInitialized()

        Dim current As Integer = Math.Min(Volatile.Read(Me.progressCurrent), Me.ElektroProgressBar1.Maximum)
        If Me.ElektroProgressBar1.Value <> current Then
            Me.ElektroProgressBar1.Value = current
        End If

        Dim filePath As String = Volatile.Read(Me.currentFilePath)
        If (filePath IsNot Nothing) AndAlso
           Not String.Equals(Me.ToolStripStatusLabelFilepaths.ToolTipText, MirrorRules.RemoveLongPathPrefix(filePath), StringComparison.Ordinal) Then
            Me.SetStatusText(filePath, isPath:=True)
        End If

    End Sub

    ''' <summary>
    ''' Sets the initial values of the progress bar (maximum, value and format string), 
    ''' if they were not set yet for the current mirror operation.
    ''' </summary>
    Private Sub EnsureProgressBarInitialized()

        If (Me.isProgressBarInitialValuesSet) Then
            Exit Sub
        End If

        Me.isProgressBarInitialValuesSet = True
        ' A progress bar needs a maximum value greater than zero (a source directory could have no subdirectories).
        Me.ElektroProgressBar1.Maximum = Math.Max(1, Me.progressMaximum)
        Me.ElektroProgressBar1.Value = Math.Min(Me.progressCurrent, Me.ElektroProgressBar1.Maximum)
        Me.ElektroProgressBar1.FormatString = "#current of #total directories (#%)"

    End Sub

    ''' <summary>
    ''' Sets the pre-mirror operation values.
    ''' </summary>
    Friend Sub SetPreMirrorValues()
        ' Note: the normal state must be restored before resetting the value, 
        ' because a native progress bar in paused (yellow) or error (red) state does not repaint its value changes.
        Me.ElektroProgressBar1.FormatString = "Listing directories, this could take some seconds. Please be patient..."
        Me.SetProgressBarResultState(MirrorReportSeverity.None)
        Me.ElektroProgressBar1.Value = 0
        Me.ElektroProgressBar1.Update()
        Me.SetStatusText("Listing directories, this could take some seconds. Please be patient...")

        ' Reset progress indicator values.
        Me.isProgressBarInitialValuesSet = False
        Me.isListingCompleted = False
        Me.currentFilePath = Nothing
        Me.progressCurrent = 0
        Me.progressMaximum = 100000
    End Sub

#End Region

End Class

#End Region
