




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

Imports DevCase.Interop.Win32.Enums

#End Region

#Region " ProgressBarState "

Namespace DevCase.UserControls.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies the state of a <see cref="ProgressBar"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum ProgressBarState As Integer

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Normal (Green Color).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Normal = CInt(WParams.PBST_Normal)

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Error (Red Color).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        [Error] = CInt(WParams.PBST_Error)

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the state of a progressbar to Paused (Yellow Color).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Paused = CInt(WParams.PBST_Paused)

    End Enum

End Namespace

#End Region
