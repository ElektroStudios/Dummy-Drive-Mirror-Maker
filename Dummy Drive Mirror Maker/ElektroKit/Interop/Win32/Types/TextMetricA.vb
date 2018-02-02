




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

Imports System.Runtime.InteropServices

#End Region

#Region " TextMetric (ANSI) "

Namespace ElektroKit.Interop.Win32.Types

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Contains basic information about a physical font. 
    ''' <para></para>
    ''' All sizes are specified in logical units; that is, they depend on the current mapping mode of the display context.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="http://msdn.microsoft.com/es-es/library/windows/desktop/dd145132(v=vs.85).aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepThrough>
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Public Structure TextMetricA

#Region " Fields "

        ''' <summary>
        ''' The height (ascent + descent) of characters.
        ''' </summary>
        Public Height As Integer

        ''' <summary>
        ''' The ascent (units above the base line) of characters. 
        ''' </summary>
        Public Ascent As Integer

        ''' <summary>
        ''' The descent (units below the base line) of characters
        ''' </summary>
        Public Descent As Integer

        ''' <summary>
        ''' The amount of leading (space) inside the bounds set by the tmHeight member.
        ''' </summary>
        Public InternalLeading As Integer

        ''' <summary>
        ''' The amount of extra leading (space) that the application adds between rows.
        ''' </summary>
        Public ExternalLeading As Integer

        ''' <summary>
        ''' The average width of characters in the font (generally defined as the width of the letter x).
        ''' </summary>
        Public AveCharWidth As Integer

        ''' <summary>
        ''' The width of the widest character in the font.
        ''' </summary>
        Public MaxCharWidth As Integer

        ''' <summary>
        ''' The weight of the font.
        ''' </summary>
        Public Weight As Integer

        ''' <summary>
        ''' The extra width per string that may be added to some synthesized fonts.
        ''' </summary>
        Public Overhang As Integer

        ''' <summary>
        ''' The horizontal aspect of the device for which the font was designed.
        ''' </summary>
        Public DigitizedAspectX As Integer

        ''' <summary>
        ''' The vertical aspect of the device for which the font was designed.
        ''' </summary>
        Public DigitizedAspectY As Integer

        ''' <summary>
        ''' The value of the first character defined in the font.
        ''' </summary>
        Public FirstChar As Byte

        ''' <summary>
        ''' The value of the last character defined in the font.
        ''' </summary>
        Public LastChar As Byte

        ''' <summary>
        ''' The value of the character to be substituted for characters not in the font.
        ''' </summary>
        Public DefaultChar As Byte

        ''' <summary>
        ''' The value of the character that will be used to define word breaks for text justification
        ''' </summary>
        Public BreakChar As Byte

        ''' <summary>
        ''' Specifies an italic font if it is nonzero
        ''' </summary>
        Public Italic As Byte

        ''' <summary>
        ''' Specifies an underlined font if it is nonzero
        ''' </summary>
        Public Underlined As Byte

        ''' <summary>
        ''' A strikeout font if it is nonzero.
        ''' </summary>
        Public StruckOut As Byte

        ''' <summary>
        ''' Specifies information about the pitch, the technology, and the family of a physical font.
        ''' </summary>
        Public PitchAndFamily As Byte

        ''' <summary>
        ''' The character set of the font.
        ''' </summary>
        Public CharSet As Byte

#End Region

    End Structure

End Namespace

#End Region
