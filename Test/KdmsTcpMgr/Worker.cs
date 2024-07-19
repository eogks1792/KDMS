using DSOAdmsRealDataLink;
using KdmsTcpMgr.Shared.Interface;
using KdmsTcpSocket.Unme.Common;

namespace KdmsTcpMgr
{
    public class Worker : BackgroundService
    {
        private readonly IApiLogger _logger;
        private readonly IConfiguration _configuration;
        private readonly KdmsHmiTcpSoket _hmiManager;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public Worker(IApiLogger logger
            , IConfiguration configuration
            , KdmsHmiTcpSoket hmiManager
            , IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _configuration = configuration;
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

            //byte[] bytes = BitConverter.GetBytes(12);
            //byte[] compressBytes = CompressUtility.CompressUsingZlib(bytes);

            _hmiManager.KdmsPdbListDownload();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("프로그램 종료");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pdbIds = new List<int> { 1, 2, 3, 4, 5, 6 };
            await _hmiManager.KdmsPdbFileDownload(pdbIds);

            _hmiManager.KdmsAnalogScan();
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //_hmiManager.KdmsSendHealthCheckData();
                _hmiManager.KemsEVTReceive();
                //_hmiManager.KemsRTAReceive();
                //_hmiManager.KemsCTLReceive();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
