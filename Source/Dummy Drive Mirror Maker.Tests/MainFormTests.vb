#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

#End Region

#Region " MainFormTests "

''' <summary>
''' Unit tests for the layout and initial state of the <see cref="Main"/> form.
''' </summary>
<TestClass>
Public NotInheritable Class MainFormTests

#Region " Helpers "

    ''' <summary>
    ''' Gets the visible controls of the form that are positioned manually (not docked).
    ''' </summary>
    ''' <param name="form">
    ''' The form.
    ''' </param>
    ''' <returns>
    ''' The controls.
    ''' </returns>
    Private Shared Function GetPositionedControls(form As Form) As Control()
        Return form.Controls.Cast(Of Control)().
                             Where(Function(c As Control) c.Visible AndAlso c.Dock = DockStyle.None).
                             ToArray()
    End Function

#End Region

#Region " Tests "

    <TestMethod>
    Public Sub Constructor_AppliesThemeConsistentWithToggleSwitch()
        Using form As New Main()
            Dim expectedPalette As ThemePalette = If(form.ToggleSwitchTheme.Checked, ThemePalette.Dark, ThemePalette.Light)

            Assert.AreEqual(expectedPalette.WindowBackColor, form.BackColor)
            Assert.AreSame(expectedPalette, form.ToggleSwitchTheme.Palette)
            Assert.AreSame(expectedPalette, form.ButtonMirror.Palette)
            Assert.AreSame(expectedPalette, form.CheckBoxAttribs.Palette)
        End Using
    End Sub

    <TestMethod>
    Public Sub InitialState_OnlySourceSelectionIsEnabled()
        Using form As New Main()
            Assert.IsTrue(form.ButtonSource.Enabled)
            Assert.IsFalse(form.ButtonDestination.Enabled)
            Assert.IsFalse(form.TextBoxDestination.Enabled)
            Assert.IsFalse(form.ButtonMirror.Enabled)
            Assert.AreEqual("Build Mirror", form.ButtonMirror.Text)
            Assert.AreEqual(ActionButtonKind.Primary, form.ButtonMirror.Kind)
            Assert.AreEqual(ActionButtonGlyph.Play, form.ButtonMirror.Glyph)
            Assert.IsFalse(form.ButtonReport.Enabled, "The report button must be disabled until a mirror operation ends.")
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_ControlsDoNotOverlap()
        Using form As New Main()
            Dim controls As Control() = GetPositionedControls(form)

            For i As Integer = 0 To controls.Length - 1
                For j As Integer = i + 1 To controls.Length - 1
                    Assert.IsFalse(controls(i).Bounds.IntersectsWith(controls(j).Bounds),
                                   $"'{controls(i).Name}' {controls(i).Bounds} overlaps '{controls(j).Name}' {controls(j).Bounds}.")
                Next j
            Next i
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_ControlsAreInsideTheClientAreaBetweenLogoAndStatusBar()
        Using form As New Main()
            Dim contentArea As Rectangle = Rectangle.FromLTRB(0, form.PictureBoxLogo.Bottom,
                                                              form.ClientSize.Width, form.StatusStripFilepaths.Top)

            For Each ctrl As Control In GetPositionedControls(form)
                Assert.IsTrue(contentArea.Contains(ctrl.Bounds), $"'{ctrl.Name}' {ctrl.Bounds} is outside of {contentArea}.")
            Next ctrl
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_ContentIsHorizontallySymmetric()
        ' The left and right margins of the content must be equal.
        ' A tolerance of 1 pixel allows the rounding of the automatic DPI scaling.
        Using form As New Main()
            Dim leftMargin As Integer = form.SectionHeaderDirectories.Left
            Dim rightMargin As Integer = form.ClientSize.Width - form.SectionHeaderDirectories.Right

            Assert.AreEqual(leftMargin, rightMargin, 1)
            Assert.AreEqual(rightMargin, form.ClientSize.Width - form.ButtonReport.Right, 1)
            Assert.AreEqual(rightMargin, form.ClientSize.Width - form.ButtonSource.Right, 1)
            Assert.AreEqual(rightMargin, form.ClientSize.Width - form.SectionHeaderOptions.Right, 1)
            Assert.AreEqual(leftMargin, form.ButtonGitHub.Left, 1)
            Assert.AreEqual(leftMargin, form.ButtonMirror.Left, 1)
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_GitHubButtonAndThemeSwitch_AreInTheTopLeftCornerBelowTheLogo()
        Using form As New Main()
            Dim topRowControls As Control() = {form.ButtonGitHub, form.ToggleSwitchTheme}
            Dim firstSectionTop As Integer = form.SectionHeaderDirectories.Top

            For Each ctrl As Control In topRowControls
                Assert.IsTrue(ctrl.Top >= form.PictureBoxLogo.Bottom, $"'{ctrl.Name}' must be below the logo.")
                Assert.IsTrue(ctrl.Bottom <= firstSectionTop, $"'{ctrl.Name}' must be above the 'Directories' section.")
            Next ctrl

            Assert.IsTrue(form.ButtonGitHub.Right <= form.ToggleSwitchTheme.Left, "The GitHub button must be at the left of the theme switch.")

            ' Both controls must be vertically centered in the same row.
            Dim githubCenter As Integer = form.ButtonGitHub.Top + (form.ButtonGitHub.Height \ 2)
            Dim toggleCenter As Integer = form.ToggleSwitchTheme.Top + (form.ToggleSwitchTheme.Height \ 2)
            Assert.AreEqual(githubCenter, toggleCenter, 1)
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_MirrorButtonProgressBarAndReportButton_AreTheBottomRow()
        Using form As New Main()
            Assert.IsTrue(form.ButtonMirror.Right <= form.ElektroProgressBar1.Left, "The mirror button must be at the left of the progress bar.")
            Assert.IsTrue(form.ElektroProgressBar1.Right <= form.ButtonReport.Left, "The report button must be at the right of the progress bar.")
            Assert.AreEqual(form.ButtonMirror.Top, form.ElektroProgressBar1.Top, "The controls must be in the same row.")
            Assert.AreEqual(form.ButtonMirror.Top, form.ButtonReport.Top, "The controls must be in the same row.")
            Assert.AreEqual(form.ButtonMirror.Height, form.ElektroProgressBar1.Height, "The controls must have the same height.")
            Assert.AreEqual(form.ButtonMirror.Height, form.ButtonReport.Height, "The controls must have the same height.")

            ' The same gap at both sides of the progress bar.
            Assert.AreEqual(form.ElektroProgressBar1.Left - form.ButtonMirror.Right,
                            form.ButtonReport.Left - form.ElektroProgressBar1.Right, 1)

            ' Nothing positioned below them, except the status bar.
            Dim bottomRow As Control() = {form.ButtonMirror, form.ElektroProgressBar1, form.ButtonReport}
            Dim bottomRowTop As Integer = form.ButtonMirror.Top
            For Each ctrl As Control In GetPositionedControls(form)
                If Not bottomRow.Contains(ctrl) Then
                    Assert.IsTrue(ctrl.Bottom <= bottomRowTop, $"'{ctrl.Name}' must be above the bottom row.")
                End If
            Next ctrl
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_OptionCheckBoxes_AreInThreeJustifiedColumns()
        Using form As New Main()
            Dim columns As CheckBox()() = {
                New CheckBox() {form.CheckBoxAttribs, form.CheckBoxDatestamps, form.CheckBoxHiddenFiles},
                New CheckBox() {form.CheckBoxSymLinks, form.CheckBoxJunctions, form.CheckBoxHardLinks},
                New CheckBox() {form.CheckBoxIgnoreSecurityExceptions, form.CheckBoxOverwrite}
            }

            ' Every check box of a column is aligned to the left.
            For Each column As CheckBox() In columns
                Assert.AreEqual(1, column.Select(Function(cb As CheckBox) cb.Left).Distinct().Count(), $"Column of '{column(0).Name}' is not aligned.")
            Next column

            ' The rows are aligned between columns.
            For row As Integer = 0 To 1
                Assert.AreEqual(columns(0)(row).Top, columns(1)(row).Top)
                Assert.AreEqual(columns(0)(row).Top, columns(2)(row).Top)
            Next row
            Assert.AreEqual(columns(0)(2).Top, columns(1)(2).Top)

            ' The last column ends at the right margin of the section.
            Dim lastColumnRight As Integer = columns(2).Max(Function(cb As CheckBox) cb.Right)
            Assert.AreEqual(form.SectionHeaderOptions.Right, lastColumnRight, 1)

            ' The gaps between columns are equal.
            Dim firstColumnRight As Integer = columns(0).Max(Function(cb As CheckBox) cb.Right)
            Dim secondColumnRight As Integer = columns(1).Max(Function(cb As CheckBox) cb.Right)
            Dim gap1 As Integer = columns(1)(0).Left - firstColumnRight
            Dim gap2 As Integer = columns(2)(0).Left - secondColumnRight
            Assert.AreEqual(gap1, gap2, 1)
            Assert.IsTrue(gap1 >= 16, $"The gap between columns is too small: {gap1} px.")
        End Using
    End Sub

    <TestMethod>
    Public Sub Layout_LinkOptions_AreGroupedInTheSameColumnInOrder()
        Using form As New Main()
            Assert.AreEqual(form.CheckBoxSymLinks.Left, form.CheckBoxJunctions.Left)
            Assert.AreEqual(form.CheckBoxSymLinks.Left, form.CheckBoxHardLinks.Left)
            Assert.IsTrue(form.CheckBoxSymLinks.Top < form.CheckBoxJunctions.Top)
            Assert.IsTrue(form.CheckBoxJunctions.Top < form.CheckBoxHardLinks.Top)
        End Using
    End Sub

    <TestMethod>
    Public Sub WindowTitle_ContainsNameAndVersion()
        Using form As New Main()
            Dim version As Version = GetType(Main).Assembly.GetName().Version
            Assert.AreEqual($"Dummy Drive Mirror Maker {version.Major}.{version.Minor}", form.Text)
        End Using
    End Sub

    <TestMethod>
    <DataRow("App", "1.4.0.0", "App 1.4")>
    <DataRow("App", "1.4.2.0", "App 1.4.2")>
    <DataRow("Dummy Drive Mirror Maker", "2.0.0.7", "Dummy Drive Mirror Maker 2.0")>
    Public Sub BuildWindowTitle_ReturnsNameAndVersion(name As String, version As String, expected As String)
        Assert.AreEqual(expected, Main.BuildWindowTitle(name, New Version(version)))
    End Sub

    <TestMethod>
    Public Sub BuildWindowTitle_InvalidArguments_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Main.BuildWindowTitle(" ", New Version(1, 0)))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Main.BuildWindowTitle("App", Nothing))
    End Sub

    <TestMethod>
    Public Sub SetInputControlsEnabled_False_DisablesAllOptionsAndDirectoryControls()
        ' Regression test: the 'junctions' and 'hard links' check boxes remained enabled during a mirror operation.
        Using form As New Main()
            form.SetInputControlsEnabled(False)

            Dim optionCheckBoxes As ThemedCheckBox() = form.Controls.OfType(Of ThemedCheckBox)().ToArray()
            Assert.AreEqual(8, optionCheckBoxes.Length, "Unexpected number of option check boxes.")
            For Each checkBox As ThemedCheckBox In optionCheckBoxes
                Assert.IsFalse(checkBox.Enabled, $"'{checkBox.Name}' must be disabled during a mirror operation.")
            Next checkBox

            Assert.IsFalse(form.CheckBoxJunctions.Enabled)
            Assert.IsFalse(form.CheckBoxHardLinks.Enabled)
            Assert.IsFalse(form.TextBoxSource.Enabled)
            Assert.IsFalse(form.ButtonSource.Enabled)
            Assert.IsFalse(form.TextBoxDestination.Enabled)
            Assert.IsFalse(form.ButtonDestination.Enabled)

            ' The theme switch and the GitHub button are not related to the mirror operation.
            Assert.IsTrue(form.ToggleSwitchTheme.Enabled)
            Assert.IsTrue(form.ButtonGitHub.Enabled)

            form.SetInputControlsEnabled(True)
            Assert.IsTrue(optionCheckBoxes.All(Function(cb As ThemedCheckBox) cb.Enabled))
        End Using
    End Sub

    <TestMethod>
    Public Sub OverwriteCheckBox_HasExplanatoryToolTip()
        Using form As New Main()
            Dim toolTip As String = form.ToolTipMain.GetToolTip(form.CheckBoxOverwrite)
            StringAssert.Contains(toolTip, "without asking")
            StringAssert.Contains(toolTip, "kept untouched")
        End Using
    End Sub

    <TestMethod>
    Public Sub ThemeSwitch_TextMatchesItsState()
        Using form As New Main()
            Dim toggle As ToggleSwitch = form.ToggleSwitchTheme
            Assert.AreEqual(If(toggle.Checked, "Dark theme", "Light theme"), toggle.Text)
        End Using
    End Sub

#End Region

#Region " ComputeJustifiedColumnPositions "

    <TestMethod>
    Public Sub ComputeJustifiedColumnPositions_ThreeColumns_FirstAtLeftLastEndsAtRight()
        Dim positions As Integer() = Main.ComputeJustifiedColumnPositions(18, 664, {190, 170, 215})

        Assert.AreEqual(3, positions.Length)
        Assert.AreEqual(18, positions(0))
        Assert.AreEqual(664 - 215, positions(2))

        Dim gap1 As Integer = positions(1) - (positions(0) + 190)
        Dim gap2 As Integer = positions(2) - (positions(1) + 170)
        Assert.AreEqual(gap1, gap2, 1)
    End Sub

    <TestMethod>
    Public Sub ComputeJustifiedColumnPositions_RemainderIsDistributed_LastColumnEndsExactlyAtRight()
        ' Free space: 100 - 30 = 70 px, distributed in 3 gaps (24 + 23 + 23).
        Dim widths As Integer() = {10, 5, 5, 10}
        Dim positions As Integer() = Main.ComputeJustifiedColumnPositions(0, 100, widths)

        CollectionAssert.AreEqual({0, 34, 62, 90}, positions)
        Assert.AreEqual(100, positions(3) + widths(3))
    End Sub

    <TestMethod>
    Public Sub ComputeJustifiedColumnPositions_ExactFit_HasNoGaps()
        CollectionAssert.AreEqual({0, 50}, Main.ComputeJustifiedColumnPositions(0, 100, {50, 50}))
    End Sub

    <TestMethod>
    Public Sub ComputeJustifiedColumnPositions_ColumnsDoNotFit_ThrowsInvalidOperationException()
        Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Main.ComputeJustifiedColumnPositions(0, 100, {60, 50}))
    End Sub

    <TestMethod>
    Public Sub ComputeJustifiedColumnPositions_InvalidArguments_Throws()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Main.ComputeJustifiedColumnPositions(0, 100, Nothing))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() Main.ComputeJustifiedColumnPositions(0, 100, {50}))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() Main.ComputeJustifiedColumnPositions(0, 100, {50, -1}))
    End Sub

#End Region

#Region " Mirror Result "

    <TestMethod>
    Public Sub ReportButton_IsARoundedActionButtonWithAnIcon()
        Using form As New Main()
            Assert.IsInstanceOfType(Of ActionButton)(form.ButtonReport)
            Assert.AreEqual(ActionButtonKind.Secondary, form.ButtonReport.Kind)
            Assert.AreEqual(ActionButtonGlyph.Report, form.ButtonReport.Glyph)
        End Using
    End Sub

    <TestMethod>
    <DataRow(False, False)>
    <DataRow(True, False)>
    <DataRow(False, True)>
    Public Sub ShowMirrorResult_AlwaysEnablesTheReportButton(isCancelled As Boolean, hasFailed As Boolean)
        ' Regression test: the button was only enabled when some item was ignored.
        Using form As New Main()
            Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)
            report.SetSourceStatistics(Enumerable.Empty(Of IO.DirectoryInfo)())

            form.ShowMirrorResult(report, isCancelled, hasFailed)

            Assert.IsTrue(form.ButtonReport.Enabled)
        End Using
    End Sub

    <TestMethod>
    Public Sub ShowMirrorResult_OnlyExcludedItems_KeepsTheBarGreenWithoutSummary()
        ' Regression test: items excluded by the options chosen by the user turned the bar yellow.
        Using form As New Main()
            Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)
            report.SetSourceStatistics(Enumerable.Empty(Of IO.DirectoryInfo)())
            report.Add(MirrorItemKind.Directory, "C:\Source\Junction", MirrorSkipReason.JunctionNotMirrored)

            form.ShowMirrorResult(report, isCancelled:=False, hasFailed:=False)

            Assert.AreEqual(DevCase.UserControls.Enums.ProgressBarState.Normal, form.ElektroProgressBar1.State)
            Assert.AreEqual("0 of 0 directories (100%)", form.ElektroProgressBar1.FormatString)
        End Using
    End Sub

    <TestMethod>
    <DataRow(CInt(MirrorSkipReason.VolumeMountPoint), CInt(DevCase.UserControls.Enums.ProgressBarState.Paused), " - Copied with warnings")>
    <DataRow(CInt(MirrorSkipReason.Error), CInt(DevCase.UserControls.Enums.ProgressBarState.Error), " - Copied with errors")>
    Public Sub ShowMirrorResult_WarningsOrErrors_ShowShortTextAndColor(reason As Integer, expectedState As Integer, expectedSuffix As String)
        Using form As New Main()
            Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)
            report.SetSourceStatistics(Enumerable.Empty(Of IO.DirectoryInfo)())
            report.Add(MirrorItemKind.Directory, "C:\Source\Item", CType(reason, MirrorSkipReason))

            form.ShowMirrorResult(report, isCancelled:=False, hasFailed:=False)

            Assert.AreEqual(CType(expectedState, DevCase.UserControls.Enums.ProgressBarState), form.ElektroProgressBar1.State)
            StringAssert.EndsWith(form.ElektroProgressBar1.FormatString, expectedSuffix)
            Assert.IsFalse(form.ElektroProgressBar1.FormatString.Contains("could not be mirrored"), "The summary is shown only in the report.")
            Assert.AreEqual(form.ElektroProgressBar1.FormatString, form.ElektroProgressBar1.FormatStringPaused)
            Assert.AreEqual(form.ElektroProgressBar1.FormatString, form.ElektroProgressBar1.FormatStringError)
        End Using
    End Sub

    <TestMethod>
    Public Sub SetPreMirrorValues_ResetsTheProgressBarImmediately()
        ' Regression test: the bar kept its previous value and color until the directories were listed.
        Using form As New Main()
            Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)
            report.SetSourceStatistics(Enumerable.Empty(Of IO.DirectoryInfo)())
            report.Add(MirrorItemKind.File, "C:\Source\File.txt", MirrorSkipReason.Error)
            form.ShowMirrorResult(report, isCancelled:=False, hasFailed:=False)
            Assert.AreEqual(DevCase.UserControls.Enums.ProgressBarState.Error, form.ElektroProgressBar1.State)

            form.SetPreMirrorValues()

            Assert.AreEqual(DevCase.UserControls.Enums.ProgressBarState.Normal, form.ElektroProgressBar1.State)
            Assert.AreEqual(0, form.ElektroProgressBar1.Value)
        End Using
    End Sub

    <TestMethod>
    <DataRow(CInt(LinkAction.Create), True)>
    <DataRow(CInt(LinkAction.Replace), True)>
    <DataRow(CInt(LinkAction.Skip), True)>
    <DataRow(CInt(LinkAction.Keep), False)>
    Public Sub RegisterLinkResult_RegistersMirroredOrKeptLinks(action As Integer, isMirrored As Boolean)
        Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)

        Main.RegisterLinkResult(report, MirrorItemKind.Directory, "C:\Source\Junction", CType(action, LinkAction), "Directory junction")

        Assert.AreEqual(If(isMirrored, 1, 0), report.MirroredDirectoryCount)
        Assert.AreEqual(If(isMirrored, 0, 1), report.ExcludedCount)
    End Sub

    <TestMethod>
    Public Sub RegisterLinkResult_InvalidArguments_Throw()
        Dim report As New MirrorReport("C:\Source", "D:\Mirror", Date.Now)
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Main.RegisterLinkResult(Nothing, MirrorItemKind.File, "C:\A", LinkAction.Create, "x"))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() Main.RegisterLinkResult(report, MirrorItemKind.File, "C:\A", LinkAction.Conflict, "x"))
    End Sub

    <TestMethod>
    <DataRow(CInt(MirrorReportSeverity.None), False, False, CInt(MirrorReportSeverity.None))>
    <DataRow(CInt(MirrorReportSeverity.Warning), False, False, CInt(MirrorReportSeverity.Warning))>
    <DataRow(CInt(MirrorReportSeverity.Error), False, False, CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorReportSeverity.None), True, False, CInt(MirrorReportSeverity.Warning))>
    <DataRow(CInt(MirrorReportSeverity.Error), True, False, CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorReportSeverity.None), False, True, CInt(MirrorReportSeverity.Error))>
    <DataRow(CInt(MirrorReportSeverity.Warning), True, True, CInt(MirrorReportSeverity.Error))>
    Public Sub GetResultSeverity_ReturnsTheWorstSeverity(reportSeverity As Integer, isCancelled As Boolean, hasFailed As Boolean, expected As Integer)
        Assert.AreEqual(CType(expected, MirrorReportSeverity),
                        Main.GetResultSeverity(CType(reportSeverity, MirrorReportSeverity), isCancelled, hasFailed))
    End Sub

    <TestMethod>
    <DataRow(3285, 3285, CInt(MirrorOutcome.Completed), CInt(MirrorReportSeverity.None), "3,285 of 3,285 directories (100%)")>
    <DataRow(3285, 3285, CInt(MirrorOutcome.Completed), CInt(MirrorReportSeverity.Warning), "3,285 of 3,285 directories (100%) - Copied with warnings")>
    <DataRow(3285, 3285, CInt(MirrorOutcome.Completed), CInt(MirrorReportSeverity.Error), "3,285 of 3,285 directories (100%) - Copied with errors")>
    <DataRow(1000, 4000, CInt(MirrorOutcome.Aborted), CInt(MirrorReportSeverity.Error), "1,000 of 4,000 directories (25%) - Aborted")>
    <DataRow(0, 3285, CInt(MirrorOutcome.Failed), CInt(MirrorReportSeverity.None), "0 of 3,285 directories (0%) - Failed")>
    <DataRow(0, 0, CInt(MirrorOutcome.Completed), CInt(MirrorReportSeverity.None), "0 of 0 directories (100%)")>
    <DataRow(0, -1, CInt(MirrorOutcome.Aborted), CInt(MirrorReportSeverity.None), "Directories not listed - Aborted")>
    Public Sub BuildResultText_ReturnsShortTextWithFormattedNumbers(processed As Integer, total As Integer, outcome As Integer, severity As Integer, expected As String)
        Assert.AreEqual(expected, Main.BuildResultText(processed, total, CType(outcome, MirrorOutcome), CType(severity, MirrorReportSeverity)))
    End Sub

    <TestMethod>
    Public Sub BuildResultText_InvalidArguments_Throw()
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() Main.BuildResultText(11, 10, MirrorOutcome.Completed, MirrorReportSeverity.None))
        Assert.ThrowsExactly(Of ArgumentException)(Function() Main.BuildResultText(1, 10, MirrorOutcome.InProgress, MirrorReportSeverity.None))
        Assert.ThrowsExactly(Of ArgumentException)(Function() Main.BuildResultText(1, 10, MirrorOutcome.Completed, CType(999, MirrorReportSeverity)))
    End Sub

    <TestMethod>
    Public Sub BuildResultText_LongestText_FitsInsideTheProgressBar()
        ' Regression test: the summary of ignored items was appended to the text, which exceeded the visible area of the bar.
        Using form As New Main()
            Dim longest As String = Main.BuildResultText(999999, 999999, MirrorOutcome.Completed, MirrorReportSeverity.Warning)
            Dim availableWidth As Integer = form.ElektroProgressBar1.ClientSize.Width - 8

            Assert.IsTrue(GraphicsHelper.FitsInWidth(longest, form.ElektroProgressBar1.Font, availableWidth),
                          $"'{longest}' does not fit in {availableWidth} px.")
        End Using
    End Sub

    <TestMethod>
    Public Sub ProgressBar_ShowsNumbersWithThousandsSeparators()
        Using form As New Main()
            form.ElektroProgressBar1.Maximum = 3285
            form.ElektroProgressBar1.Value = 1500
            form.ElektroProgressBar1.FormatString = "#current of #total directories (#%)"

            Assert.AreEqual("1,500 of 3,285 directories (46%)", form.ElektroProgressBar1.GetPercentageString())
        End Using
    End Sub

#End Region

#Region " Repository "

    <TestMethod>
    Public Sub RepositoryUrl_IsValidGitHubUrl()
        Dim uri As New Uri(Main.RepositoryUrl)

        Assert.AreEqual(Uri.UriSchemeHttps, uri.Scheme)
        Assert.AreEqual("github.com", uri.Host)
        Assert.AreEqual("/ElektroStudios/Dummy-Drive-Mirror-Maker", uri.AbsolutePath)
    End Sub

#End Region

End Class

#End Region
