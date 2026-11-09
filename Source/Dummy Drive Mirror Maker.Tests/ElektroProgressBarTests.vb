#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Globalization

Imports DevCase.UserControls.Controls
Imports DevCase.UserControls.Enums
Imports DevCase.UserControls.Types.EventArgs

#End Region

#Region " ElektroProgressBarTests "

''' <summary>
''' Unit tests for <see cref="ElektroProgressBar"/>.
''' </summary>
<TestClass>
Public NotInheritable Class ElektroProgressBarTests

#Region " Defaults "

    <TestMethod>
    Public Sub NewInstance_HasExpectedDefaults()
        Using bar As New ElektroProgressBar()
            Assert.AreEqual(0, bar.Value)
            Assert.AreEqual(100, bar.Maximum)
            Assert.AreEqual(1, bar.Step)
            Assert.AreEqual("(#current / #total) #%", bar.FormatString)
            Assert.AreEqual("(#current / #total) #%", bar.FormatStringPaused)
            Assert.AreEqual("(#current / #total) #%", bar.FormatStringError)
            Assert.AreEqual(ProgressBarState.Normal, bar.State)
            Assert.IsFalse(bar.ShowBorder)
        End Using
    End Sub

#End Region

#Region " Percentage "

    <TestMethod>
    <DataRow(0, 100, 0.0)>
    <DataRow(50, 100, 50.0)>
    <DataRow(100, 100, 100.0)>
    <DataRow(1, 4, 25.0)>
    <DataRow(150, 200, 75.0)>
    Public Sub Percentage_ReturnsValueRelativeToMaximum(value As Integer, maximum As Integer, expected As Double)
        Using bar As New ElektroProgressBar()
            bar.Maximum = maximum
            bar.Value = value

            Assert.AreEqual(expected, bar.Percentage, 0.0001)
        End Using
    End Sub

#End Region

#Region " GetPercentageString "

    <TestMethod>
    Public Sub GetPercentageString_DefaultFormat_ReplacesAllTokens()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 200
            bar.Value = 50

            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, "({0} / {1}) {2}%", 50, 200, 25), bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_MirrorFormat_ReturnsExpectedText()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 10
            bar.Value = 3
            bar.FormatString = "#current of #total directories (#%)"

            Assert.AreEqual("3 of 10 directories (30%)", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub TextOutlineColor_DefaultIsEmpty_AndRenderingWithOutlineDoesNotThrow()
        Using bar As New ElektroProgressBar() With {.Size = New Size(300, 30), .FormatString = "#current of #total"}
            Assert.IsTrue(bar.TextOutlineColor.IsEmpty)

            bar.TextOutlineColor = Color.Black
            Using bmp As New Bitmap(bar.Width, bar.Height)
                bar.DrawToBitmap(bmp, New Rectangle(Point.Empty, bar.Size))
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_WithNumberFormatAndCulture_FormatsTheValues()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 123456
            bar.Value = 12345
            bar.FormatString = "#current of #total"
            bar.NumberFormat = "N0"
            bar.FormatCulture = CultureInfo.GetCultureInfo("en-US")

            Assert.AreEqual("12,345 of 123,456", bar.GetPercentageString())

            bar.FormatCulture = CultureInfo.GetCultureInfo("es-ES")
            Assert.AreEqual("12.345 of 123.456", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_FormatWithoutTokens_ReturnsFormatUnchanged()
        Using bar As New ElektroProgressBar()
            bar.FormatString = "Listing directories, this could take some seconds. Please be patient..."

            Assert.AreEqual("Listing directories, this could take some seconds. Please be patient...", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_FormatWithCurlyBraces_DoesNotThrow()
        ' Regression test: the format string was converted to a composite format string and passed to String.Format,
        ' so any curly brace (e.g. in a file path or any user text) thrown a FormatException while painting the control.
        Using bar As New ElektroProgressBar()
            bar.Maximum = 4
            bar.Value = 1
            bar.FormatString = "{Folder} #current/#total {0} }{"

            Assert.AreEqual("{Folder} 1/4 {0} }{", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_TokenRepeated_ReplacesEveryOccurrence()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 8
            bar.Value = 2
            bar.FormatString = "#current #current #% #%"

            Assert.AreEqual("2 2 25% 25%", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_EmptyFormat_ReturnsEmptyString()
        Using bar As New ElektroProgressBar()
            bar.FormatString = String.Empty

            Assert.AreEqual(String.Empty, bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_NullFormat_ReturnsEmptyString()
        Using bar As New ElektroProgressBar()
            bar.FormatString = Nothing

            Assert.AreEqual(String.Empty, bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_PausedState_UsesPausedFormat()
        Using bar As New ElektroProgressBar()
            bar.FormatString = "normal"
            bar.FormatStringPaused = "paused #current"
            bar.FormatStringError = "error"
            bar.Value = 7
            bar.State = ProgressBarState.Paused

            Assert.AreEqual("paused 7", bar.GetPercentageString())
        End Using
    End Sub

    <TestMethod>
    Public Sub GetPercentageString_ErrorState_UsesErrorFormat()
        Using bar As New ElektroProgressBar()
            bar.FormatString = "normal"
            bar.FormatStringPaused = "paused"
            bar.FormatStringError = "error #%"
            bar.Value = 40
            bar.State = ProgressBarState.Error

            Assert.AreEqual("error 40%", bar.GetPercentageString())
        End Using
    End Sub

#End Region

#Region " Value / ValueChanged "

    <TestMethod>
    Public Sub Value_SetDifferentValue_RaisesValueChangedWithNewValue()
        Using bar As New ElektroProgressBar()
            Dim receivedValues As New List(Of Integer)
            AddHandler bar.ValueChanged, Sub(sender As Object, e As ProgressBarValueChangedEventArgs) receivedValues.Add(e.NewValue)

            bar.Value = 25
            bar.Value = 60

            CollectionAssert.AreEqual({25, 60}, receivedValues)
        End Using
    End Sub

    <TestMethod>
    Public Sub Value_SetSameValue_DoesNotRaiseValueChanged()
        Using bar As New ElektroProgressBar()
            bar.Value = 10
            Dim raised As Integer
            AddHandler bar.ValueChanged, Sub(sender As Object, e As ProgressBarValueChangedEventArgs) raised += 1

            bar.Value = 10

            Assert.AreEqual(0, raised)
        End Using
    End Sub

    <TestMethod>
    <DataRow(-1)>
    <DataRow(101)>
    Public Sub Value_OutOfRange_ThrowsArgumentOutOfRangeException(value As Integer)
        Using bar As New ElektroProgressBar()
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() bar.Value = value)
        End Using
    End Sub

    <TestMethod>
    Public Sub Increment_RaisesValueChangedAndClampsToMaximum()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 3
            Dim receivedValues As New List(Of Integer)
            AddHandler bar.ValueChanged, Sub(sender As Object, e As ProgressBarValueChangedEventArgs) receivedValues.Add(e.NewValue)

            bar.Increment(1)
            bar.Increment(1)
            bar.Increment(5)
            bar.Increment(1) ' Already at maximum: no event.

            Assert.AreEqual(3, bar.Value)
            CollectionAssert.AreEqual({1, 2, 3}, receivedValues)
        End Using
    End Sub

    <TestMethod>
    Public Sub PerformStep_IncrementsByStepAndRaisesValueChanged()
        Using bar As New ElektroProgressBar()
            bar.Step = 10
            Dim receivedValues As New List(Of Integer)
            AddHandler bar.ValueChanged, Sub(sender As Object, e As ProgressBarValueChangedEventArgs) receivedValues.Add(e.NewValue)

            bar.PerformStep()
            bar.PerformStep()

            Assert.AreEqual(20, bar.Value)
            CollectionAssert.AreEqual({10, 20}, receivedValues)
        End Using
    End Sub

    <TestMethod>
    Public Sub ProgressBarValueChangedEventArgs_NewValue_ReturnsConstructorValue()
        ' Regression test: the overridden auto-property always returned zero.
        Dim e As New ProgressBarValueChangedEventArgs(42)

        Assert.AreEqual(42, e.NewValue)
    End Sub

#End Region

#Region " ToString "

    <TestMethod>
    Public Sub ToString_ContainsTypeNameAndValues()
        Using bar As New ElektroProgressBar()
            bar.Maximum = 50
            bar.Value = 5

            Dim text As String = bar.ToString()

            StringAssert.StartsWith(text, GetType(ElektroProgressBar).FullName)
            StringAssert.Contains(text, "Minimum: 0")
            StringAssert.Contains(text, "Maximum: 50")
            StringAssert.Contains(text, "Value: 5")
        End Using
    End Sub

#End Region

End Class

#End Region
