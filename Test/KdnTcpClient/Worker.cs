using KdmsTcpSocket;
using KdmsTcpSocket.Device;
using KdnTcpClient.Shared.Interface;
using System.Net.Sockets;

namespace KdnTcpClient;

public class Worker : BackgroundService
{
    private readonly IApiLogger _logger;
    private readonly KdmsHmiTcpSoket _hmiManager;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Worker(IApiLogger logger
        , IConfiguration configuration
        , KdmsHmiTcpSoket hmiManager
        , IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _hmiManager = hmiManager;
        _applicationLifetime = applicationLifetime;
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var islogin = _hmiManager.KdmsTcpServerLogin("Admin", "1111");
        if (!islogin)
        {
            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("프로그램 종료");
        return base.StopAsync(cancellationToken);
    }

    private void onClientHandler(object sender, TcpRequestEventArgs e)
    {
        Console.WriteLine($"수신정보 - REQ:{e.Request.RequestCode} RES:{e.Request.ResponseCode} CNT:{e.Request.DataCount}");
        switch (e.Request.RequestCode)
        {
            case KdmsCodeInfo.kdmsOperLoginReqs:
                {

                }
                break;

            case KdmsCodeInfo.KdmsOperLogoutReqs:
                {

                }
                break;
        }
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _hmiManager.KemsRTAReceive();
            await Task.Delay(1000, stoppingToken);
        }
    }
}
