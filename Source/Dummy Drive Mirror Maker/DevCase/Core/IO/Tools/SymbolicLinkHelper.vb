#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices

Imports DevCase.Interop.Win32
Imports DevCase.Interop.Win32.Enums

#End Region

#Region " SymbolicLinkHelper "

Namespace DevCase.Core.IO.Tools

    ''' <summary>
    ''' Contains the shared low-level logic used to create file and directory symbolic links.
    ''' </summary>
    Friend NotInheritable Class SymbolicLinkHelper

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="SymbolicLinkHelper"/> class from being created.
        ''' </summary>
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Creates a symbolic link at the exact specified path, pointing to the specified target.
        ''' <para></para>
        ''' The link is first created using the <see cref="SymbolicLinkFlags.AllowUnprivilegedCreate"/> flag
        ''' (so it can succeed without elevation when Windows Developer Mode is enabled).
        ''' If the operating system does not support that flag, the operation is retried without it.
        ''' </summary>
        ''' <param name="linkPath">
        ''' The full path of the symbolic link to create.
        ''' </param>
        '''
        ''' <param name="targetPath">
        ''' The target path that the symbolic link will point to.
        ''' </param>
        '''
        ''' <param name="kind">
        ''' Either <see cref="SymbolicLinkFlags.File"/> or <see cref="SymbolicLinkFlags.Directory"/>.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="linkPath"/> or <paramref name="targetPath"/> is null, empty or white-spaced.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentException">
        ''' <paramref name="kind"/> is not <see cref="SymbolicLinkFlags.File"/> nor <see cref="SymbolicLinkFlags.Directory"/>.
        ''' </exception>
        '''
        ''' <exception cref="IOException">
        ''' A file or directory already exists at <paramref name="linkPath"/>.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' The native function failed.
        ''' </exception>
        <DebuggerStepThrough>
        Friend Shared Sub CreateSymbolicLink(linkPath As String, targetPath As String, kind As SymbolicLinkFlags)

            If String.IsNullOrWhiteSpace(linkPath) Then
                Throw New ArgumentNullException(NameOf(linkPath), "Value is null, empty, or white-spaced.")
            End If

            If String.IsNullOrWhiteSpace(targetPath) Then
                Throw New ArgumentNullException(NameOf(targetPath), "Value is null, empty, or white-spaced.")
            End If

            If (kind <> SymbolicLinkFlags.File) AndAlso (kind <> SymbolicLinkFlags.Directory) Then
                Throw New ArgumentException($"Value '{kind}' is not a valid symbolic link kind.", NameOf(kind))
            End If

            If File.Exists(linkPath) OrElse Directory.Exists(linkPath) Then
                Throw New IOException(message:=$"Destination path '{linkPath}' already exists.")
            End If

            Dim success As Boolean =
                NativeMethods.CreateSymbolicLink(linkPath, targetPath, kind Or SymbolicLinkFlags.AllowUnprivilegedCreate)
            Dim win32Err As Integer = Marshal.GetLastWin32Error()

            ' Windows versions prior to Windows 10 build 14972 does not recognize the 'AllowUnprivilegedCreate' flag.
            If Not success AndAlso (win32Err = Win32ErrorCode.ERROR_INVALID_PARAMETER) Then
                success = NativeMethods.CreateSymbolicLink(linkPath, targetPath, kind)
                win32Err = Marshal.GetLastWin32Error()
            End If

            If Not success Then
                Throw New Win32Exception([error]:=win32Err)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
