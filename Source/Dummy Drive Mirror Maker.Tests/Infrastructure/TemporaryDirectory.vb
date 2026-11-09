#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Threading

#End Region

#Region " TemporaryDirectory "

Namespace Infrastructure

    ''' <summary>
    ''' Represents a uniquely named directory created inside the user's temporary folder,
    ''' which is deleted (including all its contents) when the instance is disposed.
    ''' </summary>
    Friend NotInheritable Class TemporaryDirectory : Implements IDisposable

#Region " Properties "

        ''' <summary>
        ''' Gets the full path of the temporary directory.
        ''' </summary>
        Friend ReadOnly Property FullPath As String

        ''' <summary>
        ''' Gets a <see cref="DirectoryInfo"/> object that represents the temporary directory.
        ''' </summary>
        Friend ReadOnly Property Info As DirectoryInfo
            Get
                Return New DirectoryInfo(Me.FullPath)
            End Get
        End Property

#End Region

#Region " Fields "

        ''' <summary>
        ''' Flag to detect redundant calls when disposing.
        ''' </summary>
        Private isDisposed As Boolean

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TemporaryDirectory"/> class,
        ''' creating a new empty directory in the user's temporary folder.
        ''' </summary>
        Friend Sub New()
            Me.FullPath = Path.Combine(Path.GetTempPath(), $"DDMM.Tests_{Guid.NewGuid():N}")
            Directory.CreateDirectory(Me.FullPath)
        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Gets the full path of the specified relative path combined with the temporary directory path.
        ''' </summary>
        ''' <param name="relativePath">
        ''' The relative path.
        ''' </param>
        ''' <returns>
        ''' The full path.
        ''' </returns>
        Friend Function Combine(relativePath As String) As String
            Return Path.Combine(Me.FullPath, relativePath)
        End Function

        ''' <summary>
        ''' Creates the specified sub-directory (and any intermediate directories).
        ''' </summary>
        ''' <param name="relativePath">
        ''' The relative path of the sub-directory.
        ''' </param>
        ''' <returns>
        ''' The full path of the created directory.
        ''' </returns>
        Friend Function CreateDirectory(relativePath As String) As String
            Dim fullPath As String = Me.Combine(relativePath)
            Directory.CreateDirectory(fullPath)
            Return fullPath
        End Function

        ''' <summary>
        ''' Creates the specified file (and any intermediate directories) with the specified text content.
        ''' </summary>
        ''' <param name="relativePath">
        ''' The relative path of the file.
        ''' </param>
        '''
        ''' <param name="content">
        ''' The text content of the file.
        ''' </param>
        ''' <returns>
        ''' The full path of the created file.
        ''' </returns>
        Friend Function CreateFile(relativePath As String, Optional content As String = "content") As String
            Dim fullPath As String = Me.Combine(relativePath)
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath))
            File.WriteAllText(fullPath, content)
            Return fullPath
        End Function

#End Region

#Region " IDisposable Implementation "

        ''' <summary>
        ''' Deletes the temporary directory and all its contents.
        ''' <para></para>
        ''' Symbolic links and junctions are removed without following them, so their targets are never deleted.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If Me.isDisposed Then
                Exit Sub
            End If
            Me.isDisposed = True

            If Not Directory.Exists(Me.FullPath) Then
                Exit Sub
            End If

            ' Iterative (non-recursive) deletion, deepest directories first.
            Dim pending As New Stack(Of String)
            Dim ordered As New List(Of String)
            pending.Push(Me.FullPath)
            While pending.Count > 0
                Dim current As String = pending.Pop()
                ordered.Add(current)

                Dim currentInfo As New DirectoryInfo(current)
                If currentInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) Then
                    Continue While ' Never enumerate the contents of a link.
                End If

                For Each filePath As String In Directory.EnumerateFiles(current)
                    Thread.CurrentThread.Join(0)
                    TemporaryDirectory.ClearReadOnlyAttribute(filePath)
                    File.Delete(filePath)
                Next filePath

                For Each dirPath As String In Directory.EnumerateDirectories(current)
                    Thread.CurrentThread.Join(0)
                    pending.Push(dirPath)
                Next dirPath
            End While

            For index As Integer = ordered.Count - 1 To 0 Step -1
                TemporaryDirectory.ClearReadOnlyAttribute(ordered(index))
                Directory.Delete(ordered(index), recursive:=False)
            Next index

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Removes the <see cref="FileAttributes.ReadOnly"/> attribute of the specified file or directory, if present.
        ''' <para></para>
        ''' The attributes are only modified when needed, because modifying the attributes
        ''' of a dangling symbolic link fails (the target does not exist).
        ''' </summary>
        ''' <param name="fileSystemPath">
        ''' The path of the file or directory.
        ''' </param>
        Private Shared Sub ClearReadOnlyAttribute(fileSystemPath As String)

            Dim attributes As FileAttributes = File.GetAttributes(fileSystemPath)
            If attributes.HasFlag(FileAttributes.ReadOnly) Then
                File.SetAttributes(fileSystemPath, attributes And Not FileAttributes.ReadOnly)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
