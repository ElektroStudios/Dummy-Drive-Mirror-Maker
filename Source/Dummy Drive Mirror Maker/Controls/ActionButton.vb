#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

#End Region

#Region " ActionButtonKind "

''' <summary>
''' Specifies the visual kind of an <see cref="ActionButton"/>.
''' </summary>
Public Enum ActionButtonKind As Integer

    ''' <summary>
    ''' A primary action, drawn with the accent color.
    ''' </summary>
    Primary = 0

    ''' <summary>
    ''' A destructive or cancel action, drawn with the danger color.
    ''' </summary>
    Danger = 1

    ''' <summary>
    ''' A secondary action, drawn with the neutral control colors and a border.
    ''' </summary>
    Secondary = 2

End Enum

#End Region

#Region " ActionButtonGlyph "

''' <summary>
''' Specifies the vector glyph drawn at the left of the text of an <see cref="ActionButton"/>.
''' </summary>
Public Enum ActionButtonGlyph As Integer

    ''' <summary>
    ''' No glyph.
    ''' </summary>
    None = 0

    ''' <summary>
    ''' A "play" (start) triangle.
    ''' </summary>
    Play = 1

    ''' <summary>
    ''' A "stop" square.
    ''' </summary>
    [Stop] = 2

    ''' <summary>
    ''' A "report" (document with lines of text).
    ''' </summary>
    Report = 3

End Enum

#End Region

#Region " ActionButton "

''' <summary>
''' A flat, custom-painted button with rounded corners and a vector glyph,
''' intended for the main action of a window. Its colors come from the applied <see cref="ThemePalette"/>
''' and its <see cref="ActionButton.Kind"/>.
''' </summary>
<DesignerCategory("Code")>
<ToolboxItem(True)>
Public NotInheritable Class ActionButton : Inherits Button : Implements IThemedControl

#Region " Constants "

    ''' <summary>
    ''' The size of the glyph, in pixels.
    ''' </summary>
    Friend Const GlyphSize As Integer = 10

    ''' <summary>
    ''' The space between the glyph and the text, in pixels.
    ''' </summary>
    Friend Const GlyphTextSpacing As Integer = 8

    ''' <summary>
    ''' The radius of the rounded corners, in pixels.
    ''' </summary>
    Friend Const CornerRadius As Single = 4.0F

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the color palette currently applied to the control.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend ReadOnly Property Palette As ThemePalette = ThemePalette.Light

    ''' <summary>
    ''' Gets or sets the visual kind of the button.
    ''' </summary>
    <Category("Appearance")>
    <Description("The visual kind of the button.")>
    <DefaultValue(GetType(ActionButtonKind), "Primary")>
    Public Property Kind As ActionButtonKind
        Get
            Return Me.kindB
        End Get
        Set(value As ActionButtonKind)
            If Not [Enum].IsDefined(GetType(ActionButtonKind), value) Then
                Throw New InvalidEnumArgumentException(NameOf(value), CInt(value), GetType(ActionButtonKind))
            End If
            Me.kindB = value
            Me.Invalidate()
        End Set
    End Property
    Private kindB As ActionButtonKind = ActionButtonKind.Primary

    ''' <summary>
    ''' Gets or sets the vector glyph drawn at the left of the text.
    ''' </summary>
    <Category("Appearance")>
    <Description("The vector glyph drawn at the left of the text.")>
    <DefaultValue(GetType(ActionButtonGlyph), "None")>
    Public Property Glyph As ActionButtonGlyph
        Get
            Return Me.glyphB
        End Get
        Set(value As ActionButtonGlyph)
            If Not [Enum].IsDefined(GetType(ActionButtonGlyph), value) Then
                Throw New InvalidEnumArgumentException(NameOf(value), CInt(value), GetType(ActionButtonGlyph))
            End If
            Me.glyphB = value
            Me.Invalidate()
        End Set
    End Property
    Private glyphB As ActionButtonGlyph = ActionButtonGlyph.None

#End Region

#Region " Fields "

    ''' <summary>
    ''' A value indicating whether the mouse pointer is over the control.
    ''' </summary>
    Private isMouseOver As Boolean

    ''' <summary>
    ''' A value indicating whether the left mouse button is pressed over the control.
    ''' </summary>
    Private isPressed As Boolean

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ActionButton"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.ResizeRedraw, True)
        Me.Cursor = Cursors.Hand
    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Gets the background and text colors of the button for its current state.
    ''' </summary>
    ''' <param name="backColor">
    ''' Receives the background color.
    ''' </param>
    '''
    ''' <param name="foreColor">
    ''' Receives the text and glyph color.
    ''' </param>
    Friend Sub GetStateColors(ByRef backColor As Color, ByRef foreColor As Color)

        Dim p As ThemePalette = Me.Palette

        If Not Me.Enabled Then
            backColor = p.DisabledBackColor
            foreColor = p.DisabledForeColor
            Exit Sub
        End If

        Select Case Me.kindB
            Case ActionButtonKind.Primary
                backColor = If(Me.isPressed, p.AccentPressedBackColor, If(Me.isMouseOver, p.AccentHoverBackColor, p.AccentBackColor))
                foreColor = p.AccentForeColor

            Case ActionButtonKind.Danger
                backColor = If(Me.isPressed, p.DangerPressedBackColor, If(Me.isMouseOver, p.DangerHoverBackColor, p.DangerBackColor))
                foreColor = p.DangerForeColor

            Case ActionButtonKind.Secondary
                backColor = If(Me.isPressed, p.ControlPressedBackColor, If(Me.isMouseOver, p.ControlHoverBackColor, p.ControlBackColor))
                foreColor = p.WindowForeColor

            Case Else
                Throw New InvalidOperationException($"Unexpected button kind: {Me.kindB}")

        End Select

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
        Me.Invalidate()

    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.MouseEnter"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        Me.isMouseOver = True
        MyBase.OnMouseEnter(e)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.MouseLeave"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        Me.isMouseOver = False
        Me.isPressed = False
        MyBase.OnMouseLeave(e)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.MouseDown"/> event.
    ''' </summary>
    ''' <param name="mevent">
    ''' A <see cref="MouseEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseDown(mevent As MouseEventArgs)
        If mevent.Button = MouseButtons.Left Then
            Me.isPressed = True
            Me.Invalidate()
        End If
        MyBase.OnMouseDown(mevent)
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.MouseUp"/> event.
    ''' </summary>
    ''' <param name="mevent">
    ''' A <see cref="MouseEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnMouseUp(mevent As MouseEventArgs)
        Me.isPressed = False
        Me.Invalidate()
        MyBase.OnMouseUp(mevent)
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.EnabledChanged"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        If Not Me.Enabled Then
            Me.isPressed = False
        End If
        MyBase.OnEnabledChanged(e)
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.Paint"/> event.
    ''' </summary>
    ''' <param name="pevent">
    ''' A <see cref="PaintEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        Dim fillColor As Color
        Dim contentColor As Color
        Me.GetStateColors(fillColor, contentColor)

        ' Background of the parent behind the rounded corners.
        Using backBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim bounds As New RectangleF(0, 0, Me.ClientSize.Width - 0.5F, Me.ClientSize.Height - 0.5F)
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(bounds, ActionButton.CornerRadius),
              fillBrush As New SolidBrush(fillColor)
            g.FillPath(fillBrush, path)

            ' The secondary kind has a border, to distinguish it from the window background.
            If (Me.kindB = ActionButtonKind.Secondary) Then
                Using borderPen As New Pen(If(Me.Enabled, Me.Palette.ControlBorderColor, Me.Palette.DisabledForeColor))
                    g.DrawPath(borderPen, path)
                End Using
            End If
        End Using

        ' Content layout (glyph + text, centered).
        Dim flags As TextFormatFlags = GraphicsHelper.GetTextFormatFlags(Me.ShowKeyboardCues)
        Dim textSize As Size = TextRenderer.MeasureText(g, Me.Text, Me.Font, Size.Empty, flags)
        Dim hasGlyph As Boolean = (Me.glyphB <> ActionButtonGlyph.None)
        Dim glyphWidth As Integer = If(hasGlyph, ActionButton.GlyphSize + ActionButton.GlyphTextSpacing, 0)
        Dim contentX As Integer = (Me.ClientSize.Width - (glyphWidth + textSize.Width)) \ 2
        Dim centerY As Single = Me.ClientSize.Height / 2.0F

        If hasGlyph Then
            Using glyphBrush As New SolidBrush(contentColor)
                Dim half As Single = ActionButton.GlyphSize / 2.0F
                Select Case Me.glyphB
                    Case ActionButtonGlyph.Play
                        g.FillPolygon(glyphBrush, {New PointF(contentX + 1, centerY - half),
                                                   New PointF(contentX + 1, centerY + half),
                                                   New PointF(contentX + ActionButton.GlyphSize, centerY)})

                    Case ActionButtonGlyph.Stop
                        Using glyphPath As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(
                            New RectangleF(contentX, centerY - half, ActionButton.GlyphSize, ActionButton.GlyphSize), 1.5F)
                            g.FillPath(glyphBrush, glyphPath)
                        End Using

                    Case ActionButtonGlyph.Report
                        ' A sheet of paper (outline) with three lines of text.
                        Dim sheetWidth As Single = ActionButton.GlyphSize * 0.8F
                        Dim sheetLeft As Single = contentX + (ActionButton.GlyphSize - sheetWidth) / 2.0F
                        Using glyphPen As New Pen(contentColor, 1.5F),
                              sheetPath As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(
                                  New RectangleF(sheetLeft, centerY - half, sheetWidth, ActionButton.GlyphSize), 1.5F)
                            g.DrawPath(glyphPen, sheetPath)
                            For lineIndex As Integer = 1 To 3
                                Dim lineY As Single = centerY - half + (ActionButton.GlyphSize * lineIndex / 4.0F)
                                Dim lineRight As Single = sheetLeft + sheetWidth - 2.5F - If(lineIndex = 3, sheetWidth * 0.25F, 0F)
                                g.DrawLine(glyphPen, sheetLeft + 2.5F, lineY, lineRight, lineY)
                            Next lineIndex
                        End Using

                End Select
            End Using
        End If
        g.SmoothingMode = SmoothingMode.Default

        Dim textBounds As New Rectangle(contentX + glyphWidth, 0, textSize.Width + 1, Me.ClientSize.Height)
        TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds, contentColor, flags)

        ' Focus cues.
        If Me.Focused AndAlso Me.ShowFocusCues Then
            Dim focusBounds As Rectangle = Rectangle.Inflate(Me.ClientRectangle, -3, -3)
            ControlPaint.DrawFocusRectangle(g, focusBounds, contentColor, fillColor)
        End If

    End Sub

#End Region

End Class

#End Region
