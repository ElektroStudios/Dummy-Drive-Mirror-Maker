#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Reflection

#End Region

#Region " UIMessagesTests "

''' <summary>
''' Unit tests for <see cref="UIMessages"/>.
''' </summary>
<TestClass>
Public NotInheritable Class UIMessagesTests

#Region " Constants "

    ''' <summary>
    ''' The number of messages declared in <see cref="UIMessages"/>.
    ''' </summary>
    Private Const ExpectedMessageCount As Integer = 12

    ''' <summary>
    ''' The names of the messages that are used as composite format strings with one argument.
    ''' </summary>
    Private Shared ReadOnly FormattedMessageNames As String() = {
        NameOf(UIMessages.MirrorCompletedError),
        NameOf(UIMessages.MirrorCompletedWithIgnoredItems),
        NameOf(UIMessages.DirectoryExistQuestion1),
        NameOf(UIMessages.DirectoryExistQuestion2),
        NameOf(UIMessages.DirectoryException),
        NameOf(UIMessages.FileException)
    }

#End Region

#Region " Helpers "

    ''' <summary>
    ''' Gets all the message fields declared in <see cref="UIMessages"/>.
    ''' </summary>
    ''' <returns>
    ''' The message fields.
    ''' </returns>
    Private Shared Function GetMessageFields() As FieldInfo()
        Return GetType(UIMessages).GetFields(BindingFlags.Static Or BindingFlags.NonPublic Or BindingFlags.Public).
                                   Where(Function(fi As FieldInfo) fi.FieldType Is GetType(String)).
                                   ToArray()
    End Function

#End Region

#Region " Tests "

    <TestMethod>
    Public Sub AllMessages_AreNotEmpty()
        Dim fields As FieldInfo() = UIMessagesTests.GetMessageFields()

        Assert.AreEqual(UIMessagesTests.ExpectedMessageCount, fields.Length, "Unexpected message count.")
        For Each field As FieldInfo In fields
            Dim value As String = DirectCast(field.GetValue(Nothing), String)
            Assert.IsFalse(String.IsNullOrWhiteSpace(value), $"Message '{field.Name}' is empty.")
        Next field
    End Sub

    <TestMethod>
    Public Sub FormattedMessages_FormatWithOneArgument_ContainArgument()
        Const argument As String = "C:\Mirror\Some Folder"

        For Each name As String In UIMessagesTests.FormattedMessageNames
            Dim format As String = DirectCast(GetType(UIMessages).GetField(name, BindingFlags.Static Or BindingFlags.NonPublic).GetValue(Nothing), String)
            Dim result As String = String.Format(CultureInfo.InvariantCulture, format, argument)

            If format.Contains("{0}") Then
                StringAssert.Contains(result, argument, $"Message '{name}' does not contain the argument.")
            End If
            Assert.IsFalse(result.Contains("{0}"), $"Message '{name}' was not formatted.")
        Next name
    End Sub

    <TestMethod>
    Public Sub MessagesWithPlaceholder_UsedWithPathArgument_ContainPlaceholder()
        Dim messagesWithPath As String() = {
            UIMessages.MirrorCompletedError,
            UIMessages.DirectoryExistQuestion1,
            UIMessages.DirectoryException,
            UIMessages.FileException
        }

        For Each message As String In messagesWithPath
            StringAssert.Contains(message, "{0}")
        Next message
    End Sub

    <TestMethod>
    Public Sub PlainMessages_DoNotContainFormatPlaceholders()
        Dim plainMessages As String() =
            UIMessagesTests.GetMessageFields().
                            Where(Function(fi As FieldInfo) Not UIMessagesTests.FormattedMessageNames.Contains(fi.Name)).
                            Select(Function(fi As FieldInfo) DirectCast(fi.GetValue(Nothing), String)).
                            ToArray()

        Assert.IsTrue(plainMessages.Length > 0)
        For Each message As String In plainMessages
            Assert.IsFalse(message.Contains("{") OrElse message.Contains("}"), $"Unexpected placeholder in message: {message}")
        Next message
    End Sub

    <TestMethod>
    Public Sub ConflictMessages_AreDistinct()
        Dim messages As String() = {UIMessages.ButtonDestinationConflict1,
                                    UIMessages.ButtonDestinationConflict2,
                                    UIMessages.ButtonDestinationConflict3}

        Assert.AreEqual(messages.Length, messages.Distinct().Count())
    End Sub

#End Region

End Class

#End Region
