




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

#Region " Files "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Files

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcFile">
        ''' The source file.
        ''' </param>
        ''' 
        ''' <param name="dstDir">
        ''' The destination directory where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(srcFile As FileInfo, dstDir As DirectoryInfo)

            Files.CreateSymbolicLink(srcFile.FullName, dstDir.FullName)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcFile">
        ''' The source file.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory path where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(srcFile As FileInfo, dstDirPath As String)

            Files.CreateSymbolicLink(srcFile.FullName, dstDirPath)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcFilePath">
        ''' The source file path.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(srcFilePath As String, dstDirPath As DirectoryInfo)

            Files.CreateSymbolicLink(srcFilePath, dstDirPath.FullName)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a symbolic link of the specified file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="srcFilePath">
        ''' The source file path.
        ''' </param>
        ''' 
        ''' <param name="dstDirPath">
        ''' The destination directory path where to create the symbolic link.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="FileNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="IOException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateSymbolicLink(srcFilePath As String, dstDirPath As String)

            Dim dstFile As New FileInfo(Path.Combine(dstDirPath, Path.GetFileName(srcFilePath)))
            If (dstFile.Exists) Then
                Throw New IOException(message:=String.Format("Destination file '{0}' already exists.", dstFile.FullName))
            End If

            Dim win32Err As Integer
            Dim success As Boolean = NativeMethods.CreateSymbolicLink(dstFile.FullName, srcFilePath, SymbolicLinkFlags.File)
            win32Err = Marshal.GetLastWin32Error()
            If Not (success) Then
                Throw New Win32Exception([error]:=win32Err)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified file is a symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="file">
        ''' The file.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified file is a symbolic link; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsSymbolicLink(file As FileInfo) As Boolean

            Return Files.IsSymbolicLink(file.FullName)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether whether the specified file is a symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filePath">
        ''' The full file path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified file is a symbolic link; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="FileNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function IsSymbolicLink(filePath As String) As Boolean

            Return Files.IsReparsePointOf(filePath, ReparsePointTags.SymbolicLink)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the target of the specified symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="file">
        ''' The (symbolic link) file.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The target of the specified symbolic link.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetSymbolicLinkTarget(file As FileInfo) As FileInfo

            Return New FileInfo(Files.GetSymbolicLinkTarget(file.FullName))

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the target of the specified symbolic link.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filePath">
        ''' The full path of a symbolic link that points to a file.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The target of the specified symbolic link.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="FileNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="ArgumentException">
        ''' The specified file is not a symbolic link. - filePath
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetSymbolicLinkTarget(filePath As String) As String

            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException(filePath)
            End If

            Dim reparseDataBuffer As ReparseDataBuffer

            Dim handle As SafeFileHandle = NativeMethods.CreateFile(filePath, FileAccessRights.StandardRightsRead, FileShare.Read, IntPtr.Zero, FileMode.Open, CreateFileFlags.BackupSemantics Or CreateFileFlags.OpenReparsePoint, IntPtr.Zero)
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
                Throw New ArgumentException("The specified file is not a symbolic link.", "filePath")
            Else
                Dim target As String = Encoding.Unicode.GetString(reparseDataBuffer.Buffer, reparseDataBuffer.PrintNameOffset, reparseDataBuffer.PrintNameLength)
                Return target

            End If

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified reparse point (a file) 
        ''' is of the specified reparse point tag type.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filePath">
        ''' The full file path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the specified reparse point (a file) 
        ''' is of the specified reparse point tag type; 
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="FileNotFoundException">
        ''' </exception>
        ''' 
        ''' <exception cref="Win32Exception">
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function IsReparsePointOf(filePath As String, tag As ReparsePointTags) As Boolean

            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException(filePath)
            End If

            Dim reparseDataBuffer As ReparseDataBuffer

            Dim handle As SafeFileHandle = NativeMethods.CreateFile(filePath, FileAccessRights.StandardRightsRead, FileShare.Read, IntPtr.Zero, FileMode.Open, CreateFileFlags.BackupSemantics Or CreateFileFlags.OpenReparsePoint, IntPtr.Zero)
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
