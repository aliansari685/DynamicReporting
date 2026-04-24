using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClientTest.WinForm
{
    public partial class FrmMain : Form
    {
        private CancellationTokenSource? _cts;
        private HubConnection? _hubConnection;

        public FrmMain()
        {
            InitializeComponent();
        }

        async Task<string?> InitializeSignalR(int port, CancellationToken cancellationToken = default)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync(cancellationToken);
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var hubUrl = $"http://localhost:{port}/report-hub";
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            await _hubConnection.StartAsync(cancellationToken);
            return _hubConnection.ConnectionId;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();

                var port = int.Parse(txtBox_Port.Text);
                var idSignalR = await InitializeSignalR(port, _cts.Token);

                richTextBoxLog.AppendTextNewLine("Connected to SignalR Hub! :" + idSignalR);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@$"Connection failed: {ex.Message}");
                _hubConnection?.DisposeAsync();
                _hubConnection = null;
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_hubConnection == null)
                    throw new NullReferenceException("کانال هاب خالی است");

                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                richTextBoxLog.AppendTextNewLine(@"Disconnected");

            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendTextNewLine(ex.Message);
            }
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string serverRes = await (_hubConnection ?? throw new NullReferenceException("کانال هاب خالی است"))
                    .InvokeAsync<string>("Test");
                richTextBoxLog.AppendTextNewLine(serverRes);
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendTextNewLine(ex.Message);
            }
        }

        private void btnClearRichTextBox_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        private async void btnJoinGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (_hubConnection == null)
                    throw new NullReferenceException("کانال هاب خالی است");

                await _hubConnection
                    .InvokeAsync("JoinGroup", txtBox_reportGuid.Text);

                richTextBoxLog.AppendTextNewLine($"Joined to: {txtBox_reportGuid.Text}");

                _hubConnection.On<object>("ReportReady", data =>
                    {
                        Invoke(() =>
                        {
                            richTextBoxLog.AppendTextNewLine(data.ToString());
                        });
                    });
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendTextNewLine(ex.Message);
            }
            //todo : add btn create report and export and waiting for created file and show notif
        }
    }
}