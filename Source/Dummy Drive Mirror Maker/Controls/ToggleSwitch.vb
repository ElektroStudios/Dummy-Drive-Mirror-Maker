#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

#End Region

#Region " ToggleSwitch "

''' <summary>
''' An on/off toggle switch (Windows 11 style), implemented as a custom-painted <see cref="CheckBox"/>.
''' <para></para>
''' It keeps all the behavior of a check box: <see cref="CheckBox.Checked"/> property,
''' <see cref="CheckBox.CheckedChanged"/> event, keyboard support (Space key) and accessibility.
''' </summary>
<DesignerCategory("Code")>
<ToolboxItem(True)>
Public NotInheritable Class ToggleSwitch : Inherits CheckBox : Implements IThemedControl ' NOSONAR

#Region " Constants "

    ''' <summary>
    ''' The width of the switch track, in pixels.
    ''' </summary>
    Friend Const TrackWidth As Integer = 36

    ''' <summary>
    ''' The height of the switch track, in pixels.
    ''' </summary>
    Friend Const TrackHeight As Integer = 18

    ''' <summary>
    ''' The space between the switch track and the text, in pixels.
    ''' </summary>
    Friend Const TrackTextSpacing As Integer = 8

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the color palette currently applied to the control.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend ReadOnly Property Palette As ThemePalette = ThemePalette.Light

    ''' <summary>
    ''' Gets or sets the text displayed when the switch is on (checked).
    ''' <para></para>
    ''' When both <see cref="ToggleSwitch.CheckedText"/> and <see cref="ToggleSwitch.UncheckedText"/> are empty, 
    ''' the <see cref="Control.Text"/> property is not modified automatically.
    ''' </summary>
    <Category("Appearance")>
    <Description("The text displayed when the switch is on (checked).")>
    <DefaultValue("")>
    Public Property CheckedText As String
        Get
            Return Me.checkedTextB
        End Get
        Set(value As String)
            Me.checkedTextB = If(value, String.Empty)
            Me.UpdateStateText()
        End Set
    End Property
    Private checkedTextB As String = String.Empty

    ''' <summary>
    ''' Gets or sets the text displayed when the switch is off (unchecked).
    ''' <para></para>
    ''' When both <see cref="ToggleSwitch.CheckedText"/> and <see cref="ToggleSwitch.UncheckedText"/> are empty, 
    ''' the <see cref="Control.Text"/> property is not modified automatically.
    ''' </summary>
    <Category("Appearance")>
    <Description("The text displayed when the switch is off (unchecked).")>
    <DefaultValue("")>
    Public Property UncheckedText As String
        Get
            Return Me.uncheckedTextB
        End Get
        Set(value As String)
            Me.uncheckedTextB = If(value, String.Empty)
            Me.UpdateStateText()
        End Set
    End Property
    Private uncheckedTextB As String = String.Empty

#End Region

#Region " Fields "

    ''' <summary>
    ''' A value indicating whether the mouse pointer is over the control.
    ''' </summary>
    Private isMouseOver As Boolean

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ToggleSwitch"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.ResizeRedraw, True)
        Me.AutoSize = True
        Me.Cursor = Cursors.Hand
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Sets the <see cref="Control.Text"/> property according to the current state of the switch, 
    ''' if <see cref="ToggleSwitch.CheckedText"/> or <see cref="ToggleSwitch.UncheckedText"/> are set.
    ''' </summary>
    Private Sub UpdateStateText()

        If String.IsNullOrEmpty(Me.checkedTextB) AndAlso String.IsNullOrEmpty(Me.uncheckedTextB) Then
            Exit Sub
        End If

        Me.Text = If(Me.Checked, Me.checkedTextB, Me.uncheckedTextB)

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
        Me.Invalidate()

    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Retrieves the size of a rectangular area into which the control can be fitted.
    ''' </summary>
    ''' <param name="proposedSize">
    ''' The custom-sized area for the control.
    ''' </param>
    ''' <returns>
    ''' The preferred size of the control.
    ''' </returns>
    Public Overrides Function GetPreferredSize(proposedSize As Size) As Size

        If String.IsNullOrEmpty(Me.Text) Then
            Return New Size(ToggleSwitch.TrackWidth + 2, ToggleSwitch.TrackHeight + 4)
        End If

        Dim textSize As Size = TextRenderer.MeasureText(Me.Text, Me.Font, Size.Empty, GraphicsHelper.GetTextFormatFlags(showKeyboardCues:=True))
        Return New Size(ToggleSwitch.TrackWidth + ToggleSwitch.TrackTextSpacing + textSize.Width + 4,
                        Math.Max(ToggleSwitch.TrackHeight + 4, textSize.Height + 4))

    End Function

    ''' <summary>
    ''' Raises the <see cref="CheckBox.CheckedChanged"/> event, updating the text according to the new state.
    ''' </summary>
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnCheckedChanged(e As EventArgs)
        Me.UpdateStateText()
        MyBase.OnCheckedChanged(e)
        Me.Invalidate()
    End Sub

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

        Dim g As Graphics = pevent.Graphics
        Dim p As ThemePalette = Me.Palette
        Dim isEnabled As Boolean = Me.Enabled

        Using backBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        ' Track.
        Dim trackBounds As New RectangleF(0.5F, ((Me.ClientSize.Height - ToggleSwitch.TrackHeight) \ 2) + 0.5F,
                                          ToggleSwitch.TrackWidth - 1, ToggleSwitch.TrackHeight - 1)
        Dim trackFill As Color
        Dim trackBorder As Color
        Dim thumbColor As Color

        If Not isEnabled Then
            trackFill = If(Me.Checked, p.DisabledBackColor, p.InputBackColor)
            trackBorder = p.DisabledForeColor
            thumbColor = p.DisabledForeColor

        ElseIf Me.Checked Then
            trackFill = If(Me.isMouseOver, p.AccentHoverBackColor, p.AccentBackColor)
            trackBorder = trackFill
            thumbColor = p.AccentForeColor

        Else
            trackFill = If(Me.isMouseOver, p.ControlHoverBackColor, p.InputBackColor)
            trackBorder = p.SecondaryForeColor
            thumbColor = p.SecondaryForeColor

        End If

        g.SmoothingMode = SmoothingMode.AntiAlias
        Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(trackBounds, trackBounds.Height / 2.0F),
              fillBrush As New SolidBrush(trackFill),
              borderPen As New Pen(trackBorder)
            g.FillPath(fillBrush, path)
            g.DrawPath(borderPen, path)
        End Using

        ' Thumb.
        Const thumbDiameter As Single = 10.0F
        Const thumbMargin As Single = 4.0F
        Dim thumbX As Single = If(Me.Checked,
                                  trackBounds.Right - thumbMargin - thumbDiameter,
                                  trackBounds.X + thumbMargin)
        Dim thumbY As Single = trackBounds.Y + ((trackBounds.Height - thumbDiameter) / 2.0F)
        Using thumbBrush As New SolidBrush(thumbColor)
            g.FillEllipse(thumbBrush, thumbX, thumbY, thumbDiameter, thumbDiameter)
        End Using
        g.SmoothingMode = SmoothingMode.Default

        ' Text.
        If String.IsNullOrEmpty(Me.Text) Then
            Exit Sub
        End If

        Dim flags As TextFormatFlags = GraphicsHelper.GetTextFormatFlags(Me.ShowKeyboardCues)
        Dim textX As Integer = ToggleSwitch.TrackWidth + ToggleSwitch.TrackTextSpacing
        Dim textBounds As New Rectangle(textX, 0, Math.Max(0, Me.ClientSize.Width - textX), Me.ClientSize.Height)
        TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds, If(isEnabled, p.WindowForeColor, p.DisabledForeColor), flags)

        ' Focus cues.
        If Me.Focused AndAlso Me.ShowFocusCues Then
            Dim textSize As Size = TextRenderer.MeasureText(g, Me.Text, Me.Font, textBounds.Size, flags)
            Dim focusBounds As New Rectangle(textX - 2, (Me.ClientSize.Height - textSize.Height) \ 2 - 1,
                                             Math.Min(textSize.Width + 4, textBounds.Width), textSize.Height + 2)
            ControlPaint.DrawFocusRectangle(g, focusBounds, p.WindowForeColor, Me.BackColor)
        End If

    End Sub

#End Region

End Class

#End Region
