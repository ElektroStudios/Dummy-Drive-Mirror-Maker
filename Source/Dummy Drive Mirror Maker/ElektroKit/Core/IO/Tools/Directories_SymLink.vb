




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
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Imports DevCase.Interop.Win32
Imports DevCase.Interop.Win32.Enums
Imports DevCase.Interop.Win32.Types

#End Region

#Region " Directories "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Directories

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified directory.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcDir">
        ''' The source directory.
        ''' </param>
        ''' 
        ''' <param name="dstDir">
        ''' The destination directory where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(ByVal srcDir As DirectoryInfo, ByVal dstDir As DirectoryInfo)

            Directories.CreateSymbolicLink(srcDir.FullName, dstDir.FullName)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified directory.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcDir">
        ''' The source directory.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory path where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(ByVal srcDir As DirectoryInfo, ByVal dstDirPath As String)

            Directories.CreateSymbolicLink(srcDir.FullName, dstDirPath)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified directory.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcDirPath">
        ''' The source directory path.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(ByVal srcDirPath As String, ByVal dstDirPath As DirectoryInfo)

            Directories.CreateSymbolicLink(srcDirPath, dstDirPath.FullName)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified directory.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcDirPath">
        ''' The source directory path.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory path where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(ByVal srcDirPath As String, ByVal dstDirPath As String)

            Dim dstDir As New DirectoryInfo(Path.Combine(dstDirPath, Path.GetFileName(srcDirPath)))
            If (dstDir.Exists) Then
                Throw New IOException(message:=String.Format("Target directory '{0}' already exists.", dstDir.FullName))
                Exit Sub
            End If

            Dim win32Err As Integer
            Dim success As Boolean = NativeMethods.CreateSymbolicLink(dstDir.FullName, srcDirPath, SymbolicLinkFlags.Directory)
            win32Err = Marshal.GetLastWin32Error()
            If Not (success) Then
                Throw New Win32Exception([error]:=win32Err)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified directory is a symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="directory">
        ''' The directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified directory is a symbolic link; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsSymbolicLink(ByVal directory As DirectoryInfo) As Boolean

            Return Directories.IsSymbolicLink(directory.FullName)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified directory is a symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The full directory path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified directory is a symbolic link; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="DirectoryNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsSymbolicLink(ByVal dirPath As String) As Boolean

            Return Directories.IsReparsePointOf(dirPath, ReparsePointTags.SymbolicLink)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the target of the specified symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="directory">
        ''' The (symbolic link) directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The target of the specified symbolic link.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetSymbolicLinkTarget(ByVal directory As DirectoryInfo) As DirectoryInfo
            Return New DirectoryInfo(Directories.GetSymbolicLinkTarget(directory.FullName))
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the target of the specified symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The full path of a symbolic link that points to a directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The target of the specified symbolic link.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="DirectoryNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="ArgumentException">
        ''' The specified directory is not a symbolic link. - dirPath
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetSymbolicLinkTarget(ByVal dirPath As String) As String

            If Not Directory.Exists(dirPath) Then
                Throw New DirectoryNotFoundException(dirPath)
            End If

            Dim reparseDataBuffer As ReparseDataBuffer

            Dim handle As SafeFileHandle = NativeMethods.CreateFile(dirPath, FileAccessRights.StandardRightsRead, FileShare.Read, IntPtr.Zero, FileMode.Open, CreateFileFlags.BackupSemantics Or CreateFileFlags.OpenReparsePoint, IntPtr.Zero)
            Dim win32err As Integer = Marshal.GetLastWin32Error()

            Using handle
                If (handle.IsInvalid) Then
                    Throw New Win32Exception(win32err)
                End If

                Dim outBufferSize As Integer = Marshal.SizeOf(GetType(ReparseDataBuffer))
                Dim outBuffer As IntPtr = IntPtr.Zero

                Try
                    outBuffer = Marshal.AllocHGlobal(outBufferSize)
                    Dim bytesReturned As Integer = Nothing
                    Dim success As Boolean = NativeMethods.DeviceIoControl(handle.DangerousGetHandle(), DirectoryManagementControlCodes.GetReparsePoint, IntPtr.Zero, 0, outBuffer, outBufferSize, bytesReturned, IntPtr.Zero)
                    win32err = Marshal.GetLastWin32Error()

                    handle.Close()

                    If Not (success) Then
                        Throw New Win32Exception(win32err)
                    End If

                    reparseDataBuffer = DirectCast(Marshal.PtrToStructure(outBuffer, GetType(ReparseDataBuffer)), ReparseDataBuffer)

                Finally
                    Marshal.FreeHGlobal(outBuffer)

                End Try

            End Using

            If (reparseDataBuffer.ReparseTag <> ReparsePointTags.SymbolicLink) Then
                Throw New ArgumentException("The specified directory is not a symbolic link.", "dirPath")
            Else
                Dim target As String = Encoding.Unicode.GetString(reparseDataBuffer.Buffer, reparseDataBuffer.PrintNameOffset, reparseDataBuffer.PrintNameLength)
                Return target

            End If

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified directory is a mount point.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="directory">
        ''' The directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified directory is a mount point; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsMountPoint(ByVal directory As DirectoryInfo) As Boolean

            Return Directories.IsMountPoint(directory.FullName)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified directory is a mount point.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The full directory path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified directory is a mount point; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="DirectoryNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsMountPoint(ByVal dirPath As String) As Boolean

            Return Directories.IsReparsePointOf(dirPath, ReparsePointTags.MountPoint)

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified reparse point (a directory) 
        ''' is of the specified reparse point tag type.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The full directory path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified reparse point (a directory) 
        ''' is of the specified reparse point tag type; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="DirectoryNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function IsReparsePointOf(ByVal dirPath As String, ByVal tag As ReparsePointTags) As Boolean

            If Not Directory.Exists(dirPath) Then
                Throw New DirectoryNotFoundException(dirPath)
            End If

            Dim reparseDataBuffer As ReparseDataBuffer

            Dim handle As SafeFileHandle = NativeMethods.CreateFile(dirPath, FileAccessRights.StandardRightsRead, FileShare.Read, IntPtr.Zero, FileMode.Open, CreateFileFlags.BackupSemantics Or CreateFileFlags.OpenReparsePoint, IntPtr.Zero)
            Dim win32err As Integer = Marshal.GetLastWin32Error()

            Using handle
                If (handle.IsInvalid) Then
                    Throw New Win32Exception(win32err)
                End If

                Dim outBufferSize As Integer = Marshal.SizeOf(GetType(ReparseDataBuffer))
                Dim outBuffer As IntPtr = IntPtr.Zero

                Try
                    outBuffer = Marshal.AllocHGlobal(outBufferSize)
                    Dim bytesReturned As Integer = Nothing
                    Dim success As Boolean = NativeMethods.DeviceIoControl(handle.DangerousGetHandle(), DirectoryManagementControlCodes.GetReparsePoint, IntPtr.Zero, 0, outBuffer, outBufferSize, bytesReturned, IntPtr.Zero)
                    win32err = Marshal.GetLastWin32Error()

                    handle.Close()

                    If Not (success) Then
                        If (win32err = Win32ErrorCode.ERROR_NOT_A_REPARSE_POINT) Then
                            Return False
                        Else
                            Throw New Win32Exception(win32err)
                        End If
                    End If

                    reparseDataBuffer = DirectCast(Marshal.PtrToStructure(outBuffer, GetType(ReparseDataBuffer)), ReparseDataBuffer)

                Finally
                    Marshal.FreeHGlobal(outBuffer)

                End Try

            End Using

            Return (reparseDataBuffer.ReparseTag = tag)

        End Function

#End Region

    End Class

End Namespace

#End Region
