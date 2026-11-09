#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Imports DevCase.UserControls.Controls

#End Region

#Region " CustomControlsTests "

''' <summary>
''' Unit tests for the custom controls of the application
''' (<see cref="ThemedCheckBox"/>, <see cref="ToggleSwitch"/>, <see cref="ActionButton"/>,
''' <see cref="IconButton"/>, <see cref="SectionHeaderLabel"/>),
''' <see cref="GraphicsHelper"/> and the theming extensions of <see cref="ElektroProgressBar"/>.
''' </summary>
<TestClass>
Public NotInheritable Class CustomControlsTests

#Region " Helpers "

    ''' <summary>
    ''' Renders the specified control into a bitmap, to verify that its painting code does not throw.
    ''' </summary>
    ''' <param name="ctrl">
    ''' The control to render.
    ''' </param>
    ''' <returns>
    ''' The rendered bitmap. The caller is responsible for disposing it.
    ''' </returns>
    Private Shared Function Render(ctrl As Control) As Bitmap
        Dim bmp As New Bitmap(Math.Max(1, ctrl.Width), Math.Max(1, ctrl.Height))
        ctrl.DrawToBitmap(bmp, New Rectangle(Point.Empty, bmp.Size))
        Return bmp
    End Function

#End Region

#Region " GraphicsHelper "

    <TestMethod>
    Public Sub CreateRoundedRectangle_ValidArguments_PathFitsInsideBounds()
        Dim bounds As New RectangleF(10, 20, 100, 30)
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(bounds, 4)
            Dim pathBounds As RectangleF = path.GetBounds()
            Assert.AreEqual(bounds.X, pathBounds.X, 0.01F)
            Assert.AreEqual(bounds.Y, pathBounds.Y, 0.01F)
            Assert.AreEqual(bounds.Width, pathBounds.Width, 0.01F)
            Assert.AreEqual(bounds.Height, pathBounds.Height, 0.01F)
        End Using
    End Sub

    <TestMethod>
    Public Sub CreateRoundedRectangle_ZeroRadius_CreatesRegularRectangle()
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(New RectangleF(0, 0, 10, 10), 0)
            Assert.AreEqual(4, path.PointCount)
        End Using
    End Sub

    <TestMethod>
    Public Sub CreateRoundedRectangle_RadiusLargerThanHalfSize_IsClampedToPill()
        Dim bounds As New RectangleF(0, 0, 36, 18)
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(bounds, 100)
            Dim pathBounds As RectangleF = path.GetBounds()
            Assert.AreEqual(bounds.Width, pathBounds.Width, 0.01F)
            Assert.AreEqual(bounds.Height, pathBounds.Height, 0.01F)
        End Using
    End Sub

    <TestMethod>
    <DataRow(0.0F, 10.0F, 1.0F)>
    <DataRow(10.0F, 0.0F, 1.0F)>
    <DataRow(10.0F, 10.0F, -1.0F)>
    Public Sub CreateRoundedRectangle_InvalidArguments_ThrowsArgumentOutOfRangeException(width As Single, height As Single, radius As Single)
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
            Sub() GraphicsHelper.CreateRoundedRectangle(New RectangleF(0, 0, width, height), radius))
    End Sub

    <TestMethod>
    Public Sub GetTextFormatFlags_KeyboardCuesHidden_AddsHidePrefix()
        Assert.IsTrue(GraphicsHelper.GetTextFormatFlags(showKeyboardCues:=False).HasFlag(TextFormatFlags.HidePrefix))
        Assert.IsFalse(GraphicsHelper.GetTextFormatFlags(showKeyboardCues:=True).HasFlag(TextFormatFlags.HidePrefix))
    End Sub

#End Region

#Region " ThemedCheckBox "

    <TestMethod>
    Public Sub ThemedCheckBox_PreferredSize_FitsGlyphAndText()
        Using checkBox As New ThemedCheckBox() With {.Text = "Ignore security access exceptions", .AutoSize = True}
            Dim textWidth As Integer = TextRenderer.MeasureText(checkBox.Text, checkBox.Font).Width
            Assert.IsTrue(checkBox.PreferredSize.Width >= ThemedCheckBox.GlyphSize + textWidth)
        End Using
    End Sub

    <TestMethod>
    <DataRow(False, CheckState.Unchecked, True)>
    <DataRow(True, CheckState.Checked, True)>
    <DataRow(True, CheckState.Indeterminate, True)>
    <DataRow(True, CheckState.Checked, False)>
    <DataRow(False, CheckState.Checked, False)>
    Public Sub ThemedCheckBox_Render_AllStates_DoesNotThrow(isDark As Boolean, state As CheckState, enabled As Boolean)
        Using checkBox As New ThemedCheckBox() With {.Text = "Option", .ThreeState = True, .AutoSize = True}
            checkBox.ApplyPalette(If(isDark, ThemePalette.Dark, ThemePalette.Light))
            checkBox.CheckState = state
            checkBox.Enabled = enabled
            Using bmp As Bitmap = Render(checkBox)
                Assert.IsNotNull(bmp)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub ThemedCheckBox_ApplyPalette_Nothing_ThrowsArgumentNullException()
        Using checkBox As New ThemedCheckBox()
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() checkBox.ApplyPalette(Nothing))
        End Using
    End Sub

#End Region

#Region " ToggleSwitch "

    <TestMethod>
    Public Sub ToggleSwitch_PreferredSize_FitsTrackAndText()
        Using toggle As New ToggleSwitch() With {.Text = "Dark theme"}
            Dim textWidth As Integer = TextRenderer.MeasureText(toggle.Text, toggle.Font).Width
            Assert.IsTrue(toggle.PreferredSize.Width >= ToggleSwitch.TrackWidth + ToggleSwitch.TrackTextSpacing + textWidth - 4)
            Assert.IsTrue(toggle.PreferredSize.Height >= ToggleSwitch.TrackHeight)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_EmptyText_PreferredSizeFitsTrack()
        Using toggle As New ToggleSwitch() With {.Text = String.Empty}
            Assert.IsTrue(toggle.PreferredSize.Width >= ToggleSwitch.TrackWidth)
            Assert.IsTrue(toggle.PreferredSize.Width < ToggleSwitch.TrackWidth + ToggleSwitch.TrackTextSpacing)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_IsAutoSizeByDefault()
        Using toggle As New ToggleSwitch()
            Assert.IsTrue(toggle.AutoSize)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_SetChecked_RaisesCheckedChanged()
        Using toggle As New ToggleSwitch()
            Dim raised As Integer
            AddHandler toggle.CheckedChanged, Sub(sender As Object, e As EventArgs) raised += 1

            toggle.Checked = True
            toggle.Checked = False

            Assert.AreEqual(2, raised)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_StateTexts_TextFollowsCheckedState()
        Using toggle As New ToggleSwitch() With {.CheckedText = "Dark theme", .UncheckedText = "Light theme"}
            Assert.AreEqual("Light theme", toggle.Text)

            toggle.Checked = True
            Assert.AreEqual("Dark theme", toggle.Text)

            toggle.Checked = False
            Assert.AreEqual("Light theme", toggle.Text)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_StateTexts_PreferredWidthFollowsText()
        Using toggle As New ToggleSwitch() With {.CheckedText = "On", .UncheckedText = "A much longer text"}
            Dim uncheckedWidth As Integer = toggle.PreferredSize.Width
            toggle.Checked = True
            Assert.IsTrue(toggle.PreferredSize.Width < uncheckedWidth)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_NoStateTexts_TextIsNotModified()
        Using toggle As New ToggleSwitch() With {.Text = "Fixed text"}
            toggle.Checked = True
            Assert.AreEqual("Fixed text", toggle.Text)
            Assert.AreEqual(String.Empty, toggle.CheckedText)
            Assert.AreEqual(String.Empty, toggle.UncheckedText)
        End Using
    End Sub

    <TestMethod>
    Public Sub ToggleSwitch_NullStateText_IsTreatedAsEmpty()
        Using toggle As New ToggleSwitch() With {.CheckedText = Nothing}
            Assert.AreEqual(String.Empty, toggle.CheckedText)
        End Using
    End Sub

    <TestMethod>
    <DataRow(False, False, True)>
    <DataRow(False, True, True)>
    <DataRow(True, False, True)>
    <DataRow(True, True, True)>
    <DataRow(True, True, False)>
    Public Sub ToggleSwitch_Render_AllStates_DoesNotThrow(isDark As Boolean, isChecked As Boolean, enabled As Boolean)
        Using toggle As New ToggleSwitch() With {.Text = "Dark theme"}
            toggle.ApplyPalette(If(isDark, ThemePalette.Dark, ThemePalette.Light))
            toggle.Checked = isChecked
            toggle.Enabled = enabled
            toggle.Size = toggle.PreferredSize
            Using bmp As Bitmap = Render(toggle)
                Assert.IsNotNull(bmp)
            End Using
        End Using
    End Sub

#End Region

#Region " ActionButton "

    <TestMethod>
    Public Sub ActionButton_Defaults_ArePrimaryWithoutGlyph()
        Using button As New ActionButton()
            Assert.AreEqual(ActionButtonKind.Primary, button.Kind)
            Assert.AreEqual(ActionButtonGlyph.None, button.Glyph)
        End Using
    End Sub

    <TestMethod>
    Public Sub ActionButton_InvalidKind_ThrowsInvalidEnumArgumentException()
        Using button As New ActionButton()
            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() button.Kind = CType(99, ActionButtonKind))
        End Using
    End Sub

    <TestMethod>
    Public Sub ActionButton_InvalidGlyph_ThrowsInvalidEnumArgumentException()
        Using button As New ActionButton()
            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() button.Glyph = CType(99, ActionButtonGlyph))
        End Using
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub ActionButton_GetStateColors_UsesColorsOfKindAndPalette(isDark As Boolean)
        Dim p As ThemePalette = If(isDark, ThemePalette.Dark, ThemePalette.Light)
        Using button As New ActionButton()
            button.ApplyPalette(p)
            Dim backColor As Color
            Dim foreColor As Color

            button.Kind = ActionButtonKind.Primary
            button.GetStateColors(backColor, foreColor)
            Assert.AreEqual(p.AccentBackColor, backColor)
            Assert.AreEqual(p.AccentForeColor, foreColor)

            button.Kind = ActionButtonKind.Danger
            button.GetStateColors(backColor, foreColor)
            Assert.AreEqual(p.DangerBackColor, backColor)
            Assert.AreEqual(p.DangerForeColor, foreColor)

            button.Enabled = False
            button.GetStateColors(backColor, foreColor)
            Assert.AreEqual(p.DisabledBackColor, backColor)
            Assert.AreEqual(p.DisabledForeColor, foreColor)
        End Using
    End Sub

    <TestMethod>
    <DataRow(ActionButtonKind.Primary, ActionButtonGlyph.Play, True)>
    <DataRow(ActionButtonKind.Danger, ActionButtonGlyph.Stop, True)>
    <DataRow(ActionButtonKind.Primary, ActionButtonGlyph.None, False)>
    <DataRow(ActionButtonKind.Secondary, ActionButtonGlyph.Report, True)>
    <DataRow(ActionButtonKind.Secondary, ActionButtonGlyph.Report, False)>
    Public Sub ActionButton_Render_AllStates_DoesNotThrow(kind As ActionButtonKind, glyph As ActionButtonGlyph, enabled As Boolean)
        Using button As New ActionButton() With {.Text = "Build Mirror", .Size = New Size(152, 34)}
            button.Kind = kind
            button.Glyph = glyph
            button.Enabled = enabled
            Using bmp As Bitmap = Render(button)
                Assert.IsNotNull(bmp)
            End Using
        End Using
    End Sub

#End Region

#Region " IconButton "

    <TestMethod>
    Public Sub IconButton_CurrentImage_DependsOnPalette()
        Using lightImage As New Bitmap(8, 8), darkImage As New Bitmap(8, 8),
              button As New IconButton() With {.LightThemeImage = lightImage, .DarkThemeImage = darkImage}

            button.ApplyPalette(ThemePalette.Light)
            Assert.AreSame(lightImage, button.CurrentImage)

            button.ApplyPalette(ThemePalette.Dark)
            Assert.AreSame(darkImage, button.CurrentImage)
        End Using
    End Sub

    <TestMethod>
    <DataRow(0, 20)>
    <DataRow(20, 0)>
    <DataRow(-1, -1)>
    Public Sub IconButton_InvalidIconSize_ThrowsArgumentOutOfRangeException(width As Integer, height As Integer)
        Using button As New IconButton()
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() button.IconSize = New Size(width, height))
        End Using
    End Sub

    <TestMethod>
    Public Sub IconButton_Render_WithAndWithoutImage_DoesNotThrow()
        Using image As New Bitmap(64, 64),
              button As New IconButton() With {.Size = New Size(34, 34)}
            Using bmp As Bitmap = Render(button)
                Assert.IsNotNull(bmp)
            End Using

            button.LightThemeImage = image
            button.Enabled = False
            Using bmp As Bitmap = Render(button)
                Assert.IsNotNull(bmp)
            End Using
        End Using
    End Sub

#End Region

#Region " SectionHeaderLabel "

    <TestMethod>
    Public Sub SectionHeaderLabel_ApplyPalette_SetsLineAndTextColors()
        Using header As New SectionHeaderLabel() With {.Text = "Options", .Size = New Size(300, 20)}
            header.ApplyPalette(ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.SeparatorColor, header.LineColor)
            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, header.ForeColor)
            Assert.IsFalse(header.AutoSize)
            Using bmp As Bitmap = Render(header)
                Assert.IsNotNull(bmp)
            End Using
        End Using
    End Sub

#End Region

#Region " FlatToolStripRenderer "

    <TestMethod>
    Public Sub FlatToolStripRenderer_RendersStatusStrip()
        Using statusStrip As New StatusStrip() With {.Renderer = New FlatToolStripRenderer(Color.Red), .BackColor = Color.Black, .Size = New Size(200, 22)}
            statusStrip.Items.Add("Ready")
            Assert.AreEqual(Color.Red, DirectCast(statusStrip.Renderer, FlatToolStripRenderer).BorderColor)
            Using bmp As Bitmap = Render(statusStrip)
                Assert.AreEqual(Color.Black.ToArgb(), bmp.GetPixel(100, 10).ToArgb(), "The background must be a flat fill of the BackColor.")
            End Using
        End Using
    End Sub

#End Region

#Region " ElektroProgressBar Theming "

    <TestMethod>
    Public Sub ElektroProgressBar_ThemingDefaults()
        Using bar As New ElektroProgressBar()
            Assert.IsTrue(bar.UseVisualStyles)
            Assert.AreEqual(Color.Empty, bar.TextColor)
        End Using
    End Sub

    <TestMethod>
    Public Sub ElektroProgressBar_ToggleVisualStylesWithHandle_DoesNotThrow()
        Using bar As New ElektroProgressBar() With {.Size = New Size(200, 24), .Value = 40}
            Dim handle As IntPtr = bar.Handle
            Assert.AreNotEqual(IntPtr.Zero, handle)

            bar.UseVisualStyles = False
            Assert.IsFalse(bar.UseVisualStyles)
            bar.UseVisualStyles = True
            Assert.IsTrue(bar.UseVisualStyles)
        End Using
    End Sub

    <TestMethod>
    Public Sub ElektroProgressBar_HandleRecreatedWithoutVisualStyles_DoesNotThrow()
        Using bar As New ElektroProgressBar() With {.UseVisualStyles = False}
            Dim handle As IntPtr = bar.Handle
            Assert.AreNotEqual(IntPtr.Zero, handle)
            Assert.IsFalse(bar.UseVisualStyles)
        End Using
    End Sub

#End Region

End Class

#End Region
