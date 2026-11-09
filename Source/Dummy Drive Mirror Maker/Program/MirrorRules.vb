#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO

#End Region

#Region " MirrorRules "

''' <summary>
''' Contains the pure (UI-independent) path and progress rules used by a mirror operation.
''' <para></para>
''' All path comparisons follow the Windows file system rules: they are case-insensitive,
''' they ignore trailing directory separators and they ignore the long path prefix (<c>\\?\</c>).
''' </summary>
Friend NotInheritable Class MirrorRules

#Region " Constants "

    ''' <summary>
    ''' The prefix used to specify an extended-length (long) local path.
    ''' </summary>
    Friend Const LongPathPrefix As String = "\\?\"

    ''' <summary>
    ''' The prefix used to specify an extended-length (long) UNC path.
    ''' </summary>
    Friend Const LongUncPathPrefix As String = "\\?\UNC\"

    ''' <summary>
    ''' The path length from which the long path prefix is required by the Win32 functions 
    ''' when the long path support of Windows is disabled.
    ''' <para></para>
    ''' It is the limit of a directory path (<c>MAX_PATH</c> minus 12 characters for a 8.3 file name), 
    ''' which also covers the 260 characters limit of a file path.
    ''' </summary>
    Friend Const LongPathThreshold As Integer = 248

#End Region

#Region " Fields "

    ''' <summary>
    ''' The names of the top-level system directories (located at the root of a volume)
    ''' that are always excluded from a mirror operation, including all of their contents.
    ''' </summary>
    ''' <remarks>
    ''' Recycle bin locations:
    ''' <list type="bullet">
    '''   <item><description>VISTA and later: <c>X:\$Recycle.Bin</c></description></item>
    '''   <item><description>2000 (NTFS), XP (NTFS): <c>X:\RECYCLER</c></description></item>
    '''   <item><description>95, 98, 2000 (FAT32), XP (FAT32): <c>X:\RECYCLED</c></description></item>
    ''' </list>
    ''' The <c>X:\System Volume Information</c> directory has no useful purpose to be mirrored,
    ''' and the user normally has no permission to read it.
    ''' </remarks>
    Friend Shared ReadOnly ExcludedSystemDirectoryNames As IReadOnlyCollection(Of String) =
        Array.AsReadOnly({"$Recycle.Bin", "RECYCLER", "RECYCLED", "System Volume Information"})

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="MirrorRules"/> class from being created.
    ''' </summary>
    <DebuggerNonUserCode>
    Private Sub New()
    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Removes the long path prefix (<c>\\?\</c> or <c>\\?\UNC\</c>) from the specified path, if present.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' Dim value As String = MirrorRules.RemoveLongPathPrefix("\\?\C:\Folder") ' Result: "C:\Folder"
    ''' </code>
    ''' </example>
    ''' <param name="path">
    ''' The path.
    ''' </param>
    ''' <returns>
    ''' The path without the long path prefix.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="path"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Shared Function RemoveLongPathPrefix(path As String) As String

        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If

        If path.StartsWith(MirrorRules.LongUncPathPrefix, StringComparison.OrdinalIgnoreCase) Then
            Return $"\\{path.Substring(MirrorRules.LongUncPathPrefix.Length)}"
        End If

        If path.StartsWith(MirrorRules.LongPathPrefix, StringComparison.Ordinal) Then
            Return path.Substring(MirrorRules.LongPathPrefix.Length)
        End If

        Return path

    End Function

    ''' <summary>
    ''' Adds the long path prefix (<c>\\?\</c> for local paths, <c>\\?\UNC\</c> for network paths)
    ''' to the specified fully qualified path, if not already present.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' Dim value1 As String = MirrorRules.AddLongPathPrefix("C:\Folder")          ' Result: "\\?\C:\Folder"
    ''' Dim value2 As String = MirrorRules.AddLongPathPrefix("\\Server\Share\Dir") ' Result: "\\?\UNC\Server\Share\Dir"
    ''' </code>
    ''' </example>
    ''' <param name="path">
    ''' A fully qualified local path (e.g. <c>C:\Folder</c>) or UNC path (e.g. <c>\\Server\Share\Folder</c>).
    ''' </param>
    ''' <returns>
    ''' The path with the long path prefix.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="path"/> is null, empty or white-spaced.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' <paramref name="path"/> is not a fully qualified path.
    ''' </exception>
    Friend Shared Function AddLongPathPrefix(path As String) As String

        If String.IsNullOrWhiteSpace(path) Then
            Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
        End If

        If path.StartsWith(MirrorRules.LongPathPrefix, StringComparison.Ordinal) Then
            Return path
        End If

        ' The long path prefix disables the path normalization of Windows, so alternate separators must be converted.
        Dim value As String = path.Replace(IO.Path.AltDirectorySeparatorChar, IO.Path.DirectorySeparatorChar)

        If value.StartsWith("\\", StringComparison.Ordinal) Then
            Return $"{MirrorRules.LongUncPathPrefix}{value.Substring(2)}"
        End If

        If Not MirrorRules.IsFullyQualifiedLocalPath(value) Then
            Throw New ArgumentException($"The path is not fully qualified: '{path}'", NameOf(path))
        End If

        Return $"{MirrorRules.LongPathPrefix}{value}"

    End Function

    ''' <summary>
    ''' Adds the long path prefix to the specified fully qualified path only if the path is too long 
    ''' to be used without it (see <see cref="MirrorRules.LongPathThreshold"/>); 
    ''' otherwise, returns the path without the long path prefix.
    ''' <para></para>
    ''' This keeps regular paths readable (e.g. in exception messages), 
    ''' while very long paths still work when the long path support of Windows is disabled.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' Dim value As String = MirrorRules.AddLongPathPrefixIfNeeded("C:\Folder") ' Result: "C:\Folder"
    ''' </code>
    ''' </example>
    ''' <param name="path">
    ''' A fully qualified local or UNC path, with or without the long path prefix.
    ''' </param>
    ''' <returns>
    ''' The path, with the long path prefix only when it is required.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="path"/> is null, empty or white-spaced.
    ''' </exception>
    ''' 
    ''' <exception cref="ArgumentException">
    ''' <paramref name="path"/> is not a fully qualified path.
    ''' </exception>
    Friend Shared Function AddLongPathPrefixIfNeeded(path As String) As String

        If String.IsNullOrWhiteSpace(path) Then
            Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
        End If

        Dim plainPath As String = MirrorRules.RemoveLongPathPrefix(path)
        If plainPath.Length >= MirrorRules.LongPathThreshold Then
            Return MirrorRules.AddLongPathPrefix(plainPath)
        End If

        ' Validates that the path is fully qualified, as the long path version would require.
        MirrorRules.AddLongPathPrefix(plainPath)
        Return plainPath

    End Function

    ''' <summary>
    ''' Normalizes the specified directory path for comparison purposes:
    ''' removes the long path prefix, converts alternate separators (<c>/</c>) to <c>\</c>,
    ''' and removes trailing directory separators (except for a volume root like <c>C:\</c>).
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' Dim value1 As String = MirrorRules.NormalizeDirectoryPath("\\?\C:\Folder\") ' Result: "C:\Folder"
    ''' Dim value2 As String = MirrorRules.NormalizeDirectoryPath("C:\")            ' Result: "C:\"
    ''' </code>
    ''' </example>
    ''' <param name="path">
    ''' A rooted directory path.
    ''' </param>
    ''' <returns>
    ''' The normalized directory path.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="path"/> is null, empty or white-spaced.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' <paramref name="path"/> is not a rooted path.
    ''' </exception>
    Friend Shared Function NormalizeDirectoryPath(path As String) As String

        If String.IsNullOrWhiteSpace(path) Then
            Throw New ArgumentNullException(NameOf(path), "Value is null, empty, or white-spaced.")
        End If

        Dim value As String = MirrorRules.RemoveLongPathPrefix(path).Replace(IO.Path.AltDirectorySeparatorChar, IO.Path.DirectorySeparatorChar)

        If Not IO.Path.IsPathRooted(value) Then
            Throw New ArgumentException($"The path is not rooted: '{path}'", NameOf(path))
        End If

        Dim root As String = IO.Path.GetPathRoot(value)
        Dim trimmed As String = value.TrimEnd(IO.Path.DirectorySeparatorChar)

        ' Preserve the trailing separator of a volume root (e.g. "C:\").
        If trimmed.Length < root.Length Then
            Return root
        End If

        Return trimmed

    End Function

    ''' <summary>
    ''' Determines whether the specified directory paths refer to the same location.
    ''' </summary>
    ''' <param name="path1">
    ''' The first rooted directory path.
    ''' </param>
    '''
    ''' <param name="path2">
    ''' The second rooted directory path.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if both paths are equal (ignoring case, trailing separators and long path prefix);
    ''' otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function ArePathsEqual(path1 As String, path2 As String) As Boolean

        Return String.Equals(MirrorRules.NormalizeDirectoryPath(path1),
                             MirrorRules.NormalizeDirectoryPath(path2),
                             StringComparison.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Determines whether the specified path is located inside (at any depth) the specified ancestor directory.
    ''' <para></para>
    ''' A path is never considered a descendant of itself.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' MirrorRules.IsDescendantOf("C:\Data\Sub", "C:\Data") ' Result: True
    ''' MirrorRules.IsDescendantOf("C:\Data2",    "C:\Data") ' Result: False
    ''' </code>
    ''' </example>
    ''' <param name="path">
    ''' The rooted path to evaluate.
    ''' </param>
    '''
    ''' <param name="ancestorPath">
    ''' The rooted path of the ancestor directory.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if <paramref name="path"/> is a descendant of <paramref name="ancestorPath"/>;
    ''' otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function IsDescendantOf(path As String, ancestorPath As String) As Boolean

        Dim normalizedPath As String = MirrorRules.NormalizeDirectoryPath(path)
        Dim prefix As String = MirrorRules.GetDirectoryPrefix(ancestorPath)

        Return (normalizedPath.Length > prefix.Length) AndAlso
                normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Gets the path of <paramref name="fullPath"/> relative to <paramref name="rootPath"/>.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' MirrorRules.GetRelativePath("C:\Data", "C:\Data\Sub\File.txt") ' Result: "Sub\File.txt"
    ''' MirrorRules.GetRelativePath("C:\Data", "C:\Data")              ' Result: ""
    ''' </code>
    ''' </example>
    ''' <param name="rootPath">
    ''' The rooted path of the root directory.
    ''' </param>
    '''
    ''' <param name="fullPath">
    ''' The rooted path located inside <paramref name="rootPath"/> (or equal to it).
    ''' </param>
    ''' <returns>
    ''' The relative path, or <see cref="String.Empty"/> if both paths are equal.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' <paramref name="fullPath"/> is not located inside <paramref name="rootPath"/>.
    ''' </exception>
    Friend Shared Function GetRelativePath(rootPath As String, fullPath As String) As String

        If MirrorRules.ArePathsEqual(rootPath, fullPath) Then
            Return String.Empty
        End If

        If Not MirrorRules.IsDescendantOf(fullPath, rootPath) Then
            Throw New ArgumentException($"The path '{fullPath}' is not located inside '{rootPath}'.", NameOf(fullPath))
        End If

        Return MirrorRules.NormalizeDirectoryPath(fullPath).Substring(MirrorRules.GetDirectoryPrefix(rootPath).Length)

    End Function

    ''' <summary>
    ''' Builds the full destination path that mirrors the specified source file or directory path.
    ''' <para></para>
    ''' The long path prefix is added only when the resulting path requires it 
    ''' (see <see cref="MirrorRules.AddLongPathPrefixIfNeeded(String)"/>).
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' MirrorRules.BuildDestinationPath("D:\", "E:\Mirror", "D:\Music\Song.mp3") ' Result: "E:\Mirror\Music\Song.mp3"
    ''' </code>
    ''' </example>
    ''' <param name="sourceRootPath">
    ''' The fully qualified path of the source (root) directory.
    ''' </param>
    '''
    ''' <param name="destinationRootPath">
    ''' The fully qualified path of the destination (root) directory.
    ''' </param>
    '''
    ''' <param name="sourcePath">
    ''' The fully qualified source file or directory path, located inside <paramref name="sourceRootPath"/>.
    ''' </param>
    ''' <returns>
    ''' The destination path.
    ''' </returns>
    ''' <exception cref="ArgumentException">
    ''' <paramref name="sourcePath"/> is not located inside <paramref name="sourceRootPath"/>.
    ''' </exception>
    Friend Shared Function BuildDestinationPath(sourceRootPath As String, destinationRootPath As String, sourcePath As String) As String

        Dim relativePath As String = MirrorRules.GetRelativePath(sourceRootPath, sourcePath)
        Dim destinationRoot As String = MirrorRules.NormalizeDirectoryPath(destinationRootPath)

        If String.IsNullOrEmpty(relativePath) Then
            Return MirrorRules.AddLongPathPrefixIfNeeded(destinationRoot)
        End If

        Return MirrorRules.AddLongPathPrefixIfNeeded($"{destinationRoot.TrimEnd(IO.Path.DirectorySeparatorChar)}{IO.Path.DirectorySeparatorChar}{relativePath}")

    End Function

    ''' <summary>
    ''' Determines whether the specified directory is a system directory (or is located inside a system directory)
    ''' that must always be excluded from a mirror operation, such as the recycle bin
    ''' or the <c>System Volume Information</c> directory located at the root of a volume.
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' MirrorRules.IsExcludedSystemDirectory("D:\$Recycle.Bin\S-1-5-21") ' Result: True
    ''' MirrorRules.IsExcludedSystemDirectory("D:\Recycled Photos")        ' Result: False
    ''' </code>
    ''' </example>
    ''' <param name="dirPath">
    ''' The rooted directory path.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the directory must be excluded; otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function IsExcludedSystemDirectory(dirPath As String) As Boolean

        Dim normalizedPath As String = MirrorRules.NormalizeDirectoryPath(dirPath)
        Dim root As String = IO.Path.GetPathRoot(normalizedPath)

        Dim relativePath As String = normalizedPath.Substring(root.Length).TrimStart(IO.Path.DirectorySeparatorChar)
        If String.IsNullOrEmpty(relativePath) Then
            Return False
        End If

        Dim topLevelName As String = relativePath.Split(IO.Path.DirectorySeparatorChar)(0)
        Return MirrorRules.ExcludedSystemDirectoryNames.Contains(topLevelName, StringComparer.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Determines whether the specified directory path is the root directory of a volume
    ''' (e.g. <c>C:\</c> or <c>\\Server\Share</c>).
    ''' </summary>
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' MirrorRules.IsVolumeRoot("D:\")      ' Result: True
    ''' MirrorRules.IsVolumeRoot("D:\Music") ' Result: False
    ''' </code>
    ''' </example>
    ''' <param name="dirPath">
    ''' The rooted directory path.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the directory is the root directory of a volume; otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function IsVolumeRoot(dirPath As String) As Boolean

        Dim normalizedPath As String = MirrorRules.NormalizeDirectoryPath(dirPath)
        Dim root As String = IO.Path.GetPathRoot(normalizedPath)

        Return String.Equals(normalizedPath.TrimEnd(IO.Path.DirectorySeparatorChar),
                             root.TrimEnd(IO.Path.DirectorySeparatorChar),
                             StringComparison.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Validates the specified destination directory against the specified source directory.
    ''' </summary>
    ''' <param name="sourcePath">
    ''' The rooted path of the source directory.
    ''' </param>
    '''
    ''' <param name="destinationPath">
    ''' The rooted path of the destination directory.
    ''' </param>
    ''' <returns>
    ''' A <see cref="DestinationValidationResult"/> value that indicates whether the destination directory is valid.
    ''' </returns>
    Friend Shared Function ValidateDestination(sourcePath As String, destinationPath As String) As DestinationValidationResult

        If MirrorRules.ArePathsEqual(sourcePath, destinationPath) Then
            Return DestinationValidationResult.SameAsSource
        End If

        If MirrorRules.IsDescendantOf(destinationPath, sourcePath) Then
            Return DestinationValidationResult.InsideSource
        End If

        Dim sourceParentPath As String = IO.Path.GetDirectoryName(MirrorRules.NormalizeDirectoryPath(sourcePath))
        If (Not String.IsNullOrEmpty(sourceParentPath)) AndAlso
            MirrorRules.ArePathsEqual(sourceParentPath, destinationPath) Then
            Return DestinationValidationResult.ParentOfSource
        End If

        Return DestinationValidationResult.Valid

    End Function

    ''' <summary>
    ''' Decides what to do when mirroring a link (symbolic link or directory junction), 
    ''' according to what already exists at its destination path.
    ''' </summary>
    ''' <remarks>
    ''' Rules:
    ''' <list type="bullet">
    '''   <item><description>Nothing exists: <see cref="LinkAction.Create"/>.</description></item>
    '''   <item><description>An identical link exists (same kind and same target): <see cref="LinkAction.Skip"/>.</description></item>
    '''   <item><description>A different link or a regular file exists: 
    '''     <see cref="LinkAction.Replace"/> if <paramref name="overwrite"/> is <see langword="True"/>; 
    '''     otherwise, <see cref="LinkAction.Keep"/> (the existing item is kept).</description></item>
    '''   <item><description>A regular directory exists: <see cref="LinkAction.Conflict"/> 
    '''     (a directory tree is never deleted to place a link).</description></item>
    ''' </list>
    ''' </remarks>
    ''' <param name="destinationExists">
    ''' A value indicating whether a file, directory or link exists at the destination path.
    ''' </param>
    ''' 
    ''' <param name="destinationIsLink">
    ''' A value indicating whether the existing item is a link (a reparse point).
    ''' </param>
    ''' 
    ''' <param name="destinationIsDirectory">
    ''' A value indicating whether the existing item is a directory (or a directory link).
    ''' </param>
    ''' 
    ''' <param name="isIdenticalLink">
    ''' A value indicating whether the existing item is a link of the same kind that points to the same target.
    ''' </param>
    ''' 
    ''' <param name="overwrite">
    ''' A value indicating whether existing destination items can be replaced.
    ''' </param>
    ''' <returns>
    ''' The <see cref="LinkAction"/> to perform.
    ''' </returns>
    Friend Shared Function DecideLinkAction(destinationExists As Boolean,
                                            destinationIsLink As Boolean,
                                            destinationIsDirectory As Boolean,
                                            isIdenticalLink As Boolean,
                                            overwrite As Boolean) As LinkAction

        If Not destinationExists Then
            Return LinkAction.Create
        End If

        If destinationIsLink Then
            If isIdenticalLink Then
                Return LinkAction.Skip
            End If
            Return If(overwrite, LinkAction.Replace, LinkAction.Keep)
        End If

        If destinationIsDirectory Then
            Return LinkAction.Conflict
        End If

        Return If(overwrite, LinkAction.Replace, LinkAction.Keep)

    End Function

    ''' <summary>
    ''' Calculates the progress percentage of a mirror operation.
    ''' </summary>
    ''' <param name="current">
    ''' The amount of processed items.
    ''' </param>
    '''
    ''' <param name="maximum">
    ''' The total amount of items.
    ''' </param>
    ''' <returns>
    ''' The progress percentage, in the range 0 to 100.
    ''' </returns>
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' <paramref name="maximum"/> is less than 1,
    ''' or <paramref name="current"/> is negative or greater than <paramref name="maximum"/>.
    ''' </exception>
    Friend Shared Function CalculateProgressPercent(current As Integer, maximum As Integer) As Integer

        If maximum < 1 Then
            Throw New ArgumentOutOfRangeException(NameOf(maximum), maximum, "Value must be greater than zero.")
        End If

        If (current < 0) OrElse (current > maximum) Then
            Throw New ArgumentOutOfRangeException(NameOf(current), current, $"Value must be in the range 0 to {maximum}.")
        End If

        Return CInt(current / maximum * 100)

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Determines whether the specified path starts with a drive letter followed by a volume separator
    ''' and a directory separator (e.g. <c>C:\</c>).
    ''' </summary>
    ''' <param name="path">
    ''' The path.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the path is a fully qualified local path; otherwise, <see langword="False"/>.
    ''' </returns>
    Private Shared Function IsFullyQualifiedLocalPath(path As String) As Boolean

        Return (path.Length >= 3) AndAlso
               Char.IsLetter(path(0)) AndAlso
               (path(1) = IO.Path.VolumeSeparatorChar) AndAlso
               ((path(2) = IO.Path.DirectorySeparatorChar) OrElse (path(2) = IO.Path.AltDirectorySeparatorChar))

    End Function

    ''' <summary>
    ''' Gets the normalized directory path ending with a directory separator,
    ''' suitable to determine whether another path is located inside it.
    ''' </summary>
    ''' <param name="dirPath">
    ''' The rooted directory path.
    ''' </param>
    ''' <returns>
    ''' The normalized directory path ending with a directory separator.
    ''' </returns>
    Private Shared Function GetDirectoryPrefix(dirPath As String) As String

        Dim normalizedPath As String = MirrorRules.NormalizeDirectoryPath(dirPath)
        If normalizedPath.EndsWith(IO.Path.DirectorySeparatorChar) Then
            Return normalizedPath
        End If

        Return $"{normalizedPath}{IO.Path.DirectorySeparatorChar}"

    End Function

#End Region

End Class

#End Region

#Region " LinkAction "

''' <summary>
''' Specifies what to do when mirroring a link (symbolic link or directory junction).
''' </summary>
''' <seealso cref="MirrorRules.DecideLinkAction(Boolean, Boolean, Boolean, Boolean, Boolean)"/>
Friend Enum LinkAction As Integer

    ''' <summary>
    ''' Create the link (nothing exists at the destination path).
    ''' </summary>
    Create = 0

    ''' <summary>
    ''' Do nothing, because an identical link already exists (the link is already mirrored).
    ''' </summary>
    Skip = 1

    ''' <summary>
    ''' Delete the existing link or file, then create the link.
    ''' </summary>
    Replace = 2

    ''' <summary>
    ''' A regular directory exists at the destination path, and it cannot be replaced.
    ''' </summary>
    Conflict = 3

    ''' <summary>
    ''' Do nothing, because a different link or a file exists at the destination path, 
    ''' and it must be kept (the overwrite option is disabled). The link is not mirrored.
    ''' </summary>
    Keep = 4

End Enum

#End Region
