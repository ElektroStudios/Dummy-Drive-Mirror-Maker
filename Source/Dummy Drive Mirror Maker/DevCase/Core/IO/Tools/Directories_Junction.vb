#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices

Imports Microsoft.Win32.SafeHandles

Imports DevCase.Interop.Win32

#End Region

#Region " Directories (Junctions) "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Directories

#Region " Constants "

        ''' <summary>GENERIC_READ access right.</summary>
        Private Const GenericRead As UInteger = &H80000000UI

        ''' <summary>GENERIC_WRITE access right.</summary>
        Private Const GenericWrite As UInteger = &H40000000UI

        ''' <summary>FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE.</summary>
        Private Const ShareAll As UInteger = &H7UI

        ''' <summary>OPEN_EXISTING creation disposition.</summary>
        Private Const OpenExisting As UInteger = 3UI

        ''' <summary>FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT.</summary>
        Private Const BackupSemanticsOpenReparsePoint As UInteger = &H2000000UI Or &H200000UI

        ''' <summary>FSCTL_GET_REPARSE_POINT control code.</summary>
        Private Const FsctlGetReparsePoint As UInteger = &H900A8UI

        ''' <summary>FSCTL_SET_REPARSE_POINT control code.</summary>
        Private Const FsctlSetReparsePoint As UInteger = &H900A4UI

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the specified directory is a directory junction
        ''' (a mount point that points to a directory, not to a volume).
        ''' </summary>
        ''' 
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' Dim isJunction As Boolean = Directories.IsJunction("C:\Users\All Users")
        ''' </code>
        ''' </example>
        ''' 
        ''' <param name="dirPath">
        ''' The directory path.
        ''' </param>
        ''' 
        ''' <returns>
        ''' <see langword="True"/> if the directory is a directory junction; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' 
        ''' <exception cref="DirectoryNotFoundException">
        ''' The directory does not exist.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Function IsJunction(dirPath As String) As Boolean

            Return Directories.IsMountPoint(dirPath) AndAlso
               Not Directories.ReadMountPoint(dirPath).IsVolumeMountPoint

        End Function

        ''' <summary>
        ''' Gets the target of the specified mount point (directory junction or volume mount point).
        ''' </summary>
        ''' 
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' Dim target As String = Directories.GetMountPointTarget("C:\Users\All Users") ' Result: "C:\ProgramData"
        ''' </code>
        ''' </example>
        ''' 
        ''' <param name="dirPath">
        ''' The path of the mount point.
        ''' </param>
        ''' 
        ''' <returns>
        ''' The target path: a regular path for directory junctions (e.g. <c>C:\Target</c>),
        ''' or a volume GUID path for volume mount points (e.g. <c>\\?\Volume{GUID}\</c>).
        ''' </returns>
        ''' 
        ''' <exception cref="DirectoryNotFoundException">
        ''' The directory does not exist.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentException">
        ''' The directory is not a mount point.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' The native function failed.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Function GetMountPointTarget(dirPath As String) As String

            Return Directories.ReadMountPoint(dirPath).TargetPath
        End Function

        ''' <summary>
        ''' Creates a directory junction at the specified path, pointing to the specified target directory.
        ''' <para></para>
        ''' Unlike symbolic links, directory junctions do not require any special privilege to be created.
        ''' </summary>
        ''' 
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' Directories.CreateJunction("C:\Mirror\My Junction", "D:\Real Target Folder")
        ''' </code>
        ''' </example>
        ''' 
        ''' <param name="junctionPath">
        ''' The full path of the junction to create. It must not exist.
        ''' </param>
        '''
        ''' <param name="targetPath">
        ''' The fully qualified local path of the target directory. The target does not need to exist.
        ''' </param>
        ''' 
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="junctionPath"/> or <paramref name="targetPath"/> is null, empty or white-spaced.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentException">
        ''' <paramref name="targetPath"/> is not a fully qualified local path.
        ''' </exception>
        '''
        ''' <exception cref="IOException">
        ''' A file or directory already exists at <paramref name="junctionPath"/>.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' The native function failed.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Sub CreateJunction(junctionPath As String, targetPath As String)

            If String.IsNullOrWhiteSpace(junctionPath) Then
                Throw New ArgumentNullException(NameOf(junctionPath), "Value is null, empty, or white-spaced.")
            End If

            ' Validates the target and builds the reparse data before touching the file system.
            Dim reparseBuffer As Byte() = MountPointReparseData.BuildJunctionBuffer(targetPath)

            If File.Exists(junctionPath) OrElse Directory.Exists(junctionPath) Then
                Throw New IOException($"Destination path '{junctionPath}' already exists.")
            End If

            Directory.CreateDirectory(junctionPath)
            Try
                Using handle As SafeFileHandle = Directories.OpenReparsePoint(junctionPath, Directories.GenericRead Or Directories.GenericWrite)
                    Dim bytesReturned As Integer
                    If Not NativeMethods.DeviceIoControlBuffer(handle, Directories.FsctlSetReparsePoint,
                                                               reparseBuffer, reparseBuffer.Length,
                                                               Nothing, 0, bytesReturned, IntPtr.Zero) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error())
                    End If
                End Using

            Catch
                ' Do not leave an empty regular directory where the junction was expected.
                Directory.Delete(junctionPath, recursive:=False)
                Throw

            End Try

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Reads and parses the reparse data of the specified mount point.
        ''' </summary>
        ''' 
        ''' <param name="dirPath">
        ''' The path of the mount point.
        ''' </param>
        ''' 
        ''' <returns>
        ''' The parsed <see cref="MountPointReparseData"/>.
        ''' </returns>
        <DebuggerStepThrough>
        Private Shared Function ReadMountPoint(dirPath As String) As MountPointReparseData

            If Not Directory.Exists(dirPath) Then
                Throw New DirectoryNotFoundException($"Directory not found: '{dirPath}'")
            End If

            Dim buffer(MountPointReparseData.MaximumBufferSize - 1) As Byte
            Dim bytesReturned As Integer

            ' No access rights are requested (zero) to read the reparse data, because some system junctions 
            ' (e.g. the legacy "My Music" junction inside the user's Documents folder) deny the read access to everyone.
            Using handle As SafeFileHandle = Directories.OpenReparsePoint(dirPath, desiredAccess:=0UI)
                If Not NativeMethods.DeviceIoControlBuffer(handle, Directories.FsctlGetReparsePoint,
                                                           Nothing, 0,
                                                           buffer, buffer.Length, bytesReturned, IntPtr.Zero) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If
            End Using

            Return MountPointReparseData.Parse(buffer, bytesReturned)

        End Function

        ''' <summary>
        ''' Opens the specified directory itself (not its reparse point target).
        ''' </summary>
        ''' 
        ''' <param name="dirPath">
        ''' The directory path.
        ''' </param>
        '''
        ''' <param name="desiredAccess">
        ''' The requested access rights.
        ''' </param>
        ''' 
        ''' <returns>
        ''' The handle of the directory.
        ''' </returns>
        <DebuggerStepThrough>
        Private Shared Function OpenReparsePoint(dirPath As String, desiredAccess As UInteger) As SafeFileHandle

            Dim handle As SafeFileHandle = NativeMethods.CreateFileW(dirPath, desiredAccess, Directories.ShareAll, IntPtr.Zero,
                                                                     Directories.OpenExisting, Directories.BackupSemanticsOpenReparsePoint,
                                                                     IntPtr.Zero)
            If handle.IsInvalid Then
                Dim win32Err As Integer = Marshal.GetLastWin32Error()
                handle.Dispose()
                Throw New Win32Exception(win32Err)
            End If

            Return handle

        End Function

#End Region

    End Class

End Namespace

#End Region
