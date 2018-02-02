




' THIS OPEN-SOURCE APPLICATION IS POWERED BY ELEKTROKIT FRAMEWORK, CREATED BY ELEKTRO STUDIOS.

' WHAT YOU SEE HERE IS FREE CUTTED CONTENT OF THIS FRAMEWORK.

' IF YOU LIKED THIS FREE APPLICATION, THEN PLEASE CONSIDER TO BUY ELEKTROKIT FRAMEWORK FOR .NET AT:
' https://codecanyon.net/item/elektrokit-class-library-for-net/19260282

' YOU CAN FIND THESE HELPER METHODS AND A MASSIVE AMOUNT MORE!, 
' +850 EXTENSION METHODS FOR ALL KIND OF TYPES, CUSTOM USER-CONTROLS, 
' EVERYTHING FOR THE NEWBIE And THE ADVANCED USER, FOR VB.NET AND C#. 

' ElektroKit is a utility framework containing new APIs and extensions to the core .NET Framework 
' to help complete your developer toolbox.
' It Is a set of general purpose classes provided as easy to consume packages.
' These utility classes and components provide productivity in day to day software development 
' mainly focused To WindowsForms. 

' UPDATES OF ELEKTROKIT ARE MAINTAINED AND RELEASED EVERY MONTH.





#Region " Imports "

Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.InteropServices
Imports System.Security

Imports ElektroKit.Interop.Win32.Types

#End Region

#Region " P/Invoking "

Namespace ElektroKit.Interop.Win32

    Partial Public NotInheritable Class NativeMethods

#Region " GDI32.dll "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a logical pen, brush, font, bitmap, region, or palette,
        ''' freeing all system resources associated with the object.
        ''' <para></para>
        ''' After the object is deleted, the specified handle is no longer valid.
        ''' <para></para>
        ''' Do not delete a drawing object (pen or brush) while it is still selected into a DC.
        ''' <para></para>
        ''' When a pattern brush is deleted, the bitmap associated with the brush is not deleted. 
        ''' The bitmap must be deleted independently.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633540%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hObject">
        ''' A handle to a logical pen, brush, font, bitmap, region, or palette.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the specified handle is not valid or is currently selected into a DC, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False, SetLastError:=True)>
        Public Shared Function DeleteObject(ByVal hObject As IntPtr
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a logical pen, brush, font, bitmap, region, or palette,
        ''' freeing all system resources associated with the object.
        ''' <para></para>
        ''' After the object is deleted, the specified handle is no longer valid.
        ''' <para></para>
        ''' Do not delete a drawing object (pen or brush) while it is still selected into a DC.
        ''' <para></para>
        ''' When a pattern brush is deleted, the bitmap associated with the brush is not deleted. 
        ''' The bitmap must be deleted independently.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633540%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hObject">
        ''' A handle to a logical pen, brush, font, bitmap, region, or palette.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the specified handle is not valid or is currently selected into a DC, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False, SetLastError:=True)>
        Public Shared Function DeleteObject(ByVal hObject As HandleRef
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the specified buffer with the metrics for the currently selected font.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle To a DC (Device Context).
        ''' </param>
        ''' 
        ''' <param name="refMetrics">
        ''' A pointer to the <see cref="TextMetricA"/> structure that receives the text metrics.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Ansi, ExactSpelling:=False)>
        Public Shared Function GetTextMetricsA(ByVal hdc As IntPtr,
                                               ByRef refMetrics As TextMetricA
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the specified buffer with the metrics for the currently selected font.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle To a DC (Device Context).
        ''' </param>
        ''' 
        ''' <param name="refMetrics">
        ''' A pointer to the <see cref="TextMetricA"/> structure that receives the text metrics.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Ansi, ExactSpelling:=False)>
        Public Shared Function GetTextMetricsA(ByVal hdc As HandleRef,
                                               ByRef refMetrics As TextMetricA
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the specified buffer with the metrics for the currently selected font.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle To a DC (Device Context).
        ''' </param>
        ''' 
        ''' <param name="refMetrics">
        ''' A pointer to the <see cref="TextMetricW"/> structure that receives the text metrics.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=False)>
        Public Shared Function GetTextMetricsW(ByVal hdc As IntPtr,
                                               ByRef refMetrics As TextMetricW
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the specified buffer with the metrics for the currently selected font.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle To a DC (Device Context).
        ''' </param>
        ''' 
        ''' <param name="refMetrics">
        ''' A pointer to the <see cref="TextMetricW"/> structure that receives the text metrics.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=False)>
        Public Shared Function GetTextMetricsW(ByVal hdc As HandleRef,
                                               ByRef refMetrics As TextMetricW
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Selects an object into a specified device context.
        ''' <para></para>
        ''' The new object replaces the previous object of the same type. 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162957%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle to the <c>DC</c>.
        ''' </param>
        ''' 
        ''' <param name="hObject">
        ''' A handle to the object to be selected.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the selected object is not a region and the function succeeds, 
        ''' the return value is a handle to the object being replaced.
        ''' <para></para>
        ''' If the selected object is a region and the function succeeds, 
        ''' the return value is one of the following values.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False)>
        Public Shared Function SelectObject(ByVal hdc As IntPtr,
                                            ByVal hObject As IntPtr
        ) As IntPtr
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Selects an object into a specified device context.
        ''' <para></para>
        ''' The new object replaces the previous object of the same type. 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162957%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' A handle to the <c>DC</c>.
        ''' </param>
        ''' 
        ''' <param name="hObject">
        ''' A handle to the object to be selected.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the selected object is not a region and the function succeeds, 
        ''' the return value is a handle to the object being replaced.
        ''' <para></para>
        ''' If the selected object is a region and the function succeeds, 
        ''' the return value is one of the following values.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("GDI32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False)>
        Public Shared Function SelectObject(ByVal hdc As HandleRef,
                                            ByVal hObject As HandleRef
        ) As IntPtr
        End Function

#End Region

    End Class

End Namespace

#End Region
