﻿




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

Imports System.IO

#End Region

#Region " Directories "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Directories

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the directories those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' Dim dirs As List(Of DirectoryInfo) =
        '''     GetDirs("C:\Windows\System32", SearchOption.AllDirectories).ToList
        ''' 
        ''' Dim dirs As IEnumerable(Of DirectoryInfo) =
        '''     GetDirs(dirPath:="C:\Windows\System32",
        '''             searchOption:=SearchOption.TopDirectoryOnly,
        '''             dirPathPatterns:={"*"},
        '''             dirNamePatterns:={"*Microsoft*"},
        '''             ignoreCase:=True,
        '''             throwOnError:=True)
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dir">
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
        ''' <exception cref="Global.System.ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetDirs(dir As DirectoryInfo,
searchOption As SearchOption,
                                       Optional dirPathPatterns As IEnumerable(Of String) = Nothing,
                                       Optional dirNamePatterns As IEnumerable(Of String) = Nothing,
                                       Optional ignoreCase As Boolean = True,
                                       Optional throwOnError As Boolean = False) As IEnumerable(Of DirectoryInfo)

            Return FileDirSearcher.GetDirs(dir.FullName, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the filepaths those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' Dim dirPaths As List(Of String) =
        '''     GetDirPaths("C:\Windows\System32", SearchOption.AllDirectories).ToList
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dir">
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
        ''' <exception cref="Global.System.ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetDirPaths(dir As DirectoryInfo,
searchOption As SearchOption,
                                           Optional dirPathPatterns As IEnumerable(Of String) = Nothing,
                                           Optional dirNamePatterns As IEnumerable(Of String) = Nothing,
                                           Optional ignoreCase As Boolean = True,
                                           Optional throwOnError As Boolean = False) As IEnumerable(Of String)

            Return FileDirSearcher.GetDirPaths(dir.FullName, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)

        End Function

#End Region

    End Class

End Namespace

#End Region
