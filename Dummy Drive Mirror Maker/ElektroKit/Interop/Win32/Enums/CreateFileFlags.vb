




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

Imports System.IO

#End Region

#Region " CreateFileFlags "

Namespace ElektroKit.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies the file or device flags for the <see cref="NativeMethods.CreateFile"/> function.
    ''' <para></para>
    ''' These flags can be combined with any value of the <see cref="FileAttributes"/> enumeration. 
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858%28v=vs.85%29.aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <Flags>
    Public Enum CreateFileFlags As UInteger

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

        ''' <summary>
        ''' The file is being opened or created for a backup or restore operation. 
        ''' <para></para>
        ''' The system ensures that the calling process overrides file security checks when the 
        ''' process has <c>ProcessPrivileges.BackupPrivilege</c> and 
        ''' <c>ProcessPrivileges.RestorePrivilege</c> privileges. 
        ''' <para></para>
        ''' You must set this flag to obtain a handle to a directory. 
        ''' A directory handle can be passed to some functions instead of a file handle. 
        ''' </summary>
        BackupSemantics = &H2000000UI

        ''' <summary>
        ''' Normal reparse point processing will not occur; 
        ''' <see cref="NativeMethods.CreateFile"/> will attempt to open the reparse point. 
        ''' <para></para>
        ''' When a file is opened, a file handle is returned, 
        ''' whether or not the filter that controls the reparse point is operational.
        ''' <para></para>
        ''' This flag cannot be used with the <see cref="FileMode.OpenOrCreate"/> flag.
        ''' <para></para>
        ''' If the file is not a reparse point, then this flag is ignored.
        ''' </summary>
        OpenReparsePoint = &H200000UI

    End Enum

End Namespace

#End Region
