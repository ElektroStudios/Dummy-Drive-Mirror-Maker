#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " FlatToolStripRenderer "

''' <summary>
''' A flat <see cref="ToolStripRenderer"/> that fills the background with the
''' <see cref="Control.BackColor"/> of the tool strip (without gradients) and draws a single top border line.
''' <para></para>
''' Intended for themed <see cref="StatusStrip"/> controls.
''' </summary>
Friend NotInheritable Class FlatToolStripRenderer : Inherits ToolStripRenderer ' NOSONAR

#Region " Properties "

    ''' <summary>
    ''' Gets the color of the top border line.
    ''' </summary>
    Friend ReadOnly Property BorderColor As Color

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="FlatToolStripRenderer"/> class.
    ''' </summary>
    ''' <param name="borderColor">
    ''' The color of the top border line.
    ''' </param>
    Friend Sub New(borderColor As Color)
        MyBase.New()
        Me.BorderColor = borderColor
    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Raises the <see cref="ToolStripRenderer.RenderToolStripBackground"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' A <see cref="ToolStripRenderEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)
        Using backBrush As New SolidBrush(e.ToolStrip.BackColor)
            e.Graphics.FillRectangle(backBrush, e.AffectedBounds)
        End Using
    End Sub

    ''' <summary>
    ''' Raises the <see cref="ToolStripRenderer.RenderToolStripBorder"/> event.
    ''' </summary>
    ''' <param name="e">
    ''' A <see cref="ToolStripRenderEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)
        Using borderPen As New Pen(Me.BorderColor)
            e.Graphics.DrawLine(borderPen, 0, 0, e.ToolStrip.Width, 0)
        End Using
    End Sub

#End Region

End Class

#End Region
