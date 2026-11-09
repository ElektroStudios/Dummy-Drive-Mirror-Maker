#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.InteropServices

Imports Microsoft.Win32.SafeHandles

Imports DevCase.Interop.Win32.Types

#End Region

#Region " P/Invoking "

Namespace DevCase.Interop.Win32

    Partial Public NotInheritable Class NativeMethods

#Region " Kernel32.dll "

        ''' <summary>
        ''' Creates or opens a file or I/O device (Unicode version, with raw numeric flags).
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew"/>
        ''' </remarks>
        ''' <param name="fileName">
        ''' The name of the file or device to be created or opened.
        ''' </param>
        '''
        ''' <param name="desiredAccess">
        ''' The requested access to the file or device.
        ''' </param>
        '''
        ''' <param name="shareMode">
        ''' The requested sharing mode of the file or device.
        ''' </param>
        '''
        ''' <param name="securityAttributes">
        ''' A pointer to a SECURITY_ATTRIBUTES structure, or <see cref="IntPtr.Zero"/>.
        ''' </param>
        '''
        ''' <param name="creationDisposition">
        ''' An action to take on a file or device that exists or does not exist.
        ''' </param>
        '''
        ''' <param name="flagsAndAttributes">
        ''' The file or device attributes and flags.
        ''' </param>
        '''
        ''' <param name="templateFile">
        ''' A handle to a template file, or <see cref="IntPtr.Zero"/>.
        ''' </param>
        ''' <returns>
        ''' A handle to the file or device. Check <see cref="SafeHandle.IsInvalid"/> to determine whether the function failed.
        ''' </returns>
        <DllImport("Kernel32.dll", EntryPoint:="CreateFileW", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Shared Function CreateFileW(fileName As String,
                                           desiredAccess As UInteger,
                                           shareMode As UInteger,
                                           securityAttributes As IntPtr,
                                           creationDisposition As UInteger,
                                           flagsAndAttributes As UInteger,
                                           templateFile As IntPtr
        ) As SafeFileHandle
        End Function

        ''' <summary>
        ''' Sends a control code directly to a specified device driver, using managed byte buffers.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol"/>
        ''' </remarks>
        ''' <param name="device">
        ''' A handle to the device on which the operation is to be performed.
        ''' </param>
        '''
        ''' <param name="ioControlCode">
        ''' The control code for the operation.
        ''' </param>
        '''
        ''' <param name="inBuffer">
        ''' The input buffer, or <see langword="Nothing"/>.
        ''' </param>
        '''
        ''' <param name="inBufferSize">
        ''' The size of the input buffer, in bytes.
        ''' </param>
        '''
        ''' <param name="outBuffer">
        ''' The output buffer, or <see langword="Nothing"/>.
        ''' </param>
        '''
        ''' <param name="outBufferSize">
        ''' The size of the output buffer, in bytes.
        ''' </param>
        '''
        ''' <param name="bytesReturned">
        ''' Receives the size of the data stored in the output buffer, in bytes.
        ''' </param>
        '''
        ''' <param name="overlapped">
        ''' A pointer to an OVERLAPPED structure, or <see cref="IntPtr.Zero"/>.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if the operation completes successfully; otherwise, <see langword="False"/>.
        ''' </returns>
        <DllImport("Kernel32.dll", EntryPoint:="DeviceIoControl", SetLastError:=True)>
        Public Shared Function DeviceIoControlBuffer(device As SafeFileHandle,
                                                     ioControlCode As UInteger,
                                                     inBuffer As Byte(),
                                                     inBufferSize As Integer,
                                                     <Out> outBuffer As Byte(),
                                                     outBufferSize As Integer,
                                                     ByRef bytesReturned As Integer,
                                                     overlapped As IntPtr
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Retrieves file information for the specified file.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfileinformationbyhandle"/>
        ''' </remarks>
        ''' <param name="file">
        ''' A handle to the file that contains the information to be retrieved.
        ''' </param>
        '''
        ''' <param name="fileInformation">
        ''' Receives the file information.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds; otherwise, <see langword="False"/>.
        ''' </returns>
        <DllImport("Kernel32.dll", SetLastError:=True)>
        Public Shared Function GetFileInformationByHandle(file As SafeFileHandle,
                                                          ByRef fileInformation As ByHandleFileInformation
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Establishes a hard link between an existing file and a new file.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createhardlinkw"/>
        ''' </remarks>
        ''' <param name="fileName">
        ''' The name of the new file (the hard link).
        ''' </param>
        '''
        ''' <param name="existingFileName">
        ''' The name of the existing file.
        ''' </param>
        '''
        ''' <param name="securityAttributes">
        ''' Reserved; must be <see cref="IntPtr.Zero"/>.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds; otherwise, <see langword="False"/>.
        ''' </returns>
        <DllImport("Kernel32.dll", EntryPoint:="CreateHardLinkW", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Shared Function CreateHardLink(fileName As String,
                                              existingFileName As String,
                                              securityAttributes As IntPtr
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Sets the date and time that the specified file or directory was created, last accessed, or last modified.
        ''' </summary>
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-setfiletime"/>
        ''' </remarks>
        ''' <param name="file">
        ''' A handle to the file or directory, opened with the <c>FILE_WRITE_ATTRIBUTES</c> access right.
        ''' </param>
        '''
        ''' <param name="creationTime">
        ''' The new creation date and time (UTC file time).
        ''' </param>
        '''
        ''' <param name="lastAccessTime">
        ''' The new last access date and time (UTC file time).
        ''' </param>
        '''
        ''' <param name="lastWriteTime">
        ''' The new last write date and time (UTC file time).
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds; otherwise, <see langword="False"/>.
        ''' </returns>
        <DllImport("Kernel32.dll", SetLastError:=True)>
        Public Shared Function SetFileTime(file As SafeFileHandle,
                                           ByRef creationTime As Long,
                                           ByRef lastAccessTime As Long,
                                           ByRef lastWriteTime As Long
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

#End Region

    End Class

End Namespace

#End Region
