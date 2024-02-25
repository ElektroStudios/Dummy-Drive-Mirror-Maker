#Region " Imports "

Imports System.IO

#End Region

Friend NotInheritable Class Options

    ''' <summary>
    ''' Gets or sets the source directory.
    ''' </summary>
    Friend Shared Property SourceDir As DirectoryInfo

    ''' <summary>
    ''' Gets or sets the target directory.
    ''' </summary>
    Friend Shared Property TargetDir As DirectoryInfo

    ''' <summary>
    ''' Gets or sets a value indicating whether the attributes of file and folders must be preserved when mirroring.
    ''' </summary>
    Friend Shared Property PreserveAttribs As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether the timestamp of file and folders must be preserved when mirroring.
    ''' </summary>
    Friend Shared Property PreserveTimestamps As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether hidden files and folders must be mirrored.
    ''' </summary>
    Friend Shared Property MirrorHiddenFiles As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether symbolic links must be mirrored.
    ''' </summary>
    Friend Shared Property MirrorSymbolicLinks As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether security access exceptions must be handled when in a mirror operation.
    ''' <para></para>
    ''' If <see langword="True"/>, an exception will be thrown if the user has no permission to read a file or folder,
    ''' and the mirror operation will stop abruptly.
    ''' </summary>
    Friend Shared Property IgnoreSecurityExceptions As Boolean

End Class
