




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

Imports System.Runtime.InteropServices

Imports ElektroKit.Interop.Win32.Enums

#End Region

#Region " ReparseDataBuffer "

Namespace ElektroKit.Interop.Win32.Types

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains reparse point data for a Microsoft reparse point. 
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' Reparse points: <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa365503%28v=vs.85%29.aspx"/>
    ''' <para></para>
    ''' _REPARSE_DATA_BUFFER structure: <see href="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/content/ntifs/ns-ntifs-_reparse_data_buffer"/>
    ''' <para></para>
    ''' Symbolic Link Error Response: <see href="https://msdn.microsoft.com/en-us/library/cc246542.aspx"/>
    ''' <para></para>
    ''' Credits: 
    ''' <para></para>
    ''' <see href="http://troyparsons.com/blog/2012/03/symbolic-links-in-c-sharp/"/> and 
    ''' <see href="http://www.pinvoke.net/default.aspx/Structures.REPARSE_DATA_BUFFER"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepThrough>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure ReparseDataBuffer

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Reparse point tag. Must be a Microsoft reparse point tag.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <MarshalAs(UnmanagedType.U4)>
        Public ReparseTag As ReparsePointTags

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Size, in bytes, of the reparse data in the <see cref="ReparseDataBuffer.Buffer"/> member.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public ReparseDataLength As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Length, in bytes, of the unparsed portion of the file name pointed to by the 
        ''' FileName member of the associated file object. 
        ''' <para></para>
        ''' This member is only valid for create operations when the I/O fails with STATUS_REPARSE. 
        ''' For all other purposes, such as setting or querying a reparse point for the reparse data, 
        ''' this member is treated as reserved.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Reserved As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The offset, in bytes, from the beginning of the <see cref="ReparseDataBuffer.Buffer"/> member, 
        ''' at which the substitute name is located. 
        ''' <para></para>
        ''' The substitute name is the name the client MUST use to access this file if it requires to follow the symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public SubstituteNameOffset As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The length, in bytes, of the substitute name string. 
        ''' <para></para>
        ''' If there is a terminating null character at the end of the string, 
        ''' it is not included in the <see cref="ReparseDataBuffer.SubstituteNameLength"/> count. 
        ''' <para></para>
        ''' This value MUST be greater than or equal to 0.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public SubstituteNameLength As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The offset, in bytes, from the beginning of the PathBuffer field, at which the print name is located. 
        ''' <para></para>
        ''' The print name is the user-friendly name the client MUST return to the 
        ''' application if it requests the name of the symbolic link target
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public PrintNameOffset As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The length, in bytes, of the print name string. 
        ''' <para></para>
        ''' If there is a terminating null character at the end of the string, 
        ''' it is not included in the PrintNameLength count. 
        ''' <para></para>
        ''' This value MUST be greater than or equal to 0.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public PrintNameLength As UShort

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Flags that pecifies whether the substitute is an absolute target path name or 
        ''' a path name relative to the directory containing the symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Flags As ReparseDataBufferFlags

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A buffer that contains the Unicode strings for the substitute name and the print name, as described by 
        ''' <see cref="ReparseDataBuffer.SubstituteNameOffset"/>, 
        ''' <see cref="ReparseDataBuffer.SubstituteNameLength"/>, 
        ''' <see cref="ReparseDataBuffer.PrintNameOffset"/>, and 
        ''' <see cref="ReparseDataBuffer.PrintNameLength"/>.
        ''' <para></para>
        ''' The substitute name string MUST be a Unicode path to the target of the symbolic link. 
        ''' <para></para>
        ''' The print name string MUST be a Unicode string, suitable for display to a user, 
        ''' that also identifies the target of the symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H3FF0)> ' MaxUnicodePathLength
        Public Buffer() As Byte

    End Structure

End Namespace

#End Region
