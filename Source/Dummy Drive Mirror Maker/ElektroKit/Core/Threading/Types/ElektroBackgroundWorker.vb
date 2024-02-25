




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





#Region " Imports "

Imports DevCase.Core.Threading.Enums

Imports System.ComponentModel
Imports System.Threading

#End Region

#Region " Usage Examples "

'Friend WithEvents Worker As ElektroBackgroundWorker
'
'Private Sub Button_Run_Click() Handles Button_Run.Click
'
'    If (Me.Worker IsNot Nothing) Then
'
'        Select Case Me.Worker.State
'            Case ElektroBackgroundWorkerState.Running, ElektroBackgroundWorkerState.Paused
'                Me.Worker.Cancel()
'            Case Else
'                ' Do Nothing.
'        End Select
'
'    End If
'
'    Me.Worker = New ElektroBackgroundWorker
'    Me.Worker.RunAsync()
'
'End Sub
'
'Private Sub Button_Pause_Click() Handles Button_Pause.Click
'    Me.Worker.RequestPause()
'End Sub
'
'Private Sub Button_Resume_Click() Handles Button_Resume.Click
'    Me.Worker.Resume()
'End Sub
'
'Private Sub Button_Cancel_Click() Handles Button_Cancel.Click
'    Me.Worker.Cancel()
'End Sub
'
'''' ----------------------------------------------------------------------------------------------------
'''' <summary>
'''' Handles the <see cref="ElektroBackgroundWorker.DoWork"/> event of the <see cref="Worker"/> instance.
'''' </summary>
'''' ----------------------------------------------------------------------------------------------------
'''' <param name="sender">
'''' The source of the event.
'''' </param>
'''' 
'''' <param name="e">
'''' The <see cref="DoWorkEventArgs"/> instance containing the event data.
'''' </param>
'''' ----------------------------------------------------------------------------------------------------
'<DebuggerStepperBoundary>
'Private Sub Worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) _
'Handles Worker.DoWork
'
'    Dim progress As Integer
'
'    Dim lock As Object = ""
'    SyncLock lock
'
'        For i As Integer = 0 To 100
'            If (Me.Worker.CancellationPending) Then ' Cancel the background operation.
'                e.Cancel = True
'                Exit For
'
'            Else
'                If (Me.Worker.PausePending) Then ' Pause the background operation.
'                    Me.Worker.Pause() ' Blocking pause call.
'                End If
'
'                Me.DoSomething()
'
'                If Me.Worker.WorkerReportsProgress Then
'                    progress = i
'                    Me.Worker.ReportProgress(progress)
'                End If
'
'            End If
'
'        Next i
'
'    End SyncLock
'
'    If (Me.Worker.WorkerReportsProgress) AndAlso Not (Me.Worker.CancellationPending) AndAlso (progress < 100) Then
'        Me.Worker.ReportProgress(percentProgress:=100)
'    End If
'
'End Sub
'
'''' ----------------------------------------------------------------------------------------------------
'''' <summary>
'''' Handles the <see cref="ElektroBackgroundWorker.ProgressChanged"/> event of the <see cref="Worker"/> instance.
'''' </summary>
'''' ----------------------------------------------------------------------------------------------------
'''' <param name="sender">
'''' The source of the event.
'''' </param>
'''' 
'''' <param name="e">
'''' The <see cref="ProgressChangedEventArgs"/> instance containing the event data.
'''' </param>
'''' ----------------------------------------------------------------------------------------------------
'<DebuggerStepperBoundary>
'Private Sub Worker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) _
'Handles Worker.ProgressChanged
'
'    Console.WriteLine(String.Format("Background Work Progress: {0:00.00}%", e.ProgressPercentage))
'
'End Sub
'
'''' ----------------------------------------------------------------------------------------------------
'''' <summary>
'''' Handles the <see cref="ElektroBackgroundWorker.RunWorkerCompleted"/> event of the <see cref="Worker"/> instance.
'''' </summary>
'''' ----------------------------------------------------------------------------------------------------
'''' <param name="sender">
'''' The source of the event.
'''' </param>
'''' 
'''' <param name="e">
'''' The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.
'''' </param>
'''' ----------------------------------------------------------------------------------------------------
'<DebuggerStepperBoundary>
'Private Sub Worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
'Handles Worker.RunWorkerCompleted
'
'    If (e.Cancelled) Then
'        Debug.WriteLine("Background work cancelled.")
'
'    ElseIf (e.Error IsNot Nothing) Then
'        Debug.WriteLine("Background work error.")
'
'    Else
'        Debug.WriteLine("Background work done.")
'
'    End If
'
'    Console.WriteLine(String.Format("State: {0}", Me.Worker.State.ToString()))
'
'End Sub
'
'<DebuggerStepperBoundary>
'Private Sub DoSomething()
'    Thread.Sleep(TimeSpan.FromSeconds(1))
'End Sub

#End Region

#Region " ElektroBackgroundWorker "

Namespace DevCase.Core.Threading.Types

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' A extended <see cref="BackgroundWorker"/> component 
    ''' with synchronous (blocking) run/cancellation support, 
    ''' and asynchronous pause/resume features.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <example> This is a code example.
    ''' <code>
    ''' Friend WithEvents Worker As ElektroBackgroundWorker
    ''' 
    ''' Private Sub Button_Run_Click() Handles Button_Run.Click
    ''' 
    '''     If (Me.Worker IsNot Nothing) Then
    ''' 
    '''         Select Case Me.Worker.State
    '''             Case ElektroBackgroundWorkerState.Running, ElektroBackgroundWorkerState.Paused
    '''                 Me.Worker.Cancel()
    '''             Case Else
    '''                 ' Do Nothing.
    '''         End Select
    ''' 
    '''     End If
    ''' 
    '''     Me.Worker = New ElektroBackgroundWorker
    '''     Me.Worker.RunAsync()
    ''' 
    ''' End Sub
    ''' 
    ''' Private Sub Button_Pause_Click() Handles Button_Pause.Click
    '''     Me.Worker.RequestPause()
    ''' End Sub
    ''' 
    ''' Private Sub Button_Resume_Click() Handles Button_Resume.Click
    '''     Me.Worker.Resume()
    ''' End Sub
    ''' 
    ''' Private Sub Button_Cancel_Click() Handles Button_Cancel.Click
    '''     Me.Worker.Cancel()
    ''' End Sub
    ''' 
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;summary&gt;
    ''' ''' Handles the &lt;see cref="ElektroBackgroundWorker.DoWork"/&gt; event of the &lt;see cref="Worker"/&gt; instance.
    ''' ''' &lt;/summary&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;param name="sender"&gt;
    ''' ''' The source of the event.
    ''' ''' &lt;/param&gt;
    ''' ''' 
    ''' ''' &lt;param name="e"&gt;
    ''' ''' The &lt;see cref="DoWorkEventArgs"/&gt; instance containing the event data.
    ''' ''' &lt;/param&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' &lt;DebuggerStepperBoundary&gt;
    ''' Private Sub Worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) _
    ''' Handles Worker.DoWork
    ''' 
    '''     Dim progress As Integer
    ''' 
    '''     Dim lock As Object = ""
    '''     SyncLock lock
    ''' 
    '''         For i As Integer = 0 To 100
    '''             If (Me.Worker.CancellationPending) Then ' Cancel the background operation.
    '''                 e.Cancel = True
    '''                 Exit For
    ''' 
    '''             Else
    '''                 If (Me.Worker.PausePending) Then ' Pause the background operation.
    '''                     Me.Worker.Pause() ' Blocking pause call.
    '''                 End If
    ''' 
    '''                 Me.DoSomething()
    ''' 
    '''                 If Me.Worker.WorkerReportsProgress Then
    '''                     progress = i
    '''                     Me.Worker.ReportProgress(progress)
    '''                 End If
    ''' 
    '''             End If
    ''' 
    '''         Next i
    ''' 
    '''     End SyncLock
    ''' 
    '''     If (Me.Worker.WorkerReportsProgress) AndAlso Not (Me.Worker.CancellationPending) AndAlso (progress &lt; 100) Then
    '''         Me.Worker.ReportProgress(percentProgress:=100)
    '''     End If
    ''' 
    ''' End Sub
    ''' 
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;summary&gt;
    ''' ''' Handles the &lt;see cref="ElektroBackgroundWorker.ProgressChanged"/&gt; event of the &lt;see cref="Worker"/&gt; instance.
    ''' ''' &lt;/summary&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;param name="sender"&gt;
    ''' ''' The source of the event.
    ''' ''' &lt;/param&gt;
    ''' ''' 
    ''' ''' &lt;param name="e"&gt;
    ''' ''' The &lt;see cref="ProgressChangedEventArgs"/&gt; instance containing the event data.
    ''' ''' &lt;/param&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' &lt;DebuggerStepperBoundary&gt;
    ''' Private Sub Worker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) _
    ''' Handles Worker.ProgressChanged
    ''' 
    '''     Console.WriteLine(String.Format("Background Work Progress: {0:00.00}%", e.ProgressPercentage))
    ''' 
    ''' End Sub
    ''' 
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;summary&gt;
    ''' ''' Handles the &lt;see cref="ElektroBackgroundWorker.RunWorkerCompleted"/&gt; event of the &lt;see cref="Worker"/&gt; instance.
    ''' ''' &lt;/summary&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' ''' &lt;param name="sender"&gt;
    ''' ''' The source of the event.
    ''' ''' &lt;/param&gt;
    ''' ''' 
    ''' ''' &lt;param name="e"&gt;
    ''' ''' The &lt;see cref="RunWorkerCompletedEventArgs"/&gt; instance containing the event data.
    ''' ''' &lt;/param&gt;
    ''' ''' ----------------------------------------------------------------------------------------------------
    ''' &lt;DebuggerStepperBoundary&gt;
    ''' Private Sub Worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
    ''' Handles Worker.RunWorkerCompleted
    ''' 
    '''     If (e.Cancelled) Then
    '''         Debug.WriteLine("Background work cancelled.")
    ''' 
    '''     ElseIf (e.Error IsNot Nothing) Then
    '''         Debug.WriteLine("Background work error.")
    ''' 
    '''     Else
    '''         Debug.WriteLine("Background work done.")
    ''' 
    '''     End If
    ''' 
    '''     Console.WriteLine(String.Format("State: {0}", Me.Worker.State.ToString()))
    ''' 
    ''' End Sub
    ''' 
    ''' &lt;DebuggerStepperBoundary&gt;
    ''' Private Sub DoSomething()
    '''     Thread.Sleep(TimeSpan.FromSeconds(1))
    ''' End Sub
    ''' </code>
    ''' </example>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <seealso cref="BackgroundWorker" />
    ''' ----------------------------------------------------------------------------------------------------
    <DisplayName("ElektroBackgroundWorker")>
    <Description("A extended BackgroundWorker component, with synchronous (blocking) run/cancellation support, and asynchronous pause/resume features.")>
    <DesignTimeVisible(True)>
    <DesignerCategory("Component")>
    <ToolboxBitmap(GetType(Component), "Component.bmp")>
    <ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Require)>
    <DefaultEvent("DoWork")>
    Public Class ElektroBackgroundWorker : Inherits BackgroundWorker

#Region " Private Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A <see cref="ManualResetEvent"/> that serves to handle synchronous operations (Run, Cancel, Pause, Resume).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected ReadOnly mreSync As ManualResetEvent

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A <see cref="ManualResetEvent"/> that serves to handle asynchronous operations (RunAsync, CancelAsync, RequestPause).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected ReadOnly mreAsync As ManualResetEvent

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates whether the <see cref="BackGroundworker"/> has been initiated in synchronous mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected isRunSync As Boolean

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates whether a synchronous cancellation operation is requested.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected isCancelSyncRequested As Boolean

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Indicates whether a (asynchronous) pause operation is requested.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected isPauseRequested As Boolean

#End Region

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether the <see cref="ElektroBackgroundWorker"/> can report progress updates.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' <see langword="True"/> if can report progress updates; otherwise, <see langword="False"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Description("A value indicating whether the ElektroBackgroundWorker can report progress updates.")>
        Public Overloads ReadOnly Property WorkerReportsProgress As Boolean
            Get
                Return MyBase.WorkerReportsProgress
            End Get
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether the <see cref="ElektroBackgroundWorker"/> supports asynchronous cancellation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' <see langword="True"/> if supports asynchronous cancellation; otherwise, <see langword="False"/>.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Description("A value indicating whether the ElektroBackgroundWorker supports asynchronous cancellation.")>
        Public Overloads ReadOnly Property WorkerSupportsCancellation As Boolean
            Get
                Return MyBase.WorkerSupportsCancellation
            End Get
        End Property

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current state of a pending background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' The current state of a pending background operation.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Description("The current state of a pending background operation.")>
        Public ReadOnly Property State As ElektroBackgroundWorkerState
            <DebuggerStepThrough>
            Get
                Return Me.stateB
            End Get
        End Property
        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' ( Backing Field )
        ''' The current state of a pending background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private stateB As ElektroBackgroundWorkerState = ElektroBackgroundWorkerState.Stopped

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether the application has requested pause of a background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <value>
        ''' <see langword="True"/> if the application has requested pause of a background operation;
        ''' otherwise, false.
        ''' </value>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Description("A value indicating whether the application has requested pause of a background operation.")>
        Public ReadOnly Property PausePending As Boolean
            Get
                Return Me.isPauseRequested
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ElektroBackgroundWorker"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Public Sub New()
            Me.mreSync = New ManualResetEvent(initialState:=False)
            Me.mreAsync = New ManualResetEvent(initialState:=True)
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Starts execution of a background operation.
        ''' <para></para>
        ''' It blocks the caller thread until the background work is done.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to run the BackgroundWorker, the background operation must be stopped or completed.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub Run()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.Stopped, ElektroBackgroundWorkerState.Completed
                        Me.isRunSync = True
                        MyBase.WorkerReportsProgress = False
                        MyBase.WorkerSupportsCancellation = False
                        MyBase.RunWorkerAsync()
                        Me.stateB = ElektroBackgroundWorkerState.Running
                        Me.mreSync.WaitOne()

                    Case Else
                        Throw New InvalidOperationException("In order to run the BackgroundWorker, the background operation must be stopped or completed.")

                End Select

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asynchronously starts execution of a background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to run the BackgroundWorker, the background operation must be stopped or completed.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub RunAsync()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.Stopped, ElektroBackgroundWorkerState.Completed
                        MyBase.WorkerReportsProgress = True
                        MyBase.WorkerSupportsCancellation = True
                        MyBase.RunWorkerAsync()
                        Me.stateB = ElektroBackgroundWorkerState.Running

                    Case Else
                        Throw New InvalidOperationException("In order to run the BackgroundWorker, the background operation must be stopped or completed.")

                End Select

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Pause a pending background operation.
        ''' <para></para>
        ''' It blocks the caller thread until the background work is resumed. 
        ''' To resume the background work, call the <see cref="ElektroBackgroundWorker.Resume"/> method.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to pause the BackgroundWorker, firstly a pause request should be made.
        ''' </exception>
        ''' 
        ''' <exception cref="InvalidOperationException">
        ''' In order to pause the BackgroundWorker, the background operation must be be running.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub Pause()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.Running
                        If (Me.PausePending) Then
                            Me.mreAsync.WaitOne(Timeout.Infinite)
                        Else
                            Throw New InvalidOperationException("In order to pause the BackgroundWorker, firstly a pause request should be made.")
                        End If

                    Case Else
                        Throw New InvalidOperationException("In order to pause the BackgroundWorker, the background operation must be running.")

                End Select

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asynchronously requests to pause a pending background operation.
        ''' <para></para>
        ''' To pause the background work after requesting a pause, 
        ''' call the <see cref="ElektroBackgroundWorker.Pause"/> method.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to request a pause of the BackgroundWorker, the background operation must be running.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub RequestPause()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.Running
                        Me.isPauseRequested = True
                        Me.stateB = ElektroBackgroundWorkerState.Paused
                        Me.mreAsync.Reset()

                    Case Else
                        Throw New InvalidOperationException("In order to request a pause of the BackgroundWorker, the background operation must be running..")

                End Select

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Resume a pending paused background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to resume the BackgroundWorker, the background operation must be paused.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub [Resume]()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.Paused
                        Me.stateB = ElektroBackgroundWorkerState.Running
                        Me.isPauseRequested = False
                        Me.mreAsync.Set()

                    Case Else
                        Throw New InvalidOperationException("In order to resume the BackgroundWorker, the background operation must be paused.")

                End Select

            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Requests cancellation of a pending background operation.
        ''' <para></para>
        ''' It blocks the caller thread until the remaining background work is canceled.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to cancel the BackgroundWorker, the background operation must be running or paused.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Sub Cancel()

            Me.isCancelSyncRequested = True
            Me.CancelAsync()
            Me.mreSync.WaitOne()
            Me.isCancelSyncRequested = False

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asynchronously requests cancellation of a pending background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidOperationException">
        ''' In order to cancel the BackgroundWorker, the background operation must be running or paused.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Overridable Overloads Sub CancelAsync()

            If (Me Is Nothing) Then
                Throw New ObjectDisposedException(objectName:="Me")

            Else
                Select Case Me.stateB

                    Case ElektroBackgroundWorkerState.CancellationPending
                        Exit Sub

                    Case ElektroBackgroundWorkerState.Running, ElektroBackgroundWorkerState.Paused
                        Me.mreAsync.Set() ' Resume thread if it is paused.
                        Me.stateB = ElektroBackgroundWorkerState.CancellationPending
                        MyBase.CancelAsync() ' Cancel it.

                    Case Else
                        Throw New InvalidOperationException("In order to cancel the BackgroundWorker, the background operation must be running or paused.")

                End Select

            End If

        End Sub

#End Region

#Region " Event Invocators "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="BackgroundWorker.DoWork"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnDoWork(e As DoWorkEventArgs)
            MyBase.OnDoWork(e)

            If (Me.isRunSync) OrElse (Me.isCancelSyncRequested) Then
                Me.mreSync.Set()
            End If
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="BackgroundWorker.ProgressChanged"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="ProgressChangedEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnProgressChanged(e As ProgressChangedEventArgs)
            MyBase.OnProgressChanged(e)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="BackgroundWorker.RunWorkerCompleted"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="RunWorkerCompletedEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnRunWorkerCompleted(e As RunWorkerCompletedEventArgs)
            Me.stateB = ElektroBackgroundWorkerState.Completed
            MyBase.OnRunWorkerCompleted(e)
        End Sub

#End Region

#Region " Hidden Base Members "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Starts execution of a background operation.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DebuggerStepThrough>
        Public Overridable Shadows Sub RunWorkerAsync()
            MyBase.RunWorkerAsync()
        End Sub

#End Region

#Region " IDisposable Implementation "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ''' <para></para>
        ''' Releases unmanaged and, optionally, managed resources.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="isDisposing">
        ''' <see langword="True"/> to release both managed and unmanaged resources; 
        ''' <see langword="False"/> to release only unmanaged resources.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Protected Overrides Sub Dispose(isDisposing As Boolean)
            MyBase.Dispose(isDisposing)

            If (isDisposing) Then
                Me.mreSync.SafeWaitHandle.Close()
                Me.mreSync.SafeWaitHandle.Dispose()
                Me.mreSync.Close()
                Me.mreSync.Dispose()

                Me.mreAsync.SafeWaitHandle.Close()
                Me.mreAsync.SafeWaitHandle.Dispose()
                Me.mreAsync.Close()
                Me.mreAsync.Dispose()

                Me.isRunSync = False
                Me.stateB = ElektroBackgroundWorkerState.Stopped
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
