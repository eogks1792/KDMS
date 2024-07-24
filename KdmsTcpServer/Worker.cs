using KdmsTcpServer.Shared.Interface;
using KdmsTcpSocket.Device;
using KdmsTcpSocket.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace KdmsTcpServer
{
    public class Worker : BackgroundService
    {
        private readonly IApiLogger _logger;

        public Worker(IApiLogger logger
            , IConfiguration configuration
            , IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
        }

        static List<string> ExtractValuesFromQuery(string query)
        {
            List<string> valuesList = new List<string>();

            // 'VALUES' �˻� �� ���� �� ����
            int valuesIndex = query.IndexOf("VALUES", StringComparison.OrdinalIgnoreCase);
            if (valuesIndex != -1)
            {
                string valuesPart = query.Substring(valuesIndex + "VALUES".Length).Trim();

                // ������ ���� �����ݷ��� ���� ��� ����
                if (valuesPart.EndsWith(";"))
                {
                    valuesPart = valuesPart.Substring(0, valuesPart.Length - 1);
                }

                // �� ������ ���������� ���� (��ȣ ���� ���� ����)
                var matches = Regex.Matches(valuesPart, @"\([^)]*\)");

                foreach (Match match in matches)
                {
                    valuesList.Add(match.Value);
                }
            }

            return valuesList;
        }

        public void test1()
        {
            string query = "INSERT INTO `log_user_analog_his` VALUES (402036255,11921400000001,31044,30.59,145,0,'0000-00-00 00:00:00.000','2022-12-31 23:58:00.762','2023-01-01 00:00:00.000'),(402045001,11922100022690,38236,0,145,0,'0000-00-00 00:00:00.000','2022-12-31 23:02:28.816','2023-01-01 00:00:00.000');";

            List<string> valuesList = ExtractValuesFromQuery(query);

            foreach (var value in valuesList)
            {
                Console.WriteLine(value);
            }
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("���α׷� ����");
            return base.StopAsync(cancellationToken);
        }

        private void onClientHandler(object sender, TcpRequestEventArgs e)
        {
            
            Console.WriteLine($"�������� - REQ:{e.Request.RequestCode} RES:{e.Request.ResponseCode} CNT:{e.Request.DataCount}");
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Action<object, ITcpSocketMessage> action = onClientHandler;
            int port = 22011;

            TcpListener kdmsListener = new TcpListener(IPAddress.Any, port);
            kdmsListener.Start();
            var kdmsNetwork = new TcpSocketNetwork(kdmsListener, onClientHandler);
            kdmsNetwork.ListenAsync(stoppingToken).GetAwaiter().GetResult();

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}
