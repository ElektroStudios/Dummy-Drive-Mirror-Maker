




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





#Region " W-Params "

Namespace ElektroKit.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies additional message-specific information for a System-Defined Message.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644927%28v=vs.85%29.aspx#system_defined"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum WParams As UInteger

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A Null WParam.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Null = 0UI

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Normal (Green Color).
        ''' <para></para>
        ''' Used with <see cref="ProgressBarUIMessages.SetState"/> message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb760850(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        PBST_Normal = &H1

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Error (Red Color).
        ''' <para></para>
        ''' Used with <see cref="ProgressBarUIMessages.SetState"/> message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb760850(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        PBST_Error = &H2

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Paused (Yellow Color).
        ''' <para></para>
        ''' Used with <see cref="ProgressBarUIMessages.SetState"/> message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb760850(v=vs.85).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        PBST_Paused = &H3

    End Enum

End Namespace

#End Region
