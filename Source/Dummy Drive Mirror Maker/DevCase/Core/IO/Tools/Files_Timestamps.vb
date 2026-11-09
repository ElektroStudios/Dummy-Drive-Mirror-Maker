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

#Region " Files (Timestamps) "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Files

#Region " Constants "

        ''' <summary>The <c>FILE_WRITE_ATTRIBUTES</c> access right (also required to change the timestamps).</summary>
        Private Const FileWriteAttributes As UInteger = &H100UI

        ''' <summary>The <c>FILE_FLAG_BACKUP_SEMANTICS</c> flag (required to open a directory).</summary>
        Private Const BackupSemantics As UInteger = &H2000000UI

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Sets the creation, last access and last write times (UTC) of a file or a directory.
        ''' <para></para>
        ''' Unlike <see cref="File.SetCreationTime"/> and <see cref="Directory.SetCreationTime"/> (and their
        ''' <see cref="FileSystemInfo"/> counterparts) of .NET Framework, the file or directory is opened only once,
        ''' with the minimum access right (<c>FILE_WRITE_ATTRIBUTES</c>) and sharing read, write and delete access.
        ''' <para></para>
        ''' The .NET Framework methods open a directory with write access but without sharing read access,
        ''' so they fail with a sharing violation while any other process (e.g. Windows Explorer showing the directory)
        ''' has the directory open.
        ''' </summary>
        ''' <param name="path">
        ''' The path of the file or directory.
        ''' </param>
        '''
        ''' <param name="creationTimeUtc">
        ''' The creation time, in UTC.
        ''' </param>
        '''
        ''' <param name="lastAccessTimeUtc">
        ''' The last access time, in UTC.
        ''' </param>
        '''
        ''' <param name="lastWriteTimeUtc">
        ''' The last write time, in UTC.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="path"/> is <see langword="Nothing"/>, empty or white-spaced.
        ''' </exception>
        '''
        ''' <exception cref="FileNotFoundException">
        ''' Thrown when the file or directory does not exist.
        ''' </exception>
        '''
        ''' <exception cref="UnauthorizedAccessException">
        ''' Thrown when the access to the file or directory is denied.
        ''' </exception>
        '''
        ''' <exception cref="IOException">
        ''' Thrown when the file or directory cannot be opened or its timestamps cannot be changed.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Sub SetTimestampsUtc(path As String, creationTimeUtc As Date, lastAccessTimeUtc As Date, lastWriteTimeUtc As Date)

            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
            End If

            Dim creation As Long = creationTimeUtc.ToFileTimeUtc()
            Dim lastAccess As Long = lastAccessTimeUtc.ToFileTimeUtc()
            Dim lastWrite As Long = lastWriteTimeUtc.ToFileTimeUtc()

            Using handle As SafeFileHandle = NativeMethods.CreateFileW(path, Files.FileWriteAttributes, Files.ShareAll, IntPtr.Zero,
                                                                       Files.OpenExisting, Files.BackupSemantics, IntPtr.Zero)
                If handle.IsInvalid Then
                    Throw Files.CreateTimestampException(Marshal.GetLastWin32Error(), path)
                End If

                If Not NativeMethods.SetFileTime(handle, creation, lastAccess, lastWrite) Then
                    Throw Files.CreateTimestampException(Marshal.GetLastWin32Error(), path)
                End If
            End Using

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Creates the exception for a Win32 error raised while changing the timestamps of a file or directory.
        ''' </summary>
        ''' <param name="win32Error">
        ''' The Win32 error code.
        ''' </param>
        '''
        ''' <param name="path">
        ''' The path of the file or directory.
        ''' </param>
        ''' <returns>
        ''' The exception.
        ''' </returns>
        Private Shared Function CreateTimestampException(win32Error As Integer, path As String) As Exception

            Dim inner As New Win32Exception(win32Error)
            Dim message As String = $"{inner.Message} '{path}'"

            Select Case win32Error
                Case 2, 3 ' ERROR_FILE_NOT_FOUND, ERROR_PATH_NOT_FOUND
                    Return New FileNotFoundException(message, path, inner)
                Case 5 ' ERROR_ACCESS_DENIED
                    Return New UnauthorizedAccessException(message, inner)
                Case Else
                    Return New IOException(message, inner)
            End Select

        End Function

#End Region

    End Class

End Namespace

#End Region
