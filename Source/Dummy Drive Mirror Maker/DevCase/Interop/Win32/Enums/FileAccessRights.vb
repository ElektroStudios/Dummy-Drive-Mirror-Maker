




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





#Region " FileAccessRights "

Namespace DevCase.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies file-specific access rights.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/gg258116(v=vs.85).aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <Flags>
    Public Enum FileAccessRights As UInteger

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

#Region " Generic "
#End Region

#Region " Directories Only "
#End Region

#Region " Named Pipes Only "
#End Region

#Region " Standard Rights "

        ''' <summary>
        ''' Same as <see cref="StandardAccessRights.ReadControl"/>.
        ''' </summary>
        StandardRightsRead = StandardAccessRights.StandardRightsRead

        ''' <summary>
        ''' Same as <see cref="StandardAccessRights.StandardRightsRead"/>.
        ''' </summary>
        StandardRightsWrite = StandardAccessRights.StandardRightsWrite

#End Region

    End Enum

End Namespace

#End Region
