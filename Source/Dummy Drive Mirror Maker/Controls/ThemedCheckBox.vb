#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

#End Region

#Region " ThemedCheckBox "

''' <summary>
''' A <see cref="CheckBox"/> that keeps the native look of Windows in the light theme,
''' and is custom-painted in the dark theme (the native check box ignores the custom colors).
''' </summary>
<DesignerCategory("Code")>
<ToolboxItem(True)>
Public NotInheritable Class ThemedCheckBox : Inherits CheckBox : Implements IThemedControl

#Region " Constants "

    ''' <summary>
    ''' The size of the check box glyph, in pixels.
    ''' </summary>
    Friend Const GlyphSize As Integer = 13

    ''' <summary>
    ''' The space between the check box glyph and the text, in pixels.
    ''' </summary>
    Friend Const GlyphTextSpacing As Integer = 5

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the color palette currently applied to the control.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend ReadOnly Property Palette As ThemePalette = ThemePalette.Light

#End Region

#Region " Fields "

    ''' <summary>
    ''' A value indicating whether the mouse pointer is over the control.
    ''' </summary>
    Private isMouseOver As Boolean

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ThemedCheckBox"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
    End Sub

#End Region

#Region " IThemedControl Implementation "

    ''' <summary>
    ''' Applies the specified color palette to the control.
    ''' </summary>
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="palette"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub ApplyPalette(palette As ThemePalette) Implements IThemedControl.ApplyPalette

        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        Me._Palette = palette
        Me.BackColor = palette.WindowBackColor
        Me.ForeColor = palette.WindowForeColor
        Me.UseVisualStyleBackColor = Not palette.IsDark
        Me.Invalidate()

    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Retrieves the size of a rectangular area into which the control can be fitted,
    ''' ensuring that the custom-painted glyph and text always fit.
    ''' </summary>
    ''' <param name="proposedSize">
    ''' The custom-sized area for the control.
    ''' </param>
    ''' <returns>
    ''' The preferred size of the control.
    ''' </returns>
    Public Overrides Function GetPreferredSize(proposedSize As Size) As Size

        Dim baseSize As Size = MyBase.GetPreferredSize(proposedSize)
        Dim textSize As Size = TextRenderer.MeasureText(Me.Text, Me.Font, Size.Empty, GraphicsHelper.GetTextFormatFlags(showKeyboardCues:=True))
        Dim customWidth As Integer = ThemedCheckBox.GlyphSize + ThemedCheckBox.GlyphTextSpacing + textSize.Width + 2

        Return New Size(Math.Max(baseSize.Width, customWidth), Math.Max(baseSize.Height, textSize.Height + 2))

    End Function

    ''' <summary>
    ''' Raises the <see cref="Control.MouseEnter"/> event.
    ''' </summary>
    ''' <param name="eventargs">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseEnter(eventargs As EventArgs)
        Me.isMouseOver = True
        MyBase.OnMouseEnter(eventargs)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.MouseLeave"/> event.
    ''' </summary>
    ''' <param name="eventargs">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseLeave(eventargs As EventArgs)
        Me.isMouseOver = False
        MyBase.OnMouseLeave(eventargs)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.Paint"/> event.
    ''' </summary>
    ''' <param name="pevent">
    ''' A <see cref="PaintEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        ' The light theme keeps the native look of Windows.
        If Not Me.Palette.IsDark Then
            MyBase.OnPaint(pevent)
            Exit Sub
        End If

        Dim g As Graphics = pevent.Graphics
        Dim p As ThemePalette = Me.Palette
        Dim isEnabled As Boolean = Me.Enabled

        Using backBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        ' Glyph.
        Dim glyphBounds As New RectangleF(0.5F, ((Me.ClientSize.Height - ThemedCheckBox.GlyphSize) \ 2) + 0.5F,
                                          ThemedCheckBox.GlyphSize - 1, ThemedCheckBox.GlyphSize - 1)
        Dim isMarked As Boolean = (Me.CheckState <> CheckState.Unchecked)

        Dim fillColor As Color = If(isMarked,
                                    If(isEnabled, p.AccentBackColor, p.DisabledBackColor),
                                    p.InputBackColor)

        Dim borderColor As Color
        If Not isEnabled Then
            borderColor = p.DisabledForeColor
        ElseIf isMarked Then
            borderColor = fillColor
        ElseIf Me.isMouseOver Then
            borderColor = p.AccentBackColor
        Else
            borderColor = p.ControlBorderColor
        End If

        g.SmoothingMode = SmoothingMode.AntiAlias
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(glyphBounds, 2.5F),
              fillBrush As New SolidBrush(fillColor),
              borderPen As New Pen(borderColor)
            g.FillPath(fillBrush, path)
            g.DrawPath(borderPen, path)
        End Using

        Dim markColor As Color = If(isEnabled, p.AccentForeColor, p.DisabledForeColor)
        Select Case Me.CheckState
            Case CheckState.Checked
                Using markPen As New Pen(markColor, 1.6F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round, .LineJoin = LineJoin.Round}
                    g.DrawLines(markPen, {New PointF(glyphBounds.X + 2.5F, glyphBounds.Y + 6.0F),
                                          New PointF(glyphBounds.X + 5.0F, glyphBounds.Y + 8.5F),
                                          New PointF(glyphBounds.X + 9.5F, glyphBounds.Y + 3.5F)})
                End Using

            Case CheckState.Indeterminate
                Using markBrush As New SolidBrush(markColor)
                    g.FillRectangle(markBrush, glyphBounds.X + 3.0F, glyphBounds.Y + 5.0F, glyphBounds.Width - 6.0F, 2.0F)
                End Using

        End Select
        g.SmoothingMode = SmoothingMode.Default

        ' Text.
        Dim flags As TextFormatFlags = GraphicsHelper.GetTextFormatFlags(Me.ShowKeyboardCues)
        Dim textBounds As New Rectangle(ThemedCheckBox.GlyphSize + ThemedCheckBox.GlyphTextSpacing, 0,
                                        Math.Max(0, Me.ClientSize.Width - ThemedCheckBox.GlyphSize - ThemedCheckBox.GlyphTextSpacing),
                                        Me.ClientSize.Height)
        TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds, If(isEnabled, p.WindowForeColor, p.DisabledForeColor), flags)

        ' Focus cues.
        If Me.Focused AndAlso Me.ShowFocusCues Then
            Dim textSize As Size = TextRenderer.MeasureText(g, Me.Text, Me.Font, textBounds.Size, flags)
            Dim focusBounds As New Rectangle(textBounds.X - 2, (Me.ClientSize.Height - textSize.Height) \ 2 - 1,
                                             Math.Min(textSize.Width + 4, textBounds.Width + 2), textSize.Height + 2)
            ControlPaint.DrawFocusRectangle(g, focusBounds, p.WindowForeColor, Me.BackColor)
        End If

    End Sub

#End Region

End Class

#End Region
