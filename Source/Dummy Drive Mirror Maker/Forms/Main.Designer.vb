<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.PictureBoxLogo = New System.Windows.Forms.PictureBox()
        Me.SectionHeaderDirectories = New SectionHeaderLabel()
        Me.LabelSource = New System.Windows.Forms.Label()
        Me.TextBoxSource = New System.Windows.Forms.TextBox()
        Me.ButtonSource = New System.Windows.Forms.Button()
        Me.LabelDestination = New System.Windows.Forms.Label()
        Me.TextBoxDestination = New System.Windows.Forms.TextBox()
        Me.ButtonDestination = New System.Windows.Forms.Button()
        Me.SectionHeaderOptions = New SectionHeaderLabel()
        Me.CheckBoxAttribs = New ThemedCheckBox()
        Me.CheckBoxDatestamps = New ThemedCheckBox()
        Me.CheckBoxHiddenFiles = New ThemedCheckBox()
        Me.CheckBoxSymLinks = New ThemedCheckBox()
        Me.CheckBoxIgnoreSecurityExceptions = New ThemedCheckBox()
        Me.CheckBoxOverwrite = New ThemedCheckBox()
        Me.CheckBoxJunctions = New ThemedCheckBox()
        Me.CheckBoxHardLinks = New ThemedCheckBox()
        Me.ElektroProgressBar1 = New DevCase.UserControls.Controls.ElektroProgressBar()
        Me.ButtonGitHub = New IconButton()
        Me.ToggleSwitchTheme = New ToggleSwitch()
        Me.ButtonMirror = New ActionButton()
        Me.ButtonReport = New ActionButton()
        Me.StatusStripFilepaths = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabelFilepaths = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.TimerProgress = New System.Windows.Forms.Timer(Me.components)
        Me.ElektroBackgroundWorker1 = New DevCase.Core.Threading.Types.ElektroBackgroundWorker()
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStripFilepaths.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBoxLogo
        '
        Me.PictureBoxLogo.BackColor = System.Drawing.Color.Black
        Me.PictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PictureBoxLogo.Image = Global.My.Resources.Resources.Logo
        Me.PictureBoxLogo.Location = New System.Drawing.Point(0, 0)
        Me.PictureBoxLogo.Name = "PictureBoxLogo"
        Me.PictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxLogo.Size = New System.Drawing.Size(680, 91)
        Me.PictureBoxLogo.TabIndex = 0
        Me.PictureBoxLogo.TabStop = False
        '
        'SectionHeaderDirectories
        '
        Me.SectionHeaderDirectories.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.SectionHeaderDirectories.Location = New System.Drawing.Point(16, 143)
        Me.SectionHeaderDirectories.Name = "SectionHeaderDirectories"
        Me.SectionHeaderDirectories.Size = New System.Drawing.Size(648, 20)
        Me.SectionHeaderDirectories.TabIndex = 3
        Me.SectionHeaderDirectories.Text = "Directories"
        '
        'LabelSource
        '
        Me.LabelSource.AutoSize = True
        Me.LabelSource.Location = New System.Drawing.Point(16, 176)
        Me.LabelSource.Name = "LabelSource"
        Me.LabelSource.Size = New System.Drawing.Size(43, 15)
        Me.LabelSource.TabIndex = 4
        Me.LabelSource.Text = "Source"
        '
        'TextBoxSource
        '
        Me.TextBoxSource.AllowDrop = True
        Me.TextBoxSource.Location = New System.Drawing.Point(100, 172)
        Me.TextBoxSource.Name = "TextBoxSource"
        Me.TextBoxSource.ReadOnly = True
        Me.TextBoxSource.Size = New System.Drawing.Size(470, 23)
        Me.TextBoxSource.TabIndex = 5
        Me.ToolTipMain.SetToolTip(Me.TextBoxSource, "Select a directory with the 'Browse...' button, or drag and drop a directory here.")
        '
        'ButtonSource
        '
        Me.ButtonSource.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonSource.Location = New System.Drawing.Point(578, 171)
        Me.ButtonSource.Name = "ButtonSource"
        Me.ButtonSource.Size = New System.Drawing.Size(86, 25)
        Me.ButtonSource.TabIndex = 6
        Me.ButtonSource.Text = "Browse..."
        Me.ButtonSource.UseVisualStyleBackColor = True
        '
        'LabelDestination
        '
        Me.LabelDestination.AutoSize = True
        Me.LabelDestination.Location = New System.Drawing.Point(16, 208)
        Me.LabelDestination.Name = "LabelDestination"
        Me.LabelDestination.Size = New System.Drawing.Size(67, 15)
        Me.LabelDestination.TabIndex = 7
        Me.LabelDestination.Text = "Destination"
        '
        'TextBoxDestination
        '
        Me.TextBoxDestination.AllowDrop = True
        Me.TextBoxDestination.Enabled = False
        Me.TextBoxDestination.Location = New System.Drawing.Point(100, 204)
        Me.TextBoxDestination.Name = "TextBoxDestination"
        Me.TextBoxDestination.ReadOnly = True
        Me.TextBoxDestination.Size = New System.Drawing.Size(470, 23)
        Me.TextBoxDestination.TabIndex = 8
        Me.ToolTipMain.SetToolTip(Me.TextBoxDestination, "Select a directory with the 'Browse...' button, or drag and drop a directory here.")
        '
        'ButtonDestination
        '
        Me.ButtonDestination.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonDestination.Enabled = False
        Me.ButtonDestination.Location = New System.Drawing.Point(578, 203)
        Me.ButtonDestination.Name = "ButtonDestination"
        Me.ButtonDestination.Size = New System.Drawing.Size(86, 25)
        Me.ButtonDestination.TabIndex = 9
        Me.ButtonDestination.Text = "Browse..."
        Me.ButtonDestination.UseVisualStyleBackColor = True
        '
        'SectionHeaderOptions
        '
        Me.SectionHeaderOptions.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.SectionHeaderOptions.Location = New System.Drawing.Point(16, 241)
        Me.SectionHeaderOptions.Name = "SectionHeaderOptions"
        Me.SectionHeaderOptions.Size = New System.Drawing.Size(648, 20)
        Me.SectionHeaderOptions.TabIndex = 10
        Me.SectionHeaderOptions.Text = "Options"
        '
        'CheckBoxAttribs
        '
        Me.CheckBoxAttribs.AutoSize = True
        Me.CheckBoxAttribs.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxAttribs.Location = New System.Drawing.Point(18, 267)
        Me.CheckBoxAttribs.Name = "CheckBoxAttribs"
        Me.CheckBoxAttribs.Size = New System.Drawing.Size(145, 19)
        Me.CheckBoxAttribs.TabIndex = 11
        Me.CheckBoxAttribs.Text = "Preserve file attributes"
        Me.CheckBoxAttribs.UseVisualStyleBackColor = True
        '
        'CheckBoxDatestamps
        '
        Me.CheckBoxDatestamps.AutoSize = True
        Me.CheckBoxDatestamps.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxDatestamps.Location = New System.Drawing.Point(18, 292)
        Me.CheckBoxDatestamps.Name = "CheckBoxDatestamps"
        Me.CheckBoxDatestamps.Size = New System.Drawing.Size(155, 19)
        Me.CheckBoxDatestamps.TabIndex = 12
        Me.CheckBoxDatestamps.Text = "Preserve file timestamps"
        Me.CheckBoxDatestamps.UseVisualStyleBackColor = True
        '
        'CheckBoxHiddenFiles
        '
        Me.CheckBoxHiddenFiles.AutoSize = True
        Me.CheckBoxHiddenFiles.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxHiddenFiles.Location = New System.Drawing.Point(18, 317)
        Me.CheckBoxHiddenFiles.Name = "CheckBoxHiddenFiles"
        Me.CheckBoxHiddenFiles.Size = New System.Drawing.Size(187, 19)
        Me.CheckBoxHiddenFiles.TabIndex = 13
        Me.CheckBoxHiddenFiles.Text = "Mirror hidden files and folders"
        Me.CheckBoxHiddenFiles.UseVisualStyleBackColor = True
        '
        'CheckBoxSymLinks
        '
        Me.CheckBoxSymLinks.AutoSize = True
        Me.CheckBoxSymLinks.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxSymLinks.Location = New System.Drawing.Point(248, 267)
        Me.CheckBoxSymLinks.Name = "CheckBoxSymLinks"
        Me.CheckBoxSymLinks.Size = New System.Drawing.Size(135, 19)
        Me.CheckBoxSymLinks.TabIndex = 14
        Me.CheckBoxSymLinks.Text = "Mirror symbolic links"
        Me.ToolTipMain.SetToolTip(Me.CheckBoxSymLinks, "Mirror symbolic links as new symbolic links that point to the same original target.")
        Me.CheckBoxSymLinks.UseVisualStyleBackColor = True
        '
        'CheckBoxJunctions
        '
        Me.CheckBoxJunctions.AutoSize = True
        Me.CheckBoxJunctions.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxJunctions.Location = New System.Drawing.Point(248, 292)
        Me.CheckBoxJunctions.Name = "CheckBoxJunctions"
        Me.CheckBoxJunctions.Size = New System.Drawing.Size(168, 19)
        Me.CheckBoxJunctions.TabIndex = 15
        Me.CheckBoxJunctions.Text = "Mirror directory junctions"
        Me.ToolTipMain.SetToolTip(Me.CheckBoxJunctions, "Mirror directory junctions as new junctions that point to the same original target (volume mount points are never mirrored).")
        Me.CheckBoxJunctions.UseVisualStyleBackColor = True
        '
        'CheckBoxHardLinks
        '
        Me.CheckBoxHardLinks.AutoSize = True
        Me.CheckBoxHardLinks.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxHardLinks.Location = New System.Drawing.Point(248, 317)
        Me.CheckBoxHardLinks.Name = "CheckBoxHardLinks"
        Me.CheckBoxHardLinks.Size = New System.Drawing.Size(121, 19)
        Me.CheckBoxHardLinks.TabIndex = 16
        Me.CheckBoxHardLinks.Text = "Mirror hard links"
        Me.ToolTipMain.SetToolTip(Me.CheckBoxHardLinks, "Mirror all the names (hard links) of the same file as hard links of a single dummy file.")
        Me.CheckBoxHardLinks.UseVisualStyleBackColor = True
        '
        'CheckBoxIgnoreSecurityExceptions
        '
        Me.CheckBoxIgnoreSecurityExceptions.AutoSize = True
        Me.CheckBoxIgnoreSecurityExceptions.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxIgnoreSecurityExceptions.Location = New System.Drawing.Point(450, 267)
        Me.CheckBoxIgnoreSecurityExceptions.Name = "CheckBoxIgnoreSecurityExceptions"
        Me.CheckBoxIgnoreSecurityExceptions.Size = New System.Drawing.Size(206, 19)
        Me.CheckBoxIgnoreSecurityExceptions.TabIndex = 17
        Me.CheckBoxIgnoreSecurityExceptions.Text = "Ignore security access exceptions"
        Me.CheckBoxIgnoreSecurityExceptions.UseVisualStyleBackColor = True
        '
        'CheckBoxOverwrite
        '
        Me.CheckBoxOverwrite.AutoSize = True
        Me.CheckBoxOverwrite.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxOverwrite.Location = New System.Drawing.Point(450, 292)
        Me.CheckBoxOverwrite.Name = "CheckBoxOverwrite"
        Me.CheckBoxOverwrite.Size = New System.Drawing.Size(164, 19)
        Me.CheckBoxOverwrite.TabIndex = 18
        Me.CheckBoxOverwrite.Text = "Overwrite destination files"
        Me.ToolTipMain.SetToolTip(Me.CheckBoxOverwrite, "Replace the existing destination files with dummy files, without asking." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "When unchecked, the existing destination files are kept untouched.")
        Me.CheckBoxOverwrite.UseVisualStyleBackColor = True
        '
        'ElektroProgressBar1
        '
        Me.ElektroProgressBar1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ElektroProgressBar1.FormatString = "Ready"
        Me.ElektroProgressBar1.FormatStringError = ""
        Me.ElektroProgressBar1.FormatStringPaused = ""
        Me.ElektroProgressBar1.Location = New System.Drawing.Point(176, 353)
        Me.ElektroProgressBar1.Name = "ElektroProgressBar1"
        Me.ElektroProgressBar1.PreventFlickering = True
        Me.ElektroProgressBar1.Size = New System.Drawing.Size(392, 32)
        Me.ElektroProgressBar1.Step = 1
        Me.ElektroProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ElektroProgressBar1.TabIndex = 20
        '
        'ButtonGitHub
        '
        Me.ButtonGitHub.AccessibleName = "Open the GitHub repository"
        Me.ButtonGitHub.DarkThemeImage = Global.My.Resources.Resources.GitHub_Invertocat_White
        Me.ButtonGitHub.LightThemeImage = Global.My.Resources.Resources.GitHub_Invertocat_Black
        Me.ButtonGitHub.Location = New System.Drawing.Point(16, 101)
        Me.ButtonGitHub.Name = "ButtonGitHub"
        Me.ButtonGitHub.Size = New System.Drawing.Size(32, 32)
        Me.ButtonGitHub.TabIndex = 1
        Me.ToolTipMain.SetToolTip(Me.ButtonGitHub, "View the source code on GitHub")
        '
        'ToggleSwitchTheme
        '
        Me.ToggleSwitchTheme.AutoSize = True
        Me.ToggleSwitchTheme.Location = New System.Drawing.Point(54, 106)
        Me.ToggleSwitchTheme.Name = "ToggleSwitchTheme"
        Me.ToggleSwitchTheme.Size = New System.Drawing.Size(117, 22)
        Me.ToggleSwitchTheme.TabIndex = 2
        Me.ToggleSwitchTheme.CheckedText = "Dark theme"
        Me.ToggleSwitchTheme.Text = "Light theme"
        Me.ToggleSwitchTheme.UncheckedText = "Light theme"
        Me.ToolTipMain.SetToolTip(Me.ToggleSwitchTheme, "Switch between the light and dark themes")
        '
        'ButtonMirror
        '
        Me.ButtonMirror.Enabled = False
        Me.ButtonMirror.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!)
        Me.ButtonMirror.Glyph = ActionButtonGlyph.Play
        Me.ButtonMirror.Location = New System.Drawing.Point(16, 353)
        Me.ButtonMirror.Name = "ButtonMirror"
        Me.ButtonMirror.Size = New System.Drawing.Size(150, 32)
        Me.ButtonMirror.TabIndex = 19
        Me.ButtonMirror.Text = "Build Mirror"
        '
        'ButtonReport
        '
        Me.ButtonReport.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonReport.Enabled = False
        Me.ButtonReport.Glyph = ActionButtonGlyph.Report
        Me.ButtonReport.Kind = ActionButtonKind.Secondary
        Me.ButtonReport.Location = New System.Drawing.Point(578, 353)
        Me.ButtonReport.Name = "ButtonReport"
        Me.ButtonReport.Size = New System.Drawing.Size(86, 32)
        Me.ButtonReport.TabIndex = 21
        Me.ButtonReport.Text = "Details..."
        Me.ToolTipMain.SetToolTip(Me.ButtonReport, "Shows the files and directories that were ignored (not copied) by the last mirror operation.")
        '
        'StatusStripFilepaths
        '
        Me.StatusStripFilepaths.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelFilepaths})
        Me.StatusStripFilepaths.Location = New System.Drawing.Point(0, 399)
        Me.StatusStripFilepaths.Name = "StatusStripFilepaths"
        Me.StatusStripFilepaths.Size = New System.Drawing.Size(680, 22)
        Me.StatusStripFilepaths.ShowItemToolTips = True
        Me.StatusStripFilepaths.SizingGrip = False
        Me.StatusStripFilepaths.TabIndex = 22
        Me.StatusStripFilepaths.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelFilepaths
        '
        Me.ToolStripStatusLabelFilepaths.Image = Global.My.Resources.Resources.File
        Me.ToolStripStatusLabelFilepaths.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolStripStatusLabelFilepaths.Name = "ToolStripStatusLabelFilepaths"
        Me.ToolStripStatusLabelFilepaths.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripStatusLabelFilepaths.Size = New System.Drawing.Size(665, 17)
        Me.ToolStripStatusLabelFilepaths.Spring = True
        Me.ToolStripStatusLabelFilepaths.Text = "Ready"
        Me.ToolStripStatusLabelFilepaths.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TimerProgress
        '
        Me.TimerProgress.Interval = 100
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(680, 421)
        Me.Controls.Add(Me.PictureBoxLogo)
        Me.Controls.Add(Me.SectionHeaderDirectories)
        Me.Controls.Add(Me.LabelSource)
        Me.Controls.Add(Me.TextBoxSource)
        Me.Controls.Add(Me.ButtonSource)
        Me.Controls.Add(Me.LabelDestination)
        Me.Controls.Add(Me.TextBoxDestination)
        Me.Controls.Add(Me.ButtonDestination)
        Me.Controls.Add(Me.SectionHeaderOptions)
        Me.Controls.Add(Me.CheckBoxAttribs)
        Me.Controls.Add(Me.CheckBoxDatestamps)
        Me.Controls.Add(Me.CheckBoxHiddenFiles)
        Me.Controls.Add(Me.CheckBoxSymLinks)
        Me.Controls.Add(Me.CheckBoxJunctions)
        Me.Controls.Add(Me.CheckBoxHardLinks)
        Me.Controls.Add(Me.CheckBoxIgnoreSecurityExceptions)
        Me.Controls.Add(Me.CheckBoxOverwrite)
        Me.Controls.Add(Me.ElektroProgressBar1)
        Me.Controls.Add(Me.ButtonMirror)
        Me.Controls.Add(Me.ButtonReport)
        Me.Controls.Add(Me.ButtonGitHub)
        Me.Controls.Add(Me.ToggleSwitchTheme)
        Me.Controls.Add(Me.StatusStripFilepaths)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Dummy Drive Mirror Maker"
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStripFilepaths.ResumeLayout(False)
        Me.StatusStripFilepaths.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PictureBoxLogo As PictureBox
    Friend WithEvents SectionHeaderDirectories As SectionHeaderLabel
    Friend WithEvents LabelSource As Label
    Friend WithEvents TextBoxSource As TextBox
    Friend WithEvents ButtonSource As Button
    Friend WithEvents LabelDestination As Label
    Friend WithEvents TextBoxDestination As TextBox
    Friend WithEvents ButtonDestination As Button
    Friend WithEvents SectionHeaderOptions As SectionHeaderLabel
    Friend WithEvents CheckBoxAttribs As ThemedCheckBox
    Friend WithEvents CheckBoxDatestamps As ThemedCheckBox
    Friend WithEvents CheckBoxHiddenFiles As ThemedCheckBox
    Friend WithEvents CheckBoxSymLinks As ThemedCheckBox
    Friend WithEvents CheckBoxJunctions As ThemedCheckBox
    Friend WithEvents CheckBoxHardLinks As ThemedCheckBox
    Friend WithEvents CheckBoxIgnoreSecurityExceptions As ThemedCheckBox
    Friend WithEvents CheckBoxOverwrite As ThemedCheckBox
    Friend WithEvents ElektroProgressBar1 As DevCase.UserControls.Controls.ElektroProgressBar
    Friend WithEvents ButtonGitHub As IconButton
    Friend WithEvents ToggleSwitchTheme As ToggleSwitch
    Friend WithEvents ButtonMirror As ActionButton
    Friend WithEvents ButtonReport As ActionButton
    Friend WithEvents StatusStripFilepaths As StatusStrip
    Friend WithEvents ToolStripStatusLabelFilepaths As ToolStripStatusLabel
    Friend WithEvents ToolTipMain As ToolTip
    Friend WithEvents TimerProgress As Timer
    Friend WithEvents ElektroBackgroundWorker1 As DevCase.Core.Threading.Types.ElektroBackgroundWorker
End Class
