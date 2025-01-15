




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





#Region " DirectoryManagementControlCodes "

Namespace DevCase.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies Directory Management Control Codes to use with the <see cref="NativeMethods.DeviceIoControl"/> function.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa363948(v=vs.85).aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum DirectoryManagementControlCodes As UInteger

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a reparse point on a file or directory.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa364595(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        SetReparsePoint = &H900A4

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves the reparse point data associated with the file or directory identified by the specified handle.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa364571(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        GetReparsePoint = &H900A8

    End Enum

End Namespace

#End Region
