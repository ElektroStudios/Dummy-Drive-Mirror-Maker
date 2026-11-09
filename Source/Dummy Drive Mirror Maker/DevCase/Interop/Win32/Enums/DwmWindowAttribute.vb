#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " DwmWindowAttribute "

Namespace DevCase.Interop.Win32.Enums

    ''' <summary>
    ''' Flags used by the <see cref="NativeMethods.DwmSetWindowAttribute"/> function
    ''' to specify window attributes for Desktop Window Manager (DWM) non-client rendering.
    ''' </summary>
    ''' <remarks>
    ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute"/>
    ''' </remarks>
    Public Enum DwmWindowAttribute As Integer

        ''' <summary>
        ''' Undocumented value used to enable the dark mode title bar on
        ''' Windows 10 versions prior to 20H1 (builds 17763 to 18362).
        ''' </summary>
        UseImmersiveDarkModeBefore20H1 = 19

        ''' <summary>
        ''' Allows the window frame for this window to be drawn in dark mode colors
        ''' (Windows 10 20H1 / build 18985 or later).
        ''' </summary>
        UseImmersiveDarkMode = 20

    End Enum

End Namespace

#End Region
