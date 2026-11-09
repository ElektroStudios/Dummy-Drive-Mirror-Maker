#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

#End Region

#Region " SectionHeaderLabel "

''' <summary>
''' A section header: a text followed by a horizontal separator line that fills the remaining width.
''' <para></para>
''' It replaces the classic <see cref="GroupBox"/>, whose border and text colors cannot be themed.
''' </summary>
<DesignerCategory("Code")>
<ToolboxItem(True)>
Public NotInheritable Class SectionHeaderLabel : Inherits Label : Implements IThemedControl

#Region " Constants "

    ''' <summary>
    ''' The space between the text and the separator line, in pixels.
    ''' </summary>
    Friend Const TextLineSpacing As Integer = 8

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the color of the separator line.
    ''' </summary>
    <Category("Appearance")>
    <Description("The color of the separator line. It is set by the applied theme.")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property LineColor As Color
        Get
            Return Me.lineColorB
        End Get
        Set(value As Color)
            Me.lineColorB = value
            Me.Invalidate()
        End Set
    End Property
    Private lineColorB As Color = ThemePalette.Light.SeparatorColor

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="SectionHeaderLabel"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw, True)
        Me.AutoSize = False
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

        Me.BackColor = palette.WindowBackColor
        Me.ForeColor = palette.WindowForeColor
        Me.LineColor = palette.SeparatorColor

    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.Paint"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' A <see cref="PaintEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        Using backBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        Dim flags As TextFormatFlags = GraphicsHelper.GetTextFormatFlags(showKeyboardCues:=False)
        Dim textSize As Size = TextRenderer.MeasureText(g, Me.Text, Me.Font, Size.Empty, flags)
        TextRenderer.DrawText(g, Me.Text, Me.Font, New Rectangle(0, 0, textSize.Width + 1, Me.ClientSize.Height), Me.ForeColor, flags)

        Dim lineX As Integer = If(textSize.Width > 0, textSize.Width + SectionHeaderLabel.TextLineSpacing, 0)
        If lineX < Me.ClientSize.Width Then
            Dim lineY As Integer = Me.ClientSize.Height \ 2
            Using linePen As New Pen(Me.lineColorB)
                g.DrawLine(linePen, lineX, lineY, Me.ClientSize.Width - 1, lineY)
            End Using
        End If

    End Sub

#End Region

End Class

#End Region
