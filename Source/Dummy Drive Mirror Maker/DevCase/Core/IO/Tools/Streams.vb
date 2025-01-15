




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

#Region " Stream Util "

Namespace DevCase.Core.IO.Tools

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains <see cref="Stream"/> related utilities.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class Streams

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="Streams"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Given a filesize, estimates the proper buffer size to boost the performance of a <see cref="FileStream"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filesize">
        ''' The size, in bytes, of the data to be read or write by a <see cref="FileStream"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The returned value is the proper buffer size, in bytes, 
        ''' that should be set as <c>BufferSize</c> parameter when instancing a <see cref="FileStream"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetFileStreamBufferSize(filesize As Long) As Integer

            Select Case filesize

                Case Is = 0
                    Return 0

                Case Is > 1073741824L ' Greater than 1 GB.
                    Return 1048576I   ' 1 MB.

                Case Is > 524288000L  ' Greater than 500 MB.
                    Return 524288I    ' 512 KB.

                Case Is > 314572800L  ' Greater than 300 MB.
                    Return 262144I    ' 256 KB.

                Case Is > 209715200L  ' Greater than 200 MB.
                    Return 131072I    ' 128 KB.

                Case Is > 104857600L  ' Greater than 100 MB.
                    Return 65536I     ' 64 KB.

                Case Is > 52428800L   ' Greater than 50 MB.
                    Return 32768I     ' 32 KB.

                Case Is > 10485760L   ' Greater than 10 MB.
                    Return 16384I     ' 16 KB.

                Case Is > 1048576L    ' Greater than 1 MB.
                    Return 8192I      ' 8 KB.

                Case Else             ' Less or Equal than 1 MB.
                    Return 4096I      ' 4 KB.

            End Select

        End Function

#End Region

    End Class

End Namespace

#End Region
