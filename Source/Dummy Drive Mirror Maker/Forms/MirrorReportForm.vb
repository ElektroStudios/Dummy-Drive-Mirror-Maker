' ***********************************************************************
' Assembly : Dummy Drive Mirror Maker
' ***********************************************************************

#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Text

#End Region

#Region " MirrorReportForm "

''' <summary>
''' A window that shows the report of the files and directories
''' that were ignored (not copied) by a mirror operation, and the reason of each one.
''' <para></para>
''' The report can be copied to the clipboard or saved to a text file.
''' </summary>
Friend NotInheritable Class MirrorReportForm : Inherits Form

#Region " Private Fields "

    ''' <summary>
    ''' The report shown in this window.
    ''' </summary>
    Private ReadOnly report As MirrorReport

    ''' <summary>
    ''' A value indicating whether the title bar uses the dark mode.
    ''' </summary>
    Private ReadOnly useDarkTitleBar As Boolean

#End Region

#Region " Controls "

    ''' <summary>
    ''' The label that shows the summary of the report.
    ''' </summary>
    Friend ReadOnly LabelSummary As Label

    ''' <summary>
    ''' The read-only text box that shows the full report.
    ''' </summary>
    Friend ReadOnly TextBoxReport As TextBox

    ''' <summary>
    ''' The button that copies the report to the clipboard.
    ''' </summary>
    Friend ReadOnly ButtonCopy As Button

    ''' <summary>
    ''' The button that saves the report to a text file.
    ''' </summary>
    Friend ReadOnly ButtonSave As Button

    ''' <summary>
    ''' The button that closes the window.
    ''' </summary>
    Friend ReadOnly ButtonClose As Button

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MirrorReportForm"/> class.
    ''' </summary>
    ''' <param name="report">
    ''' The report to show.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette of the current theme.
    ''' </param>
    '''
    ''' <param name="useDarkTitleBar">
    ''' <see langword="True"/> to use the dark mode in the title bar.
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="report"/> or <paramref name="palette"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub New(report As MirrorReport, palette As ThemePalette, useDarkTitleBar As Boolean)

        If report Is Nothing Then
            Throw New ArgumentNullException(NameOf(report))
        End If
        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        Me.report = report
        Me.useDarkTitleBar = useDarkTitleBar

        Me.SuspendLayout()

        Me.Font = New Font("Segoe UI", 9.0!)
        Me.Text = "Ignored files and directories"
        Me.ClientSize = New Size(760, 480)
        Me.MinimumSize = New Size(480, 300)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.MinimizeBox = False
        Me.MaximizeBox = True

        Me.LabelSummary = New Label() With {
            .Name = NameOf(Me.LabelSummary),
            .AutoEllipsis = True,
            .Location = New Point(12, 12),
            .Size = New Size(736, 20),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right,
            .Text = If(report.HasProblems,
                       $"{report.GetSummary()}.",
                       "All the files and directories were mirrored, except the items excluded by the options or by design (if any).")
        }

        Me.TextBoxReport = New TextBox() With {
            .Name = NameOf(Me.TextBoxReport),
            .Multiline = True,
            .ReadOnly = True,
            .MaxLength = 0,
            .WordWrap = False,
            .ScrollBars = ScrollBars.Both,
            .Font = New Font("Consolas", 9.0!),
            .Location = New Point(12, 38),
            .Size = New Size(736, 386),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right,
            .Text = report.ToText()
        }

        Me.ButtonCopy = MirrorReportForm.CreateButton(NameOf(Me.ButtonCopy), "Copy to clipboard", New Point(12, 436), 140)
        Me.ButtonCopy.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left

        Me.ButtonSave = MirrorReportForm.CreateButton(NameOf(Me.ButtonSave), "Save as...", New Point(160, 436), 100)
        Me.ButtonSave.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left

        Me.ButtonClose = MirrorReportForm.CreateButton(NameOf(Me.ButtonClose), "Close", New Point(648, 436), 100)
        Me.ButtonClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.ButtonClose.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({Me.LabelSummary, Me.TextBoxReport, Me.ButtonCopy, Me.ButtonSave, Me.ButtonClose})
        Me.AcceptButton = Me.ButtonClose
        Me.CancelButton = Me.ButtonClose

        AddHandler Me.ButtonCopy.Click, AddressOf Me.ButtonCopy_Click
        AddHandler Me.ButtonSave.Click, AddressOf Me.ButtonSave_Click

        ThemeManager.ApplyTheme(Me, palette)

        Me.ResumeLayout(performLayout:=True)

    End Sub

#End Region

#Region " Event Invocators "

    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event.
    ''' <para></para>
    ''' The theme of the title bar is applied here, as soon as the window handle exists.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        ThemeManager.ApplyTitleBarTheme(Me, Me.useDarkTitleBar)
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Form.Shown"/> event.
    ''' <para></para>
    ''' The caret is moved to the beginning of the report, so the text is not fully selected.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        Me.TextBoxReport.Select(0, 0)
        Me.ButtonClose.Focus()
    End Sub

#End Region

#Region " Event Handlers "

    ''' <summary>
    ''' Handles the <see cref="Control.Click"/> event of the <see cref="MirrorReportForm.ButtonCopy"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    '''
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonCopy_Click(sender As Object, e As EventArgs)
        Clipboard.SetText(Me.TextBoxReport.Text, TextDataFormat.UnicodeText)
    End Sub

    ''' <summary>
    ''' Handles the <see cref="Control.Click"/> event of the <see cref="MirrorReportForm.ButtonSave"/> control.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    '''
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub ButtonSave_Click(sender As Object, e As EventArgs)

        Using dialog As New SaveFileDialog() With {
            .Title = "Save the report",
            .Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            .DefaultExt = "txt",
            .AddExtension = True,
            .OverwritePrompt = True,
            .FileName = MirrorReportForm.GetDefaultFileName(Me.report.StartTime)
        }

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                File.WriteAllText(dialog.FileName, Me.TextBoxReport.Text, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))
            End If
        End Using

    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Gets the default file name to save a report.
    ''' </summary>
    ''' <param name="startTime">
    ''' The date and time when the mirror operation started.
    ''' </param>
    ''' <returns>
    ''' The file name (for example: <c>Mirror report 2026-11-08 18.30.05.txt</c>).
    ''' </returns>
    Friend Shared Function GetDefaultFileName(startTime As Date) As String
        Return $"Mirror report {startTime:yyyy-MM-dd HH.mm.ss}.txt"
    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Creates a button of the bottom row.
    ''' </summary>
    ''' <param name="name">
    ''' The name of the button.
    ''' </param>
    '''
    ''' <param name="text">
    ''' The text of the button.
    ''' </param>
    '''
    ''' <param name="location">
    ''' The location of the button.
    ''' </param>
    '''
    ''' <param name="width">
    ''' The width of the button.
    ''' </param>
    ''' <returns>
    ''' The button.
    ''' </returns>
    Private Shared Function CreateButton(name As String, text As String, location As Point, width As Integer) As Button
        Return New Button() With {
            .Name = name,
            .Text = text,
            .Location = location,
            .Size = New Size(width, 32),
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = True
        }
    End Function

#End Region

End Class

#End Region
