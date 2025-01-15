




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

#Region " Files "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Files

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the files those matches the criteria inside the specified directory and/or sub-directories.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' Dim files As List(Of FileInfo) =
        '''     GetFiles("C:\Windows\System32", SearchOption.AllDirectories).ToList
        ''' 
        ''' Dim files As IEnumerable(Of FileInfo) =
        '''     GetFiles(dirPath:="C:\Windows\System32",
        '''              searchOption:=SearchOption.TopDirectoryOnly,
        '''              fileNamePatterns:={"*"},
        '''              fileExtPatterns:={"*.dll", "*.exe"},
        '''              ignoreCase:=True,
        '''              throwOnError:=True)
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dir">
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
        ''' <exception cref="Global.System.ArgumentException">dirPath or searchOption
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetFiles(dir As DirectoryInfo,
searchOption As SearchOption,
                                        Optional fileNamePatterns As IEnumerable(Of String) = Nothing,
                                        Optional fileExtPatterns As IEnumerable(Of String) = Nothing,
                                        Optional ignoreCase As Boolean = True,
                                        Optional throwOnError As Boolean = False) As IEnumerable(Of FileInfo)

            Return FileDirSearcher.GetFiles(dir.FullName, searchOption, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)

        End Function

#End Region

    End Class

End Namespace

#End Region
