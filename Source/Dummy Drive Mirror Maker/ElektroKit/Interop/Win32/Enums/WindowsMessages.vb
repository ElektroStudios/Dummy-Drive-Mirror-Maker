




' THIS OPEN-SOURCE APPLICATION IS POWERED BY DEVCASE CLASS LIBRARY, CREATED BY ELEKTRO STUDIOS.

' WHAT YOU SEE HERE IS FREE CUTTED CONTENT OF THIS FRAMEWORK.

' IF YOU LIKED THIS FREE APPLICATION, THEN PLEASE CONSIDER TO BUY DEVCASE CLASS LIBRARY FOR .NET AT:
' https://codecanyon.net/item/DevCase-class-library-for-net/19260282

' YOU CAN FIND THESE HELPER METHODS AND A MASSIVE AMOUNT MORE!, 
' +850 EXTENSION METHODS FOR ALL KIND OF TYPES, CUSTOM USER-CONTROLS, 
' EVERYTHING FOR THE NEWBIE And THE ADVANCED USER, FOR VB.NET AND C#. 

' DevCase is a utility framework containing new APIs and extensions to the core .NET Framework 
' to help complete your developer toolbox.
' It Is a set of general purpose classes provided as easy to consume packages.
' These utility classes and components provide productivity in day to day software development 
' mainly focused To WindowsForms. 

' UPDATES OF DevCase ARE MAINTAINED AND RELEASED EVERY MONTH.





#Region " Windows UIMessages "

Namespace DevCase.Interop.Win32.Enums

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The system sends or posts a system-defined message when it communicates with an application. 
    ''' <para></para>
    ''' It uses these UIMessages to control the operations of applications and to provide input and other information for applications to process. 
    ''' <para></para>
    ''' An application can also send or post system-defined UIMessages.
    ''' <para></para>
    ''' Applications generally use these UIMessages to control the operation of control windows created by using preregistered window classes.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <remarks>
    ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644927%28v=vs.85%29.aspx"/>
    ''' </remarks>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum WindowsMessages As Integer

        ' *****************************************************************************
        '                            WARNING!, NEED TO KNOW...
        '
        '  THIS ENUMERATION IS PARTIALLY DEFINED TO MEET THE PURPOSES OF THIS API
        ' *****************************************************************************

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="Null"/> message performs no operation.
        ''' <para></para>
        ''' An application sends the <see cref="Null"/> message if it wants to 
        ''' post a message that the recipient window will ignore.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Null = &H0

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Occurs when the control needs repainting.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href=""/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_Paint = &HF

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The message is sent to a window to request that it draw its client area in the specified device context 
        ''' most commonly in a printer device context.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href=""/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_PrintClient = &H318

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the font that a control is to use when drawing text.
        ''' <para></para>
        ''' 
        ''' <c>wParam</c> 
        ''' A handle to the font (HFONT).
        ''' <para></para>
        ''' If this parameter is <see langword="Nothing"/>, the control uses the default system font to draw text. 
        ''' <para></para>
        ''' 
        ''' <c>lParam</c> 
        ''' The low-order word of lParam specifies whether the control should be redrawn immediately upon setting the font. 
        ''' <para></para>
        ''' If this parameter is TRUE, the control redraws itself.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632642%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_SetFont = &H30

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the text of a window.
        ''' <para></para>
        ''' 
        ''' <c>wParam</c> 
        ''' This parameter is not used
        ''' <para></para>
        ''' 
        ''' <c>lParam</c> 
        ''' A pointer to a null-terminated string that is the window text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632644%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_SetText = &HC

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Copies the text that corresponds to a window into a buffer provided by the caller.
        ''' <para></para>
        ''' 
        ''' <c>wParam</c> 
        ''' The maximum number of characters to be copied, including the terminating null character.
        ''' <para></para>
        ''' ANSI applications may have the string in the buffer reduced in size 
        ''' (to a minimum of half that of the wParam value) due to conversion from ANSI to Unicode.
        ''' <para></para>
        ''' 
        ''' <c>lParam</c> 
        ''' A pointer to the buffer that is to receive the text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632627%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_GetText = &HD

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines the length, in characters, of the text associated with a window.
        ''' <para></para>
        ''' 
        ''' <c>wParam</c> 
        ''' This parameter is not used and must be zero
        ''' <para></para>
        ''' 
        ''' <c>lParam</c> 
        ''' This parameter is not used and must be zero
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632628%28v=vs.85%29.aspx"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        WM_GetTextLength = &HE

    End Enum

End Namespace

#End Region
