




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
Imports System.Windows.Forms.Design
Imports System.Windows.Forms.Design.Behavior

Imports ElektroKit.Interop.Win32
Imports ElektroKit.Interop.Win32.Types
Imports ElektroKit.UserControls.Controls

#End Region

#Region " ElektroProgressBar Designer "

Namespace ElektroKit.UserControls.Types.Designers

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides design-time support for the <see cref="ElektroProgressBar"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <seealso cref="ControlDesigner"/>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class ElektroProgressBarDesigner : Inherits ControlDesigner

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of <see cref="SnapLine"/> objects representing significant alignment points for this control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The snap lines.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        Public Overrides ReadOnly Property SnapLines As IList
            Get
                ' Get the SnapLines collection from the base class
                Dim snapList As ArrayList = TryCast(MyBase.SnapLines, ArrayList)

                ' Calculate the Baseline for the Font used by the Control and add it to the SnapLines
                Dim textBaseline As Integer = ElektroProgressBarDesigner.GetBaseline(MyBase.Control, ContentAlignment.MiddleCenter)
                If (textBaseline > 0) Then
                    snapList.Add(New SnapLine(SnapLineType.Baseline, textBaseline, SnapLinePriority.Medium))
                End If

                Return snapList
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ElektroProgressBarDesigner"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the baseline.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="ctrl">
        ''' The control.
        ''' </param>
        ''' 
        ''' <param name="alignment">
        ''' The alignment.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting baseline.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function GetBaseline(ByVal ctrl As Control, ByVal alignment As ContentAlignment) As Integer

            Dim textAscent As Integer = 0
            Dim textHeight As Integer = 0

            ' Retrieve the client rectangle of the Control.
            Dim clientRect As Rectangle = ctrl.ClientRectangle

            ' Create a Graphics object for the Control.
            Using g As Graphics = ctrl.CreateGraphics()

                ' Retrieve the device context Handle
                Dim hdc As IntPtr = g.GetHdc()

                ' Create a wrapper for the text font of the control.
                Dim controlFont As Font = ctrl.Font
                Dim tempFontHandle As New HandleRef(controlFont, controlFont.ToHfont())

                Try
                    ' Create a wrapper for the device context.
                    Dim dcHandle As New HandleRef(ctrl, hdc)

                    ' Select the text font into the device context.
                    Dim font As IntPtr = NativeMethods.SelectObject(dcHandle, tempFontHandle)

                    ' Create a TextMetricW structure to calculate metrics for the selected text font.
                    Dim textMetric As New TextMetricW()
                    If (ElektroProgressBarDesigner.GetTextMetrics(dcHandle, textMetric) = True) Then
                        textAscent = (textMetric.Ascent + 1)
                        textHeight = textMetric.Height
                    End If

                    ' Restore original Font
                    Dim originalFontHandle As New HandleRef(ctrl, font)
                    NativeMethods.SelectObject(dcHandle, originalFontHandle)

                Finally
                    ' Cleanup tempFont
                    NativeMethods.DeleteObject(tempFontHandle)

                    ' Release device context
                    g.ReleaseHdc(hdc)
                End Try

            End Using

            ' Calculate return value based on the specified alignment; first check top alignment
            If (alignment And (ContentAlignment.TopLeft Or ContentAlignment.TopCenter Or ContentAlignment.TopRight)) <> 0 Then
                Return (clientRect.Top + textAscent)
            End If

            ' Check middle alignment
            If (alignment And (ContentAlignment.MiddleLeft Or ContentAlignment.MiddleCenter Or ContentAlignment.MiddleRight)) = 0 Then
                Return ((clientRect.Bottom - textHeight) + textAscent)
            End If

            ' Assume bottom alignment
            Return CInt(Math.Round(CDbl(clientRect.Top) + CDbl(clientRect.Height) / 2 - CDbl(textHeight) / 2 + CDbl(textAscent)))
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the specified buffer with the metrics for the currently selected font.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hdc">
        ''' The device context handle.
        ''' </param>
        ''' 
        ''' <param name="refMetricUnicode">
        ''' A <see cref="TextMetricW"/> (Unicode) structure.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' If the function succeeds, the return value is <see langword="True"/>.
        ''' <para></para>
        ''' If the function fails, the return value is <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function GetTextMetrics(ByVal hdc As HandleRef, ByRef refMetricUnicode As TextMetricW) As Boolean

            If Marshal.SystemDefaultCharSize <> 1 Then
                ' Handle Unicode
                Return NativeMethods.GetTextMetricsW(hdc, refMetricUnicode)
            End If

            ' Handle ANSI. Call GetTextMetricsA to get an ANSI TextMetricA struct then translate to Unicode TextMetricW struct.
            Dim metricAnsi As New TextMetricA()
            Dim result As Boolean = NativeMethods.GetTextMetricsA(hdc, metricAnsi)

            With refMetricUnicode
                .Height = metricAnsi.Height
                .Ascent = metricAnsi.Ascent
                .Descent = metricAnsi.Descent
                .InternalLeading = metricAnsi.InternalLeading
                .ExternalLeading = metricAnsi.ExternalLeading
                .AveCharWidth = metricAnsi.AveCharWidth
                .MaxCharWidth = metricAnsi.MaxCharWidth
                .Weight = metricAnsi.Weight
                .Overhang = metricAnsi.Overhang
                .DigitizedAspectX = metricAnsi.DigitizedAspectX
                .DigitizedAspectY = metricAnsi.DigitizedAspectY
                .FirstChar = Convert.ToChar(metricAnsi.FirstChar)
                .LastChar = Convert.ToChar(metricAnsi.LastChar)
                .DefaultChar = Convert.ToChar(metricAnsi.DefaultChar)
                .BreakChar = Convert.ToChar(metricAnsi.BreakChar)
                .Italic = metricAnsi.Italic
                .Underlined = metricAnsi.Underlined
                .StruckOut = metricAnsi.StruckOut
                .PitchAndFamily = metricAnsi.PitchAndFamily
                .CharSet = metricAnsi.CharSet
            End With

            Return result

        End Function

#End Region

    End Class

End Namespace

#End Region
