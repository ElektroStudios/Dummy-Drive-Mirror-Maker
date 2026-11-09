#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Threading
Imports System.Threading.Tasks

Imports DevCase.Core.Threading.Enums
Imports DevCase.Core.Threading.Types

#End Region

#Region " ElektroBackgroundWorkerTests "

''' <summary>
''' Unit tests for <see cref="ElektroBackgroundWorker"/>.
''' </summary>
''' <remarks>
''' The tests run without a <see cref="SynchronizationContext"/>,
''' so the <see cref="BackgroundWorker.ProgressChanged"/> and <see cref="BackgroundWorker.RunWorkerCompleted"/>
''' events are raised on thread-pool threads.
''' <para></para>
''' Every potentially blocking call is executed with a timeout, so a regression never hangs the test run.
''' </remarks>
<TestClass>
Public NotInheritable Class ElektroBackgroundWorkerTests

#Region " Constants "

    ''' <summary>
    ''' The maximum time to wait for an operation that is expected to finish.
    ''' </summary>
    Private Shared ReadOnly OperationTimeout As TimeSpan = TimeSpan.FromSeconds(10)

#End Region

#Region " Nested Types "

    ''' <summary>
    ''' An <see cref="ElektroBackgroundWorker"/> whose completion notification can be delayed,
    ''' to reproduce the time window in which the background work has finished
    ''' but the <see cref="BackgroundWorker.RunWorkerCompleted"/> event was not raised yet
    ''' (e.g. because the UI thread is busy).
    ''' </summary>
    Private NotInheritable Class DelayedCompletionWorker : Inherits ElektroBackgroundWorker

        ''' <summary>
        ''' Gets the event that must be set to allow the completion notification to continue.
        ''' </summary>
        Friend ReadOnly Property AllowCompletion As New ManualResetEventSlim(initialState:=False)

        ''' <summary>
        ''' Gets the event that is set when the background work has finished.
        ''' </summary>
        Friend ReadOnly Property WorkFinished As New ManualResetEventSlim(initialState:=False)

        ''' <summary>
        ''' Raises the <see cref="BackgroundWorker.DoWork"/> event and signals <see cref="DelayedCompletionWorker.WorkFinished"/>.
        ''' </summary>
        Protected Overrides Sub OnDoWork(e As DoWorkEventArgs)
            Try
                MyBase.OnDoWork(e)
            Finally
                Me.WorkFinished.Set()
            End Try
        End Sub

        ''' <summary>
        ''' Waits for <see cref="DelayedCompletionWorker.AllowCompletion"/> before raising the completion notification.
        ''' </summary>
        Protected Overrides Sub OnRunWorkerCompleted(e As RunWorkerCompletedEventArgs)
            Me.AllowCompletion.Wait(ElektroBackgroundWorkerTests.OperationTimeout)
            MyBase.OnRunWorkerCompleted(e)
        End Sub

    End Class

#End Region

#Region " Helpers "

    ''' <summary>
    ''' Runs the specified (potentially blocking) action in another thread
    ''' and returns a value indicating whether it finished within <see cref="OperationTimeout"/>.
    ''' </summary>
    ''' <param name="action">
    ''' The action to run.
    ''' </param>
    ''' <returns>
    ''' <see langword="True"/> if the action finished in time; otherwise, <see langword="False"/>.
    ''' </returns>
    Private Shared Function CompletesInTime(action As Action) As Boolean
        Dim runner As Task = Task.Run(action)
        Return runner.Wait(OperationTimeout)
    End Function

    ''' <summary>
    ''' Creates a <see cref="ManualResetEventSlim"/> that is set when the
    ''' <see cref="BackgroundWorker.RunWorkerCompleted"/> event of the specified worker is raised.
    ''' </summary>
    ''' <param name="worker">
    ''' The worker.
    ''' </param>
    '''
    ''' <param name="completedArgs">
    ''' A one-element array that receives the event data of the last completion.
    ''' </param>
    ''' <returns>
    ''' The created event.
    ''' </returns>
    Private Shared Function WatchCompletion(worker As ElektroBackgroundWorker, completedArgs As RunWorkerCompletedEventArgs()) As ManualResetEventSlim
        Dim completed As New ManualResetEventSlim(initialState:=False)
        AddHandler worker.RunWorkerCompleted,
            Sub(sender As Object, e As RunWorkerCompletedEventArgs)
                completedArgs(0) = e
                completed.Set()
            End Sub
        Return completed
    End Function

#End Region

#Region " State "

    <TestMethod>
    Public Sub NewInstance_StateIsStopped()
        Using worker As New ElektroBackgroundWorker()
            Assert.AreEqual(ElektroBackgroundWorkerState.Stopped, worker.State)
            Assert.IsFalse(worker.PausePending)
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_SetsReportProgressAndCancellationSupport()
        Using worker As New ElektroBackgroundWorker()
            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                worker.RunAsync()

                Assert.IsTrue(worker.WorkerReportsProgress)
                Assert.IsTrue(worker.WorkerSupportsCancellation)
                Assert.IsTrue(completed.Wait(OperationTimeout))
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_WorkFinishes_StateIsCompletedAndCompletedEventIsRaised()
        Using worker As New ElektroBackgroundWorker()
            Dim workExecuted As Boolean
            AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs) workExecuted = True

            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                worker.RunAsync()

                Assert.IsTrue(completed.Wait(OperationTimeout), "The RunWorkerCompleted event was not raised.")
                Assert.IsTrue(workExecuted)
                Assert.IsFalse(completedArgs(0).Cancelled)
                Assert.IsNull(completedArgs(0).Error)
                Assert.AreEqual(ElektroBackgroundWorkerState.Completed, worker.State)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_StateIsRunningWhileWorkIsInProgress()
        Using worker As New ElektroBackgroundWorker()
            Using workStarted As New ManualResetEventSlim(False), releaseWork As New ManualResetEventSlim(False)
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        workStarted.Set()
                        releaseWork.Wait(OperationTimeout)
                    End Sub

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    Assert.IsTrue(workStarted.Wait(OperationTimeout))
                    Assert.AreEqual(ElektroBackgroundWorkerState.Running, worker.State)

                    releaseWork.Set()
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_WhileRunning_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Using releaseWork As New ManualResetEventSlim(False)
                AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs) releaseWork.Wait(OperationTimeout)

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.RunAsync())
                    Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.Run())

                    releaseWork.Set()
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_AfterCompleted_CanRunAgain()
        Using worker As New ElektroBackgroundWorker()
            Dim executions As Integer
            AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs) Interlocked.Increment(executions)

            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                worker.RunAsync()
                Assert.IsTrue(completed.Wait(OperationTimeout))

                completed.Reset()
                worker.RunAsync()
                Assert.IsTrue(completed.Wait(OperationTimeout))
            End Using

            Assert.AreEqual(2, executions)
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_ReportProgress_RaisesProgressChanged()
        Using worker As New ElektroBackgroundWorker()
            Dim reported As New List(Of Integer)
            Dim reportedLock As New Object()
            Using allReported As New CountdownEvent(3)
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        Dim w As ElektroBackgroundWorker = DirectCast(sender, ElektroBackgroundWorker)
                        w.ReportProgress(10)
                        w.ReportProgress(50)
                        w.ReportProgress(100)
                    End Sub
                AddHandler worker.ProgressChanged,
                    Sub(sender As Object, e As ProgressChangedEventArgs)
                        SyncLock reportedLock
                            reported.Add(e.ProgressPercentage)
                        End SyncLock
                        allReported.Signal()
                    End Sub

                worker.RunAsync()
                Assert.IsTrue(allReported.Wait(OperationTimeout))
            End Using

            SyncLock reportedLock
                CollectionAssert.AreEquivalent({10, 50, 100}, reported)
            End SyncLock
        End Using
    End Sub

    <TestMethod>
    Public Sub RunAsync_WorkThrows_CompletedEventContainsError()
        Using worker As New ElektroBackgroundWorker()
            AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs) Throw New IOException("Test exception.")

            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                worker.RunAsync()

                Assert.IsTrue(completed.Wait(OperationTimeout))
                Assert.IsInstanceOfType(Of IOException)(completedArgs(0).Error)
                Assert.AreEqual(ElektroBackgroundWorkerState.Completed, worker.State)
            End Using
        End Using
    End Sub

#End Region

#Region " Run (Synchronous) "

    <TestMethod>
    Public Sub Run_BlocksUntilWorkFinishes()
        Using worker As New ElektroBackgroundWorker()
            Dim workFinished As Boolean
            AddHandler worker.DoWork,
                Sub(sender As Object, e As DoWorkEventArgs)
                    Thread.Sleep(200)
                    workFinished = True
                End Sub

            Assert.IsTrue(CompletesInTime(Sub() worker.Run()), "Run did not return.")
            Assert.IsTrue(workFinished, "Run returned before the background work finished.")
            Assert.IsFalse(worker.WorkerReportsProgress)
            Assert.IsFalse(worker.WorkerSupportsCancellation)
        End Using
    End Sub

    <TestMethod>
    Public Sub Run_WorkThrows_DoesNotBlockForever()
        ' Regression test: the synchronization event was only signaled after a successful work,
        ' so an exception thrown in the DoWork event handler blocked the caller of Run forever.
        Using worker As New ElektroBackgroundWorker()
            AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs) Throw New InvalidOperationException("Test exception.")

            Assert.IsTrue(CompletesInTime(Sub() worker.Run()), "Run blocked forever after an exception in the background work.")
        End Using
    End Sub

    <TestMethod>
    Public Sub Run_SecondRun_BlocksUntilSecondWorkFinishes()
        ' Regression test: the synchronization event was never reset,
        ' so after the first synchronous run, the next runs returned immediately without waiting.
        Using worker As New ElektroBackgroundWorker()
            Dim executions As Integer
            Dim lastWorkFinished As Boolean
            AddHandler worker.DoWork,
                Sub(sender As Object, e As DoWorkEventArgs)
                    lastWorkFinished = False
                    Thread.Sleep(200)
                    Interlocked.Increment(executions)
                    lastWorkFinished = True
                End Sub

            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                Assert.IsTrue(CompletesInTime(Sub() worker.Run()))
                Assert.IsTrue(completed.Wait(OperationTimeout))
                completed.Reset()
                lastWorkFinished = False

                Assert.IsTrue(CompletesInTime(Sub() worker.Run()))
                Assert.IsTrue(lastWorkFinished, "The second Run returned before the background work finished.")
                Assert.IsTrue(completed.Wait(OperationTimeout))
            End Using

            Assert.AreEqual(2, executions)
        End Using
    End Sub

#End Region

#Region " Cancellation "

    <TestMethod>
    Public Sub Cancel_WhileRunning_StopsWorkAndCompletesAsCancelled()
        Using worker As New ElektroBackgroundWorker()
            Using workStarted As New ManualResetEventSlim(False)
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        Dim w As ElektroBackgroundWorker = DirectCast(sender, ElektroBackgroundWorker)
                        workStarted.Set()
                        Dim deadline As Date = Date.UtcNow.Add(OperationTimeout)
                        While Not w.CancellationPending AndAlso Date.UtcNow < deadline
                            Thread.Sleep(10)
                        End While
                        e.Cancel = w.CancellationPending
                    End Sub

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    Assert.IsTrue(workStarted.Wait(OperationTimeout))

                    Assert.IsTrue(CompletesInTime(Sub() worker.Cancel()), "Cancel did not return.")
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                    Assert.IsTrue(completedArgs(0).Cancelled)
                    Assert.AreEqual(ElektroBackgroundWorkerState.Completed, worker.State)
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub CancelAsync_WhileRunning_SetsCancellationPendingState()
        Using worker As New ElektroBackgroundWorker()
            Using workStarted As New ManualResetEventSlim(False), releaseWork As New ManualResetEventSlim(False)
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        workStarted.Set()
                        releaseWork.Wait(OperationTimeout)
                    End Sub

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    Assert.IsTrue(workStarted.Wait(OperationTimeout))

                    worker.CancelAsync()
                    Assert.AreEqual(ElektroBackgroundWorkerState.CancellationPending, worker.State)
                    Assert.IsTrue(worker.CancellationPending)

                    ' A second cancellation request is ignored.
                    worker.CancelAsync()

                    releaseWork.Set()
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Cancel_AfterWorkFinishedButBeforeCompletion_DoesNotBlockForever()
        ' Regression test: if the background work finished just before the cancellation was requested
        ' (but the completion was not processed yet), Cancel waited forever for a signal that never came.
        ' In the application this froze the UI thread when the user pressed 'ABORT' at the very end of a mirror operation.
        Using worker As New DelayedCompletionWorker()
            AddHandler worker.DoWork, Sub(sender As Object, e As DoWorkEventArgs)
                                      End Sub

            worker.RunAsync()
            Assert.IsTrue(worker.WorkFinished.Wait(OperationTimeout))
            Assert.AreEqual(ElektroBackgroundWorkerState.Running, worker.State)

            Dim cancelReturned As Boolean = CompletesInTime(Sub() worker.Cancel())

            worker.AllowCompletion.Set()
            Assert.IsTrue(cancelReturned, "Cancel blocked forever after the background work already finished.")
        End Using
    End Sub

    <TestMethod>
    Public Sub CancelAsync_WhenStopped_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.CancelAsync())
        End Using
    End Sub

#End Region

#Region " Pause / Resume "

    <TestMethod>
    Public Sub RequestPauseAndResume_WhileRunning_ChangesState()
        Using worker As New ElektroBackgroundWorker()
            Using workStarted As New ManualResetEventSlim(False), releaseWork As New ManualResetEventSlim(False)
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        workStarted.Set()
                        releaseWork.Wait(OperationTimeout)
                    End Sub

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    Assert.IsTrue(workStarted.Wait(OperationTimeout))

                    worker.RequestPause()
                    Assert.AreEqual(ElektroBackgroundWorkerState.Paused, worker.State)
                    Assert.IsTrue(worker.PausePending)

                    worker.Resume()
                    Assert.AreEqual(ElektroBackgroundWorkerState.Running, worker.State)
                    Assert.IsFalse(worker.PausePending)

                    releaseWork.Set()
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Pause_WithinWorkAfterPauseRequest_BlocksUntilResumed()
        ' Regression test: RequestPause changes the state to 'Paused', 
        ' but Pause only accepted the 'Running' state, so it always thrown an InvalidOperationException.
        Using worker As New ElektroBackgroundWorker()
            Using pauseRequested As New ManualResetEventSlim(False), reachedPause As New ManualResetEventSlim(False)
                Dim resumedAfterPause As Boolean
                AddHandler worker.DoWork,
                    Sub(sender As Object, e As DoWorkEventArgs)
                        Dim w As ElektroBackgroundWorker = DirectCast(sender, ElektroBackgroundWorker)
                        pauseRequested.Wait(OperationTimeout)
                        reachedPause.Set()
                        w.Pause() ' Blocks until resumed.
                        resumedAfterPause = True
                    End Sub

                Dim completedArgs(0) As RunWorkerCompletedEventArgs
                Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                    worker.RunAsync()
                    worker.RequestPause()
                    pauseRequested.Set()

                    Assert.IsTrue(reachedPause.Wait(OperationTimeout))
                    Thread.Sleep(100)
                    Assert.IsFalse(resumedAfterPause, "Pause did not block the background work.")

                    worker.Resume()
                    Assert.IsTrue(completed.Wait(OperationTimeout))
                    Assert.IsTrue(resumedAfterPause)
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub RequestPause_WhenStopped_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.RequestPause())
        End Using
    End Sub

    <TestMethod>
    Public Sub Resume_WhenNotPaused_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.Resume())
        End Using
    End Sub

    <TestMethod>
    Public Sub Pause_WhenStopped_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() worker.Pause())
        End Using
    End Sub

    <TestMethod>
    Public Sub Pause_WithoutPauseRequest_ThrowsInvalidOperationException()
        Using worker As New ElektroBackgroundWorker()
            Dim pauseException As Exception = Nothing
            AddHandler worker.DoWork,
                Sub(sender As Object, e As DoWorkEventArgs)
                    Try
                        DirectCast(sender, ElektroBackgroundWorker).Pause()
                    Catch ex As Exception
                        pauseException = ex
                    End Try
                End Sub

            Dim completedArgs(0) As RunWorkerCompletedEventArgs
            Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
                worker.RunAsync()
                Assert.IsTrue(completed.Wait(OperationTimeout))
            End Using

            Assert.IsInstanceOfType(Of InvalidOperationException)(pauseException)
        End Using
    End Sub

#End Region

#Region " Dispose "

    <TestMethod>
    Public Sub Dispose_AfterCompleted_ResetsStateToStopped()
        Dim worker As New ElektroBackgroundWorker()
        Dim completedArgs(0) As RunWorkerCompletedEventArgs
        Using completed As ManualResetEventSlim = WatchCompletion(worker, completedArgs)
            worker.RunAsync()
            Assert.IsTrue(completed.Wait(OperationTimeout))
        End Using

        worker.Dispose()

        Assert.AreEqual(ElektroBackgroundWorkerState.Stopped, worker.State)
    End Sub

#End Region

End Class

#End Region
