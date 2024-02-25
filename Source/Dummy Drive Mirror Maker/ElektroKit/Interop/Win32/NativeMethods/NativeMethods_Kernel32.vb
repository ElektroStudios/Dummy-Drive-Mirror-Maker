




' THIS OPEN-SOURCE APPLICATION IS POWERED BY DEVCASE CLASS LIBRARY, CREATED BY ELEKTRO STUDIOS.

' WHAT YOU SEE HERE IS FREE CUTTED CONTENT OF THIS FRAMEWORK.

' IF YOU LIKED THIS FREE APPLICATION, THEN PLEASE CONSIDER TO BUY DEVCASE CLASS LIBRARY FOR .NET AT:
' https://codecanyon.net/item/DevCase-class-library-for-net/19260282

' YOU CAN FIND THESE HELPER METHODS AND A MASSIVE AMOUNT MORE!, 
' +850 EXTENSION METHODS FOR ALL KIND OF TYPES, CUSTOM USER-CONTROLS, 
' EVERYTHING FOR THE NEWBIE And THE ADVANCED USER, FOR VB.NET AND C#. 

' DevCase is a utility framework containing new APIs and extensions to the core .NET Framework 
' to help complete your developer toolbox.
' It Is a set of general purpose classes provided as easy to consume packages.
' These utility classes and components provide productivity in day to day software development 
' mainly focused To WindowsForms. 

' UPDATES OF DevCase ARE MAINTAINED AND RELEASED EVERY MONTH.





#Region " Imports "

Imports Microsoft.Win32.SafeHandles
Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security

Imports DevCase.Interop.Win32.Enums

#End Region

#Region " P/Invoking "

Namespace DevCase.Interop.Win32

    Partial Public NotInheritable Class NativeMethods

#Region " Kernel32.dll "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the thread identifier of the calling thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms683183%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The thread identifier of the calling thread.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <SuppressUnmanagedCodeSecurity>
        <DllImport("Kernel32.dll", SetLastError:=False)>
        Public Shared Function GetCurrentThreadId(
        ) As UInteger
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363866%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dstPath">
        ''' The path of the symbolic link to be created.
        ''' </param>
        ''' 
        ''' <param name="srcPath">
        ''' The path of the target for the symbolic link to be created.
        ''' <para></para>
        ''' If <paramref name="srcPath"/> has a device name associated with it, 
        ''' the link is treated as an absolute link; 
        ''' otherwise, the link is treated as a relative link.
        ''' </param>
        ''' 
        ''' <param name="flags">
        ''' Indicates whether the link target is a file or is a directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' <para></para>
        ''' To get extended error information, call <see cref="Marshal.GetLastWin32Error()"/>.
        ''' <para></para>
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("Kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)>
        Public Shared Function CreateSymbolicLink(ByVal dstPath As String,
                                                  ByVal srcPath As String,
                    <MarshalAs(UnmanagedType.I4)> ByVal flags As SymbolicLinkFlags
        ) As <MarshalAs(UnmanagedType.I1)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sends a control code directly to a specified device driver, 
        ''' causing the corresponding device to perform the corresponding operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa363216(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hDevice">
        ''' A handle to the device on which the operation is to be performed. 
        ''' <para></para>
        ''' The device is typically a volume, directory, file, or stream. 
        ''' <para></para>
        ''' To retrieve a device handle, use the <see cref="NativeMethods.CreateFile"/> function.
        ''' </param>
        ''' 
        ''' <param name="ioControlCode">
        ''' The directory management control code for the operation. 
        ''' </param>
        ''' 
        ''' <param name="inputBuffer">
        ''' A pointer to the input buffer that contains the data required to perform the operation. 
        ''' <para></para>
        ''' The format of this data depends on the value of the <paramref name="ioControlCode"/> parameter.
        ''' <para></para>
        ''' This parameter can be <see langword="Nothing"/> if 
        ''' <paramref name="ioControlCode"/> parameter specifies an operation that does not require input data.
        ''' </param>
        ''' 
        ''' <param name="inputBufferSize">
        ''' The size of the input buffer specified in the <paramref name="inputBuffer"/> parameter, in bytes.
        ''' </param>
        ''' 
        ''' <param name="outputBuffer">
        ''' A pointer to the output buffer that is to receive the data returned by the operation. 
        ''' <para></para>
        ''' The format of this data depends on the value of the dwIoControlCode parameter.
        ''' <para></para>
        ''' This parameter can be <see langword="Nothing"/> if 
        ''' <paramref name="ioControlCode"/> parameter specifies an operation that does not return data.
        ''' </param>
        ''' 
        ''' <param name="outputBufferSize">
        ''' The size of the input buffer specified in the <paramref name="outputBuffer"/> parameter, in bytes.
        ''' </param>
        ''' 
        ''' <param name="refBytesReturned">
        ''' A pointer to a variable that receives the size of the data stored in the 
        ''' output buffer of <paramref name="outputBuffer"/> parameter, in bytes.
        ''' <para></para>
        ''' If the output buffer is too small to receive any data, the call fails, 
        ''' <see cref="Marshal.GetLastWin32Error"/> returns <c>Win32ErrorCode.ERROR_INSUFFICIENT_BUFFER</c>, 
        ''' and <paramref name="refBytesReturned"/> is zero.
        ''' <para></para>
        ''' If the output buffer is too small to hold all of the data but can hold some entries, 
        ''' some drivers will return as much data as fits. 
        ''' In this case, the call fails, <see cref="Marshal.GetLastWin32Error"/> returns 
        ''' <c>Win32ErrorCode.ERROR_MORE_DATA</c>, and <paramref name="refBytesReturned"/> indicates the 
        ''' amount of data received. 
        ''' Your application should call <see cref="NativeMethods.DeviceIoControl"/> again with the same operation, 
        ''' specifying a new starting point.
        ''' <para></para>
        ''' If <paramref name="overlapped"/> is <see langword="Nothing"/>, 
        ''' <paramref name="refBytesReturned"/> cannot be <see langword="Nothing"/>. 
        ''' Even when an operation returns no output data and <paramref name="outputBuffer"/> is <see langword="Nothing"/>, 
        ''' <see cref="NativeMethods.DeviceIoControl"/> makes use of <paramref name="refBytesReturned"/>. 
        ''' After such an operation, the value of <paramref name="refBytesReturned"/> is meaningless.
        ''' <para></para>
        ''' If <paramref name="overlapped"/> is not <see langword="Nothing"/>, 
        ''' <paramref name="refBytesReturned"/> can be <see langword="Nothing"/>. 
        ''' If this parameter is not <see langword="Nothing"/> and the operation returns data, 
        ''' <paramref name="refBytesReturned"/> is meaningless until the overlapped operation has completed. 
        ''' To retrieve the number of bytes returned, call <c>GetOverlappedResult</c> function. 
        ''' <para></para>
        ''' If <paramref name="hDevice"/> is associated with an I/O completion port, 
        ''' you can retrieve the number of bytes returned by calling <c>GetQueuedCompletionStatus</c> function.
        ''' </param>
        ''' 
        ''' <param name="overlapped">
        ''' A pointer to an <c>OVERLAPPED</c> structure.
        ''' <para></para>
        ''' If <paramref name="hDevice"/> was opened without specifying <c>CreateFileFlags.Overlapped</c> flag, 
        ''' <paramref name="overlapped"/> parameter is ignored.
        ''' <para></para>
        ''' If <paramref name="hDevice"/> was opened with the <c>CreateFileFlags.Overlapped</c> flag, 
        ''' the operation is performed as an overlapped (asynchronous) operation. 
        ''' In this case, <paramref name="overlapped"/> parameter must point to a valid <c>OVERLAPPED</c> structure 
        ''' that contains a handle to an event object. Otherwise, the function fails in unpredictable ways.
        ''' <para></para>
        ''' For overlapped operations, <see cref="NativeMethods.DeviceIoControl"/> returns immediately, 
        ''' and the event object is signaled when the operation has been completed. 
        ''' Otherwise, the function does not return until the operation has been completed or an error occurs.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the operation completes successfully, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the operation fails or is pending, the return value is <see langword="False"/>. 
        ''' <para></para>
        ''' To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function DeviceIoControl(ByVal hDevice As IntPtr,
                                               ByVal ioControlCode As DirectoryManagementControlCodes,
                                               ByVal inputBuffer As IntPtr,
                                               ByVal inputBufferSize As Integer,
                                               ByVal outputBuffer As IntPtr,
                                               ByVal outputBufferSize As Integer,
                                               ByRef refBytesReturned As Integer,
                                               ByVal overlapped As IntPtr
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates or opens a file or I/O device.
        ''' <para></para>
        ''' The most commonly used I/O devices are as follows: 
        ''' <para></para>
        ''' file, file stream, directory, physical disk, volume, console buffer, tape drive, 
        ''' communications resource, mailslot, and pipe. 
        ''' <para></para>
        ''' The function returns a handle that can be used to access the file or device 
        ''' for various types of I/O depending on the file or device and the flags and attributes specified.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filepath">
        ''' The name of the file or device to be created or opened.
        ''' <para></para>
        ''' In the <c>ANSI</c> version of this function (<c>NativeMethods.CreateFileA</c>), 
        ''' the name is limited to <c>MAX_PATH</c> characters. 
        ''' To extend this limit to <c>32,767</c> wide characters, 
        ''' call the <c>Unicode</c> version of the function (<c>NativeMethods.CreateFileW</c>) 
        ''' and prepend "<c>\\?\</c>" to the path, for example: <c>CreateFileW("\\?\C:\Very Long Path\File.txt")</c>
        ''' </param>
        ''' 
        ''' <param name="access">
        ''' The requested access to the file or device, 
        ''' which can be summarized as read, write, both or neither zero.
        ''' </param>
        ''' 
        ''' <param name="share">
        ''' The requested sharing mode of the file or device, 
        ''' which can be read, write, both, delete, all of these, or none (refer to the following table).
        ''' <para></para>
        ''' Access requests to attributes or extended attributes are not affected by this flag.
        ''' </param>
        ''' 
        ''' <param name="securityAttributes">
        ''' A pointer to a <c>SECURITY_ATTRIBUTES</c> structure 
        ''' that contains two separate but related data members: 
        ''' an optional security descriptor, and a <see cref="Boolean"/> value that determines whether 
        ''' the returned handle can be inherited by child processes.
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
        ''' A valid handle to a template file with the GENERIC_READ access right.
        ''' <para></para>
        ''' The template file supplies file attributes and extended attributes for the file that is being created.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, 
        ''' the return value is an open handle to the specified file, device, named pipe, or mail slot.
        ''' <para></para>
        ''' If the function fails, the return value is a <c>INVALID_HANDLE_VALUE</c> value (<c>IntPtr(-1)</c>). 
        ''' <para></para>
        ''' To get extended error information, call <see cref="Marshal.GetLastWin32Error()"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification:="Visible for API")>
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, BestFitMapping:=False, ThrowOnUnmappableChar:=True, SetLastError:=True)>
        Public Shared Function CreateFile(<MarshalAs(UnmanagedType.LPTStr)> ByVal filepath As String,
                                          <MarshalAs(UnmanagedType.U4)> ByVal access As FileAccessRights,
                                          <MarshalAs(UnmanagedType.U4)> ByVal share As FileShare,
                                          <MarshalAs(UnmanagedType.SysInt)> ByVal securityAttributes As IntPtr,
                                          <MarshalAs(UnmanagedType.U4)> ByVal creationDisposition As FileMode,
                                          <MarshalAs(UnmanagedType.U4)> ByVal flagsAndAttributes As UInteger,
                                          <MarshalAs(UnmanagedType.SysInt)> ByVal templateFile As IntPtr
        ) As SafeFileHandle
        End Function

#End Region

    End Class

End Namespace

#End Region
