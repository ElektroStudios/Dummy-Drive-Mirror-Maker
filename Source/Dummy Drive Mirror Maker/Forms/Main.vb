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

''' ----------------------------------------------------------------------------------------------------
''' <summary>
''' The main application Form.
''' </summary>
''' ----------------------------------------------------------------------------------------------------
Public NotInheritable Class Main : Inherits Form

#Region " Fields "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Keeps track of the current source filepath to be mirrored.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private currentFilePath As String

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Flag to determine whether the initial values of the progress bar has been set.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isProgressBarInitialValuesSet As Boolean

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Flag to determine whether the progressbar should be updated.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private incrementProgressBarValue As Boolean

    ' Progress indicator values.
    Private progressPercent As Integer '%
    Private progressCurrent As Integer ' of progressMaximum.
    Private progressMaximum As Integer

#End Region

#Region " Event Handlers "

#Region " Form "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Form.Load"/> event of the <see cref="Main"/> Form.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

#End Region

#Region " TextBoxes "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.DragEnter"/> event of the <see cref="Main.TextBoxSource"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
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

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.DragDrop"/> event of the <see cref="Main.TextBoxSource"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub TextBoxSource_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxSource.DragDrop

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            Dim dirpath As String = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String)).Single()
            Options.SourceDir = New DirectoryInfo(dirpath)

            Me.TextBoxSource.Text = Options.SourceDir.FullName
            If (Options.TargetDir IsNot Nothing) Then
                Me.ButtonMirror.Enabled = False
                Me.ValidateTargetDirectory()

            Else
                Me.ButtonTarget.Enabled = True
                Me.TextBoxTarget.Enabled = True

            End If

        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.DragEnter"/> event of the <see cref="Main.TextBoxTarget"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub TextBoxTarget_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxTarget.DragEnter

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

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.DragDrop"/> event of the <see cref="Main.TextBoxTarget"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DragEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub TextBoxTarget_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxTarget.DragDrop

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            Dim dirpath As String = DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String)).Single()

            Options.TargetDir = New DirectoryInfo(dirpath)
            Me.ButtonMirror.Enabled = False
            Me.ValidateTargetDirectory()
        End If

    End Sub

#End Region

#Region " CheckBoxes "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxAttribs"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub CheckBoxAttribs_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAttribs.CheckedChanged
        Options.PreserveAttribs = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxDatestamps"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub CheckBoxDatestamps_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxDatestamps.CheckedChanged
        Options.PreserveTimestamps = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxHiddenFiles"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub CheckBoxHiddenFiles_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHiddenFiles.CheckedChanged
        Options.MirrorHiddenFiles = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxSymLinks"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub CheckBoxSymLinks_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSymLinks.CheckedChanged
        Options.MirrorSymbolicLinks = DirectCast(sender, CheckBox).Checked
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="CheckBox.CheckedChanged"/> event of the <see cref="Main.CheckBoxIgnoreSecurityExceptions"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub CheckBoxIgnoreSecurityExceptions_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIgnoreSecurityExceptions.CheckedChanged
        Options.IgnoreSecurityExceptions = DirectCast(sender, CheckBox).Checked
    End Sub

#End Region

#Region " Buttons "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonSource"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
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
                If (Options.TargetDir IsNot Nothing) Then
                    Me.ButtonMirror.Enabled = False
                    Me.ValidateTargetDirectory()

                Else
                    Me.ButtonTarget.Enabled = True
                    Me.TextBoxTarget.Enabled = True

                End If

            End If
        End Using

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonTarget"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonTarget_Click(sender As Object, e As EventArgs) Handles ButtonTarget.Click

        Using vfbd As New VistaFolderBrowserDialog
            vfbd.Description = "Select a target directory."
            vfbd.UseDescriptionForTitle = True
            vfbd.ShowNewFolderButton = False
            vfbd.SelectedPath = If(Options.TargetDir IsNot Nothing, Options.TargetDir.FullName, vfbd.SelectedPath)

            Dim dlgResult As DialogResult = vfbd.ShowDialog()
            If (dlgResult = DialogResult.OK) Then
                Options.TargetDir = New DirectoryInfo(vfbd.SelectedPath)
                Me.ValidateTargetDirectory()
            End If

        End Using

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="Main.ButtonMirror"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonMirror_Click(sender As Object, e As EventArgs) Handles ButtonMirror.Click

        ' Abort a pending mirror operation by user demand.
        If (Me.ElektroBackgroundWorker1.State = ElektroBackgroundWorkerState.Running) Then
            Me.ElektroBackgroundWorker1.Cancel()
            Exit Sub
        End If

        ' Disable UI controls to prevent bad things (such as button double-click).
        Me.ButtonMirror.Enabled = False
        Me.GroupBoxOptions.Enabled = False
        Me.GroupBoxDirectories.Enabled = False

        ' Set pre-mirror information on controls, and reset progress values.
        Me.SetPreMirrorValues()

        ' Run the mirror operation in background.
        Me.ElektroBackgroundWorker1 = New ElektroBackgroundWorker()
        Me.ElektroBackgroundWorker1.RunAsync()

        ' Set the 'ABORT' visuals.
        Me.ButtonMirror.ForeColor = Color.Red
        Me.ButtonMirror.BackgroundImage = My.Resources.Abort
        Me.ButtonMirror.Text = "ABORT"
        Me.ButtonMirror.Enabled = True

    End Sub

#End Region

#End Region

#Region " Background Worker "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ElektroBackgroundWorker.DoWork"/> event 
    ''' of the <see cref="Main.ElektroBackgroundWorker1"/> component.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="DoWorkEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepperBoundary>
    Private Sub ElektroBackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles ElektroBackgroundWorker1.DoWork

        ' Short reference for this 'ElektroBackgroundWorker' instance.
        Dim worker As ElektroBackgroundWorker = DirectCast(sender, ElektroBackgroundWorker)

        ' Flag to determine whether file-exists conflicts must be ignored.
        Dim ignoreFileExistsConflict As Boolean
        ' Flag to determine whether the user answered to file-exists conflicts question.
        Dim ignoreFileExistsConflictAnswered As Boolean

        ' Flag to determine whether folder-exists conflicts must be ignored.
        Dim ignoreFolderExistsConflict As Boolean
        ' Flag to determine whether the user answered to folder-exists conflicts question.
        Dim ignoreFolderExistsConflictAnswered As Boolean

        ' Get all the sub-directories in the root source dir.
        Dim srcSubDirs As IEnumerable(Of DirectoryInfo) =
            {Options.SourceDir}.Concat(Directories.GetDirs(Options.SourceDir, SearchOption.AllDirectories, {"*"}, {"*"}, True,
                                throwOnError:=False).OrderBy(Function(di As DirectoryInfo) di.FullName))

        ' Set progressbar ma. value.
        Me.progressMaximum = srcSubDirs.Count

        For Each srcSubDir As DirectoryInfo In srcSubDirs
            ' Cancel mirror operation by user demand.
            If (worker.CancellationPending) Then ' Cancel the background operation.
                e.Cancel = True
                Exit For
            End If

            ' By default ignore the recycle bin folder, 
            ' it hasn't useful purpose in real-world usage to will mirror it.
            '
            ' Reminder of recycle bin locations:
            ' ----------------------------------
            ' VISTA, 7, 8/8.1, 10.............: X:\$Recycle.Bin
            ' 2000 (NTFS), XP (NTFS)..........: X:\RECYCLER
            ' 95, 98, 2000 (FAT32), XP (FAT32): X:\RECYCLED
            If (srcSubDir.FullName.ToLower() Like "?:\$recycle.bin*") OrElse
               (srcSubDir.FullName.ToLower() Like "?:\recycler*") OrElse
               (srcSubDir.FullName.ToLower() Like "?:\recycled*") Then
                Continue For
            End If

            ' By default ignore the 'X:\System Volume Information' folder, 
            ' it hasn't useful purpose in real-world usage to mirror it, 
            ' and maybe the user have not permission to read this directory.
            If (srcSubDir.FullName.ToLower() Like "?:\system volume information*") Then
                Continue For
            End If

            ' Build destination directory full path.
            Dim dstDir As New DirectoryInfo(Options.TargetDir.FullName & "\" & (srcSubDir.FullName.Replace(Options.SourceDir.FullName, "")))

#If DEBUG Then
            Debug.WriteLine(String.Format("About to create dir.: '{0}'", dstDir.FullName))
#End If

            '' Ask the user when a target directory already exists before creating dummy files inside, 
            '' because the target directory could contain user files
            '' and those files will be end messed up with dummy files.
            'If (dstDir.Exists()) AndAlso Not (ignoreFolderExistsConflict) Then
            '    Dim dlgResult As DialogResult =
            '        MessageBox.Show(String.Format(UIMessages.DirectoryExistQuestion1, dstDir.FullName),
            '                        Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)


            '    ' In case of the user is really aware of what he is doing, 
            '    ' we simplify things by brinding to him the chance to discard more folder conflict questions like this.
            '    If (dlgResult = DialogResult.Yes) AndAlso Not (ignoreFolderExistsConflictAnswered) Then
            '        dlgResult = MessageBox.Show(String.Format(UIMessages.DirectoryExistQuestion2, dstDir.FullName),
            '                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            '        Select Case dlgResult
            '            Case DialogResult.Yes
            '                ignoreFolderExistsConflict = True
            '            Case Else
            '                ignoreFolderExistsConflict = False
            '        End Select
            '        ignoreFolderExistsConflictAnswered = True
            '    ElseIf (dlgResult = DialogResult.No) Then
            '        e.Cancel = True
            '        Exit For
            '    End If
            'End If

            Try
                ' Ignore hidden folder by user-demand.
                If Not (Options.MirrorHiddenFiles) AndAlso
                       (srcSubDir.Attributes.HasFlag(FileAttributes.Hidden)) Then
#If DEBUG Then
                    Debug.WriteLine(String.Format("Directory is hidden. Ignored.: '{0}'", srcSubDir.FullName))
#End If
                    Continue For
                End If

                ' Ignore mount points.
                If (Directories.IsMountPoint(srcSubDir)) Then
#If DEBUG Then
                    Debug.WriteLine(String.Format("Directory is a mount point. Ignored.: '{0}'", srcSubDir.FullName))
#End If
                    Continue For
                End If

                Dim isSymbolicLink As Boolean = Directories.IsSymbolicLink(srcSubDir)
                If (isSymbolicLink) Then
                    If (Options.MirrorSymbolicLinks) Then
                        ' Create a copy of the symbolic link pointing to the same original target path.
                        Dim targetDir As DirectoryInfo = Directories.GetSymbolicLinkTarget(srcSubDir)
                        Directories.CreateSymbolicLink(targetDir, dstDir.Parent)
                        Continue For

                    Else ' Ignore symbolic links by user-demand.
#If DEBUG Then
                        Debug.WriteLine(String.Format("Directory is a symbolic link. Ignored.: '{0}'", srcSubDir.FullName))
#End If
                        Continue For

                    End If
                End If

                ' Create the destination directory.
                dstDir.Create()

                ' Set directory timestamps.
                If (Options.PreserveTimestamps) Then
                    dstDir.CreationTime = srcSubDir.CreationTime
                    dstDir.LastAccessTime = srcSubDir.LastAccessTime
                    dstDir.LastWriteTime = srcSubDir.LastWriteTime
                End If

                ' Set directory attributes.
                If (Options.PreserveAttribs) Then
                    dstDir.Attributes = srcSubDir.Attributes
                End If

                ' Ignore security access exceptions by user-demand.
            Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) AndAlso
                                       (Options.IgnoreSecurityExceptions)
                Continue For

            Catch ex As Exception
                Dim dlgResult As DialogResult =
                    MessageBox.Show(String.Format(UIMessages.DirectoryException, ex.Message),
                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                If (dlgResult = DialogResult.No) Then
                    e.Cancel = True
                    Exit Sub
                End If

            End Try

            ' Get top files of the current source sub-directory.
            Dim topFiles As IEnumerable(Of FileInfo) =
                Files.GetFiles(srcSubDir, SearchOption.TopDirectoryOnly, {"*"}, {"*"}, True,
                               throwOnError:=False).OrderBy(Function(fi As FileInfo) fi.FullName)

            For Each topFile As FileInfo In topFiles

                If (worker.CancellationPending) Then ' Cancel the background operation.
                    e.Cancel = True
                    Exit For
                End If

                Me.currentFilePath = topFile.FullName

                ' Build destination file full path.
                Dim dstFile As New FileInfo(Options.TargetDir.FullName & "\" & (topFile.FullName.Replace(Options.SourceDir.FullName, "")))

#If DEBUG Then
                Debug.WriteLine(String.Format("About to create file: '{0}'", dstFile.FullName))
#End If

                ' Ask the user when a target file already exists before replacing it with a dummy file.
                If (dstFile.Exists()) AndAlso Not (ignoreFileExistsConflict) Then
                    Dim dlgResult As DialogResult =
                        MessageBox.Show(String.Format(UIMessages.FileExistQuestion1, dstFile.FullName),
                                        Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                    ' In case of the user is really aware of what he is doing, 
                    ' we simplify things by brinding to him the chance to discard more file conflict questions like this.
                    If (dlgResult = DialogResult.Yes) AndAlso Not (ignoreFileExistsConflictAnswered) Then
                        dlgResult = MessageBox.Show(String.Format(UIMessages.FileExistQuestion2, dstFile.FullName),
                                                    Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        Select Case dlgResult
                            Case DialogResult.Yes
                                ignoreFileExistsConflict = True
                            Case Else
                                ignoreFileExistsConflict = False
                        End Select
                        ignoreFileExistsConflictAnswered = True
                    ElseIf (dlgResult = DialogResult.No) Then
                        e.Cancel = True
                        Exit For
                    End If
                End If

                Try
                    ' Ignore hidden file by user-demand.
                    If Not (Options.MirrorHiddenFiles) AndAlso
                           (topFile.Attributes.HasFlag(FileAttributes.Hidden)) Then
                        Continue For
                    End If

                    Dim isSymbolicLink As Boolean = Files.IsSymbolicLink(topFile)
                    If (isSymbolicLink) Then
                        If (Options.MirrorSymbolicLinks) Then
                            ' Create a copy of the symbolic link pointing to the same original target path.
                            Dim targetFile As FileInfo = Files.GetSymbolicLinkTarget(topFile)
                            Files.CreateSymbolicLink(topFile, dstDir)
                            Continue For

                        Else ' Ignore symbolic links by user-demand.
#If DEBUG Then
                            Debug.WriteLine(String.Format("Directory is a symbolic link. Ignored.: '{0}'", srcSubDir.FullName))
#End If
                            Continue For

                        End If
                    End If

                    ' Create the (dummy) destination file.
                    Using fs As FileStream = dstFile.Create()
                    End Using

                    ' Set file timestamps.
                    If (Options.PreserveTimestamps) Then
                        dstFile.CreationTime = topFile.CreationTime
                        dstFile.LastAccessTime = topFile.LastAccessTime
                        dstFile.LastWriteTime = topFile.LastWriteTime
                    End If

                    ' Set file attributes.
                    If (Options.PreserveAttribs) Then
                        dstFile.Attributes = topFile.Attributes
                    End If

                    ' Ignore security access exceptions by user-demand.
                Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) AndAlso
                                           (Options.IgnoreSecurityExceptions)
                    Continue For

                Catch ex As Exception
                    Dim dlgResult As DialogResult =
                        MessageBox.Show(String.Format(UIMessages.FileException, ex.Message),
                                        Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                    If (dlgResult = DialogResult.No) Then
                        e.Cancel = True
                        Exit Sub

                    End If

                End Try

                worker.ReportProgress(Me.progressPercent)

            Next topFile

            Me.progressPercent = CInt((Interlocked.Increment(Me.progressCurrent) / Me.progressMaximum) * 100)
            Me.incrementProgressBarValue = True

        Next srcSubDir

        If Not (worker.CancellationPending) AndAlso (Me.progressPercent < 100) Then
            worker.ReportProgress(100)
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ElektroBackgroundWorker.ProgressChanged"/> event 
    ''' of the <see cref="Main.ElektroBackgroundWorker1"/> component.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="ProgressChangedEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepperBoundary>
    Private Sub ElektroBackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) _
    Handles ElektroBackgroundWorker1.ProgressChanged

        If Not (Me.isProgressBarInitialValuesSet) Then
            Me.isProgressBarInitialValuesSet = True
            Me.ElektroProgressBar1.Maximum = Me.progressMaximum
            Me.ElektroProgressBar1.Value = Me.progressCurrent
            Me.ElektroProgressBar1.FormatString = "#current of #total directories (#%)"
            Exit Sub

        End If

        Me.ToolStripStatusLabelFilepaths.Text = Me.currentFilePath

        If (Me.incrementProgressBarValue) Then
            Me.ElektroProgressBar1.Increment(1)
            Me.incrementProgressBarValue = False
        End If

#If DEBUG Then
        Debug.WriteLine(String.Format("[+] Background Work Progress: {00}%", e.ProgressPercentage))
#End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ElektroBackgroundWorker.RunWorkerCompleted"/> event 
    ''' of the <see cref="Main.ElektroBackgroundWorker1"/> component.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' 
    ''' <param name="e">
    ''' The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepperBoundary>
    Private Sub ElektroBackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) _
    Handles ElektroBackgroundWorker1.RunWorkerCompleted

        ' Restore button style.
        Me.ButtonMirror.ForeColor = SystemColors.ControlText
        Me.ButtonMirror.BackgroundImage = My.Resources.Copy
        Me.ButtonMirror.Text = "Build Mirror"

        ' Update UI progress information.
        Me.ElektroProgressBar1.Value = Me.progressCurrent
        Dim ignoredDirectoryCount As Integer = (Me.ElektroProgressBar1.Maximum - Me.ElektroProgressBar1.Value)
        Me.ElektroProgressBar1.FormatString = String.Format("#current of #total directories (100%) - {0} directories were ignored/not copied.", ignoredDirectoryCount)
        Me.ToolStripStatusLabelFilepaths.Text = "Waiting..."

        ' Show termination message.
        If (e.Cancelled) Then
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(UIMessages.MirrorCompletedAborted, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using

        ElseIf (e.Error IsNot Nothing) Then
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(String.Format(UIMessages.MirrorCompletedError, e.Error.Message), Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using

        Else
            Using msg As New CenteredMessageBox(owner:=Me)
                msg.Show(UIMessages.MirrorCompletedSuccessful, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        End If

        ' Re-enable UI controls.
        Me.GroupBoxOptions.Enabled = True
        Me.GroupBoxDirectories.Enabled = True
        Me.ButtonMirror.Enabled = True

    End Sub

#End Region

#Region " Private Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Validates the source directory (<see cref="Options.TargetDir"/>).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ValidateTargetDirectory()

        If (Options.TargetDir.FullName.Equals(Options.SourceDir.FullName)) Then
            Options.TargetDir = Nothing
            Me.TextBoxTarget.Text = ""
            Using msg As New CenteredMessageBox(owner:=Me, timeOut:=Timeout.Infinite)
                msg.Show(UIMessages.ButtonTargetConflict1, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using

        ElseIf (Options.TargetDir.FullName.StartsWith(Options.SourceDir.FullName)) Then
            Options.TargetDir = Nothing
            Me.TextBoxTarget.Text = ""
            Using msg As New CenteredMessageBox(owner:=Me, timeOut:=Timeout.Infinite)
                msg.Show(UIMessages.ButtonTargetConflict2, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using

        ElseIf (Options.SourceDir.Parent IsNot Nothing) AndAlso (Options.SourceDir.Parent.FullName.Equals(Options.TargetDir.FullName)) Then
            Options.TargetDir = Nothing
            Me.TextBoxTarget.Text = ""
            Using msg As New CenteredMessageBox(owner:=Me, timeOut:=Timeout.Infinite)
                msg.Show(UIMessages.ButtonTargetConflict3, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using

        Else
            Me.TextBoxTarget.Text = Options.TargetDir.FullName
            Me.ButtonMirror.Enabled = (Options.TargetDir IsNot Nothing)

        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Sets the pre-mirror operation values.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub SetPreMirrorValues()
        Me.ElektroProgressBar1.FormatString = "Listing directories, this could take some seconds. Please be patient..."
        Me.ToolStripStatusLabelFilepaths.Text = "Listing directories, this could take some seconds. Please be patient..."
        Me.ElektroProgressBar1.Value = 0

        ' Reset progress indicator values.
        Me.isProgressBarInitialValuesSet = False
        Me.incrementProgressBarValue = False
        Me.progressPercent = 0
        Me.progressCurrent = 0
        Me.progressMaximum = 100000
    End Sub

#End Region

End Class

#End Region
