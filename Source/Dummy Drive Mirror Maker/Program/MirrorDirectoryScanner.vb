#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Security
Imports System.Threading

Imports DevCase.Core.IO.Tools

#End Region

#Region " MirrorDirectoryScanner "

''' <summary>
''' Lists the directories to process in a mirror operation.
''' <para></para>
''' Unlike a plain recursive search, it never descends into directories whose contents will not be mirrored
''' (symbolic links, junctions, mount points and excluded directories).
''' Otherwise, the contents of the link targets would be counted (and reported as ignored)
''' although they are not part of the source tree.
''' </summary>
Friend NotInheritable Class MirrorDirectoryScanner

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="MirrorDirectoryScanner"/> class from being created.
    ''' </summary>
    <DebuggerNonUserCode>
    Private Sub New()
    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Lists the root directory and its subdirectories (iteratively, without recursion),
    ''' descending only into the directories accepted by <paramref name="shouldDescend"/>.
    ''' <para></para>
    ''' The root directory is always the first item and its contents are always listed.
    ''' A directory rejected by <paramref name="shouldDescend"/> is included in the result, but its subdirectories are not.
    ''' </summary>
    ''' <param name="root">
    ''' The root directory.
    ''' </param>
    '''
    ''' <param name="shouldDescend">
    ''' A function that determines whether the subdirectories of a (non-root) directory must be listed.
    ''' </param>
    '''
    ''' <param name="onListingError">
    ''' An action that is invoked when the subdirectories of a directory cannot be listed
    ''' (access denied, I/O error...). The scan continues with the next directory.
    ''' </param>
    ''' <returns>
    ''' The directories, sorted by path (every directory appears after its parent directory).
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' Any argument is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="DirectoryNotFoundException">
    ''' The root directory does not exist.
    ''' </exception>
    Friend Shared Function ListDirectories(root As DirectoryInfo,
                                           shouldDescend As Func(Of DirectoryInfo, Boolean),
                                           onListingError As Action(Of DirectoryInfo, Exception)) As List(Of DirectoryInfo)

        If root Is Nothing Then
            Throw New ArgumentNullException(NameOf(root))
        End If

        If shouldDescend Is Nothing Then
            Throw New ArgumentNullException(NameOf(shouldDescend))
        End If

        If onListingError Is Nothing Then
            Throw New ArgumentNullException(NameOf(onListingError))
        End If

        If Not root.Exists Then
            Throw New DirectoryNotFoundException($"Directory not found: '{root.FullName}'")
        End If

        Dim subdirectories As New List(Of DirectoryInfo)
        Dim pending As New Stack(Of DirectoryInfo)
        pending.Push(root)

        While pending.Count > 0
            Dim current As DirectoryInfo = pending.Pop()

            Dim children As DirectoryInfo()
            Try
                children = current.GetDirectories()

            Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) OrElse
                                       (TypeOf ex Is SecurityException) OrElse
                                       (TypeOf ex Is IOException)
                onListingError(current, ex)
                Continue While

            End Try

            For Each child As DirectoryInfo In children
                Thread.CurrentThread.Join(0)
                subdirectories.Add(child)
                If shouldDescend(child) Then
                    pending.Push(child)
                End If
            Next child
        End While

        Dim result As New List(Of DirectoryInfo)(subdirectories.Count + 1) From {root}
        result.AddRange(subdirectories.OrderBy(Function(di As DirectoryInfo) di.FullName, StringComparer.OrdinalIgnoreCase))
        Return result

    End Function

    ''' <summary>
    ''' Determines whether the contents of the specified directory must be listed in a mirror operation
    ''' with the specified hidden items option.
    ''' </summary>
    ''' <param name="directory">
    ''' The directory (not the root directory of the mirror operation).
    ''' </param>
    '''
    ''' <param name="mirrorHiddenItems">
    ''' A value indicating whether hidden items are mirrored.
    ''' </param>
    ''' <returns>
    ''' <see langword="False"/> for symbolic links, junctions, mount points, system directories
    ''' and hidden directories (when they are not mirrored); otherwise, <see langword="True"/>.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="directory"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Shared Function ShouldDescendInto(directory As DirectoryInfo, mirrorHiddenItems As Boolean) As Boolean

        If directory Is Nothing Then
            Throw New ArgumentNullException(NameOf(directory))
        End If

        Dim attributes As FileAttributes = directory.Attributes

        ' The contents of a link are never mirrored as real directories and files.
        If attributes.HasFlag(FileAttributes.ReparsePoint) Then
            Return False
        End If

        If MirrorRules.IsExcludedSystemDirectory(directory.FullName) Then
            Return False
        End If

        If attributes.HasFlag(FileAttributes.Hidden) AndAlso Not mirrorHiddenItems Then
            Return False
        End If

        Return True

    End Function

#End Region

    ''' <summary>
    ''' Determines whether a listed subdirectory must be excluded from the mirror operation, 
    ''' due to the options chosen by the user (hidden directories, directory junctions and symbolic links) 
    ''' or by design (system directories and volume mount points).
    ''' </summary>
    ''' <param name="directory">
    ''' The directory (never the source directory itself, which is always mirrored).
    ''' </param>
    ''' 
    ''' <param name="mirrorHiddenItems">
    ''' A value indicating whether hidden items are mirrored.
    ''' </param>
    ''' 
    ''' <param name="mirrorJunctions">
    ''' A value indicating whether directory junctions are mirrored.
    ''' </param>
    ''' 
    ''' <param name="mirrorSymbolicLinks">
    ''' A value indicating whether symbolic links are mirrored.
    ''' </param>
    ''' <returns>
    ''' The reason why the directory is excluded, or <see langword="Nothing"/> if the directory must be mirrored.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="directory"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Shared Function GetExclusionReason(directory As DirectoryInfo,
                                              mirrorHiddenItems As Boolean,
                                              mirrorJunctions As Boolean,
                                              mirrorSymbolicLinks As Boolean) As MirrorSkipReason?

        If directory Is Nothing Then
            Throw New ArgumentNullException(NameOf(directory))
        End If

        If MirrorRules.IsExcludedSystemDirectory(directory.FullName) Then
            Return MirrorSkipReason.SystemDirectory
        End If

        If Not mirrorHiddenItems AndAlso directory.Attributes.HasFlag(FileAttributes.Hidden) Then
            Return MirrorSkipReason.HiddenItem
        End If

        If directory.Attributes.HasFlag(FileAttributes.ReparsePoint) Then
            If Directories.IsMountPoint(directory) Then
                If Not Directories.IsJunction(directory.FullName) Then
                    Return MirrorSkipReason.VolumeMountPoint
                End If
                If Not mirrorJunctions Then
                    Return MirrorSkipReason.JunctionNotMirrored
                End If

            ElseIf Directories.IsSymbolicLink(directory) AndAlso Not mirrorSymbolicLinks Then
                Return MirrorSkipReason.SymbolicLinkNotMirrored

            End If
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Selects the directories that will be processed by the mirror operation: 
    ''' the source directory itself (always the first listed directory) 
    ''' and the subdirectories that are not excluded (see <see cref="MirrorDirectoryScanner.GetExclusionReason"/>).
    ''' <para></para>
    ''' The excluded directories are not processed nor counted in the progress, 
    ''' but they are notified through <paramref name="onExcluded"/> so they can be registered in the report.
    ''' </summary>
    ''' <param name="listedDirectories">
    ''' The listed directories (see <see cref="MirrorDirectoryScanner.ListDirectories"/>), with the source directory first.
    ''' </param>
    ''' 
    ''' <param name="mirrorHiddenItems">
    ''' A value indicating whether hidden items are mirrored.
    ''' </param>
    ''' 
    ''' <param name="mirrorJunctions">
    ''' A value indicating whether directory junctions are mirrored.
    ''' </param>
    ''' 
    ''' <param name="mirrorSymbolicLinks">
    ''' A value indicating whether symbolic links are mirrored.
    ''' </param>
    ''' 
    ''' <param name="onExcluded">
    ''' The action invoked for every excluded directory, with the reason of the exclusion.
    ''' </param>
    ''' <returns>
    ''' The directories to process, with the source directory first.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="listedDirectories"/> or <paramref name="onExcluded"/> is <see langword="Nothing"/>.
    ''' </exception>
    ''' 
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="listedDirectories"/> is empty.
    ''' </exception>
    Friend Shared Function SelectDirectoriesToMirror(listedDirectories As IList(Of DirectoryInfo),
                                                     mirrorHiddenItems As Boolean,
                                                     mirrorJunctions As Boolean,
                                                     mirrorSymbolicLinks As Boolean,
                                                     onExcluded As Action(Of DirectoryInfo, MirrorSkipReason)) As List(Of DirectoryInfo)

        If listedDirectories Is Nothing Then
            Throw New ArgumentNullException(NameOf(listedDirectories))
        End If
        If onExcluded Is Nothing Then
            Throw New ArgumentNullException(NameOf(onExcluded))
        End If
        If listedDirectories.Count = 0 Then
            Throw New ArgumentException("The list must contain at least the source directory.", NameOf(listedDirectories))
        End If

        ' The source directory was explicitly selected by the user, so it is always mirrored.
        Dim result As New List(Of DirectoryInfo)(listedDirectories.Count) From {listedDirectories(0)}

        For index As Integer = 1 To listedDirectories.Count - 1
            Thread.CurrentThread.Join(0)

            Dim directory As DirectoryInfo = listedDirectories(index)
            Dim reason As MirrorSkipReason? = MirrorDirectoryScanner.GetExclusionReason(directory, mirrorHiddenItems, mirrorJunctions, mirrorSymbolicLinks)
            If reason.HasValue Then
                onExcluded(directory, reason.Value)
            Else
                result.Add(directory)
            End If
        Next index

        Return result

    End Function

End Class

#End Region
