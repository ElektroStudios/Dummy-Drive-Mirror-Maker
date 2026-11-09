#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

Imports DevCase.UserControls.Controls
Imports DevCase.UserControls.Enums

#End Region

#Region " ThemeManagerTests "

''' <summary>
''' Unit tests for <see cref="ThemeManager"/> and <see cref="ThemePalette"/>.
''' </summary>
<TestClass>
Public NotInheritable Class ThemeManagerTests

#Region " Helpers "

    ''' <summary>
    ''' Gets the WCAG contrast ratio between two colors.
    ''' </summary>
    ''' <param name="color1">
    ''' The first color.
    ''' </param>
    '''
    ''' <param name="color2">
    ''' The second color.
    ''' </param>
    ''' <returns>
    ''' The contrast ratio, in the range 1 to 21.
    ''' </returns>
    Private Shared Function GetContrastRatio(color1 As Color, color2 As Color) As Double
        Dim l1 As Double = ThemeManagerTests.GetRelativeLuminance(color1)
        Dim l2 As Double = ThemeManagerTests.GetRelativeLuminance(color2)
        Return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05)
    End Function

    ''' <summary>
    ''' Gets the WCAG relative luminance of a color.
    ''' </summary>
    ''' <param name="value">
    ''' The color.
    ''' </param>
    ''' <returns>
    ''' The relative luminance, in the range 0 to 1.
    ''' </returns>
    Private Shared Function GetRelativeLuminance(value As Color) As Double
        Dim linearize As Func(Of Byte, Double) =
            Function(channel As Byte)
                Dim c As Double = channel / 255.0
                Return If(c <= 0.03928, c / 12.92, Math.Pow((c + 0.055) / 1.055, 2.4))
            End Function

        Return (0.2126 * linearize(value.R)) + (0.7152 * linearize(value.G)) + (0.0722 * linearize(value.B))
    End Function

#End Region

#Region " ResolveTheme "

    <TestMethod>
    <DataRow("Light", True, CInt(AppTheme.Light))>
    <DataRow("Light", False, CInt(AppTheme.Light))>
    <DataRow("Dark", True, CInt(AppTheme.Dark))>
    <DataRow("Dark", False, CInt(AppTheme.Dark))>
    <DataRow("System", True, CInt(AppTheme.Light))>
    <DataRow("System", False, CInt(AppTheme.Dark))>
    <DataRow("light", True, CInt(AppTheme.Light))>
    <DataRow(" DARK ", True, CInt(AppTheme.Dark))>
    <DataRow("system", False, CInt(AppTheme.Dark))>
    Public Sub ResolveTheme_ValidValues_ReturnsExpectedTheme(settingValue As String, systemUsesLightTheme As Boolean, expected As Integer)
        ' Note: the enum is passed as Integer because the test method is public and AppTheme is a Friend type.
        Assert.AreEqual(CType(expected, AppTheme), ThemeManager.ResolveTheme(settingValue, systemUsesLightTheme))
    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("   ")>
    <DataRow("Blue")>
    <DataRow("1")>
    Public Sub ResolveTheme_InvalidValue_ThrowsArgumentException(settingValue As String)
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ThemeManager.ResolveTheme(settingValue, True))
    End Sub

    <TestMethod>
    Public Sub ResolveTheme_Nothing_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ResolveTheme(Nothing, True))
    End Sub

    <TestMethod>
    <DataRow(CInt(AppTheme.Light))>
    <DataRow(CInt(AppTheme.Dark))>
    Public Sub ToSettingValue_ThenResolveTheme_ReturnsSameTheme(themeValue As Integer)
        Dim theme As AppTheme = CType(themeValue, AppTheme)
        Dim settingValue As String = ThemeManager.ToSettingValue(theme)

        Assert.AreEqual(theme, ThemeManager.ResolveTheme(settingValue, systemUsesLightTheme:=True))
        Assert.AreEqual(theme, ThemeManager.ResolveTheme(settingValue, systemUsesLightTheme:=False))
    End Sub

    <TestMethod>
    Public Sub ToSettingValue_InvalidTheme_ThrowsArgumentException()
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ThemeManager.ToSettingValue(CType(99, AppTheme)))
    End Sub

    <TestMethod>
    Public Sub ThemeSetting_DefaultValue_IsSystemAndCanBeResolved()
        ' The default value of the "Theme" user setting must be a valid value, or the application would not start.
        Dim settingsType As Type = GetType(Main).Assembly.GetType("My.MySettings", throwOnError:=True)
        Dim themeProperty As Reflection.PropertyInfo = settingsType.GetProperty("Theme")
        Dim defaultValue As Global.System.Configuration.DefaultSettingValueAttribute =
            DirectCast(Attribute.GetCustomAttribute(themeProperty, GetType(Global.System.Configuration.DefaultSettingValueAttribute)), Global.System.Configuration.DefaultSettingValueAttribute)

        Assert.AreEqual(ThemeManager.SystemSettingValue, defaultValue.Value)
        Assert.AreEqual(AppTheme.Light, ThemeManager.ResolveTheme(defaultValue.Value, systemUsesLightTheme:=True))
        Assert.AreEqual(AppTheme.Dark, ThemeManager.ResolveTheme(defaultValue.Value, systemUsesLightTheme:=False))
    End Sub

    <TestMethod>
    Public Sub IsSystemLightThemeEnabled_DoesNotThrow()
        ' The result depends on the user settings of Windows, so only the absence of exceptions can be verified.
        ThemeManager.IsSystemLightThemeEnabled()
    End Sub

#End Region

#Region " Palettes "

    <TestMethod>
    Public Sub GetPalette_ReturnsPredefinedPalettes()
        Assert.AreSame(ThemePalette.Light, ThemeManager.GetPalette(AppTheme.Light))
        Assert.AreSame(ThemePalette.Dark, ThemeManager.GetPalette(AppTheme.Dark))
        Assert.IsFalse(ThemePalette.Light.IsDark)
        Assert.IsTrue(ThemePalette.Dark.IsDark)
    End Sub

    <TestMethod>
    Public Sub GetPalette_InvalidTheme_ThrowsArgumentException()
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ThemeManager.GetPalette(CType(99, AppTheme)))
    End Sub

    <TestMethod>
    Public Sub LightPalette_UsesWindowsSystemColors()
        Assert.AreEqual(SystemColors.Control, ThemePalette.Light.WindowBackColor)
        Assert.AreEqual(SystemColors.ControlText, ThemePalette.Light.WindowForeColor)
        Assert.AreEqual(SystemColors.Window, ThemePalette.Light.InputBackColor)
        Assert.AreEqual(SystemColors.WindowText, ThemePalette.Light.InputForeColor)
    End Sub

    <TestMethod>
    Public Sub DarkPalette_BackgroundIsDarkerThanLightPalette()
        Assert.IsTrue(ThemePalette.Dark.WindowBackColor.GetBrightness() < 0.2F)
        Assert.IsTrue(ThemePalette.Dark.WindowForeColor.GetBrightness() > 0.8F)
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub Palettes_TextColors_HaveReadableContrast(isDark As Boolean)
        Dim p As ThemePalette = If(isDark, ThemePalette.Dark, ThemePalette.Light)
        Const minimum As Double = 4.5 ' WCAG AA for normal text.

        Assert.IsTrue(GetContrastRatio(p.WindowForeColor, p.WindowBackColor) >= minimum, "Window text.")
        Assert.IsTrue(GetContrastRatio(p.InputForeColor, p.InputBackColor) >= minimum, "Input text.")
        Assert.IsTrue(GetContrastRatio(p.AccentForeColor, p.AccentBackColor) >= minimum, "Accent button text.")
        Assert.IsTrue(GetContrastRatio(p.AccentForeColor, p.AccentHoverBackColor) >= 4.0, "Accent button text (hover).")
        Assert.IsTrue(GetContrastRatio(p.DangerForeColor, p.DangerBackColor) >= minimum, "Danger button text.")
        Assert.IsTrue(GetContrastRatio(p.StatusForeColor, p.StatusBackColor) >= minimum, "Status bar text.")
        Assert.IsTrue(GetContrastRatio(p.ProgressTextColor, p.ProgressTrackColor) >= minimum, "Progress text over the track.")
    End Sub

    <TestMethod>
    Public Sub DarkPalette_ProgressText_IsReadableOverTheBar()
        Dim p As ThemePalette = ThemePalette.Dark
        Assert.IsTrue(GetContrastRatio(p.ProgressTextColor, p.ProgressBarColor) >= 4.5)
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub Palettes_ProgressBarColor_IsGreenAndDifferentFromTheAccentColor(isDark As Boolean)
        ' The progress bar must not share the color of the 'Build Mirror' button, which is confusing.
        Dim p As ThemePalette = If(isDark, ThemePalette.Dark, ThemePalette.Light)

        Assert.AreNotEqual(p.AccentBackColor.ToArgb(), p.ProgressBarColor.ToArgb())
        Assert.IsTrue(p.ProgressBarColor.G > p.ProgressBarColor.R AndAlso p.ProgressBarColor.G > p.ProgressBarColor.B, "The progress bar color must be green.")
        Assert.IsTrue(p.ProgressBarColor.GetSaturation() < 0.7F, "The green must not be too saturated (garish).")
    End Sub

    <TestMethod>
    <DataRow(False)>
    <DataRow(True)>
    Public Sub Palettes_ProgressStateColors_AreYellowAndRedWithReadableText(isDark As Boolean)
        Dim p As ThemePalette = If(isDark, ThemePalette.Dark, ThemePalette.Light)

        Assert.IsTrue(p.ProgressWarningColor.R > p.ProgressWarningColor.B AndAlso p.ProgressWarningColor.G > p.ProgressWarningColor.B,
                      "The warning color must be yellow/amber.")
        Assert.IsTrue(p.ProgressErrorColor.R > p.ProgressErrorColor.G AndAlso p.ProgressErrorColor.R > p.ProgressErrorColor.B,
                      "The error color must be red.")
        Assert.IsTrue(GetReadableContrast(p, p.ProgressWarningColor) >= 4.5, "Text over the warning color.")
        Assert.IsTrue(GetReadableContrast(p, p.ProgressErrorColor) >= 4.5, "Text over the error color.")
    End Sub

    ''' <summary>
    ''' Gets the contrast that keeps the progress text readable over a bar color: 
    ''' the contrast of the text color, or the contrast of its outline color if the palette draws an outline.
    ''' </summary>
    Private Shared Function GetReadableContrast(p As ThemePalette, barColor As Color) As Double
        Dim textContrast As Double = GetContrastRatio(p.ProgressTextColor, barColor)
        If p.ProgressTextOutlineColor.IsEmpty Then
            Return textContrast
        End If
        Return Math.Max(textContrast, GetContrastRatio(p.ProgressTextOutlineColor, barColor))
    End Function

    <TestMethod>
    Public Sub DarkPalette_WarningColor_IsAMutedYellow()
        ' Regression test: the previous yellow was too garish, and the white text was not readable over it.
        Dim warning As Color = ThemePalette.Dark.ProgressWarningColor
        Assert.IsTrue(warning.GetSaturation() < 0.7F, "The yellow must not be too saturated.")
        Assert.IsTrue(warning.GetBrightness() < 0.5F, "The yellow must not be too bright.")
        Assert.IsFalse(ThemePalette.Dark.ProgressTextOutlineColor.IsEmpty, "The dark theme draws an outline around the text.")
    End Sub

    <TestMethod>
    Public Sub ApplyTheme_ProgressBar_AppliesTheTextOutline()
        Using progressBar As New ElektroProgressBar()
            ThemeManager.ApplyTheme(progressBar, ThemePalette.Dark)
            Assert.AreEqual(ThemePalette.Dark.ProgressTextOutlineColor, progressBar.TextOutlineColor)

            ThemeManager.ApplyTheme(progressBar, ThemePalette.Light)
            Assert.AreEqual(ThemePalette.Light.ProgressTextOutlineColor, progressBar.TextOutlineColor)
        End Using
    End Sub

    <TestMethod>
    <DataRow(CInt(ProgressBarState.Normal))>
    <DataRow(CInt(ProgressBarState.Paused))>
    <DataRow(CInt(ProgressBarState.Error))>
    Public Sub GetProgressBarColor_ReturnsTheColorOfTheState(stateValue As Integer)
        Dim p As ThemePalette = ThemePalette.Dark
        Dim state As ProgressBarState = CType(stateValue, ProgressBarState)
        Dim expected As Color

        Select Case state
            Case ProgressBarState.Normal
                expected = p.ProgressBarColor
            Case ProgressBarState.Paused
                expected = p.ProgressWarningColor
            Case Else
                expected = p.ProgressErrorColor
        End Select

        Assert.AreEqual(expected, ThemeManager.GetProgressBarColor(p, state))
    End Sub

    <TestMethod>
    Public Sub GetProgressBarColor_InvalidArguments_Throw()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.GetProgressBarColor(Nothing, ProgressBarState.Normal))
        Assert.ThrowsExactly(Of System.ComponentModel.InvalidEnumArgumentException)(Sub() ThemeManager.GetProgressBarColor(ThemePalette.Dark, CType(999, ProgressBarState)))
    End Sub

    <TestMethod>
    Public Sub ApplyTheme_ProgressBarInErrorState_KeepsTheErrorColorAfterChangingTheTheme()
        Using progressBar As New ElektroProgressBar()
            progressBar.State = ProgressBarState.Error

            ThemeManager.ApplyTheme(progressBar, ThemePalette.Light)
            ThemeManager.ApplyTheme(progressBar, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.ProgressErrorColor, progressBar.ForeColor)
        End Using
    End Sub

#End Region

#Region " ApplyTheme "

    ''' <summary>
    ''' Creates a form that contains one control of every type handled by the theme manager,
    ''' including a nested container.
    ''' </summary>
    Private Shared Function CreateTestForm() As Form
        Dim form As New Form()
        Dim panel As New Panel() With {.Name = "Panel"}
        panel.Controls.Add(New Label() With {.Name = "NestedLabel", .Text = "Nested"})
        Dim statusStrip As New StatusStrip() With {.Name = "StatusStrip"}
        statusStrip.Items.Add(New ToolStripStatusLabel("Ready") With {.Name = "StatusLabel"})

        form.Controls.AddRange({
            New TextBox() With {.Name = "TextBox"},
            New TextBox() With {.Name = "ReadOnlyTextBox", .ReadOnly = True},
            New Button() With {.Name = "Button"},
            New Label() With {.Name = "Label"},
            New ThemedCheckBox() With {.Name = "ThemedCheckBox"},
            New ToggleSwitch() With {.Name = "ToggleSwitch"},
            New ActionButton() With {.Name = "ActionButton", .Kind = ActionButtonKind.Danger},
            New IconButton() With {.Name = "IconButton"},
            New SectionHeaderLabel() With {.Name = "SectionHeaderLabel"},
            New ElektroProgressBar() With {.Name = "ProgressBar"},
            panel,
            statusStrip
        })
        Return form
    End Function

    <TestMethod>
    Public Sub ApplyTheme_Dark_AppliesDarkColorsToAllControls()
        Dim p As ThemePalette = ThemePalette.Dark
        Using form As Form = CreateTestForm()
            ThemeManager.ApplyTheme(form, p)

            Assert.AreEqual(p.WindowBackColor, form.BackColor)
            Assert.AreEqual(p.WindowForeColor, form.ForeColor)

            Dim textBox As TextBox = DirectCast(form.Controls("TextBox"), TextBox)
            Assert.AreEqual(p.InputBackColor, textBox.BackColor)
            Assert.AreEqual(p.InputForeColor, textBox.ForeColor)
            Assert.AreEqual(BorderStyle.FixedSingle, textBox.BorderStyle)

            Dim button As Button = DirectCast(form.Controls("Button"), Button)
            Assert.AreEqual(FlatStyle.Flat, button.FlatStyle)
            Assert.AreEqual(p.ControlBackColor, button.BackColor)
            Assert.AreEqual(p.ControlBorderColor, button.FlatAppearance.BorderColor)

            Assert.AreEqual(p.WindowForeColor, form.Controls("Label").ForeColor)
            Assert.AreEqual(p.WindowForeColor, form.Controls("Panel").Controls("NestedLabel").ForeColor, "Nested controls must be themed.")

            Assert.AreSame(p, DirectCast(form.Controls("ThemedCheckBox"), ThemedCheckBox).Palette)
            Assert.AreSame(p, DirectCast(form.Controls("ToggleSwitch"), ToggleSwitch).Palette)
            Assert.AreSame(p, DirectCast(form.Controls("IconButton"), IconButton).Palette)

            Dim actionButton As ActionButton = DirectCast(form.Controls("ActionButton"), ActionButton)
            Assert.AreSame(p, actionButton.Palette)
            Assert.AreEqual(ActionButtonKind.Danger, actionButton.Kind, "The kind of the button must be preserved.")

            Assert.AreEqual(p.SeparatorColor, DirectCast(form.Controls("SectionHeaderLabel"), SectionHeaderLabel).LineColor)

            Dim progressBar As ElektroProgressBar = DirectCast(form.Controls("ProgressBar"), ElektroProgressBar)
            Assert.IsFalse(progressBar.UseVisualStyles)
            Assert.AreEqual(p.ProgressTrackColor, progressBar.BackColor)
            Assert.AreEqual(p.ProgressBarColor, progressBar.ForeColor)
            Assert.AreEqual(p.ProgressTextColor, progressBar.TextColor)
            Assert.IsTrue(progressBar.ShowBorder)

            Dim statusStrip As StatusStrip = DirectCast(form.Controls("StatusStrip"), StatusStrip)
            Assert.AreEqual(p.StatusBackColor, statusStrip.BackColor)
            Assert.AreEqual(p.StatusForeColor, statusStrip.Items("StatusLabel").ForeColor)
            Assert.IsInstanceOfType(Of FlatToolStripRenderer)(statusStrip.Renderer)
        End Using
    End Sub

    <TestMethod>
    Public Sub ApplyTheme_DarkThenLight_RestoresNativeLook()
        Using form As Form = CreateTestForm()
            ThemeManager.ApplyTheme(form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(form, ThemePalette.Light)

            Dim textBox As TextBox = DirectCast(form.Controls("TextBox"), TextBox)
            Assert.AreEqual(SystemColors.Window, textBox.BackColor)
            Assert.AreEqual(BorderStyle.Fixed3D, textBox.BorderStyle)

            Dim readOnlyTextBox As TextBox = DirectCast(form.Controls("ReadOnlyTextBox"), TextBox)
            Assert.AreEqual(SystemColors.Control, readOnlyTextBox.BackColor, "A read-only text box uses the system 'Control' color by default.")

            Dim button As Button = DirectCast(form.Controls("Button"), Button)
            Assert.AreEqual(FlatStyle.Standard, button.FlatStyle)
            Assert.IsTrue(button.UseVisualStyleBackColor)

            Dim progressBar As ElektroProgressBar = DirectCast(form.Controls("ProgressBar"), ElektroProgressBar)
            Assert.IsTrue(progressBar.UseVisualStyles)
            Assert.IsFalse(progressBar.ShowBorder)

            Assert.AreEqual(SystemColors.Control, form.BackColor)
            Assert.AreSame(ThemePalette.Light, DirectCast(form.Controls("ThemedCheckBox"), ThemedCheckBox).Palette)
        End Using
    End Sub

    <TestMethod>
    Public Sub ApplyTheme_WithCreatedHandles_DoesNotThrow()
        Using form As Form = CreateTestForm()
            form.CreateControl()
            Dim progressBarHandle As IntPtr = form.Controls("ProgressBar").Handle
            Assert.AreNotEqual(IntPtr.Zero, progressBarHandle)

            ThemeManager.ApplyTheme(form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(form, ThemePalette.Light)
            ThemeManager.ApplyTheme(form, ThemePalette.Dark)
        End Using
    End Sub

    <TestMethod>
    Public Sub ApplyTheme_NullArguments_ThrowsArgumentNullException()
        Using form As New Form()
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyTheme(Nothing, ThemePalette.Dark))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyTheme(form, Nothing))
        End Using
    End Sub

#End Region

#Region " ApplyTitleBarTheme "

    <TestMethod>
    Public Sub ApplyTitleBarTheme_NullForm_ThrowsArgumentNullException()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyTitleBarTheme(Nothing, True))
    End Sub

    <TestMethod>
    Public Sub ApplyTitleBarTheme_HandleNotCreated_ThrowsInvalidOperationException()
        Using form As New Form()
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() ThemeManager.ApplyTitleBarTheme(form, True))
        End Using
    End Sub

    <TestMethod>
    Public Sub ApplyTitleBarTheme_HandleCreated_IsSupportedOnWindows10AndLater()
        ' Note: Environment.OSVersion is not reliable (it reports Windows 8 to non-manifested processes).
        Dim buildNumber As Integer
        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion")
            buildNumber = CInt(key.GetValue("CurrentBuildNumber"))
        End Using

        If buildNumber < 17763 Then
            Assert.Inconclusive("The dark title bar requires Windows 10 1809 or later.")
        End If

        Using form As New Form()
            Dim handle As IntPtr = form.Handle
            Assert.AreNotEqual(IntPtr.Zero, handle)

            Assert.IsTrue(ThemeManager.ApplyTitleBarTheme(form, useDarkMode:=True))
            Assert.IsTrue(ThemeManager.ApplyTitleBarTheme(form, useDarkMode:=False))
        End Using
    End Sub

#End Region

End Class

#End Region
