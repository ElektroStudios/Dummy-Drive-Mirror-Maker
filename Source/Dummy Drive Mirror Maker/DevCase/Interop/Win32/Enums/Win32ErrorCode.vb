




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

Imports System.Runtime.InteropServices

#End Region

#Region " Win32ErrorCode "

Namespace DevCase.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies system error codes for the Windows API functions. 
    ''' <para></para>
    ''' These error codes are returned by the <see cref="Marshal.GetLastWin32Error()"/> function when a function fails.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' List of System Error Codes: <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx"/>.
    ''' <para></para>
    ''' C# Source: <see href="http://github.com/josemesona/Win32Ex"/>.
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum Win32ErrorCode As Integer

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

#Region "0 - 499"

        ''' <summary>
        ''' The operation completed successfully.
        ''' </summary>
        ERROR_SUCCESS = &H0 ' 0

#End Region

#Region "4000 - 5999"

        ''' <summary>
        ''' The file or directory is not a re parse point.
        ''' </summary>
        ERROR_NOT_A_REPARSE_POINT = &H1126 ' 4390

#End Region

    End Enum

End Namespace

#End Region
