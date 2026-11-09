#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32

Imports DevCase.Interop.Win32
Imports DevCase.Interop.Win32.Enums
Imports DevCase.UserControls.Controls
Imports DevCase.UserControls.Enums
Imports System.ComponentModel

#End Region

#Region " ThemeManager "

''' <summary>
''' Provides the logic to resolve, persist and apply the visual theme of the application.
''' </summary>
Friend NotInheritable Class ThemeManager

#Region " Constants "

    ''' <summary>
    ''' The setting value that means "follow the Windows apps theme".
    ''' </summary>
    Friend Const SystemSettingValue As String = "System"

    ''' <summary>
    ''' The registry key that contains the personalization settings of the current user.
    ''' </summary>
    Private Const PersonalizeRegistryKey As String = "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"

    ''' <summary>
    ''' The registry value that indicates whether the apps use the light theme (1) or the dark theme (0).
    ''' </summary>
    Private Const AppsUseLightThemeRegistryValue As String = "AppsUseLightTheme"

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="ThemeManager"/> class from being created.
    ''' </summary>
    <DebuggerNonUserCode>
    Private Sub New()
    End Sub

#End Region

#Region " Settings "

    ''' <summary>
    ''' Resolves the theme to use from the specified setting value.
    ''' </summary>
    ''' <param name="settingValue">
    ''' The setting value: <c>"Light"</c>, <c>"Dark"</c> or <c>"System"</c> (case-insensitive).
    ''' </param>
    '''
    ''' <param name="systemUsesLightTheme">
    ''' A value indicating whether Windows is configured to use the light theme for apps.
    ''' Only used when <paramref name="settingValue"/> is <c>"System"</c>.
    ''' </param>
    ''' <returns>
    ''' The resolved <see cref="AppTheme"/>.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="settingValue"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' <paramref name="settingValue"/> is not a valid theme setting value.
    ''' </exception>
    Friend Shared Function ResolveTheme(settingValue As String, systemUsesLightTheme As Boolean) As AppTheme

        If settingValue Is Nothing Then
            Throw New ArgumentNullException(NameOf(settingValue))
        End If

        Dim value As String = settingValue.Trim()

        If value.Equals(ThemeManager.SystemSettingValue, StringComparison.OrdinalIgnoreCase) Then
            Return If(systemUsesLightTheme, AppTheme.Light, AppTheme.Dark)
        End If

        If value.Equals(NameOf(AppTheme.Light), StringComparison.OrdinalIgnoreCase) Then
            Return AppTheme.Light
        End If

        If value.Equals(NameOf(AppTheme.Dark), StringComparison.OrdinalIgnoreCase) Then
            Return AppTheme.Dark
        End If

        Throw New ArgumentException($"Invalid theme setting value: '{settingValue}'.", NameOf(settingValue))

    End Function

    ''' <summary>
    ''' Gets the setting value that represents the specified theme.
    ''' </summary>
    ''' <param name="theme">
    ''' The theme.
    ''' </param>
    ''' <returns>
    ''' The setting value (<c>"Light"</c> or <c>"Dark"</c>).
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' <paramref name="theme"/> is not a valid <see cref="AppTheme"/> value.
    ''' </exception>
    Friend Shared Function ToSettingValue(theme As AppTheme) As String

        Select Case theme
            Case AppTheme.Light
                Return NameOf(AppTheme.Light)

            Case AppTheme.Dark
                Return NameOf(AppTheme.Dark)

            Case Else
                Throw New ArgumentException($"Invalid theme: '{theme}'.", NameOf(theme))

        End Select

    End Function

    ''' <summary>
    ''' Gets the color palette of the specified theme.
    ''' </summary>
    ''' <param name="theme">
    ''' The theme.
    ''' </param>
    ''' <returns>
    ''' The <see cref="ThemePalette"/>.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' <paramref name="theme"/> is not a valid <see cref="AppTheme"/> value.
    ''' </exception>
    Friend Shared Function GetPalette(theme As AppTheme) As ThemePalette

        Select Case theme
            Case AppTheme.Light
                Return ThemePalette.Light

            Case AppTheme.Dark
                Return ThemePalette.Dark

            Case Else
                Throw New ArgumentException($"Invalid theme: '{theme}'.", NameOf(theme))

        End Select

    End Function

    ''' <summary>
    ''' Determines whether Windows is configured to use the light theme for apps.
    ''' </summary>
    ''' <returns>
    ''' <see langword="True"/> if the apps use the light theme, or if the operating system does not support
    ''' a dark theme for apps (Windows versions prior to Windows 10);
    ''' <see langword="False"/> if the apps use the dark theme.
    ''' </returns>
    Friend Shared Function IsSystemLightThemeEnabled() As Boolean

        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(ThemeManager.PersonalizeRegistryKey, writable:=False)
            ' The key and the value do not exist in Windows versions without dark theme support.
            If key Is Nothing Then
                Return True
            End If

            Dim value As Object = key.GetValue(ThemeManager.AppsUseLightThemeRegistryValue)
            If value Is Nothing Then
                Return True
            End If

            If TypeOf value IsNot Integer Then
                Throw New InvalidOperationException($"Unexpected registry value type for '{ThemeManager.AppsUseLightThemeRegistryValue}': {value.GetType().Name}")
            End If

            Return DirectCast(value, Integer) <> 0
        End Using

    End Function

#End Region

#Region " Apply Theme "

    ''' <summary>
    ''' Applies the specified color palette to the specified control and all its child controls.
    ''' </summary>
    ''' <param name="root">
    ''' The root control (usually a <see cref="Form"/>).
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="root"/> or <paramref name="palette"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Shared Sub ApplyTheme(root As Control, palette As ThemePalette)

        If root Is Nothing Then
            Throw New ArgumentNullException(NameOf(root))
        End If

        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        root.SuspendLayout()
        Try
            ' Iterative (non-recursive) traversal of the control tree.
            Dim pending As New Stack(Of Control)
            pending.Push(root)
            While pending.Count > 0
                Dim ctrl As Control = pending.Pop()
                ThemeManager.ApplyThemeToControl(ctrl, palette)

                For Each child As Control In ctrl.Controls
                    pending.Push(child)
                Next child
            End While

        Finally
            root.ResumeLayout(performLayout:=True)

        End Try

        root.Invalidate(invalidateChildren:=True)

    End Sub

    ''' <summary>
    ''' Enables or disables the dark mode of the title bar (non-client area) of the specified window,
    ''' using the <see cref="DevCase.Interop.Win32.NativeMethods.DwmSetWindowAttribute"/> function.
    ''' <para></para>
    ''' The window handle must be created; call this method from <see cref="Form.OnHandleCreated"/>
    ''' (it has no effect if called before the handle is created, like in the <see cref="Form.Load"/> event),
    ''' and every time the theme changes.
    ''' </summary>
    ''' <param name="targetForm">
    ''' The form.
    ''' </param>
    '''
    ''' <param name="useDarkMode">
    ''' <see langword="True"/> to enable the dark title bar; <see langword="False"/> to disable it.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the operating system supports the dark title bar and the attribute was applied;
    ''' <see langword="False"/> if the operating system does not support it (Windows versions prior to Windows 10 1809).
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="targetForm"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="InvalidOperationException">
    ''' The handle of the form is not created.
    ''' </exception>
    Friend Shared Function ApplyTitleBarTheme(targetForm As Form, useDarkMode As Boolean) As Boolean

        If targetForm Is Nothing Then
            Throw New ArgumentNullException(NameOf(targetForm))
        End If

        If Not targetForm.IsHandleCreated Then
            Throw New InvalidOperationException("The handle of the form is not created.")
        End If

        Dim hwnd As IntPtr = targetForm.Handle
        Dim value As Integer = If(useDarkMode, 1, 0)

        ' Windows 10 20H1 and later.
        Dim hResult As Integer = NativeMethods.DwmSetWindowAttribute(hwnd, DwmWindowAttribute.UseImmersiveDarkMode, value, Runtime.InteropServices.Marshal.SizeOf(value))
        If hResult <> 0 Then
            ' Windows 10 1809 to 1909.
            hResult = NativeMethods.DwmSetWindowAttribute(hwnd, DwmWindowAttribute.UseImmersiveDarkModeBefore20H1, value, Runtime.InteropServices.Marshal.SizeOf(value))
        End If

        If hResult <> 0 Then
            Return False
        End If

        ' Windows 10 does not repaint the non-client area until the activation state of the window changes,
        ' so force the repaint by sending the activation messages again.
        If targetForm.Visible Then
            Dim isActive As Boolean = (Form.ActiveForm Is targetForm)
            NativeMethods.SendMessage(hwnd, WindowsMessages.WM_NCActivate, New IntPtr(If(isActive, 0, 1)), IntPtr.Zero)
            NativeMethods.SendMessage(hwnd, WindowsMessages.WM_NCActivate, New IntPtr(If(isActive, 1, 0)), IntPtr.Zero)
        End If

        Return True

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Applies the specified color palette to a single control (not to its child controls).
    ''' </summary>
    ''' <param name="ctrl">
    ''' The control.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Private Shared Sub ApplyThemeToControl(ctrl As Control, palette As ThemePalette)

        ' Custom controls know how to theme themselves.
        Dim themedControl As IThemedControl = TryCast(ctrl, IThemedControl)
        If themedControl IsNot Nothing Then
            themedControl.ApplyPalette(palette)
            Exit Sub
        End If

        Select Case True

            Case TypeOf ctrl Is ElektroProgressBar
                ThemeManager.ApplyThemeToProgressBar(DirectCast(ctrl, ElektroProgressBar), palette)

            Case TypeOf ctrl Is StatusStrip
                ThemeManager.ApplyThemeToStatusStrip(DirectCast(ctrl, StatusStrip), palette)

            Case TypeOf ctrl Is TextBoxBase
                ThemeManager.ApplyThemeToTextBox(DirectCast(ctrl, TextBoxBase), palette)

            Case TypeOf ctrl Is ButtonBase
                ThemeManager.ApplyThemeToButton(DirectCast(ctrl, ButtonBase), palette)

            Case TypeOf ctrl Is PictureBox
                ' Images keep their own colors.

            Case Else
                ' Forms, containers and labels.
                ctrl.BackColor = palette.WindowBackColor
                ctrl.ForeColor = palette.WindowForeColor

        End Select

    End Sub

    ''' <summary>
    ''' Gets the bar color of a progress bar for the specified state.
    ''' </summary>
    ''' <param name="palette">
    ''' The color palette.
    ''' </param>
    ''' 
    ''' <param name="state">
    ''' The state of the progress bar.
    ''' </param>
    ''' <returns>
    ''' The bar color: <see cref="ThemePalette.ProgressBarColor"/> (normal), 
    ''' <see cref="ThemePalette.ProgressWarningColor"/> (paused) or <see cref="ThemePalette.ProgressErrorColor"/> (error).
    ''' </returns>
    Friend Shared Function GetProgressBarColor(palette As ThemePalette, state As ProgressBarState) As Color

        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        Select Case state
            Case ProgressBarState.Normal
                Return palette.ProgressBarColor
            Case ProgressBarState.Paused
                Return palette.ProgressWarningColor
            Case ProgressBarState.Error
                Return palette.ProgressErrorColor
            Case Else
                Throw New InvalidEnumArgumentException(NameOf(state), CInt(state), GetType(ProgressBarState))
        End Select

    End Function

    ''' <summary>
    ''' Applies the specified color palette to a <see cref="ElektroProgressBar"/>.
    ''' </summary>
    ''' <param name="progressBar">
    ''' The progress bar.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Private Shared Sub ApplyThemeToProgressBar(progressBar As ElektroProgressBar, palette As ThemePalette)

        ' The light theme keeps the native (visual styles) look of Windows.
        ' The dark theme disables the visual styles, so the classic flat bar respects the custom colors.
        progressBar.BackColor = palette.ProgressTrackColor
        progressBar.ForeColor = ThemeManager.GetProgressBarColor(palette, progressBar.State)
        progressBar.TextColor = palette.ProgressTextColor
        progressBar.TextOutlineColor = palette.ProgressTextOutlineColor
        progressBar.BorderColor = palette.SeparatorColor
        progressBar.ShowBorder = palette.IsDark
        progressBar.UseVisualStyles = Not palette.IsDark

    End Sub

    ''' <summary>
    ''' Applies the specified color palette to a <see cref="StatusStrip"/> and its items.
    ''' </summary>
    ''' <param name="statusStrip">
    ''' The status strip.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Private Shared Sub ApplyThemeToStatusStrip(statusStrip As StatusStrip, palette As ThemePalette)

        statusStrip.Renderer = New FlatToolStripRenderer(palette.SeparatorColor)
        statusStrip.BackColor = palette.StatusBackColor
        statusStrip.ForeColor = palette.StatusForeColor

        For Each item As ToolStripItem In statusStrip.Items
            item.BackColor = palette.StatusBackColor
            item.ForeColor = palette.StatusForeColor
        Next item

    End Sub

    ''' <summary>
    ''' Applies the specified color palette to a <see cref="TextBoxBase"/>.
    ''' </summary>
    ''' <param name="textBox">
    ''' The text box.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Private Shared Sub ApplyThemeToTextBox(textBox As TextBoxBase, palette As ThemePalette)

        If palette.IsDark Then
            textBox.BackColor = palette.InputBackColor
            textBox.ForeColor = palette.InputForeColor
            textBox.BorderStyle = BorderStyle.FixedSingle

        Else
            ' Restore the default system colors (which also respects the read-only state).
            textBox.ResetBackColor()
            textBox.ResetForeColor()
            textBox.BorderStyle = BorderStyle.Fixed3D

        End If

    End Sub

    ''' <summary>
    ''' Applies the specified color palette to a standard <see cref="ButtonBase"/>.
    ''' </summary>
    ''' <param name="button">
    ''' The button.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Private Shared Sub ApplyThemeToButton(button As ButtonBase, palette As ThemePalette)

        If palette.IsDark Then
            button.FlatStyle = FlatStyle.Flat
            button.UseVisualStyleBackColor = False
            button.BackColor = palette.ControlBackColor
            button.ForeColor = palette.WindowForeColor
            button.FlatAppearance.BorderColor = palette.ControlBorderColor
            button.FlatAppearance.MouseOverBackColor = palette.ControlHoverBackColor
            button.FlatAppearance.MouseDownBackColor = palette.ControlPressedBackColor

        Else
            ' Restore the native look of Windows.
            button.FlatStyle = FlatStyle.Standard
            button.ResetBackColor()
            button.ResetForeColor()
            button.UseVisualStyleBackColor = True

        End If

    End Sub

#End Region

End Class

#End Region
