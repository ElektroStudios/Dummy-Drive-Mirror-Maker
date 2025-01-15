




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

Imports DevCase.Core.Threading.Types

#End Region

#Region " ElektroBackgroundWorker State "

Namespace DevCase.Core.Threading.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies the state of a <see cref="ElektroBackgroundWorker"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum ElektroBackgroundWorkerState As Integer

        ''' <summary>
        ''' The <see cref="ElektroBackgroundWorker"/> is stopped.
        ''' </summary>
        Stopped = 0

        ''' <summary>
        ''' The <see cref="ElektroBackgroundWorker"/> is running.
        ''' </summary>
        Running = 1

        ''' <summary>
        ''' The <see cref="ElektroBackgroundWorker"/> is paused.
        ''' </summary>
        Paused = 2

        ''' <summary>
        ''' The <see cref="ElektroBackgroundWorker"/> is pending on a cancellation.
        ''' </summary>
        CancellationPending = 3

        ''' <summary>
        ''' The <see cref="ElektroBackgroundWorker"/> is completed (stopped).
        ''' </summary>
        Completed = 4

    End Enum

End Namespace

#End Region
