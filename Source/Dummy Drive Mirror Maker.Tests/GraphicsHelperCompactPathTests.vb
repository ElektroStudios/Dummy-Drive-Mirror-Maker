#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " GraphicsHelperCompactPathTests "

''' <summary>
''' Unit tests for <see cref="GraphicsHelper.CompactPath"/>,
''' used to keep long paths (and the icon) visible in the status bar.
''' </summary>
<TestClass>
Public NotInheritable Class GraphicsHelperCompactPathTests

#Region " Fields "

    ''' <summary>
    ''' The font used to measure the texts.
    ''' </summary>
    Private Shared ReadOnly TestFont As New Font("Segoe UI", 9.0!)

    ''' <summary>
    ''' A long path.
    ''' </summary>
    Private Const LongPath As String =
        "C:\Users\Administrator\Documents\Some very long directory name\Another long directory name\Last directory\File name.txt"

#End Region

#Region " Tests "

    <TestMethod>
    Public Sub CompactPath_PathThatFits_IsReturnedUnchanged()
        Assert.AreEqual(LongPath, GraphicsHelper.CompactPath(LongPath, TestFont, 5000))
    End Sub

    <TestMethod>
    <DataRow(400)>
    <DataRow(250)>
    <DataRow(150)>
    Public Sub CompactPath_LongPath_FitsAndKeepsRootAndFileName(maxWidth As Integer)
        Dim result As String = GraphicsHelper.CompactPath(LongPath, TestFont, maxWidth)

        Assert.IsTrue(GraphicsHelper.FitsInWidth(result, TestFont, maxWidth), $"'{result}' does not fit in {maxWidth} px.")
        StringAssert.StartsWith(result, "C:\...\")
        StringAssert.EndsWith(result, "\File name.txt")
    End Sub

    <TestMethod>
    Public Sub CompactPath_KeepsAsManyTrailingSegmentsAsPossible()
        Dim expected As String = "C:\...\Last directory\File name.txt"
        Dim width As Integer = TextRendererWidth(expected)

        Assert.AreEqual(expected, GraphicsHelper.CompactPath(LongPath, TestFont, width))
    End Sub

    <TestMethod>
    Public Sub CompactPath_FileNameDoesNotFit_TruncatesTheFileName()
        Dim result As String = GraphicsHelper.CompactPath(LongPath, TestFont, 60)

        Assert.IsTrue(GraphicsHelper.FitsInWidth(result, TestFont, 60), $"'{result}' does not fit.")
        StringAssert.EndsWith(result, GraphicsHelper.Ellipsis)
    End Sub

    <TestMethod>
    Public Sub CompactPath_UncPath_KeepsTheShare()
        Dim uncPath As String = "\\Server\Share\Some very long directory name\Another long directory name\File name.txt"
        Dim result As String = GraphicsHelper.CompactPath(uncPath, TestFont, 250)

        StringAssert.StartsWith(result, "\\Server\Share\...\")
        StringAssert.EndsWith(result, "\File name.txt")
    End Sub

    <TestMethod>
    Public Sub CompactPath_PlainText_IsTruncatedAtTheEnd()
        Dim text As String = New String("x"c, 500)
        Dim result As String = GraphicsHelper.CompactPath(text, TestFont, 100)

        Assert.IsTrue(GraphicsHelper.FitsInWidth(result, TestFont, 100))
        StringAssert.EndsWith(result, GraphicsHelper.Ellipsis)
    End Sub

    <TestMethod>
    Public Sub CompactPath_InvalidArguments_Throw()
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() GraphicsHelper.CompactPath(Nothing, TestFont, 100))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() GraphicsHelper.CompactPath(LongPath, Nothing, 100))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() GraphicsHelper.CompactPath(LongPath, TestFont, 0))
    End Sub

#End Region

#Region " Helpers "

    ''' <summary>
    ''' Gets the minimum width in which the specified text fits.
    ''' </summary>
    ''' <param name="text">
    ''' The text.
    ''' </param>
    ''' <returns>
    ''' The width, in pixels.
    ''' </returns>
    Private Shared Function TextRendererWidth(text As String) As Integer
        Dim width As Integer = 1
        While Not GraphicsHelper.FitsInWidth(text, TestFont, width)
            width += 1
        End While
        Return width
    End Function

#End Region

End Class

#End Region
