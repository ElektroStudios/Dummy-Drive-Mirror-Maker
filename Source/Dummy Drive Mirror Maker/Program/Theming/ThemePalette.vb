#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " ThemePalette "

''' <summary>
''' Represents the set of colors that defines a visual theme of the application.
''' <para></para>
''' Use the predefined <see cref="ThemePalette.Light"/> and <see cref="ThemePalette.Dark"/> palettes.
''' </summary>
Friend NotInheritable Class ThemePalette

#Region " Predefined Palettes "

    ''' <summary>
    ''' Gets the light palette, based on the default Windows system colors.
    ''' </summary>
    Friend Shared ReadOnly Property Light As New ThemePalette(
        isDark:=False,
        windowBackColor:=SystemColors.Control,
        windowForeColor:=SystemColors.ControlText,
        secondaryForeColor:=SystemColors.GrayText,
        separatorColor:=Color.FromArgb(204, 204, 204),
        inputBackColor:=SystemColors.Window,
        inputForeColor:=SystemColors.WindowText,
        controlBackColor:=SystemColors.Control,
        controlHoverBackColor:=Color.FromArgb(222, 222, 222),
        controlPressedBackColor:=Color.FromArgb(204, 204, 204),
        controlBorderColor:=Color.FromArgb(138, 138, 138),
        disabledBackColor:=Color.FromArgb(204, 204, 204),
        disabledForeColor:=SystemColors.GrayText,
        accentBackColor:=Color.FromArgb(0, 103, 192),
        accentHoverBackColor:=Color.FromArgb(25, 117, 197),
        accentPressedBackColor:=Color.FromArgb(0, 84, 158),
        accentForeColor:=Color.White,
        dangerBackColor:=Color.FromArgb(196, 43, 28),
        dangerHoverBackColor:=Color.FromArgb(209, 59, 44),
        dangerPressedBackColor:=Color.FromArgb(164, 33, 21),
        dangerForeColor:=Color.White,
        statusBackColor:=SystemColors.Control,
        statusForeColor:=SystemColors.ControlText,
        progressTrackColor:=SystemColors.Control,
        progressBarColor:=Color.FromArgb(40, 110, 50),
        progressWarningColor:=Color.FromArgb(225, 170, 40),
        progressErrorColor:=Color.FromArgb(230, 110, 100),
        progressTextColor:=SystemColors.ControlText,
        progressTextOutlineColor:=Color.Empty
    )

    ''' <summary>
    ''' Gets the dark palette.
    ''' </summary>
    Friend Shared ReadOnly Property Dark As New ThemePalette(
        isDark:=True,
        windowBackColor:=Color.FromArgb(32, 32, 32),
        windowForeColor:=Color.FromArgb(240, 240, 240),
        secondaryForeColor:=Color.FromArgb(166, 166, 166),
        separatorColor:=Color.FromArgb(64, 64, 64),
        inputBackColor:=Color.FromArgb(45, 45, 45),
        inputForeColor:=Color.FromArgb(240, 240, 240),
        controlBackColor:=Color.FromArgb(45, 45, 45),
        controlHoverBackColor:=Color.FromArgb(58, 58, 58),
        controlPressedBackColor:=Color.FromArgb(70, 70, 70),
        controlBorderColor:=Color.FromArgb(96, 96, 96),
        disabledBackColor:=Color.FromArgb(58, 58, 58),
        disabledForeColor:=Color.FromArgb(120, 120, 120),
        accentBackColor:=Color.FromArgb(20, 100, 180),
        accentHoverBackColor:=Color.FromArgb(31, 113, 194),
        accentPressedBackColor:=Color.FromArgb(16, 84, 152),
        accentForeColor:=Color.White,
        dangerBackColor:=Color.FromArgb(196, 43, 28),
        dangerHoverBackColor:=Color.FromArgb(209, 59, 44),
        dangerPressedBackColor:=Color.FromArgb(164, 33, 21),
        dangerForeColor:=Color.White,
        statusBackColor:=Color.FromArgb(43, 43, 43),
        statusForeColor:=Color.FromArgb(200, 200, 200),
        progressTrackColor:=Color.FromArgb(45, 45, 45),
        progressBarColor:=Color.FromArgb(40, 110, 50),
        progressWarningColor:=Color.FromArgb(176, 140, 40),
        progressErrorColor:=Color.FromArgb(170, 40, 30),
        progressTextColor:=Color.FromArgb(240, 240, 240),
        progressTextOutlineColor:=Color.FromArgb(16, 16, 16)
    )

#End Region

#Region " Properties "

    ''' <summary>Gets a value indicating whether this is a dark palette.</summary>
    Friend ReadOnly Property IsDark As Boolean

    ''' <summary>Gets the background color of windows and containers.</summary>
    Friend ReadOnly Property WindowBackColor As Color

    ''' <summary>Gets the main text color.</summary>
    Friend ReadOnly Property WindowForeColor As Color

    ''' <summary>Gets the color for secondary (less important) text and glyphs.</summary>
    Friend ReadOnly Property SecondaryForeColor As Color

    ''' <summary>Gets the color of separator lines and subtle borders.</summary>
    Friend ReadOnly Property SeparatorColor As Color

    ''' <summary>Gets the background color of input controls, like text boxes and check boxes.</summary>
    Friend ReadOnly Property InputBackColor As Color

    ''' <summary>Gets the text color of input controls.</summary>
    Friend ReadOnly Property InputForeColor As Color

    ''' <summary>Gets the background color of standard buttons.</summary>
    Friend ReadOnly Property ControlBackColor As Color

    ''' <summary>Gets the background color of standard buttons when the mouse is over them.</summary>
    Friend ReadOnly Property ControlHoverBackColor As Color

    ''' <summary>Gets the background color of standard buttons when they are pressed.</summary>
    Friend ReadOnly Property ControlPressedBackColor As Color

    ''' <summary>Gets the border color of standard buttons and input controls.</summary>
    Friend ReadOnly Property ControlBorderColor As Color

    ''' <summary>Gets the background color of disabled controls.</summary>
    Friend ReadOnly Property DisabledBackColor As Color

    ''' <summary>Gets the text color of disabled controls.</summary>
    Friend ReadOnly Property DisabledForeColor As Color

    ''' <summary>Gets the background color of accent (primary action) elements.</summary>
    Friend ReadOnly Property AccentBackColor As Color

    ''' <summary>Gets the background color of accent elements when the mouse is over them.</summary>
    Friend ReadOnly Property AccentHoverBackColor As Color

    ''' <summary>Gets the background color of accent elements when they are pressed.</summary>
    Friend ReadOnly Property AccentPressedBackColor As Color

    ''' <summary>Gets the text color of accent elements.</summary>
    Friend ReadOnly Property AccentForeColor As Color

    ''' <summary>Gets the background color of danger (destructive action) elements.</summary>
    Friend ReadOnly Property DangerBackColor As Color

    ''' <summary>Gets the background color of danger elements when the mouse is over them.</summary>
    Friend ReadOnly Property DangerHoverBackColor As Color

    ''' <summary>Gets the background color of danger elements when they are pressed.</summary>
    Friend ReadOnly Property DangerPressedBackColor As Color

    ''' <summary>Gets the text color of danger elements.</summary>
    Friend ReadOnly Property DangerForeColor As Color

    ''' <summary>Gets the background color of the status bar.</summary>
    Friend ReadOnly Property StatusBackColor As Color

    ''' <summary>Gets the text color of the status bar.</summary>
    Friend ReadOnly Property StatusForeColor As Color

    ''' <summary>Gets the background (track) color of progress bars.</summary>
    Friend ReadOnly Property ProgressTrackColor As Color

    ''' <summary>Gets the bar color of progress bars.</summary>
    Friend ReadOnly Property ProgressBarColor As Color

    ''' <summary>Gets the bar color of progress bars in warning (paused) state: some items were ignored.</summary>
    Friend ReadOnly Property ProgressWarningColor As Color

    ''' <summary>Gets the bar color of progress bars in error state: some items failed.</summary>
    Friend ReadOnly Property ProgressErrorColor As Color

    ''' <summary>Gets the text color of progress bars.</summary>
    Friend ReadOnly Property ProgressTextColor As Color

    ''' <summary>
    ''' Gets the color of the 1 pixel outline drawn around the text of progress bars, 
    ''' or <see cref="Color.Empty"/> for no outline.
    ''' </summary>
    Friend ReadOnly Property ProgressTextOutlineColor As Color

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ThemePalette"/> class.
    ''' <para></para>
    ''' Each parameter sets the property with the same name.
    ''' </summary>
    Private Sub New(isDark As Boolean,
                    windowBackColor As Color,
                    windowForeColor As Color,
                    secondaryForeColor As Color,
                    separatorColor As Color,
                    inputBackColor As Color,
                    inputForeColor As Color,
                    controlBackColor As Color,
                    controlHoverBackColor As Color,
                    controlPressedBackColor As Color,
                    controlBorderColor As Color,
                    disabledBackColor As Color,
                    disabledForeColor As Color,
                    accentBackColor As Color,
                    accentHoverBackColor As Color,
                    accentPressedBackColor As Color,
                    accentForeColor As Color,
                    dangerBackColor As Color,
                    dangerHoverBackColor As Color,
                    dangerPressedBackColor As Color,
                    dangerForeColor As Color,
                    statusBackColor As Color,
                    statusForeColor As Color,
                    progressTrackColor As Color,
                    progressBarColor As Color,
                    progressWarningColor As Color,
                    progressErrorColor As Color,
                    progressTextColor As Color,
                    progressTextOutlineColor As Color)
        Me.IsDark = isDark
        Me.WindowBackColor = windowBackColor
        Me.WindowForeColor = windowForeColor
        Me.SecondaryForeColor = secondaryForeColor
        Me.SeparatorColor = separatorColor
        Me.InputBackColor = inputBackColor
        Me.InputForeColor = inputForeColor
        Me.ControlBackColor = controlBackColor
        Me.ControlHoverBackColor = controlHoverBackColor
        Me.ControlPressedBackColor = controlPressedBackColor
        Me.ControlBorderColor = controlBorderColor
        Me.DisabledBackColor = disabledBackColor
        Me.DisabledForeColor = disabledForeColor
        Me.AccentBackColor = accentBackColor
        Me.AccentHoverBackColor = accentHoverBackColor
        Me.AccentPressedBackColor = accentPressedBackColor
        Me.AccentForeColor = accentForeColor
        Me.DangerBackColor = dangerBackColor
        Me.DangerHoverBackColor = dangerHoverBackColor
        Me.DangerPressedBackColor = dangerPressedBackColor
        Me.DangerForeColor = dangerForeColor
        Me.StatusBackColor = statusBackColor
        Me.StatusForeColor = statusForeColor
        Me.ProgressTrackColor = progressTrackColor
        Me.ProgressBarColor = progressBarColor
        Me.ProgressWarningColor = progressWarningColor
        Me.ProgressErrorColor = progressErrorColor
        Me.ProgressTextColor = progressTextColor
        Me.ProgressTextOutlineColor = progressTextOutlineColor
    End Sub

#End Region

End Class

#End Region
