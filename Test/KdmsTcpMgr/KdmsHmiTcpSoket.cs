using KdmsTcpMgr.Shared.Interface;
using KdmsTcpSocket;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;

namespace DSOAdmsRealDataLink;

public class KdmsHmiTcpSoket : ISingletonService
{
    private readonly IApiLogger _logger;
    private readonly IConfiguration _configuration;

    private ITcpSocketMaster? _rtaMaster = null;
    private ITcpSocketMaster? _ctlMaster = null;
    private ITcpSocketMaster? _evtMaster = null;
    public KdmsHmiTcpSoket(IApiLogger logger
            , IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public bool KdmsTcpServerLogin(string username, string password)
    {
        // config에서 IP, 포트 가져옴
        string serverAddr = "192.168.1.172";
        int rtPort = 29001, ctlPort = 29002, evtPort = 29003;
        bool isLogin = false;
        try
        {
            TcpClient rtaClient = new TcpClient(serverAddr, rtPort);
            _rtaMaster = KdmsTcpClient.CreateKdmsSocketMaster(rtaClient);

            _logger.LogInformation($"LOGIN user:{username} pass:{password}");
            var response = _rtaMaster.SendData<OperLogReq>(KdmsCodeInfo.kdmsOperLoginReqs, KdmsCodeInfo.KdmsOperLoginReps
                   , new OperLogReq { szUserId = username, szUserPw = password });

            if(response.RecvDatas != null)
            {
                var loginResult = KdmsValueConverter.ByteToStruct<OperLogRes>(response.RecvDatas);
                if(loginResult.usSt == 1) isLogin = true;

                if(isLogin)
                {
                    _logger.LogInformation($"KDMS SERVER USER:{username} LOGIN SUCC");
                    // CTL/EVT 연결
                    TcpClient ctlClient = new TcpClient(serverAddr, ctlPort);
                    TcpClient evtClient = new TcpClient(serverAddr, evtPort);

                    _ctlMaster = KdmsTcpClient.CreateKdmsSocketMaster(ctlClient);
                    _evtMaster = KdmsTcpClient.CreateKdmsSocketMaster(evtClient);
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"LOGIN FAIL(ex:{ex.Message})");
            if (_rtaMaster != null)
            {
                _rtaMaster.Dispose();
                _rtaMaster = null;
            }
        }

        return isLogin;
    }

    public void KdmsPdbListDownload()
    {
        try
        {
            if (_ctlMaster != null)
            {
                var response = _ctlMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsPdbListReqs, KdmsCodeInfo.KdmsPdbListReps, null);
                if (response.RecvDatas != null)
                {
                    var pdbResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);

                    for (int i = 0; i < response.DataCount; i++)
                    {
                        _logger.LogDebug($"PDB => ID:{pdbResult[i].iPdbId} PDB:{pdbResult[i].szPdbName} MD5:{pdbResult[i].szPdbMd5}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"PDB LIST RCV FAIL(ex:{ex.Message})");
        }
    }

    public void KemsEVTReceive()
    {
        try
        {
            if (_evtMaster != null)
            {
                var response = _rtaMaster.Recv();
                if (response != null)
                {
                    _logger.LogInformation($"EVT RCV => FC:0x{response.RequestCode.ToString("X2")}");
                    if (response.RecvDatas != null)
                    {
                        //var evtResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ALARM RCV FAIL(ex:{ex.Message})");
        }
    }

    public void KemsRTAReceive()
    {
        try
        {
            if (_rtaMaster != null)
            {
                var response = _rtaMaster.Recv();
                if(response != null)
                {
                    _logger.LogInformation($"RTA RCV => FC:0x{response.RequestCode.ToString("X2")}");
                    if (response.RecvDatas != null)
                    {
                        //var evtResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"REAL RCV FAIL(ex:{ex.Message})");
        }
    }


    public void KemsCTLReceive()
    {
        try
        {
            if (_ctlMaster != null)
            {
                var response = _ctlMaster.Recv();
                if (response != null)
                {
                    _logger.LogInformation($"CTL RCV => FC:0x{response.RequestCode.ToString("X2")}");
                    if (response.RecvDatas != null)
                    {
                        //var evtResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"CONTRL RCV FAIL(ex:{ex.Message})");
        }
    }
}
