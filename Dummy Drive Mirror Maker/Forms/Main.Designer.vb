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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.TextBoxSource = New System.Windows.Forms.TextBox()
        Me.LabelSource = New System.Windows.Forms.Label()
        Me.LabelTarget = New System.Windows.Forms.Label()
        Me.TextBoxTarget = New System.Windows.Forms.TextBox()
        Me.ButtonSource = New System.Windows.Forms.Button()
        Me.ButtonTarget = New System.Windows.Forms.Button()
        Me.CheckBoxHiddenFiles = New System.Windows.Forms.CheckBox()
        Me.CheckBoxDatestamps = New System.Windows.Forms.CheckBox()
        Me.CheckBoxAttribs = New System.Windows.Forms.CheckBox()
        Me.ButtonMirror = New System.Windows.Forms.Button()
        Me.GroupBoxOptions = New System.Windows.Forms.GroupBox()
        Me.CheckBoxIgnoreSecurityExceptions = New System.Windows.Forms.CheckBox()
        Me.GroupBoxDirectories = New System.Windows.Forms.GroupBox()
        Me.PictureBoxLogo = New System.Windows.Forms.PictureBox()
        Me.StatusStripFilepaths = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabelFilepaths = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ElektroProgressBar1 = New ElektroKit.UserControls.Controls.ElektroProgressBar()
        Me.ElektroBackgroundWorker1 = New ElektroKit.Core.Threading.Types.ElektroBackgroundWorker()
        Me.GroupBoxOptions.SuspendLayout()
        Me.GroupBoxDirectories.SuspendLayout()
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStripFilepaths.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBoxSource
        '
        Me.TextBoxSource.AllowDrop = True
        Me.TextBoxSource.Location = New System.Drawing.Point(98, 25)
        Me.TextBoxSource.Name = "TextBoxSource"
        Me.TextBoxSource.ReadOnly = True
        Me.TextBoxSource.Size = New System.Drawing.Size(390, 20)
        Me.TextBoxSource.TabIndex = 1
        '
        'LabelSource
        '
        Me.LabelSource.AutoSize = True
        Me.LabelSource.ForeColor = System.Drawing.SystemColors.ControlText
        Me.LabelSource.Location = New System.Drawing.Point(6, 28)
        Me.LabelSource.Name = "LabelSource"
        Me.LabelSource.Size = New System.Drawing.Size(86, 13)
        Me.LabelSource.TabIndex = 0
        Me.LabelSource.Text = "Source Directory"
        '
        'LabelTarget
        '
        Me.LabelTarget.AutoSize = True
        Me.LabelTarget.ForeColor = System.Drawing.SystemColors.ControlText
        Me.LabelTarget.Location = New System.Drawing.Point(6, 67)
        Me.LabelTarget.Name = "LabelTarget"
        Me.LabelTarget.Size = New System.Drawing.Size(83, 13)
        Me.LabelTarget.TabIndex = 3
        Me.LabelTarget.Text = "Target Directory"
        '
        'TextBoxTarget
        '
        Me.TextBoxTarget.AllowDrop = True
        Me.TextBoxTarget.Enabled = False
        Me.TextBoxTarget.Location = New System.Drawing.Point(98, 64)
        Me.TextBoxTarget.Name = "TextBoxTarget"
        Me.TextBoxTarget.ReadOnly = True
        Me.TextBoxTarget.Size = New System.Drawing.Size(390, 20)
        Me.TextBoxTarget.TabIndex = 4
        '
        'ButtonSource
        '
        Me.ButtonSource.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonSource.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonSource.Location = New System.Drawing.Point(494, 23)
        Me.ButtonSource.Name = "ButtonSource"
        Me.ButtonSource.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSource.TabIndex = 2
        Me.ButtonSource.Text = "Browse..."
        Me.ButtonSource.UseVisualStyleBackColor = True
        '
        'ButtonTarget
        '
        Me.ButtonTarget.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonTarget.Enabled = False
        Me.ButtonTarget.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonTarget.Location = New System.Drawing.Point(494, 62)
        Me.ButtonTarget.Name = "ButtonTarget"
        Me.ButtonTarget.Size = New System.Drawing.Size(75, 23)
        Me.ButtonTarget.TabIndex = 5
        Me.ButtonTarget.Text = "Browse..."
        Me.ButtonTarget.UseVisualStyleBackColor = True
        '
        'CheckBoxHiddenFiles
        '
        Me.CheckBoxHiddenFiles.AutoSize = True
        Me.CheckBoxHiddenFiles.Checked = True
        Me.CheckBoxHiddenFiles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxHiddenFiles.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxHiddenFiles.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBoxHiddenFiles.Location = New System.Drawing.Point(6, 65)
        Me.CheckBoxHiddenFiles.Name = "CheckBoxHiddenFiles"
        Me.CheckBoxHiddenFiles.Size = New System.Drawing.Size(163, 17)
        Me.CheckBoxHiddenFiles.TabIndex = 2
        Me.CheckBoxHiddenFiles.Text = "Mirror hidden files and folders"
        Me.CheckBoxHiddenFiles.UseVisualStyleBackColor = True
        '
        'CheckBoxDatestamps
        '
        Me.CheckBoxDatestamps.AutoSize = True
        Me.CheckBoxDatestamps.Checked = True
        Me.CheckBoxDatestamps.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxDatestamps.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxDatestamps.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBoxDatestamps.Location = New System.Drawing.Point(6, 42)
        Me.CheckBoxDatestamps.Name = "CheckBoxDatestamps"
        Me.CheckBoxDatestamps.Size = New System.Drawing.Size(123, 17)
        Me.CheckBoxDatestamps.TabIndex = 1
        Me.CheckBoxDatestamps.Text = "Preserve timestamps"
        Me.CheckBoxDatestamps.UseVisualStyleBackColor = True
        '
        'CheckBoxAttribs
        '
        Me.CheckBoxAttribs.AutoSize = True
        Me.CheckBoxAttribs.Checked = True
        Me.CheckBoxAttribs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxAttribs.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxAttribs.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBoxAttribs.Location = New System.Drawing.Point(6, 19)
        Me.CheckBoxAttribs.Name = "CheckBoxAttribs"
        Me.CheckBoxAttribs.Size = New System.Drawing.Size(114, 17)
        Me.CheckBoxAttribs.TabIndex = 0
        Me.CheckBoxAttribs.Text = "Preserve attributes"
        Me.CheckBoxAttribs.UseVisualStyleBackColor = True
        '
        'ButtonMirror
        '
        Me.ButtonMirror.BackgroundImage = Global.My.Resources.Resources.Copy
        Me.ButtonMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ButtonMirror.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonMirror.Enabled = False
        Me.ButtonMirror.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!)
        Me.ButtonMirror.Location = New System.Drawing.Point(218, 206)
        Me.ButtonMirror.Name = "ButtonMirror"
        Me.ButtonMirror.Size = New System.Drawing.Size(370, 130)
        Me.ButtonMirror.TabIndex = 2
        Me.ButtonMirror.Text = "Build Mirror"
        Me.ButtonMirror.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonMirror.UseVisualStyleBackColor = True
        '
        'GroupBoxOptions
        '
        Me.GroupBoxOptions.Controls.Add(Me.CheckBoxIgnoreSecurityExceptions)
        Me.GroupBoxOptions.Controls.Add(Me.CheckBoxAttribs)
        Me.GroupBoxOptions.Controls.Add(Me.CheckBoxHiddenFiles)
        Me.GroupBoxOptions.Controls.Add(Me.CheckBoxDatestamps)
        Me.GroupBoxOptions.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBoxOptions.Location = New System.Drawing.Point(12, 200)
        Me.GroupBoxOptions.Name = "GroupBoxOptions"
        Me.GroupBoxOptions.Size = New System.Drawing.Size(200, 136)
        Me.GroupBoxOptions.TabIndex = 1
        Me.GroupBoxOptions.TabStop = False
        Me.GroupBoxOptions.Text = "File and folder options"
        '
        'CheckBoxIgnoreSecurityExceptions
        '
        Me.CheckBoxIgnoreSecurityExceptions.AutoSize = True
        Me.CheckBoxIgnoreSecurityExceptions.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxIgnoreSecurityExceptions.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBoxIgnoreSecurityExceptions.Location = New System.Drawing.Point(6, 88)
        Me.CheckBoxIgnoreSecurityExceptions.Name = "CheckBoxIgnoreSecurityExceptions"
        Me.CheckBoxIgnoreSecurityExceptions.Size = New System.Drawing.Size(186, 17)
        Me.CheckBoxIgnoreSecurityExceptions.TabIndex = 4
        Me.CheckBoxIgnoreSecurityExceptions.Text = "Ignore security access exceptions"
        Me.CheckBoxIgnoreSecurityExceptions.UseVisualStyleBackColor = True
        '
        'GroupBoxDirectories
        '
        Me.GroupBoxDirectories.Controls.Add(Me.LabelSource)
        Me.GroupBoxDirectories.Controls.Add(Me.TextBoxSource)
        Me.GroupBoxDirectories.Controls.Add(Me.LabelTarget)
        Me.GroupBoxDirectories.Controls.Add(Me.TextBoxTarget)
        Me.GroupBoxDirectories.Controls.Add(Me.ButtonSource)
        Me.GroupBoxDirectories.Controls.Add(Me.ButtonTarget)
        Me.GroupBoxDirectories.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBoxDirectories.Location = New System.Drawing.Point(12, 90)
        Me.GroupBoxDirectories.Name = "GroupBoxDirectories"
        Me.GroupBoxDirectories.Size = New System.Drawing.Size(576, 100)
        Me.GroupBoxDirectories.TabIndex = 0
        Me.GroupBoxDirectories.TabStop = False
        Me.GroupBoxDirectories.Text = "Directories"
        '
        'PictureBoxLogo
        '
        Me.PictureBoxLogo.BackColor = System.Drawing.Color.Black
        Me.PictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PictureBoxLogo.Image = Global.My.Resources.Resources.Logo
        Me.PictureBoxLogo.Location = New System.Drawing.Point(0, 0)
        Me.PictureBoxLogo.Name = "PictureBoxLogo"
        Me.PictureBoxLogo.Size = New System.Drawing.Size(600, 80)
        Me.PictureBoxLogo.TabIndex = 6
        Me.PictureBoxLogo.TabStop = False
        '
        'StatusStripFilepaths
        '
        Me.StatusStripFilepaths.BackColor = System.Drawing.Color.FromArgb(CType(CType(63, Byte), Integer), CType(CType(63, Byte), Integer), CType(CType(70, Byte), Integer))
        Me.StatusStripFilepaths.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelFilepaths})
        Me.StatusStripFilepaths.Location = New System.Drawing.Point(0, 371)
        Me.StatusStripFilepaths.Name = "StatusStripFilepaths"
        Me.StatusStripFilepaths.Size = New System.Drawing.Size(600, 22)
        Me.StatusStripFilepaths.TabIndex = 8
        Me.StatusStripFilepaths.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelFilepaths
        '
        Me.ToolStripStatusLabelFilepaths.BackColor = System.Drawing.Color.Transparent
        Me.ToolStripStatusLabelFilepaths.ForeColor = System.Drawing.Color.Gainsboro
        Me.ToolStripStatusLabelFilepaths.Image = Global.My.Resources.Resources.File
        Me.ToolStripStatusLabelFilepaths.Name = "ToolStripStatusLabelFilepaths"
        Me.ToolStripStatusLabelFilepaths.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripStatusLabelFilepaths.Size = New System.Drawing.Size(68, 17)
        Me.ToolStripStatusLabelFilepaths.Text = "Waiting..."
        '
        'ElektroProgressBar1
        '
        Me.ElektroProgressBar1.FormatString = "Waiting..."
        Me.ElektroProgressBar1.FormatStringError = ""
        Me.ElektroProgressBar1.FormatStringPaused = ""
        Me.ElektroProgressBar1.Location = New System.Drawing.Point(12, 342)
        Me.ElektroProgressBar1.Name = "ElektroProgressBar1"
        Me.ElektroProgressBar1.PreventFlickering = True
        Me.ElektroProgressBar1.Size = New System.Drawing.Size(576, 23)
        Me.ElektroProgressBar1.Step = 1
        Me.ElektroProgressBar1.TabIndex = 7
        '
        'ElektroBackgroundWorker1
        '
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(600, 393)
        Me.Controls.Add(Me.StatusStripFilepaths)
        Me.Controls.Add(Me.ElektroProgressBar1)
        Me.Controls.Add(Me.GroupBoxDirectories)
        Me.Controls.Add(Me.GroupBoxOptions)
        Me.Controls.Add(Me.ButtonMirror)
        Me.Controls.Add(Me.PictureBoxLogo)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Dummy Drive Mirror Maker"
        Me.GroupBoxOptions.ResumeLayout(False)
        Me.GroupBoxOptions.PerformLayout()
        Me.GroupBoxDirectories.ResumeLayout(False)
        Me.GroupBoxDirectories.PerformLayout()
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStripFilepaths.ResumeLayout(False)
        Me.StatusStripFilepaths.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TextBoxSource As TextBox
    Friend WithEvents LabelSource As Label
    Friend WithEvents LabelTarget As Label
    Friend WithEvents TextBoxTarget As TextBox
    Friend WithEvents ButtonSource As Button
    Friend WithEvents ButtonTarget As Button
    Friend WithEvents PictureBoxLogo As PictureBox
    Friend WithEvents CheckBoxHiddenFiles As CheckBox
    Friend WithEvents CheckBoxDatestamps As CheckBox
    Friend WithEvents CheckBoxAttribs As CheckBox
    Friend WithEvents ButtonMirror As Button
    Friend WithEvents GroupBoxOptions As GroupBox
    Friend WithEvents GroupBoxDirectories As GroupBox
    Friend WithEvents CheckBoxIgnoreSecurityExceptions As CheckBox
    Friend WithEvents ElektroProgressBar1 As ElektroKit.UserControls.Controls.ElektroProgressBar
    Friend WithEvents StatusStripFilepaths As StatusStrip
    Friend WithEvents ToolStripStatusLabelFilepaths As ToolStripStatusLabel
    Friend WithEvents ElektroBackgroundWorker1 As ElektroKit.Core.Threading.Types.ElektroBackgroundWorker
End Class
