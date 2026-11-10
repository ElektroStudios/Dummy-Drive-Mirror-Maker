#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " DestinationValidationResult "

''' <summary>
''' Specifies the result of validating a destination directory against a source directory.
''' </summary>
''' <seealso cref="MirrorRules.ValidateDestination(String, String)"/>
Friend Enum DestinationValidationResult As Integer ' NOSONAR

    ''' <summary>
    ''' The destination directory is valid.
    ''' </summary>
    Valid = 0

    ''' <summary>
    ''' The destination directory is the same as the source directory.
    ''' </summary>
    SameAsSource = 1

    ''' <summary>
    ''' The destination directory is located inside the source directory.
    ''' </summary>
    InsideSource = 2

    ''' <summary>
    ''' The destination directory is the parent directory of the source directory.
    ''' </summary>
    ParentOfSource = 3

End Enum

#End Region
