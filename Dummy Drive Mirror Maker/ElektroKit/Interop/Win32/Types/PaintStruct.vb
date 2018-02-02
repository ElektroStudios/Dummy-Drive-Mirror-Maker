




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

#End Region

#Region " PaintStruct "

Namespace ElektroKit.Interop.Win32.Types

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains information for an application. 
    ''' <para></para>
    ''' This information can be used to paint the client area of a window owned by that application.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162768(v=vs.85).aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepThrough>
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure PaintStruct

#Region " Fields "

        ''' <summary>
        ''' A handle To the display DC To be used For painting.
        ''' </summary>
        <SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", Justification:="Visible for API")>
        Public Hdc As IntPtr

        ''' <summary>
        ''' Indicates whether the background must be erased. 
        ''' <para></para>
        ''' This value is non-zero if the application should erase the background. 
        ''' <para></para>
        ''' The application is responsible for erasing the background if a window class is created without a background brush. 
        ''' </summary>
        Public [Erase] As Boolean

        ''' <summary>
        ''' A <see cref="NativeRectangle"/> structure that specifies the upper left and lower right corners of the 
        ''' rectangle in which the painting is requested, in device units relative to the upper-left corner of the 
        ''' client area.
        ''' </summary>
        Public Rect As NativeRectangle

        ''' <summary>
        ''' Reserved; used internally by the system.
        ''' </summary>
        Public Restore As Boolean

        ''' <summary>
        ''' Reserved; used internally by the system.
        ''' </summary>
        Public IncUpdate As Boolean

#End Region

    End Structure

End Namespace

#End Region
