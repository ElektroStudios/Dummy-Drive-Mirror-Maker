#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices

#End Region

#Region " ByHandleFileInformation "

Namespace DevCase.Interop.Win32.Types

    ''' <summary>
    ''' Contains information that the <see cref="NativeMethods.GetFileInformationByHandle"/> function retrieves.
    ''' </summary>
    ''' <remarks>
    ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information"/>
    ''' <para></para>
    ''' Each FILETIME is declared as two 32-bit fields to keep the native 4-byte alignment.
    ''' </remarks>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure ByHandleFileInformation

        ''' <summary>The file attributes.</summary>
        Public FileAttributes As UInteger

        ''' <summary>The low-order part of the creation time.</summary>
        Public CreationTimeLow As UInteger

        ''' <summary>The high-order part of the creation time.</summary>
        Public CreationTimeHigh As UInteger

        ''' <summary>The low-order part of the last access time.</summary>
        Public LastAccessTimeLow As UInteger

        ''' <summary>The high-order part of the last access time.</summary>
        Public LastAccessTimeHigh As UInteger

        ''' <summary>The low-order part of the last write time.</summary>
        Public LastWriteTimeLow As UInteger

        ''' <summary>The high-order part of the last write time.</summary>
        Public LastWriteTimeHigh As UInteger

        ''' <summary>The serial number of the volume that contains the file.</summary>
        Public VolumeSerialNumber As UInteger

        ''' <summary>The high-order part of the file size.</summary>
        Public FileSizeHigh As UInteger

        ''' <summary>The low-order part of the file size.</summary>
        Public FileSizeLow As UInteger

        ''' <summary>The number of links to this file (hard links).</summary>
        Public NumberOfLinks As UInteger

        ''' <summary>The high-order part of a unique identifier that is associated with the file.</summary>
        Public FileIndexHigh As UInteger

        ''' <summary>The low-order part of a unique identifier that is associated with the file.</summary>
        Public FileIndexLow As UInteger

    End Structure

End Namespace

#End Region
