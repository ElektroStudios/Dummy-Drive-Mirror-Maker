#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Text

#End Region

#Region " MountPointReparseData "

Namespace DevCase.Core.IO.Tools

    ''' <summary>
    ''' Builds and parses the reparse data buffer of a mount point (<c>IO_REPARSE_TAG_MOUNT_POINT</c>),
    ''' which is used by directory junctions and volume mount points.
    ''' </summary>
    ''' <remarks>
    ''' Layout of the buffer (<c>REPARSE_DATA_BUFFER.MountPointReparseBuffer</c>):
    ''' <code>
    ''' Offset  Size  Field
    ''' 0       4     ReparseTag
    ''' 4       2     ReparseDataLength (size of the data that follows the 8-byte header)
    ''' 6       2     Reserved
    ''' 8       2     SubstituteNameOffset (in bytes, relative to PathBuffer)
    ''' 10      2     SubstituteNameLength (in bytes, without null terminator)
    ''' 12      2     PrintNameOffset
    ''' 14      2     PrintNameLength
    ''' 16      ...   PathBuffer (Unicode)
    ''' </code>
    ''' <see href="https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/ns-ntifs-_reparse_data_buffer"/>
    ''' </remarks>
    Friend NotInheritable Class MountPointReparseData

#Region " Constants "

        ''' <summary>
        ''' The reparse tag of mount points (directory junctions and volume mount points).
        ''' </summary>
        Friend Const MountPointTag As UInteger = &HA0000003UI

        ''' <summary>
        ''' The NT namespace prefix of the substitute name of a mount point.
        ''' </summary>
        Friend Const NtPathPrefix As String = "\??\"

        ''' <summary>
        ''' The maximum size of a reparse data buffer (<c>MAXIMUM_REPARSE_DATA_BUFFER_SIZE</c>).
        ''' </summary>
        Friend Const MaximumBufferSize As Integer = 16 * 1024

        ''' <summary>
        ''' The size of the fixed part of the buffer, before the path buffer.
        ''' </summary>
        Friend Const PathBufferOffset As Integer = 16

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the substitute name (the NT path of the target, e.g. <c>\??\C:\Target</c>).
        ''' </summary>
        Friend ReadOnly Property SubstituteName As String

        ''' <summary>
        ''' Gets the print name (the user-friendly path of the target, e.g. <c>C:\Target</c>). It can be empty.
        ''' </summary>
        Friend ReadOnly Property PrintName As String

        ''' <summary>
        ''' Gets the target path in Win32 format:
        ''' a regular path for directory junctions (e.g. <c>C:\Target</c>),
        ''' or a volume GUID path for volume mount points (e.g. <c>\\?\Volume{GUID}\</c>).
        ''' </summary>
        Friend ReadOnly Property TargetPath As String
            Get
                If Me.IsVolumeMountPoint Then
                    Return $"\\?\{Me.SubstituteName.Substring(MountPointReparseData.NtPathPrefix.Length)}"
                End If

                If Me.SubstituteName.StartsWith(MountPointReparseData.NtPathPrefix, StringComparison.Ordinal) Then
                    Return Me.SubstituteName.Substring(MountPointReparseData.NtPathPrefix.Length)
                End If

                Return Me.SubstituteName
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this is a volume mount point (the target is a volume GUID path)
        ''' instead of a directory junction.
        ''' </summary>
        Friend ReadOnly Property IsVolumeMountPoint As Boolean
            Get
                Return Me.SubstituteName.StartsWith($"{MountPointReparseData.NtPathPrefix}Volume{{", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MountPointReparseData"/> class.
        ''' </summary>
        ''' <param name="substituteName">
        ''' The substitute name.
        ''' </param>
        '''
        ''' <param name="printName">
        ''' The print name.
        ''' </param>
        Private Sub New(substituteName As String, printName As String)
            Me.SubstituteName = substituteName
            Me.PrintName = printName
        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Builds the reparse data buffer of a directory junction that points to the specified target directory.
        ''' </summary>
        ''' <param name="targetPath">
        ''' The fully qualified local path of the target directory (e.g. <c>C:\Target</c>).
        ''' </param>
        ''' <returns>
        ''' The reparse data buffer.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="targetPath"/> is null, empty or white-spaced.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentException">
        ''' <paramref name="targetPath"/> is not a fully qualified local path
        ''' (junctions cannot point to relative or network paths), or it is too long.
        ''' </exception>
        Friend Shared Function BuildJunctionBuffer(targetPath As String) As Byte()

            If String.IsNullOrWhiteSpace(targetPath) Then
                Throw New ArgumentNullException(NameOf(targetPath), "Value is null, empty, or white-spaced.")
            End If

            Dim printName As String = targetPath.Replace("/"c, "\"c)
            If printName.StartsWith("\\?\", StringComparison.Ordinal) Then
                printName = printName.Substring(4)
            End If

            Dim isFullyQualified As Boolean = (printName.Length >= 3) AndAlso Char.IsLetter(printName(0)) AndAlso
                                              (printName(1) = ":"c) AndAlso (printName(2) = "\"c)
            If Not isFullyQualified Then
                Throw New ArgumentException($"A junction target must be a fully qualified local path: '{targetPath}'", NameOf(targetPath))
            End If

            ' Keep the trailing separator only for a volume root (e.g. "C:\").
            If printName.Length > 3 Then
                printName = printName.TrimEnd("\"c)
            End If

            Dim substituteName As String = $"{MountPointReparseData.NtPathPrefix}{printName}"
            Dim substituteBytes As Byte() = Encoding.Unicode.GetBytes(substituteName)
            Dim printBytes As Byte() = Encoding.Unicode.GetBytes(printName)

            ' PathBuffer: SubstituteName + NUL + PrintName + NUL.
            Dim pathBufferLength As Integer = substituteBytes.Length + 2 + printBytes.Length + 2
            Dim reparseDataLength As Integer = 8 + pathBufferLength
            Dim totalLength As Integer = 8 + reparseDataLength

            If totalLength > MountPointReparseData.MaximumBufferSize Then
                Throw New ArgumentException("The junction target path is too long.", NameOf(targetPath))
            End If

            Dim buffer(totalLength - 1) As Byte
            Using stream As New MemoryStream(buffer), writer As New BinaryWriter(stream)
                writer.Write(MountPointReparseData.MountPointTag)
                writer.Write(CUShort(reparseDataLength))
                writer.Write(CUShort(0))                              ' Reserved.
                writer.Write(CUShort(0))                              ' SubstituteNameOffset.
                writer.Write(CUShort(substituteBytes.Length))         ' SubstituteNameLength.
                writer.Write(CUShort(substituteBytes.Length + 2))     ' PrintNameOffset.
                writer.Write(CUShort(printBytes.Length))              ' PrintNameLength.
                writer.Write(substituteBytes)
                writer.Write(CUShort(0))                              ' NUL.
                writer.Write(printBytes)
                writer.Write(CUShort(0))                              ' NUL.
            End Using

            Return buffer

        End Function

        ''' <summary>
        ''' Parses the reparse data buffer of a mount point.
        ''' </summary>
        ''' <param name="buffer">
        ''' The buffer returned by <c>FSCTL_GET_REPARSE_POINT</c>.
        ''' </param>
        '''
        ''' <param name="length">
        ''' The number of valid bytes in the buffer.
        ''' </param>
        ''' <returns>
        ''' The parsed <see cref="MountPointReparseData"/>.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="buffer"/> is <see langword="Nothing"/>.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' <paramref name="length"/> is out of the bounds of the buffer.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentException">
        ''' The buffer does not contain mount point reparse data, or it is malformed.
        ''' </exception>
        Friend Shared Function Parse(buffer As Byte(), length As Integer) As MountPointReparseData

            If buffer Is Nothing Then
                Throw New ArgumentNullException(NameOf(buffer))
            End If

            If (length < 0) OrElse (length > buffer.Length) Then
                Throw New ArgumentOutOfRangeException(NameOf(length), length, "The length is out of the bounds of the buffer.")
            End If

            If length < MountPointReparseData.PathBufferOffset Then
                Throw New ArgumentException("The reparse data buffer is too small.", NameOf(buffer))
            End If

            Dim tag As UInteger = BitConverter.ToUInt32(buffer, 0)
            If tag <> MountPointReparseData.MountPointTag Then
                Throw New ArgumentException($"The reparse point is not a mount point (tag: 0x{tag:X8}).", NameOf(buffer))
            End If

            Dim substituteOffset As Integer = BitConverter.ToUInt16(buffer, 8)
            Dim substituteLength As Integer = BitConverter.ToUInt16(buffer, 10)
            Dim printOffset As Integer = BitConverter.ToUInt16(buffer, 12)
            Dim printLength As Integer = BitConverter.ToUInt16(buffer, 14)

            Dim pathBufferLength As Integer = length - MountPointReparseData.PathBufferOffset
            If (substituteOffset + substituteLength > pathBufferLength) OrElse (printOffset + printLength > pathBufferLength) Then
                Throw New ArgumentException("The reparse data buffer is malformed.", NameOf(buffer))
            End If

            Dim substituteName As String = Encoding.Unicode.GetString(buffer, MountPointReparseData.PathBufferOffset + substituteOffset, substituteLength)
            Dim printName As String = Encoding.Unicode.GetString(buffer, MountPointReparseData.PathBufferOffset + printOffset, printLength)

            If String.IsNullOrEmpty(substituteName) Then
                Throw New ArgumentException("The reparse data buffer has an empty substitute name.", NameOf(buffer))
            End If

            Return New MountPointReparseData(substituteName, printName)

        End Function

#End Region

    End Class

End Namespace

#End Region
