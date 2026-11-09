#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.InteropServices

Imports DevCase.Interop.Win32.Enums

#End Region

#Region " P/Invoking "

Namespace DevCase.Interop.Win32

    Partial Public NotInheritable Class NativeMethods

#Region " DwmApi.dll "

        ''' <summary>
        ''' Sets the value of Desktop Window Manager (DWM) non-client rendering attributes for a window.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute"/>
        ''' </remarks>
        ''' <param name="hwnd">
        ''' The handle to the window for which the attribute value is to be set.
        ''' </param>
        '''
        ''' <param name="attribute">
        ''' The attribute to set.
        ''' </param>
        '''
        ''' <param name="attributeValue">
        ''' The value of the attribute.
        ''' </param>
        '''
        ''' <param name="attributeSize">
        ''' The size, in bytes, of the attribute value.
        ''' </param>
        ''' <returns>
        ''' If the function succeeds, it returns <c>S_OK</c> (zero). Otherwise, it returns an <c>HRESULT</c> error code.
        ''' </returns>
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("DwmApi.dll", SetLastError:=False)>
        Public Shared Function DwmSetWindowAttribute(hwnd As IntPtr,
                                                     attribute As DwmWindowAttribute,
                                                     ByRef attributeValue As Integer,
                                                     attributeSize As Integer
        ) As Integer
        End Function

#End Region

#Region " UxTheme.dll "

        ''' <summary>
        ''' Causes a window to use a different set of visual style information than its class normally uses.
        ''' <para></para>
        ''' Pass empty strings to disable the visual styles of the window,
        ''' or <see langword="Nothing"/> in both parameters to restore the default visual styles.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/uxtheme/nf-uxtheme-setwindowtheme"/>
        ''' </remarks>
        ''' <param name="hwnd">
        ''' Handle to the window whose visual style information is to be changed.
        ''' </param>
        '''
        ''' <param name="subAppName">
        ''' The application name to use in place of the calling application's name.
        ''' </param>
        '''
        ''' <param name="subIdList">
        ''' A semicolon-separated list of CLSID names to use in place of the actual list passed by the window's class.
        ''' </param>
        ''' <returns>
        ''' If the function succeeds, it returns <c>S_OK</c> (zero). Otherwise, it returns an <c>HRESULT</c> error code.
        ''' </returns>
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("UxTheme.dll", CharSet:=CharSet.Unicode, SetLastError:=False)>
        Public Shared Function SetWindowTheme(hwnd As IntPtr,
                                              subAppName As String,
                                              subIdList As String
        ) As Integer
        End Function

#End Region

#Region " User32.dll "

        ''' <summary>
        ''' Sends the specified window message to a window.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessagew"/>
        ''' </remarks>
        ''' <param name="hwnd">
        ''' A handle to the window whose window procedure will receive the message.
        ''' </param>
        '''
        ''' <param name="msg">
        ''' The message to be sent.
        ''' </param>
        '''
        ''' <param name="wParam">
        ''' Additional message-specific information.
        ''' </param>
        '''
        ''' <param name="lParam">
        ''' Additional message-specific information.
        ''' </param>
        ''' <returns>
        ''' The result of the message processing; it depends on the message sent.
        ''' </returns>
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("User32.dll", EntryPoint:="SendMessageW", SetLastError:=True)>
        Public Shared Function SendMessage(hwnd As IntPtr,
                                           msg As WindowsMessages,
                                           wParam As IntPtr,
                                           lParam As IntPtr
        ) As IntPtr
        End Function

#End Region

    End Class

End Namespace

#End Region
