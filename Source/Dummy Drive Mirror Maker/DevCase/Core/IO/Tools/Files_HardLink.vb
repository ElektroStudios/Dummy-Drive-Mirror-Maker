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
Imports DevCase.Interop.Win32.Types

#End Region

#Region " FileLinkInfo "

Namespace DevCase.Core.IO.Tools

    ''' <summary>
    ''' Identifies a file in a volume and its number of hard links.
    ''' <para></para>
    ''' Two paths with the same <see cref="FileLinkInfo.VolumeSerialNumber"/> and
    ''' <see cref="FileLinkInfo.FileIndex"/> are hard links of the same file.
    ''' </summary>
    Public Structure FileLinkInfo : Implements IEquatable(Of FileLinkInfo)

        ''' <summary>Gets the serial number of the volume that contains the file.</summary>
        Public ReadOnly Property VolumeSerialNumber As UInteger

        ''' <summary>Gets the unique identifier of the file in its volume.</summary>
        Public ReadOnly Property FileIndex As ULong

        ''' <summary>Gets the number of hard links (names) of the file.</summary>
        Public ReadOnly Property LinkCount As Integer

        ''' <summary>
        ''' Initializes a new instance of the <see cref="FileLinkInfo"/> structure.
        ''' </summary>
        ''' <param name="volumeSerialNumber">
        ''' The serial number of the volume that contains the file.
        ''' </param>
        '''
        ''' <param name="fileIndex">
        ''' The unique identifier of the file in its volume.
        ''' </param>
        '''
        ''' <param name="linkCount">
        ''' The number of hard links of the file.
        ''' </param>
        Public Sub New(volumeSerialNumber As UInteger, fileIndex As ULong, linkCount As Integer)
            Me.VolumeSerialNumber = volumeSerialNumber
            Me.FileIndex = fileIndex
            Me.LinkCount = linkCount
        End Sub

        ''' <summary>
        ''' Determines whether this instance identifies the same file as another instance
        ''' (the <see cref="FileLinkInfo.LinkCount"/> is not compared).
        ''' </summary>
        ''' <param name="other">
        ''' The other instance.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if both instances identify the same file; otherwise, <see langword="False"/>.
        ''' </returns>
        Public Overloads Function Equals(other As FileLinkInfo) As Boolean Implements IEquatable(Of FileLinkInfo).Equals
            Return (Me.VolumeSerialNumber = other.VolumeSerialNumber) AndAlso (Me.FileIndex = other.FileIndex)
        End Function

        ''' <summary>
        ''' Determines whether this instance identifies the same file as the specified object.
        ''' </summary>
        ''' <param name="obj">
        ''' The object to compare.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="obj"/> is a <see cref="FileLinkInfo"/> that identifies the same file.
        ''' </returns>
        Public Overrides Function Equals(obj As Object) As Boolean
            Return (TypeOf obj Is FileLinkInfo) AndAlso Me.Equals(DirectCast(obj, FileLinkInfo))
        End Function

        ''' <summary>
        ''' Returns the hash code for this instance.
        ''' </summary>
        ''' <returns>
        ''' The hash code.
        ''' </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.VolumeSerialNumber.GetHashCode() Xor Me.FileIndex.GetHashCode()
        End Function

    End Structure

End Namespace

#End Region

#Region " Files (Hard Links) "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Files

#Region " Constants "

        ''' <summary>FILE_READ_ATTRIBUTES access right.</summary>
        Private Const FileReadAttributes As UInteger = &H80UI

        ''' <summary>FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE.</summary>
        Private Const ShareAll As UInteger = &H7UI

        ''' <summary>OPEN_EXISTING creation disposition.</summary>
        Private Const OpenExisting As UInteger = 3UI

        ''' <summary>FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT.</summary>
        Private Const BackupSemanticsOpenReparsePoint As UInteger = &H2000000UI Or &H200000UI

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the unique identifier and the number of hard links of the specified file.
        ''' </summary>
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' Dim info As FileLinkInfo = Files.GetLinkInfo("C:\File.txt")
        ''' Dim hasOtherNames As Boolean = (info.LinkCount > 1)
        ''' </code>
        ''' </example>
        ''' <param name="filePath">
        ''' The file path.
        ''' </param>
        ''' <returns>
        ''' The <see cref="FileLinkInfo"/> of the file.
        ''' </returns>
        ''' <exception cref="FileNotFoundException">
        ''' The file does not exist.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' The native function failed.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Function GetLinkInfo(filePath As String) As FileLinkInfo

            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException("File not found.", filePath)
            End If

            Using handle As SafeFileHandle = NativeMethods.CreateFileW(filePath, Files.FileReadAttributes, Files.ShareAll, IntPtr.Zero,
                                                                       Files.OpenExisting, Files.BackupSemanticsOpenReparsePoint,
                                                                       IntPtr.Zero)
                If handle.IsInvalid Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If

                Dim info As New ByHandleFileInformation()
                If Not NativeMethods.GetFileInformationByHandle(handle, info) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If

                Dim fileIndex As ULong = (CULng(info.FileIndexHigh) << 32) Or CULng(info.FileIndexLow)
                Return New FileLinkInfo(info.VolumeSerialNumber, fileIndex, CInt(info.NumberOfLinks))
            End Using

        End Function

        ''' <summary>
        ''' Creates a hard link (a new name) for an existing file.
        ''' Both paths must be located in the same volume.
        ''' </summary>
        ''' <example> This is a code example.
        ''' <code language="VB.NET">
        ''' Files.CreateHardLink("C:\Other Name.txt", "C:\File.txt")
        ''' </code>
        ''' </example>
        ''' <param name="linkPath">
        ''' The full path of the hard link to create. It must not exist.
        ''' </param>
        '''
        ''' <param name="existingFilePath">
        ''' The full path of the existing file.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="linkPath"/> or <paramref name="existingFilePath"/> is null, empty or white-spaced.
        ''' </exception>
        '''
        ''' <exception cref="FileNotFoundException">
        ''' The existing file does not exist.
        ''' </exception>
        '''
        ''' <exception cref="IOException">
        ''' A file or directory already exists at <paramref name="linkPath"/>.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' The native function failed (e.g. the paths are in different volumes).
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Sub CreateHardLink(linkPath As String, existingFilePath As String)

            If String.IsNullOrWhiteSpace(linkPath) Then
                Throw New ArgumentNullException(NameOf(linkPath), "Value is null, empty, or white-spaced.")
            End If

            If String.IsNullOrWhiteSpace(existingFilePath) Then
                Throw New ArgumentNullException(NameOf(existingFilePath), "Value is null, empty, or white-spaced.")
            End If

            If Not File.Exists(existingFilePath) Then
                Throw New FileNotFoundException("File not found.", existingFilePath)
            End If

            If File.Exists(linkPath) OrElse Directory.Exists(linkPath) Then
                Throw New IOException($"Destination path '{linkPath}' already exists.")
            End If

            If Not NativeMethods.CreateHardLink(linkPath, existingFilePath, IntPtr.Zero) Then
                Throw New Win32Exception(Marshal.GetLastWin32Error())
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
