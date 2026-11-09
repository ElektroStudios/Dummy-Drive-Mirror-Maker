#Region " Imports "

Imports System.IO

#End Region

''' <summary>
''' Represents the strings displayed in the <see cref="MessageBox"/> of the <see cref="Main"/> Form.
''' </summary>
Friend NotInheritable Class UIMessages

    Friend Shared MirrorCompletedSuccessful As String =
"Mirror operation completed."

    Friend Shared MirrorCompletedWithIgnoredItems As String =
"Mirror operation completed, but {0}.

Do you want to see the details?"

    Friend Shared MirrorCompletedAborted As String =
"Mirror operation aborted on demand."

    Friend Shared MirrorCompletedError As String =
"Mirror operation terminated due error: {0}"

    Friend Shared ButtonDestinationConflict1 As String =
"The destination directory cannot be the same as the source directory. 
 
Please, try again and select other directory."

    Friend Shared ButtonDestinationConflict2 As String =
"The destination directory cannot be a subfolder of the source directory. 
 
 Please, try again and select other directory."

    Friend Shared ButtonDestinationConflict3 As String =
"The destination directory cannot be the parent folder of the source directory. 
        
 Please, try again and select other directory."

    Friend Shared DirectoryExistQuestion1 As String =
"The destination directory '{0}' already exists. 

Do you want to continue?. Please note that dummy files will be placed there.

(Enable the 'Overwrite destination files' option to skip this question.)"

    Friend Shared DirectoryExistQuestion2 As String =
"Do you want to ignore future directory conflicts like this one?.

(This choice applies only to the current mirror operation.)"

    Friend Shared DirectoryException As String =
"An exception occurred attempting to create the directory. 

Exception message: '{0}'

Do you want to ignore and continue the operation?."

    Friend Shared FileException As String =
"An exception occurred attempting to create the file. 

Exception message: '{0}'. 

Do you want to ignore and continue the operation?."

    Friend Shared FormClosingWarning As String =
"A mirror operation is running. Do you really want to quit the application?."

End Class
