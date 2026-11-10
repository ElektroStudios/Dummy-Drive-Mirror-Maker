#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " IThemedControl "

''' <summary>
''' Represents a control that knows how to apply a <see cref="ThemePalette"/> to itself.
''' <para></para>
''' <see cref="ThemeManager.ApplyTheme(Control, ThemePalette)"/> delegates the theming
''' of any control that implements this interface to the control itself.
''' </summary>
Friend Interface IThemedControl ' NOSONAR

    ''' <summary>
    ''' Applies the specified color palette to the control.
    ''' </summary>
    ''' <param name="palette">
    ''' The color palette to apply.
    ''' </param>
    Sub ApplyPalette(palette As ThemePalette)

End Interface

#End Region
