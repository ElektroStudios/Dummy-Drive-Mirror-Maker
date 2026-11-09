#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " MirrorRulesProgressTests "

''' <summary>
''' Unit tests for <see cref="MirrorRules.CalculateProgressPercent(Integer, Integer)"/>.
''' </summary>
<TestClass>
Public NotInheritable Class MirrorRulesProgressTests

    <TestMethod>
    <DataRow(0, 10, 0)>
    <DataRow(5, 10, 50)>
    <DataRow(10, 10, 100)>
    <DataRow(1, 3, 33)>
    <DataRow(2, 3, 67)>
    <DataRow(1, 1, 100)>
    <DataRow(0, 1, 0)>
    <DataRow(1, 200, 0)>
    <DataRow(199, 200, 100)>
    Public Sub CalculateProgressPercent_ValidValues_ReturnsRoundedPercent(current As Integer, maximum As Integer, expected As Integer)
        Assert.AreEqual(expected, MirrorRules.CalculateProgressPercent(current, maximum))
    End Sub

    <TestMethod>
    Public Sub CalculateProgressPercent_LargeValues_DoesNotOverflow()
        Assert.AreEqual(50, MirrorRules.CalculateProgressPercent(Integer.MaxValue \ 2, Integer.MaxValue))
    End Sub

    <TestMethod>
    Public Sub CalculateProgressPercent_AllValuesInRange_ReturnsValueBetweenZeroAndHundred()
        Const maximum As Integer = 137
        Dim previous As Integer = -1
        For current As Integer = 0 To maximum
            Dim percent As Integer = MirrorRules.CalculateProgressPercent(current, maximum)
            Assert.IsTrue(percent >= 0 AndAlso percent <= 100, $"Percent out of range: {percent}")
            Assert.IsTrue(percent >= previous, "The percent must never decrease.")
            previous = percent
        Next current
    End Sub

    <TestMethod>
    <DataRow(0)>
    <DataRow(-1)>
    <DataRow(Integer.MinValue)>
    Public Sub CalculateProgressPercent_MaximumLessThanOne_ThrowsArgumentOutOfRangeException(maximum As Integer)
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() MirrorRules.CalculateProgressPercent(0, maximum))
    End Sub

    <TestMethod>
    <DataRow(-1, 10)>
    <DataRow(11, 10)>
    Public Sub CalculateProgressPercent_CurrentOutOfRange_ThrowsArgumentOutOfRangeException(current As Integer, maximum As Integer)
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() MirrorRules.CalculateProgressPercent(current, maximum))
    End Sub

End Class

#End Region
