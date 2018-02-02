




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

Imports System.ComponentModel
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

Imports ElektroKit.Interop.Win32
Imports ElektroKit.Interop.Win32.Enums
Imports ElektroKit.Interop.Win32.Types
Imports ElektroKit.UserControls.Enums
Imports ElektroKit.UserControls.Types
Imports ElektroKit.UserControls.Types.Designers
Imports ElektroKit.UserControls.Types.EventArgs

#End Region

#Region " ElektroProgressBar "

Namespace ElektroKit.UserControls.Controls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' A extended <see cref="ProgressBar"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <DisplayName("ElektroProgressBar")>
    <Description("A extended ProgressBar control.")>
    <DesignTimeVisible(True)>
    <DesignerCategory("UserControl")>
    <Designer(GetType(ElektroProgressBarDesigner))>
    <ToolboxBitmap(GetType(ProgressBar), "ProgressBar.bmp")>
    <ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Require)>
    <ClassInterface(ClassInterfaceType.AutoDispatch)>
    <ComVisible(True)>
    <DefaultBindingProperty("Value")>
    <DefaultProperty("Value")>
    <DefaultEvent("ValueChanged")>
    <PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
    Public Class ElektroProgressBar : Inherits ProgressBar : Implements IBufferedControl

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the required creation parameters when the control handle is created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The creation parameters.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <Description("The required creation parameters when the control handle is created.")>
        Protected Overrides ReadOnly Property CreateParams As CreateParams Implements IBufferedControl.CreateParams
            Get
                If (Me.preventFlickeringB) Then
                    Dim cp As CreateParams = MyBase.CreateParams
                    cp.ExStyle = (cp.ExStyle Or CInt(WindowStylesEx.Composited))
                    Return cp
                Else
                    Return MyBase.CreateParams
                End If
            End Get
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value indicating whether this control should redraw its surface using a secondary buffer 
        ''' to reduce or prevent flicker.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' <see langword="True"/> if the surface of the control should be drawn using double buffering; 
        ''' otherwise, <see langword="False"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Behavior")>
        <Description("Indicates whether this control should redraw its surface using a secondary buffer to reduce or prevent flicker.")>
        <DefaultValue(GetType(Boolean), "True")>
        Public Overridable Shadows Property DoubleBuffered As Boolean Implements IBufferedControl.DoubleBuffered
            Get
                Return MyBase.DoubleBuffered
            End Get
            Set(ByVal value As Boolean)
                Me.SetStyle(ControlStyles.DoubleBuffer, value)
                MyBase.DoubleBuffered = value
            End Set
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value that indicates whether the control should avoid unwanted flickering effects.
        ''' <para></para>
        ''' If <see langword="True"/>, this will avoid any flickering effect on the control, however,
        ''' it will also have a negative impact by slowing down the responsiveness of the control about to 30% slower.
        ''' <para></para>
        ''' This negative impact doesn't affect to the performance of the application itself, 
        ''' just to the performance of this control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' A value that indicates whether the control should avoid unwanted flickering effects.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(True)>
        <Category("(Extended) Behavior")>
        <Description("Indicates whether the control should avoid unwanted flickering effects. If True, this will avoid any flickering effect on the control, however, it will also have a negative impact by slowing down the responsiveness of the control about to 30% slower.")>
        <DefaultValue(GetType(Boolean), "False")>
        Public Overridable Property PreventFlickering As Boolean Implements IBufferedControl.PreventFlickering
            Get
                Return Me.preventFlickeringB
            End Get
            Set(ByVal value As Boolean)
                Me.preventFlickeringB = value
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field )
        ''' A value that indicates whether the control should avoid unwanted flickering effects.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private preventFlickeringB As Boolean

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the current value of the progress bar.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The current value of the progress bar.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("Behavior")>
        <Description("The current value of the progress bar.")>
        <DefaultValue(GetType(Integer), "0")>
        Public Shadows Property Value As Integer
            Get
                Return MyBase.Value
            End Get
            <DebuggerStepThrough>
            Set(ByVal value As Integer)
                Dim oldValue As Integer = MyBase.Value
                MyBase.Value = value
                If (value <> oldValue) Then
                    If MyBase.DesignMode Then
                        MyBase.Invalidate(invalidateChildren:=False)
                    End If
                    Me.OnValueChanged(New ProgressBarValueChangedEventArgs(value))
                End If
            End Set
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the default size of the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The default size of the control.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overrides ReadOnly Property DefaultSize As Size
            Get
                Return New Size(200, MyBase.DefaultSize.Height)
            End Get
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current percentage of the progress bar.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The current percentage of the progress bar.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Localizable(False)>
        <Category("(Extended) Behavior")>
        <Description("The current percentage of the progress bar.")>
        <DefaultValue(GetType(Double), "0")>
        Public ReadOnly Property Percentage As Double
            Get
                Return ((MyBase.Value / MyBase.Maximum) * 100) '((MyBase.Value - MyBase.Minimum) / (MyBase.Maximum - MyBase.Minimum)) * 100
            End Get
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the percentage format string that will be displayed on the control 
        ''' when its current state is <see cref="ProgressBarState.Normal"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Normal"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The percentage format string that will be displayed on the control when its current state is 'Normal'. #current=CurrentValue #total=MaximumValue #%=CurrentPercentage")>
        <DefaultValue(GetType(String), "(#current / #total) #%")>
        Public Overridable Property FormatString As String
            Get
                Return Me.formatStringB
            End Get
            Set(ByVal value As String)
                Me.formatStringB = value
                If (MyBase.Visible) Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field)
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Normal"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private formatStringB As String

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the percentage format string that will be displayed on the control 
        ''' when its current state is <see cref="ProgressBarState.Error"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Error"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The percentage format string that will be displayed on the control when its current state is 'Error'. #current=CurrentValue #total=MaximumValue #%=CurrentPercentage")>
        <DefaultValue(GetType(String), "(#current / #total) #%")>
        Public Overridable Property FormatStringError As String
            Get
                Return Me.formatStringErrorB
            End Get
            Set(ByVal value As String)
                Me.formatStringErrorB = value
                If (MyBase.Visible) Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field)
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Error"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private formatStringErrorB As String

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the percentage format string that will be displayed on the control 
        ''' when its current state is <see cref="ProgressBarState.Paused"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Paused"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The percentage format string that will be displayed on the control when its current state is 'Paused'. #current=CurrentValue #total=MaximumValue #%=CurrentPercentage")>
        <DefaultValue(GetType(String), "(#current / #total) #%")>
        Public Overridable Property FormatStringPaused As String
            Get
                Return Me.formatStringPausedB
            End Get
            Set(ByVal value As String)
                Me.formatStringPausedB = value
                If (MyBase.Visible) Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field)
        ''' The percentage format string that will be displayed in the control 
        ''' when its current state is <see cref="ProgressBarState.Paused"/>.
        ''' <para></para>
        ''' #current=Current Value 
        ''' #total=Maximum Value 
        ''' #%=Current Percentage 
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private formatStringPausedB As String

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the <see cref="System.Drawing.Font"/> of the text displayed by the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The <see cref="System.Drawing.Font"/> of the text displayed by the control.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The font of the text displayed by the control.")>
        Public Overrides Property Font As Font
            Get
                Return MyBase.Font
            End Get
            Set(ByVal value As Font)
                MyBase.Font = value
                If MyBase.Visible Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the <see cref="ElektroProgressBar.Font"/> property to its default value.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/53b8022e(v=vs.110).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        Public Overrides Sub ResetFont()
            Me.Font = Nothing
        End Sub
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Checks whether the <see cref="ElektroProgressBar.Font"/> property has changed from its default value 
        ''' and write code into the Form only if the property is changed, thus allowing for more efficient code generation.
        ''' <para></para>
        ''' The designer writes code to the form only if <see langword="True"/> is returned.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/53b8022e(v=vs.110).aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' Returns <see langword="True"/> if the font has changed; otherwise, returns <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Private Function ShouldSerializeFont() As Boolean
            Return Not (Me.Font Is Control.DefaultFont)
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the color of the text displayed by the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The color of the text displayed by the control.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The color of the text displayed by the control.")>
        <DefaultValue(GetType(Color), "ControlText")>
        Public Overrides Property ForeColor As Color
            Get
                Return MyBase.ForeColor
            End Get
            Set(ByVal value As Color)
                MyBase.ForeColor = value
                If MyBase.Visible Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the state (and fill color) of the progress bar.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The state (and fill color) of the progress bar.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The state (and fill color) of the progress bar..")>
        <DefaultValue(GetType(ProgressBarState), "Normal")>
        Public Overridable Property State As ProgressBarState
            Get
                Return Me.stateB
            End Get
            Set(ByVal value As ProgressBarState)
                Me.stateB = value
                NativeMethods.SendMessage(Me.Handle, ProgressBarUIMessages.SetState, New IntPtr(Me.stateB), IntPtr.Zero)
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field )
        ''' The state (and fill color) of the progress bar.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private stateB As ProgressBarState

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the border color of the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The border color of the control.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("The border color of the control.")>
        <DefaultValue(GetType(Color), "ControlLight")>
        Public Overridable Property BorderColor As Color
            Get
                Return Me.borderColorB
            End Get
            Set(ByVal value As Color)
                Me.borderColorB = value
                If MyBase.Visible Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field )
        ''' The border color of the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private borderColorB As Color

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value indicating whether the border of the control is shown.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' <see langword="True"/> if the border of the control is shown; otherwise <see langword="False"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <Localizable(False)>
        <Category("(Extended) Appearance")>
        <Description("A value indicating whether the border of the control is shown.")>
        <DefaultValue(GetType(Boolean), "False")>
        Public Overridable Property ShowBorder As Boolean
            Get
                Return Me.showBorderB
            End Get
            Set(ByVal value As Boolean)
                Me.showBorderB = value
                If MyBase.Visible Then
                    MyBase.Invalidate(invalidateChildren:=False)
                End If
            End Set
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field )
        ''' A value indicating whether the border of the control is shown.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private showBorderB As Boolean

#End Region

#Region " Events "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Occurs when the progress bar value changes.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <example> This is a code example.
        ''' <code>
        ''' ''' ----------------------------------------------------------------------------------------------------
        ''' ''' &lt;summary&gt;
        ''' ''' Handles the &lt;see cref="ElektroProgressBar.ValueChanged"/&gt; event of the &lt;see cref="ElektroProgressBar1"/&gt; control.
        ''' ''' &lt;/summary&gt;
        ''' ''' ----------------------------------------------------------------------------------------------------
        ''' ''' &lt;param name="sender"&gt;
        ''' ''' The source of the event.
        ''' ''' &lt;/param&gt;
        ''' ''' 
        ''' ''' &lt;param name="e"&gt;
        ''' ''' The &lt;see cref="ProgressBarValueChangedEventArgs"/&gt; instance containing the event data.
        ''' ''' &lt;/param&gt;
        ''' ''' ----------------------------------------------------------------------------------------------------
        ''' Private Sub ElektroProgressBar1_ValueChanged(sender As Object, e As ProgressBarValueChangedEventArgs) _
        ''' Handles ElektroProgressBar1.ValueChanged
        ''' 
        '''     ' Dim pb As ElektroProgressBar = DirectCast(sender, ElektroProgressBar)
        '''     Console.WriteLine(e.NewValue)
        '''     
        ''' End Sub
        ''' </code>
        ''' </example>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Localizable(True)>
        <Category("Behavior")>
        <Description("Occurs when the progress value changes.")>
        Public Custom Event ValueChanged As EventHandler(Of ProgressBarValueChangedEventArgs)

            <DebuggerNonUserCode>
            <DebuggerStepThrough>
            AddHandler(ByVal value As EventHandler(Of ProgressBarValueChangedEventArgs))
                MyBase.Events.AddHandler("ValueChangedEvent", value)
            End AddHandler

            <DebuggerNonUserCode>
            <DebuggerStepThrough>
            RemoveHandler(ByVal value As EventHandler(Of ProgressBarValueChangedEventArgs))
                MyBase.Events.RemoveHandler("ValueChangedEvent", value)
            End RemoveHandler

            <DebuggerNonUserCode>
            <DebuggerStepThrough>
            RaiseEvent(ByVal sender As Object, ByVal e As ProgressBarValueChangedEventArgs)
                Dim handler As EventHandler(Of ProgressBarValueChangedEventArgs) =
                DirectCast(MyBase.Events("ValueChangedEvent"), EventHandler(Of ProgressBarValueChangedEventArgs))

                If (handler IsNot Nothing) Then
                    handler.Invoke(sender, e)
                End If
            End RaiseEvent

        End Event

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ElektroProgressBar"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Sub New()

            MyBase.SuspendLayout()

            MyBase.DoubleBuffered = True
            Me.preventFlickeringB = False
            MyBase.Step = 1
            Me.formatStringB = "(#current / #total) #%"
            Me.formatStringPausedB = "(#current / #total) #%"
            Me.formatStringErrorB = "(#current / #total) #%"
            MyBase.ForeColor = SystemColors.ControlText
            Me.stateB = ProgressBarState.Normal
            MyBase.Font = Control.DefaultFont
            Me.borderColorB = SystemColors.ControlLight
            Me.showBorderB = False

            MyBase.ResumeLayout(performLayout:=False)

        End Sub

#End Region

#Region " Event Invocators "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises <see cref="ElektroProgressBar.ValueChanged"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' The <see cref="ProgressBarValueChangedEventArgs"/> instance containing the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Protected Overridable Sub OnValueChanged(ByVal e As ProgressBarValueChangedEventArgs)

            RaiseEvent ValueChanged(Me, e)

        End Sub

#End Region

#Region " Window Procedure (WndProc) "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the default window procedure associated with this window to process windows UIMessages.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="m">
        ''' A <see cref="Message"/> that is associated with the current Windows message.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Protected Overrides Sub WndProc(ByRef m As Message)

            Select Case m.Msg
                Case WindowsMessages.WM_Paint
                    Me.DoWmPaint(m)
                    Return

                Case WindowsMessages.WM_PrintClient
                    Me.DoWmPrintClient(m)
                    Return

            End Select

            MyBase.WndProc(m)

        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Advances the current position of the progress bar by the specified amount.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="value">
        ''' The amount by which to increment the progress bar's current position.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Shadows Sub Increment(ByVal value As Integer)

            Dim oldValue As Integer = MyBase.Value
            MyBase.Increment(value)

            If (oldValue <> MyBase.Maximum) Then
                Me.OnValueChanged(New ProgressBarValueChangedEventArgs(MyBase.Value))
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Advances the current position of the progress bar by the amount of the 
        ''' <see cref="ElektroProgressBar.Step"/> property.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Shadows Sub PerformStep()

            Dim oldValue As Integer = MyBase.Value
            MyBase.PerformStep()

            If (oldValue <> MyBase.Maximum) Then
                Me.OnValueChanged(New ProgressBarValueChangedEventArgs(MyBase.Value))
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Returns a string representation for this <see cref="ElektroProgressBar"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' A <see cref="String" /> that describes this control.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Public Overrides Function ToString() As String

            Dim sb As New StringBuilder()
            With sb
                .Append([GetType]().FullName)
                .Append(", Minimum: ")
                .Append(MyBase.Minimum.ToString(CultureInfo.CurrentCulture))
                .Append(", Maximum: ")
                .Append(MyBase.Maximum.ToString(CultureInfo.CurrentCulture))
                .Append(", Value: ")
                .Append(MyBase.Value.ToString(CultureInfo.CurrentCulture))
            End With

            Return sb.ToString()

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the percentage string that will be drawn inside the progress bar rectangle.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The percentage string that will be drawn inside the progress bar rectangle.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Private Function GetPercentageString() As String

            Dim formatString As String = String.Empty
            Select Case Me.stateB

                Case ProgressBarState.Normal
                    formatString = Me.formatStringB

                Case ProgressBarState.Paused
                    formatString = Me.formatStringPausedB

                Case Else 'ProgressBarState.Error
                    formatString = Me.formatStringErrorB

            End Select

            Dim format As String = formatString.Replace("#current", "{0}").
                                                Replace("#total", "{1}").
                                                Replace("#%", "{2}%")

            Return String.Format(format, MyBase.Value, MyBase.Maximum, CInt(Me.Percentage))

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Draws the text on the progress bar rectangle.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="g">
        ''' The graphics surface.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Private Sub DrawText(ByVal g As Graphics)
            TextRenderer.DrawText(g, Me.GetPercentageString(), Me.Font, MyBase.ClientRectangle, Me.ForeColor)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Draws a border on the control surface.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Private Sub DrawBorder(ByVal g As Graphics)
            ControlPaint.DrawBorder(g, MyBase.ClientRectangle, Me.borderColorB, ButtonBorderStyle.Solid)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prepares the <see cref="WindowsMessages.WM_Paint"/> message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="m">
        ''' A <see cref="Message"/> that is associated with the current Windows message.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Private Sub DoWmPaint(ByRef m As Message)

            ' Create a wrapper for the Handle.
            Dim myHandle As New HandleRef(Me, MyBase.Handle)

            ' Prepare the window for painting and retrieve a device context.
            Dim ps As New PaintStruct()
            Dim hdc As IntPtr = NativeMethods.BeginPaint(myHandle, ps)

            Try
                ' Apply device context to message.
                m.WParam = hdc

                ' Let Windows paint.
                MyBase.WndProc(m)

                ' Custom painting.
                Using g As Graphics = Graphics.FromHdc(hdc)
                    Me.DrawText(g)
                    If (Me.showBorderB) Then
                        Me.DrawBorder(g)
                    End If
                End Using

            Finally
                ' Release the device context retrieved by BeginPaint function .
                NativeMethods.EndPaint(myHandle, ps)

            End Try

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prepares the <see cref="WindowsMessages.WM_PrintClient"/> message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="m">
        ''' A <see cref="Message"/> that is associated with the current Windows message.
        ''' </param>
        <DebuggerStepperBoundary>
        Private Sub DoWmPrintClient(ByRef m As Message)

            ' Retrieve the device context.
            Dim hdc As IntPtr = m.WParam

            ' Let Windows paint.
            MyBase.WndProc(m)

            ' Custom painting.
            Using g As Graphics = Graphics.FromHdc(hdc)
                Me.DrawText(g)
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region