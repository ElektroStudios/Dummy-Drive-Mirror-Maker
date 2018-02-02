#Region " Imports "

Imports System.IO

#End Region

''' <summary>
''' Represents the strings displayed in the <see cref="MessageBox"/> of the <see cref="Main"/> Form.
''' </summary>
Friend NotInheritable Class UIMessages

    Friend Shared MirrorCompletedSuccessful As String =
"Mirror operation completed."

    Friend Shared MirrorCompletedAborted As String =
"Mirror operation aborted by user demand."

    Friend Shared MirrorCompletedError As String =
"Mirror operation terminated due to error: {0}"

    Friend Shared ButtonTargetConflict1 As String =
"The target directory cannot be the same as the source directory. 
 
Please, try again and select other directory."

    Friend Shared ButtonTargetConflict2 As String =
"The target directory cannot be a subfolder of the source directory. 
 
 Please, try again and select other directory."

    Friend Shared ButtonTargetConflict3 As String =
"The target directory cannot be the parent folder of the source directory. 
        
 Please, try again and select other directory."

    Friend Shared DirectoryExistQuestion1 As String =
"The target directorty '{0}' already exists. 

Do you want to continue?, please note that dummy files will be placed there."

    Friend Shared DirectoryExistQuestion2 As String =
"Do you want to ignore next directory conflicts like this?. 

(the answer only affects to the current mirror operation)"

    Friend Shared DirectoryException As String =
"An exception occured attempting to create the directory. 

Exception message: '{0}'

Do you want to ignore and continue the operation?."

    Friend Shared FileExistQuestion1 As String =
"The target file '{0}' already exists. 

Do you want to continue?. (the original file will not be overwritten)"

    Friend Shared FileExistQuestion2 As String =
"Do you want to ignore next file conflicts like this?. 

(the answer only affects to the current mirror operation)."

    Friend Shared FileException As String =
"An exception occured attempting to create the file. 

Exception message: '{0}'. 

Do you want to ignore and continue the operation?."

End Class
