




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

Imports System.Collections.Concurrent
Imports System.IO
Imports System.Security

#End Region

#Region " File/Directory Searcher "

Namespace DevCase.Core.IO.Tools

    Friend NotInheritable Class FileDirSearcher

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="FileDirSearcher"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the files those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The root directory path to search for files.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="fileNamePatterns">
        ''' The file name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="fileExtPatterns">
        ''' The file extension pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' An <see cref="IEnumerable(Of FileInfo)"/> instance containing the files information.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Friend Shared Function GetFiles(dirPath As String,
searchOption As SearchOption,
                                         Optional fileNamePatterns As IEnumerable(Of String) = Nothing,
                                         Optional fileExtPatterns As IEnumerable(Of String) = Nothing,
                                         Optional ignoreCase As Boolean = True,
                                         Optional throwOnError As Boolean = False) As IEnumerable(Of FileInfo)

            ' Analyze and resolve path problems. (eg. 'C:' -> 'C:\')
            FileDirSearcher.AnalyzePath(dirPath)

            ' Analyze the passed arguments.
            FileDirSearcher.AnalyzeArgs(dirPath, searchOption)

            ' Get and return the files.
            Dim queue As New ConcurrentQueue(Of FileInfo)
            FileDirSearcher.CollectFiles(queue, dirPath, searchOption, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the filepaths those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The root directory path to search for files.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="fileNamePatterns">
        ''' The file name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="fileExtPatterns">
        ''' The file extension pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' An <see cref="IEnumerable(Of String)"/> instance containing the filepaths.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Friend Shared Function GetFilePaths(dirPath As String,
searchOption As SearchOption,
                                             Optional fileNamePatterns As IEnumerable(Of String) = Nothing,
                                             Optional fileExtPatterns As IEnumerable(Of String) = Nothing,
                                             Optional ignoreCase As Boolean = True,
                                             Optional throwOnError As Boolean = False) As IEnumerable(Of String)

            ' Analyze and resolve path problems. (eg. 'C:' -> 'C:\')
            FileDirSearcher.AnalyzePath(dirPath)

            ' Analyze the passed arguments.
            FileDirSearcher.AnalyzeArgs(dirPath, searchOption)

            ' Get and return the filepaths.
            Dim queue As New ConcurrentQueue(Of String)
            FileDirSearcher.CollectFilePaths(queue, dirPath, searchOption, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the directories those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The root directory path to search for directories.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="dirPathPatterns">
        ''' The directory path pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="dirNamePatterns">
        ''' The directory name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' An <see cref="IEnumerable(Of DirectoryInfo)"/> instance containing the dirrectories information.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Friend Shared Function GetDirs(dirPath As String,
searchOption As SearchOption,
                                        Optional dirPathPatterns As IEnumerable(Of String) = Nothing,
                                        Optional dirNamePatterns As IEnumerable(Of String) = Nothing,
                                        Optional ignoreCase As Boolean = True,
                                        Optional throwOnError As Boolean = False) As IEnumerable(Of DirectoryInfo)

            ' Analyze and resolve path problems. (eg. 'C:' -> 'C:\')
            FileDirSearcher.AnalyzePath(dirPath)

            ' Analyze the passed arguments.
            FileDirSearcher.AnalyzeArgs(dirPath, searchOption)

            ' Get and return the directories.
            Dim queue As New ConcurrentQueue(Of DirectoryInfo)
            FileDirSearcher.CollectDirs(queue, dirPath, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the filepaths those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The root directory path to search for directories.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="dirPathPatterns">
        ''' The directory path pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="dirNamePatterns">
        ''' The directory name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' An <see cref="IEnumerable(Of String)"/> instance containing the directory paths.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Friend Shared Function GetDirPaths(dirPath As String,
searchOption As SearchOption,
                                            Optional dirPathPatterns As IEnumerable(Of String) = Nothing,
                                            Optional dirNamePatterns As IEnumerable(Of String) = Nothing,
                                            Optional ignoreCase As Boolean = True,
                                            Optional throwOnError As Boolean = False) As IEnumerable(Of String)

            ' Analyze and resolve path problems. (eg. 'C:' -> 'C:\')
            FileDirSearcher.AnalyzePath(dirPath)

            ' Analyze the passed arguments.
            FileDirSearcher.AnalyzeArgs(dirPath, searchOption)

            ' Get and return the directory paths.
            Dim queue As New ConcurrentQueue(Of String)
            FileDirSearcher.CollectDirPaths(queue, dirPath, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Analyzes a directory path and perform specific changes on it.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="refDirPath">
        ''' The directory path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">dirPath;Value is null, empty, or white-spaced.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub AnalyzePath(ByRef refDirPath As String)

            If String.IsNullOrEmpty(refDirPath) OrElse String.IsNullOrWhiteSpace(refDirPath) Then
                Throw New ArgumentNullException("dirPath", "Value is null, empty, or white-spaced.")

            Else
                ' Trim unwanted characters.
                refDirPath = refDirPath.TrimStart({" "c}).TrimEnd({" "c})

                If Path.IsPathRooted(refDirPath) Then
                    ' The root paths contained on the returned FileInfo objects will start with the same string-case as this root path.
                    ' So just for a little visual improvement, I'll treat this root path as a Drive-Letter and I convert it to UpperCase.
                    refDirPath = Char.ToUpper(refDirPath.First()) & refDirPath.Substring(1)
                End If

                If Not refDirPath.EndsWith("\"c) Then
                    ' Possibly its a drive letter without backslash ('C:') or else just a normal path without backslash ('C\Dir').
                    ' In any case, fix the ending backslash.
                    refDirPath = refDirPath.Insert(refDirPath.Length, "\"c)
                End If

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Analyzes the specified directory values.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The root directory path to search for files.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub AnalyzeArgs(dirPath As String, searchOption As SearchOption)

            If Not Directory.Exists(dirPath) Then
                Throw New ArgumentException(String.Format("Directory doesn't exists: '{0}'", dirPath), "dirPath")

            ElseIf (searchOption <> SearchOption.TopDirectoryOnly) AndAlso (searchOption <> SearchOption.AllDirectories) Then
                Throw New ArgumentException(String.Format("Value of '{0}' is not valid enumeration value.", CStr(searchOption)), "searchOption")

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Tries to instance the by-reference <see cref="DirectoryInfo"/> object using the given directory path.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dirPath">
        ''' The directory path used to instance the by-reference <see cref="DirectoryInfo"/> object.
        ''' </param>
        ''' 
        ''' <param name="refDirInfo">
        ''' The by-reference <see cref="DirectoryInfo"/> object to instance it using the given directory path.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub SetupDirInfoObject(dirPath As String,
                                              ByRef refDirInfo As DirectoryInfo,
throwOnError As Boolean)

            Try
                refDirInfo = New DirectoryInfo(dirPath)

            Catch ex As Exception

                Select Case ex.GetType ' Handle or suppress exceptions by its type, 

                    ' I've wrote different types just to feel free to expand this feature in the future.
                    Case GetType(ArgumentNullException),
                         GetType(ArgumentException),
                         GetType(SecurityException),
                         GetType(PathTooLongException),
                         ex.GetType

                        If throwOnError Then
                            Throw
                        End If

                End Select

            End Try

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Tries to instance the by-reference <paramref name="refCol"/> object using the given directory path.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <typeparam name="A">
        ''' The type of the <paramref name="refCol"/> object used to cast and fill the by-reference collection.
        ''' </typeparam>
        ''' 
        ''' <param name="objectAction">
        ''' The method to invoke, only for <see cref="FileInfo"/> or <see cref="DirectoryInfo"/> objects, this parameter can be <see langword="Nothing"/>.
        ''' </param>
        ''' 
        ''' <param name="sharedAction">
        ''' The method to invoke, only for filepaths or directorypaths, this parameter can be <see langword="Nothing"/>.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The directory path used to instance the by-reference <paramref name="refCol"/> object.
        ''' </param>
        ''' 
        ''' <param name="searchPattern">
        ''' The search pattern to list files or directories.
        ''' </param>
        ''' 
        ''' <param name="refCol">
        ''' The by-reference <see cref="IEnumerable(Of A)"/> object to instance it using the given directory path.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub SetupFileDirCollection(Of A)(objectAction As Func(Of String, SearchOption,
                                                                                              IEnumerable(Of A)),
sharedAction As Func(Of String, String,
                                                                                          SearchOption,
                                                                                          IEnumerable(Of A)),
dirPath As String,
searchPattern As String,
                                                    ByRef refCol As IEnumerable(Of A),
throwOnError As Boolean)

            Try
                If objectAction IsNot Nothing Then
                    refCol = objectAction.Invoke(searchPattern, SearchOption.TopDirectoryOnly)

                ElseIf sharedAction IsNot Nothing Then
                    refCol = sharedAction.Invoke(dirPath, searchPattern, SearchOption.TopDirectoryOnly)

                Else
                    Throw New ArgumentException("Any Action has been defined.")

                End If

            Catch ex As Exception

                Select Case ex.GetType ' Handle or suppress exceptions by its type, 

                    ' I've wrote different types just to feel free to expand this feature in the future.
                    Case GetType(UnauthorizedAccessException),
                         GetType(DirectoryNotFoundException),
                         ex.GetType

                        If throwOnError Then
                            Throw
                        End If

                End Select

            End Try

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether at least one of the specified patterns matches the given value.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="value">
        ''' The value, which can be a filename, file extension, direcrory path, or directory name.
        ''' </param>
        ''' 
        ''' <param name="patterns">
        ''' The patterns to match the given value.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If set to <see langword="True"/>, compares ignoring string-case rules.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> at least one of the specified patterns matches the given value; <see langword="False"/> otherwise.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function IsMatchPattern(value As String,
patterns As IEnumerable(Of String),
ignoreCase As Boolean) As Boolean

            ' Iterate the filename pattern(s) to match each name pattern on the current name.
            For Each pattern As String In patterns

                ' Supress consecuent conditionals if pattern its an asterisk.
                If pattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                    Return True

                ElseIf ignoreCase Then ' Compare name ignoring string-case rules.
                    If value.ToLower Like pattern.ToLower Then
                        Return True
                    End If

                Else ' Compare filename unignoring string-case rules.
                    If value Like pattern Then
                        Return True
                    End If

                End If ' ignoreCase

            Next pattern

            Return False

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Runs the next collector tasks synchronouslly.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <typeparam name="T"></typeparam>
        ''' <param name="action">
        ''' The collector method to invoke.
        ''' </param>
        ''' 
        ''' <param name="queue">
        ''' The <see cref="ConcurrentQueue(Of FileInfo)"/> instance.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The directory path.
        ''' </param>
        ''' 
        ''' <param name="firstPatterns">
        ''' The first comparison patterns.
        ''' </param>
        ''' 
        ''' <param name="secondPatterns">
        ''' The second comparison patterns.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If set to <see langword="True"/>, compares ignoring string-case rules.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub RunNextTasks(Of T)(action As Action(Of ConcurrentQueue(Of T),
                                                                     String,
                                                                     SearchOption,
                                                                     IEnumerable(Of String),
                                                                     IEnumerable(Of String),
                                                                     Boolean,
                                                                     Boolean),
queue As ConcurrentQueue(Of T),
dirPath As String,
firstPatterns As IEnumerable(Of String),
secondPatterns As IEnumerable(Of String),
ignoreCase As Boolean,
throwOnError As Boolean)

            Try
                Task.WaitAll(New DirectoryInfo(dirPath).
                                 GetDirectories.
                                 Select(Function(dir As DirectoryInfo)
                                            Return Task.Factory.StartNew(Sub()
                                                                             action.Invoke(queue,
                                                                                           dir.FullName, SearchOption.AllDirectories,
                                                                                           firstPatterns, secondPatterns,
                                                                                           ignoreCase, throwOnError)
                                                                         End Sub)
                                        End Function).ToArray)

            Catch ex As Exception

                Select Case ex.GetType ' Handle or suppress exceptions by its type, 

                    ' I've wrote different types just to feel free to expand this feature in the future.
                    Case GetType(UnauthorizedAccessException),
                         GetType(DirectoryNotFoundException),
                         ex.GetType

                        If throwOnError Then
                            Throw
                        End If

                End Select

            End Try

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Collects the files those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="queue">
        ''' The <see cref="ConcurrentQueue(Of FileInfo)"/> instance to enqueue new files.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The root directory path to search for files.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="fileNamePatterns">
        ''' The file name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="fileExtPatterns">
        ''' The file extension pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub CollectFiles(queue As ConcurrentQueue(Of FileInfo),
dirPath As String,
searchOption As SearchOption,
fileNamePatterns As IEnumerable(Of String),
fileExtPatterns As IEnumerable(Of String),
ignoreCase As Boolean,
throwOnError As Boolean)

            ' Initialize a FileInfo collection.
            Dim fileInfoCol As IEnumerable(Of FileInfo) = Nothing

            ' Initialize a DirectoryInfo.
            Dim dirInfo As DirectoryInfo = Nothing
            SetupDirInfoObject(dirPath, dirInfo, throwOnError)

            If fileExtPatterns IsNot Nothing Then
                ' Decrease time execution by searching for files that has extension.
                SetupFileDirCollection(Of FileInfo)(AddressOf dirInfo.GetFiles, Nothing,
                                                    dirInfo.FullName, "*.*", fileInfoCol, throwOnError)
            Else
                ' Search for all files.
                SetupFileDirCollection(Of FileInfo)(AddressOf dirInfo.GetFiles, Nothing,
                                                    dirInfo.FullName, "*", fileInfoCol, throwOnError)
            End If

            ' If the fileInfoCol collection is not empty then...
            If fileInfoCol IsNot Nothing Then

                ' Iterate the files.
                For Each fInfo As FileInfo In fileInfoCol

                    ' Flag to determine whether a filename pattern is matched. Activated by default.
                    Dim flagNamePattern As Boolean = True

                    ' Flag to determine whether a file extension pattern is matched. Activated by default.
                    Dim flagExtPattern As Boolean = True

                    ' If filename patterns collection is not empty then...
                    If fileNamePatterns IsNot Nothing Then
                        flagNamePattern = IsMatchPattern(fInfo.Name, fileNamePatterns, ignoreCase)
                    End If

                    ' If file extension patterns collection is not empty then...
                    If fileExtPatterns IsNot Nothing Then
                        flagExtPattern = IsMatchPattern(fInfo.Extension, fileExtPatterns, ignoreCase)
                    End If

                    ' If fileName and also fileExtension patterns are matched then...
                    If flagNamePattern AndAlso flagExtPattern Then
                        queue.Enqueue(fInfo) ' Enqueue this FileInfo object.
                    End If

                Next fInfo

            End If ' fileInfoCol IsNot Nothing

            ' If searchOption is recursive then...
            If searchOption = SearchOption.AllDirectories Then
                RunNextTasks(Of FileInfo)(AddressOf CollectFiles,
                                          queue, dirInfo.FullName, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Collects the filepaths those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="queue">
        ''' The <see cref="ConcurrentQueue(Of String)"/> instance to enqueue new filepaths.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The root directory path to search for files.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="fileNamePatterns">
        ''' The file name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="fileExtPatterns">
        ''' The file extension pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to file or directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub CollectFilePaths(queue As ConcurrentQueue(Of String),
dirPath As String,
searchOption As SearchOption,
fileNamePatterns As IEnumerable(Of String),
fileExtPatterns As IEnumerable(Of String),
ignoreCase As Boolean,
throwOnError As Boolean)

            ' Initialize a filepath collection.
            Dim filePathCol As IEnumerable(Of String) = Nothing

            If fileExtPatterns IsNot Nothing Then
                ' Decrease time execution by searching for files that has extension.
                SetupFileDirCollection(Of String)(Nothing, AddressOf Directory.GetFiles,
                                                  dirPath, "*.*", filePathCol, throwOnError)
            Else
                ' Search for all files.
                SetupFileDirCollection(Of String)(Nothing, AddressOf Directory.GetFiles,
                                                  dirPath, "*", filePathCol, throwOnError)
            End If

            ' If the filepath collection is not empty then...
            If filePathCol IsNot Nothing Then

                ' Iterate the filepaths.
                For Each filePath As String In filePathCol

                    ' Flag to determine whether a filename pattern is matched. Activated by default.
                    Dim flagNamePattern As Boolean = True

                    ' Flag to determine whether a file extension pattern is matched. Activated by default.
                    Dim flagExtPattern As Boolean = True

                    ' If filename patterns collection is not empty then...
                    If fileNamePatterns IsNot Nothing Then
                        flagNamePattern = IsMatchPattern(Path.GetFileNameWithoutExtension(filePath), fileNamePatterns, ignoreCase)
                    End If

                    ' If file extension patterns collection is not empty then...
                    If fileExtPatterns IsNot Nothing Then
                        flagExtPattern = IsMatchPattern(Path.GetExtension(filePath), fileExtPatterns, ignoreCase)
                    End If

                    ' If fileName and also fileExtension patterns are matched then...
                    If flagNamePattern AndAlso flagExtPattern Then
                        queue.Enqueue(filePath) ' Enqueue this filepath.
                    End If

                Next filePath

            End If ' filePathCol IsNot Nothing

            ' If searchOption is recursive then...
            If searchOption = SearchOption.AllDirectories Then
                RunNextTasks(Of String)(AddressOf CollectFilePaths,
                                        queue, dirPath, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Collects the directories those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="queue">
        ''' The <see cref="ConcurrentQueue(Of DirectoryInfo)"/> instance to enqueue new directories.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The root directory path to search for directories.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="dirPathPatterns">
        ''' The directory path pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="dirNamePatterns">
        ''' The directory name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub CollectDirs(queue As ConcurrentQueue(Of DirectoryInfo),
dirPath As String,
searchOption As SearchOption,
dirPathPatterns As IEnumerable(Of String),
dirNamePatterns As IEnumerable(Of String),
ignoreCase As Boolean,
throwOnError As Boolean)

            ' Initialize a DirectoryInfo collection.
            Dim dirInfoCol As IEnumerable(Of DirectoryInfo) = Nothing

            ' Initialize a DirectoryInfo.
            Dim dirInfo As DirectoryInfo = Nothing
            SetupDirInfoObject(dirPath, dirInfo, throwOnError)

            ' Get the top directories of the current directory.
            SetupFileDirCollection(Of DirectoryInfo)(AddressOf dirInfo.GetDirectories, Nothing,
                                                     dirInfo.FullName, "*", dirInfoCol, throwOnError)

            ' If the fileInfoCol collection is not empty then...
            If dirInfoCol IsNot Nothing Then

                ' Iterate the files.
                For Each dir As DirectoryInfo In dirInfoCol

                    ' Flag to determine whether a directory path pattern is matched. Activated by default.
                    Dim flagPathPattern As Boolean = True

                    ' Flag to determine whether a directory name pattern is matched. Activated by default.
                    Dim flagNamePattern As Boolean = True

                    ' If directory path patterns collection is not empty then...
                    If dirPathPatterns IsNot Nothing Then
                        flagPathPattern = IsMatchPattern(dir.FullName, dirPathPatterns, ignoreCase)
                    End If

                    ' If directory name patterns collection is not empty then...
                    If dirNamePatterns IsNot Nothing Then
                        flagNamePattern = IsMatchPattern(dir.Name, dirNamePatterns, ignoreCase)
                    End If

                    ' If directory path and also directory name patterns are matched then...
                    If flagPathPattern AndAlso flagNamePattern Then
                        queue.Enqueue(dir) ' Enqueue this DirectoryInfo object.
                    End If

                Next dir

            End If ' dirInfoCol IsNot Nothing

            ' If searchOption is recursive then...
            If searchOption = SearchOption.AllDirectories Then
                RunNextTasks(Of DirectoryInfo)(AddressOf CollectDirs,
                                               queue, dirPath, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Collects the directory paths those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="queue">
        ''' The <see cref="ConcurrentQueue(Of String)"/> instance to enqueue new directory paths.
        ''' </param>
        ''' 
        ''' <param name="dirPath">
        ''' The root directory path to search for directories.
        ''' </param>
        ''' 
        ''' <param name="searchOption">
        ''' The searching mode.
        ''' </param>
        ''' 
        ''' <param name="dirPathPatterns">
        ''' The directory path pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="dirNamePatterns">
        ''' The directory name pattern(s) to match.
        ''' </param>
        ''' 
        ''' <param name="ignoreCase">
        ''' If <see langword="True"/>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.
        ''' </param>
        ''' 
        ''' <param name="throwOnError">
        ''' If set to <see langword="True"/>, exceptions will be thrown, like access denied to directory.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub CollectDirPaths(queue As ConcurrentQueue(Of String),
dirPath As String,
searchOption As SearchOption,
dirPathPatterns As IEnumerable(Of String),
dirNamePatterns As IEnumerable(Of String),
ignoreCase As Boolean,
throwOnError As Boolean)

            ' Initialize a directory paths collection.
            Dim dirPathCol As IEnumerable(Of String) = Nothing

            ' Get the top directory paths of the current directory.
            SetupFileDirCollection(Of String)(Nothing, AddressOf Directory.GetDirectories,
                                              dirPath, "*", dirPathCol, throwOnError)

            ' If the fileInfoCol collection is not empty then...
            If dirPathCol IsNot Nothing Then

                ' Iterate the files.
                For Each dir As String In dirPathCol

                    ' Flag to determine whether a directory path pattern is matched. Activated by default.
                    Dim flagPathPattern As Boolean = True

                    ' Flag to determine whether a directory name pattern is matched. Activated by default.
                    Dim flagNamePattern As Boolean = True

                    ' If directory path patterns collection is not empty then...
                    If dirPathPatterns IsNot Nothing Then
                        flagPathPattern = IsMatchPattern(dir, dirPathPatterns, ignoreCase)
                    End If

                    ' If directory name patterns collection is not empty then...
                    If dirNamePatterns IsNot Nothing Then
                        flagNamePattern = IsMatchPattern(Path.GetFileName(dir), dirNamePatterns, ignoreCase)
                    End If

                    ' If directory path and also directory name patterns are matched then...
                    If flagPathPattern AndAlso flagNamePattern Then
                        queue.Enqueue(dir) ' Enqueue this directory path.
                    End If

                Next dir

            End If ' dirPathCol IsNot Nothing

            ' If searchOption is recursive then...
            If searchOption = SearchOption.AllDirectories Then
                RunNextTasks(Of String)(AddressOf CollectDirPaths,
                                        queue, dirPath, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
