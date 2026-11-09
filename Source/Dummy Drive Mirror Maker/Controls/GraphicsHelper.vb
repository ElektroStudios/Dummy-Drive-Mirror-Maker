#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Drawing2D

#End Region

#Region " GraphicsHelper "

''' <summary>
''' Drawing helpers shared by the custom controls of the application.
''' </summary>
Friend NotInheritable Class GraphicsHelper

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="GraphicsHelper"/> class from being created.
    ''' </summary>
    <DebuggerNonUserCode>
    Private Sub New()
    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Creates a <see cref="GraphicsPath"/> that represents a rectangle with rounded corners.
    ''' </summary>
    ''' <param name="bounds">
    ''' The bounds of the rectangle.
    ''' </param>
    '''
    ''' <param name="radius">
    ''' The radius of the corners.
    ''' It is limited to half of the smallest side of the rectangle; zero creates a regular rectangle.
    ''' </param>
    ''' <returns>
    ''' The resulting <see cref="GraphicsPath"/>. The caller is responsible for disposing it.
    ''' </returns>
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' The width or height of <paramref name="bounds"/> is not greater than zero, or <paramref name="radius"/> is negative.
    ''' </exception>
    Friend Shared Function CreateRoundedRectangle(bounds As RectangleF, radius As Single) As GraphicsPath

        If (bounds.Width <= 0) OrElse (bounds.Height <= 0) Then
            Throw New ArgumentOutOfRangeException(NameOf(bounds), bounds, "The width and height must be greater than zero.")
        End If

        If (radius < 0) OrElse Single.IsNaN(radius) Then
            Throw New ArgumentOutOfRangeException(NameOf(radius), radius, "The radius cannot be negative.")
        End If

        Dim path As New GraphicsPath()
        Dim effectiveRadius As Single = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2.0F)

        If effectiveRadius = 0 Then
            path.AddRectangle(bounds)
            Return path
        End If

        Dim diameter As Single = effectiveRadius * 2.0F
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90)
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90)
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90)
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90)
        path.CloseFigure()

        Return path

    End Function

    ''' <summary>
    ''' Gets the text format flags used to draw the text of a control,
    ''' hiding the mnemonic prefix underline when the keyboard cues are not shown.
    ''' </summary>
    ''' <param name="showKeyboardCues">
    ''' A value indicating whether the keyboard cues are shown.
    ''' </param>
    ''' <returns>
    ''' The <see cref="TextFormatFlags"/>.
    ''' </returns>
    Friend Shared Function GetTextFormatFlags(showKeyboardCues As Boolean) As TextFormatFlags

        Dim flags As TextFormatFlags = TextFormatFlags.Left Or
                                       TextFormatFlags.VerticalCenter Or
                                       TextFormatFlags.SingleLine Or
                                       TextFormatFlags.NoPadding

        If Not showKeyboardCues Then
            flags = flags Or TextFormatFlags.HidePrefix
        End If

        Return flags

    End Function

#End Region

    ''' <summary>
    ''' The ellipsis used to compact a path.
    ''' </summary>
    Friend Const Ellipsis As String = "..."

    ''' <summary>
    ''' Compacts a file system path with an ellipsis, so it fits in the specified width when drawn with the specified font.
    ''' <para></para>
    ''' The root of the path and as many trailing segments as possible are kept
    ''' (for example: <c>C:\...\Folder\File.txt</c>).
    ''' If not even the last segment fits, it is truncated at the end.
    ''' </summary>
    ''' <param name="path">
    ''' The path to compact.
    ''' </param>
    '''
    ''' <param name="font">
    ''' The font used to draw the path.
    ''' </param>
    '''
    ''' <param name="maxWidth">
    ''' The maximum width, in pixels.
    ''' </param>
    ''' <returns>
    ''' The same path if it already fits; otherwise, the compacted path.
    ''' </returns>
    Friend Shared Function CompactPath(path As String, font As Font, maxWidth As Integer) As String

        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If
        If font Is Nothing Then
            Throw New ArgumentNullException(NameOf(font))
        End If
        If maxWidth <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(maxWidth), maxWidth, "The width must be greater than zero.")
        End If

        If GraphicsHelper.FitsInWidth(path, font, maxWidth) Then
            Return path
        End If

        ' Split the path in its root (for example "C:\" or "\\Server\Share\") and the remaining segments.
        Dim root As String = GraphicsHelper.GetPathRootText(path)
        Dim segments As String() = path.Substring(root.Length).Split({"\"c}, StringSplitOptions.RemoveEmptyEntries)
        If segments.Length = 0 Then
            Return GraphicsHelper.TruncateEnd(path, String.Empty, font, maxWidth)
        End If

        ' Keep as many trailing segments as possible.
        For count As Integer = segments.Length - 1 To 1 Step -1
            Dim tail As String = String.Join("\", segments, segments.Length - count, count)
            Dim candidate As String = $"{root}{GraphicsHelper.Ellipsis}\{tail}"
            If GraphicsHelper.FitsInWidth(candidate, font, maxWidth) Then
                Return candidate
            End If
        Next count

        ' Not even the last segment fits: truncate it.
        Return GraphicsHelper.TruncateEnd(segments(segments.Length - 1), $"{GraphicsHelper.Ellipsis}\", font, maxWidth)

    End Function

    ''' <summary>
    ''' Determines whether a single-line text fits in the specified width when drawn with the specified font.
    ''' </summary>
    ''' <param name="text">
    ''' The text.
    ''' </param>
    '''
    ''' <param name="font">
    ''' The font.
    ''' </param>
    '''
    ''' <param name="maxWidth">
    ''' The maximum width, in pixels.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the text fits; otherwise, <see langword="False"/>.
    ''' </returns>
    Friend Shared Function FitsInWidth(text As String, font As Font, maxWidth As Integer) As Boolean

        Dim size As Size = TextRenderer.MeasureText(text, font, New Size(Integer.MaxValue, Integer.MaxValue),
                                                    TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding)
        Return size.Width <= maxWidth

    End Function

    ''' <summary>
    ''' Gets the root of a path without validating its characters:
    ''' a drive (<c>C:\</c>), an UNC share (<c>\\Server\Share\</c>), or an empty string.
    ''' </summary>
    ''' <param name="path">
    ''' The path.
    ''' </param>
    ''' <returns>
    ''' The root of the path.
    ''' </returns>
    Private Shared Function GetPathRootText(path As String) As String

        If path.StartsWith("\\", StringComparison.Ordinal) Then
            ' UNC share: "\\Server\Share\".
            Dim separatorCount As Integer = 0
            For index As Integer = 2 To path.Length - 1
                If path(index) = "\"c Then
                    separatorCount += 1
                    If separatorCount = 2 Then
                        Return path.Substring(0, index + 1)
                    End If
                End If
            Next index
            Return path

        ElseIf (path.Length >= 3) AndAlso (path(1) = ":"c) AndAlso (path(2) = "\"c) Then
            Return path.Substring(0, 3)

        Else
            Return String.Empty

        End If

    End Function

    ''' <summary>
    ''' Truncates a text at the end with an ellipsis (after the specified prefix) so it fits in the specified width.
    ''' </summary>
    ''' <param name="text">
    ''' The text to truncate.
    ''' </param>
    '''
    ''' <param name="prefix">
    ''' The prefix that precedes the text.
    ''' </param>
    '''
    ''' <param name="font">
    ''' The font.
    ''' </param>
    '''
    ''' <param name="maxWidth">
    ''' The maximum width, in pixels.
    ''' </param>
    ''' <returns>
    ''' The truncated text, or just an ellipsis if nothing fits.
    ''' </returns>
    Private Shared Function TruncateEnd(text As String, prefix As String, font As Font, maxWidth As Integer) As String

        ' Binary search of the longest beginning of the text that fits.
        Dim low As Integer = 0
        Dim high As Integer = text.Length
        While low < high
            Dim middle As Integer = (low + high + 1) \ 2
            If GraphicsHelper.FitsInWidth($"{prefix}{text.Substring(0, middle)}{GraphicsHelper.Ellipsis}", font, maxWidth) Then
                low = middle
            Else
                high = middle - 1
            End If
        End While

        Dim result As String = $"{prefix}{text.Substring(0, low)}{GraphicsHelper.Ellipsis}"
        Return If(GraphicsHelper.FitsInWidth(result, font, maxWidth), result, GraphicsHelper.Ellipsis)

    End Function

End Class

#End Region
