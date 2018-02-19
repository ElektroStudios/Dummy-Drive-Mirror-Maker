




' THIS OPEN-SOURCE APPLICATION IS POWERED BY ELEKTROKIT FRAMEWORK, CREATED BY ELEKTRO STUDIOS.

' WHAT YOU SEE HERE IS FREE CUTTED CONTENT OF THIS FRAMEWORK.

' IF YOU LIKED THIS FREE APPLICATION, THEN PLEASE CONSIDER TO BUY ELEKTROKIT FRAMEWORK FOR .NET AT:
' https://codecanyon.net/item/elektrokit-class-library-for-net/19260282

' YOU CAN FIND THESE HELPER METHODS AND A MASSIVE AMOUNT MORE!, 
' +850 EXTENSION METHODS FOR ALL KIND OF TYPES, CUSTOM USER-CONTROLS, 
' EVERYTHING FOR THE NEWBIE And THE ADVANCED USER, FOR VB.NET AND C#. 

' ElektroKit is a utility framework containing new APIs and extensions to the core .NET Framework 
' to help complete your developer toolbox.
' It Is a set of general purpose classes provided as easy to consume packages.
' These utility classes and components provide productivity in day to day software development 
' mainly focused To WindowsForms. 

' UPDATES OF ELEKTROKIT ARE MAINTAINED AND RELEASED EVERY MONTH.





#Region " Imports "

Imports ElektroKit.Interop.Win32.Types

#End Region

#Region " ReparsePointTags "

Namespace ElektroKit.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies a predefined Microsoft reparse point tag, for the <see cref="ReparseDataBuffer.ReparseTag"/> member.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/es-es/library/windows/desktop/aa365511(v=vs.85).aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum ReparsePointTags As UInteger

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

        ''' <summary>
        ''' Tag used for a mount point.
        ''' <para></para>
        ''' For a symbolic link, see: <see cref="ReparsePointTags.SymbolicLink"/>
        ''' </summary>
        MountPoint = &HA0000003UI

        ''' <summary>
        ''' Tag used for for a symbolic link. 
        ''' <para></para>
        ''' For a mount point, see: <see cref="ReparsePointTags.MountPoint"/>
        ''' </summary>
        SymbolicLink = &HA000000CUI

        ''' <summary>
        ''' GlobalReparse reparse point tag.
        ''' </summary>
        GlobalReparse = &HA0000019UI

        ''' <summary>
        ''' AppExecLink reparse point tag.
        ''' </summary>
        AppExecLink = &H8000001BUI

        ''' <summary>
        ''' Tag used by OneDrive.
        ''' </summary>
        OneDrive = &H80000021UI

    End Enum

End Namespace

#End Region
