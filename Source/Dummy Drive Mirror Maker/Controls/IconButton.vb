#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

#End Region

#Region " IconButton "

''' <summary>
''' A flat, borderless, custom-painted button that only shows an icon,
''' with a different image for the light and dark themes.
''' </summary>
<DesignerCategory("Code")>
<ToolboxItem(True)>
Public NotInheritable Class IconButton : Inherits Button : Implements IThemedControl

#Region " Properties "

    ''' <summary>
    ''' Gets the color palette currently applied to the control.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend ReadOnly Property Palette As ThemePalette = ThemePalette.Light

    ''' <summary>
    ''' Gets or sets the image shown in the light theme (it should be a dark image).
    ''' </summary>
    <Category("Appearance")>
    <Description("The image shown in the light theme (it should be a dark image).")>
    Public Property LightThemeImage As Image
        Get
            Return Me.lightThemeImageB
        End Get
        Set(value As Image)
            Me.lightThemeImageB = value
            Me.Invalidate()
        End Set
    End Property
    Private lightThemeImageB As Image

    ''' <summary>
    ''' Gets or sets the image shown in the dark theme (it should be a light image).
    ''' </summary>
    <Category("Appearance")>
    <Description("The image shown in the dark theme (it should be a light image).")>
    Public Property DarkThemeImage As Image
        Get
            Return Me.darkThemeImageB
        End Get
        Set(value As Image)
            Me.darkThemeImageB = value
            Me.Invalidate()
        End Set
    End Property
    Private darkThemeImageB As Image

    ''' <summary>
    ''' Gets or sets the size in which the image is drawn, in pixels.
    ''' </summary>
    <Category("Appearance")>
    <Description("The size in which the image is drawn, in pixels.")>
    <DefaultValue(GetType(Size), "20, 20")>
    Public Property IconSize As Size
        Get
            Return Me.iconSizeB
        End Get
        Set(value As Size)
            If (value.Width <= 0) OrElse (value.Height <= 0) Then
                Throw New ArgumentOutOfRangeException(NameOf(value), value, "The width and height must be greater than zero.")
            End If
            Me.iconSizeB = value
            Me.Invalidate()
        End Set
    End Property
    Private iconSizeB As New Size(20, 20)

    ''' <summary>
    ''' Gets the image that corresponds to the current theme.
    ''' </summary>
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Friend ReadOnly Property CurrentImage As Image
        Get
            Return If(Me.Palette.IsDark, Me.darkThemeImageB, Me.lightThemeImageB)
        End Get
    End Property

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
    ''' Initializes a new instance of the <see cref="IconButton"/> class.
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
    ''' Raises the <see cref="Control.Paint"/> event.
    ''' </summary>
    ''' <param name="pevent">
    ''' A <see cref="PaintEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        Dim p As ThemePalette = Me.Palette

        Using backBrush As New SolidBrush(Me.BackColor)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        ' Hover / pressed background.
        If Me.Enabled AndAlso (Me.isMouseOver OrElse Me.isPressed) Then
            g.SmoothingMode = SmoothingMode.AntiAlias
            Dim bounds As New RectangleF(0, 0, Me.ClientSize.Width - 0.5F, Me.ClientSize.Height - 0.5F)
            Using path As GraphicsPath = GraphicsHelper.CreateRoundedRectangle(bounds, 4.0F),
                  hoverBrush As New SolidBrush(If(Me.isPressed, p.ControlPressedBackColor, p.ControlHoverBackColor))
                g.FillPath(hoverBrush, path)
            End Using
            g.SmoothingMode = SmoothingMode.Default
        End If

        ' Icon.
        Dim image As Image = Me.CurrentImage
        If image IsNot Nothing Then
            Dim imageBounds As New Rectangle((Me.ClientSize.Width - Me.iconSizeB.Width) \ 2,
                                             (Me.ClientSize.Height - Me.iconSizeB.Height) \ 2,
                                             Me.iconSizeB.Width, Me.iconSizeB.Height)
            If Me.Enabled Then
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.DrawImage(image, imageBounds)
            Else
                ControlPaint.DrawImageDisabled(g, image, imageBounds.X, imageBounds.Y, Me.BackColor)
            End If
        End If

        ' Focus cues.
        If Me.Focused AndAlso Me.ShowFocusCues Then
            ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(Me.ClientRectangle, -2, -2), p.WindowForeColor, Me.BackColor)
        End If

    End Sub

#End Region

End Class

#End Region
