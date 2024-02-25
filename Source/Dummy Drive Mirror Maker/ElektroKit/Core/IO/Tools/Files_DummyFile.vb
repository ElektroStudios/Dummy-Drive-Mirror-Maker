




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

#Region " File Util "

Namespace DevCase.Core.IO.Tools

    Partial Public NotInheritable Class Files

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a dummy (zero-byte filled) file of zero size.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' CreateDummyFile("C:\DummyFile.tmp"))
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filepath">
        ''' The target filepath.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="IOException">
        ''' Target file already exists.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateDummyFile(ByVal filepath As String)

            Files.CreateDummyFile(filepath, 0L)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a dummy (zero-byte filled) file of the specified filesize.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' CreateDummyFile("C:\DummyFile.tmp", CLng(Math.Pow(1024L, 3L))) ' 1 GB filesize.
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filepath">
        ''' The target filepath.
        ''' </param>
        ''' 
        ''' <param name="filesize">
        ''' The filesize, in Bytes.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="IOException">
        ''' Target file already exists.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Sub CreateDummyFile(ByVal filepath As String, ByVal filesize As Long)

            If File.Exists(filepath) Then
                Throw New IOException(message:="Target file already exists.")

            Else
                Dim bufferSize As Integer = Streams.GetFileStreamBufferSize(filesize)

                Using fs As New FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.Read, bufferSize)
                    fs.SetLength(filesize)
                End Using

            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
