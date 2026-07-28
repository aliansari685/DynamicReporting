using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClientTest.WinForm;

public partial class FrmMain : Form
{
    private HubConnection? _hubConnection;

    public FrmMain()
    {
        InitializeComponent();
    }

    private async Task<string?> InitializeSignalR(int port, CancellationToken cancellationToken = default)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync(cancellationTokenSource.Token);
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var hubUrl = $"https://localhost:{port}/report-hub";
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        await _hubConnection.StartAsync(cancellationTokenSource.Token);
        return _hubConnection.ConnectionId;
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        try
        {
            var port = int.Parse(txtBox_Port.Text);
            var idSignalR = await InitializeSignalR(port);
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
            var serverRes = await (_hubConnection ?? throw new NullReferenceException("کانال هاب خالی است"))
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
            await JoinGroup(txtBox_reportGuid.Text);
        }
        catch (Exception ex)
        {
            richTextBoxLog.AppendTextNewLine(ex.Message);
        }
    }

    private async void btnFullTestExport_Click(object sender, EventArgs e)
    {
        try
        {
            using var client = new HttpClient();
            var port = int.Parse(txtBox_Port.Text);
            var backEndUrl = $"https://localhost:{port}";
            client.BaseAddress = new Uri(backEndUrl);
            var response = await client.GetAsync("api/report-export/export/3?type=excel");

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var result = await response.Content.ReadFromJsonAsync<ExportResponse>();
                richTextBoxLog.AppendTextNewLine(result?.Message);
                var idSignalR = await InitializeSignalR(port);
                richTextBoxLog.AppendTextNewLine("Connected to SignalR Hub! :" + idSignalR);
                await JoinGroup(result?.ReportId ?? throw new NullReferenceException("شناسه گزارش خالی است"));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(@$"Connection failed: {ex.Message}");
            _hubConnection?.DisposeAsync();
            _hubConnection = null;
        }
    }

    private async Task JoinGroup(string id)
    {
        if (_hubConnection == null)
            throw new NullReferenceException("کانال هاب خالی است");

        await _hubConnection
            .InvokeAsync("JoinGroup", id);

        richTextBoxLog.AppendTextNewLine($"Joined to: {id}");

        _hubConnection.On<object>("ReportReady",
            data => { Invoke(() => { richTextBoxLog.AppendTextNewLine(data.ToString()); }); });
    }
}