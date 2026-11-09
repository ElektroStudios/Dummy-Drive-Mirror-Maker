#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

Imports DevCase.Core.IO.Tools
Imports DevCase.Interop.Win32.Enums

#End Region

#Region " LinkFactory "

Namespace Infrastructure

    ''' <summary>
    ''' Helper methods to create symbolic links and junctions (mount points) for testing purposes.
    ''' </summary>
    Friend NotInheritable Class LinkFactory

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="LinkFactory"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Creates a directory junction (a mount point reparse point) using the <c>mklink /J</c> command,
        ''' which does not require elevated privileges.
        ''' </summary>
        ''' <param name="junctionPath">
        ''' The full path of the junction to create.
        ''' </param>
        '''
        ''' <param name="targetPath">
        ''' The full path of the existing target directory.
        ''' </param>
        ''' <exception cref="InvalidOperationException">
        ''' The junction could not be created.
        ''' </exception>
        Friend Shared Sub CreateJunction(junctionPath As String, targetPath As String)

            Dim startInfo As New ProcessStartInfo("cmd.exe", $"/C mklink /J ""{junctionPath}"" ""{targetPath}""") With {
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True
            }

            Using process As Process = Process.Start(startInfo)
                Dim output As String = process.StandardOutput.ReadToEnd()
                Dim errorOutput As String = process.StandardError.ReadToEnd()
                process.WaitForExit()

                If (process.ExitCode <> 0) OrElse Not Directory.Exists(junctionPath) Then
                    Throw New InvalidOperationException($"Failed to create the junction (exit code {process.ExitCode}): {output} {errorOutput}")
                End If
            End Using

        End Sub

        ''' <summary>
        ''' Creates a file or directory symbolic link using <see cref="Files.CreateSymbolicLinkAt"/>
        ''' or <see cref="Directories.CreateSymbolicLinkAt"/>.
        ''' <para></para>
        ''' If the current user does not hold the privilege to create symbolic links
        ''' (not elevated and Windows Developer Mode disabled), the current test is marked as inconclusive.
        ''' </summary>
        ''' <param name="linkPath">
        ''' The full path of the symbolic link to create.
        ''' </param>
        '''
        ''' <param name="targetPath">
        ''' The target path.
        ''' </param>
        '''
        ''' <param name="isDirectory">
        ''' <see langword="True"/> to create a directory symbolic link;
        ''' <see langword="False"/> to create a file symbolic link.
        ''' </param>
        Friend Shared Sub CreateSymbolicLinkOrInconclusive(linkPath As String, targetPath As String, isDirectory As Boolean)

            Try
                If isDirectory Then
                    Directories.CreateSymbolicLinkAt(linkPath, targetPath)
                Else
                    Files.CreateSymbolicLinkAt(linkPath, targetPath)
                End If

            Catch ex As Win32Exception When ex.NativeErrorCode = Win32ErrorCode.ERROR_PRIVILEGE_NOT_HELD
                Assert.Inconclusive("The current user does not hold the privilege to create symbolic links. Run the tests elevated or enable Windows Developer Mode.")

            End Try

        End Sub

#End Region

    End Class

End Namespace

#End Region
