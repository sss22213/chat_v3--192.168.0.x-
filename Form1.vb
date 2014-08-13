Imports System.Net.Sockets
Imports System.Text
Imports System.Net
Imports System.Threading
Imports System.Media
Imports System.IO
Imports System.Resources

Public Class Form1

    Dim inip() As String = {"192.168.0.2", "192.168.0.3", "192.168.0.4", "192.168.0.5", "192.168.0.6", "192.168.0.7", "192.168.0.8", "192.168.0.9", "192.168.0.10", "192.168.0.11", "192.168.0.12", "192.168.0.13", "192.168.0.14", "192.168.0.15", "192.168.0.16", "192.168.0.17", "192.168.0.18", "192.168.0.19", "192.168.0.20"}
    Dim onip(20) As String
    Dim k2 As Integer = 0
    Dim ininip As String
    Dim t1 As Integer
    Dim number As Integer = 0
    Public know As String


    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        
        Form.CheckForIllegalCrossThreadCalls = False
        Me.Text = "就緒"
        TextBox2.SelectionStart = TextBox2.Text.Length


        Dim ComputerHost As IPHostEntry = Dns.Resolve(Dns.GetHostName)
        Dim ComputerIP() As IPAddress = ComputerHost.AddressList
        Dim iplength As Integer = ComputerIP.Length

        ininip = ComputerIP(iplength - 1).ToString
   
        Dim k1 As Integer = 0
        ListBox1.Items.Add("廣播")
        ListBox2.Items.Add("")
        For i = 0 To inip.Length - 1
            If My.Computer.Network.Ping(inip(i)) Then
                onip(k1) = inip(i)
                ListBox1.Items.Add(onip(k1))
                Try
                    Dim checkhost As IPHostEntry = System.Net.Dns.GetHostEntry(onip(k1))
                    ListBox2.Items.Add(checkhost.HostName)
                Catch ex As SocketException
                    ListBox2.Items.Add("行動裝置")
                End Try
                k1 += 1
            End If
        Next

      



        Timer1.Interval = 30
        Timer1.Start()
        Timer3.Interval = 65
        Timer3.Start()
        Timer2.Interval = 3000
        Timer2.Start()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        If BackgroundWorker2.IsBusy = 0 Then
            BackgroundWorker2.RunWorkerAsync()
        End If

    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        Dim ip As IPAddress
        Dim port As Integer = 1580
        ip = IPAddress.Parse(onip(k2))
        Dim TcpListener01 As New TcpListener(ip, port)
        Dim tcpclient01 As New TcpClient
        TcpListener01.Start()


        Try
            tcpclient01 = TcpListener01.AcceptTcpClient
            tcpclient01.ReceiveBufferSize = 1024576
            Dim networkstream01 As NetworkStream = tcpclient01.GetStream
            Dim bytes(tcpclient01.ReceiveBufferSize) As Byte

            If bytes.Length > 0 And Me.WindowState = 1 And LinkLabel1.Text = "鈴聲開" Then

                My.Computer.Audio.Play(My.Resources.ambulance_siren, AudioPlayMode.WaitToComplete)
                'ElseIf bytes.Length > 0 And LinkLabel1.Text = "鈴聲開" Then
                '    My.Computer.Audio.Play(My.Resources.NOTIFY, AudioPlayMode.WaitToComplete)

            End If

            networkstream01.Read(bytes, 0, CInt(tcpclient01.ReceiveBufferSize))
            TextBox2.Text = TextBox2.Text + vbCrLf + vbCrLf + Encoding.UTF8.GetChars(bytes)
            TextBox2.SelectionStart = TextBox2.Text.Length
            TextBox2.ScrollToCaret()
        Catch ex As Exception

        End Try
        tcpclient01.Close()
        TcpListener01.Stop()


    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If BackgroundWorker1.IsBusy = 0 Then
            BackgroundWorker1.RunWorkerAsync()
            k2 += 1
            If k2 > ListBox1.Items.Count Then
                k2 = 0
            End If
        End If
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button1.PerformClick()
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Dim pcname As String
        Dim networkStream As Net.Sockets.NetworkStream
        Me.Text = "傳送中......"
        TextBox1.Visible = False
        Try
            If Not TextBox3.Text = "廣播" Then
                TextBox1.Visible = False
                Dim tcpclient As New TcpClient
                Dim ip As String = TextBox3.Text
                Dim port As Integer = 1580

                tcpclient.Connect(ip, port)

                networkStream = tcpclient.GetStream
                If (TextBox1.Text).Length < 1 Then
                    TextBox1.Text = "空白"
                End If
                Try
                    Dim checkhost As IPHostEntry = System.Net.Dns.GetHostEntry(ininip)
                    pcname = checkhost.HostName
                Catch ex As SocketException
                    pcname = "那個不能說出名字的人"
                End Try
                Dim send() As Byte = Encoding.UTF8.GetBytes((pcname + "說:" + TextBox1.Text).ToCharArray)
                networkStream.Write(send, 0, send.Length)
                TextBox2.Text = TextBox2.Text + vbCrLf + vbCrLf + (pcname + "說:" + TextBox1.Text)
                TextBox1.Text = ""
                TextBox2.SelectionStart = TextBox2.Text.Length
                TextBox2.ScrollToCaret()

            End If
        Catch ex As SocketException
            MsgBox("此人未上線")
        Catch ex As Exception
            MsgBox("此人未上線")
        End Try
        TextBox1.Visible = True
        Me.Text = "就緒"
        '全部傳送

        If TextBox3.Text = "廣播" Then
            Me.Text = "傳送中......"
            TextBox1.Visible = False

            For i = 0 To ListBox1.Items.Count - 2

                Try
                    Dim tcpclient As New TcpClient
                    Dim ip As String = ListBox1.Text
                    Dim port As Integer = 1580
                    tcpclient.Connect(onip(i), port)
                    Thread.Sleep("200")
                    networkStream = tcpclient.GetStream
                    If (TextBox1.Text).Length < 1 Then
                        TextBox1.Text = "空白"
                    End If

                    Try
                        Dim checkhost As IPHostEntry = System.Net.Dns.GetHostEntry(ininip)
                        pcname = checkhost.HostName
                    Catch ex As SocketException
                        pcname = "那個不能說出名字的人"
                    End Try
                    Dim send() As Byte = Encoding.UTF8.GetBytes((pcname + "說:" + TextBox1.Text).ToCharArray)
                    networkStream.Write(send, 0, send.Length)
                    TextBox2.Text = TextBox2.Text + vbCrLf + vbCrLf + "已傳送至" + pcname
                    tcpclient.Close()
                Catch ex As Exception


                End Try

            Next
            TextBox1.Text = ""
            TextBox2.SelectionStart = TextBox2.Text.Length
            TextBox2.ScrollToCaret()
        End If
        Me.Text = "就緒"
        TextBox1.Visible = True
    End Sub

    Private Sub TextBox2_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox2.TextChanged
        'TextBox2.SelectionStart = TextBox2.Text.Length
        'TextBox2.SelectionLength = 0
    End Sub

    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick
        If BackgroundWorker3.IsBusy = 0 Then
            BackgroundWorker3.RunWorkerAsync()
        End If
    End Sub

    Private Sub BackgroundWorker3_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork
        'For i = 0 To 13
        '    If My.Computer.Network.Ping(inip(i)) Then
        '        onip(k1) = inip(i)
        '        Try
        '            Dim checkhost As IPHostEntry = System.Net.Dns.GetHostEntry(onip(k1))
        '            host(k1) = checkhost.HostName
        '        Catch ex As SocketException
        '            host(k1) = "行動裝置"
        '        End Try
        '        k1 += 1
        '    End If
        'Next


        REM

        'For i = 0 To onip.Length
        '    If onip(i).Length > 0 Then
        '        ListBox1.Items.Add(onip(i))
        '        ListBox2.Items.Add(host(i))
        '    End If
        'Next
        'ListBox1.Items.Add("K")


        Dim k1 As Integer = 0
        Dim host(20) As String
        Dim fnip(20) As String

        For i = 0 To fnip.Length - 1
            fnip(i) = " "
        Next

        For i = 0 To inip.Length - 1

            If My.Computer.Network.Ping(inip(i)) Then

                If check(inip(i)) = 1 Then
                    fnip(k1) = inip(i)

                    Try
                        Dim checkhost As IPHostEntry = System.Net.Dns.GetHostEntry(fnip(k1))
                        host(k1) = checkhost.HostName
                    Catch ex As Exception
                        host(k1) = "行動裝置"
                    End Try
                    k1 += 1

                End If
            End If
        Next
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
        ListBox1.Items.Add("廣播")
        ListBox2.Items.Add("")

        For i = 0 To fnip.Length - 1

            If fnip(i).Length > 5 Then
                onip(i) = fnip(i)
                ListBox1.Items.Add(fnip(i))
                ListBox2.Items.Add(host(i))
            End If
        Next
    End Sub

    Private Sub BackgroundWorker4_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork
        Form2.Show()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        TextBox3.Text = ListBox1.SelectedItem
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        If LinkLabel1.Text = "鈴聲開" Then
            LinkLabel1.Text = "鈴聲關"
        Else
            LinkLabel1.Text = "鈴聲開"
        End If

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs)

    End Sub
    Private Function check(ByVal ip As String) As Integer
        Dim tcpclient As New TcpClient
        Dim port As Integer = 1581
        Try

            tcpclient.Connect(ip, port)

            Return 1
            tcpclient.Close()
        Catch ex As Exception

            Return 0
            tcpclient.Close()


        End Try

    End Function

    Private Sub BackgroundWorker5_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker5.DoWork
        Dim ip As IPAddress
        Dim port As Integer = 1581
        ip = IPAddress.Parse(onip(k2))
        Dim TcpListener01 As New TcpListener(ip, port)
        Dim tcpclient01 As New TcpClient
        TcpListener01.Start()


        Try
            tcpclient01 = TcpListener01.AcceptTcpClient
            tcpclient01.ReceiveBufferSize = 1024576
            Dim networkstream01 As NetworkStream = tcpclient01.GetStream
            Dim bytes(tcpclient01.ReceiveBufferSize) As Byte
            networkstream01.Read(bytes, 0, CInt(tcpclient01.ReceiveBufferSize))
        Catch ex As Exception

        End Try
        tcpclient01.Close()
        TcpListener01.Stop()
    End Sub

    Private Sub Timer3_Tick(sender As System.Object, e As System.EventArgs) Handles Timer3.Tick
        If BackgroundWorker5.IsBusy = 0 Then
            BackgroundWorker5.RunWorkerAsync()
            k2 += 1
            If k2 > ListBox1.Items.Count Then
                k2 = 0
            End If
        End If
    End Sub
End Class
