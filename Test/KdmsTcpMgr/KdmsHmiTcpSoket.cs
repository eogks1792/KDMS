using KdmsTcpMgr.Shared.Interface;
using KdmsTcpSocket;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using KdmsTcpSocket.Message;
using System.Net.Sockets;

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
            if (response == null)
            {
                throw new Exception("LOGIN Response NULL");
            }

            if (response.RecvDatas != null)
            {
                var loginResult = KdmsValueConverter.ByteToStruct<OperLogRes>(response.RecvDatas);
                if (loginResult.usSt == 1) isLogin = true;

                if (isLogin)
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

    public void KdmsSendHealthCheckData()
    {
        if (_rtaMaster != null)
            _rtaMaster.Transport.NoResponseData((byte)eActionCode.rt_req, KdmsCodeInfo.KdmsUnknownReqs);


        //if (_rtaMaster != null)
        //{
        //    var response = _ctlMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsUnknownReqs, KdmsCodeInfo.KdmsUnknownReqs, null);
        //    Console.WriteLine();
        //}

    }

    public void KdmsPdbListDownload()
    {
        try
        {
            if (_rtaMaster != null)
            {
                var response = _rtaMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsPdbListReqs, KdmsCodeInfo.KdmsPdbListReps, null);
                if (response != null && response.RecvDatas != null)
                {
                    var pdbResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);

                    for (int i = 0; i < response.DataCount; i++)
                    {
                        _logger.LogInformation($"PDB => ID:{pdbResult[i].iPdbId} PDB:{pdbResult[i].szPdbName} MD5:{pdbResult[i].szPdbMd5}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"PDB LIST RCV FAIL(ex:{ex.Message})");
        }
    }

    public void KdmsAnalogScan()
    {
        try
        {
            if (_rtaMaster != null)
            {
                var response = _rtaMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsRtAIReqs, KdmsCodeInfo.KdmsRtAIReqs, null);
                if (response != null && response.RecvDatas != null)
                {
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void KdmsDMCScan()
    {
        try
        {
            if (_rtaMaster != null)
            {
                var response = _rtaMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsRtDMCReqs, KdmsCodeInfo.KdmsRtDMCReqs, null);
                if (response != null && response.RecvDatas != null)
                {

                }
            }
        }
        catch (Exception ex)
        {
        }
    }
    public async Task KdmsPdbFileDownload(IEnumerable<int> pdbIds)
    {
        try
        {
            //CtlDispose();
            //if(_rtaMaster == null)
            //{
            //    TcpClient ctlClient = new TcpClient("192.168.1.172", 29001);
            //    _rtaMaster = KdmsTcpClient.CreateKdmsSocketMaster(ctlClient);
            //}

            await Task.Delay(500);

            var pdbDatas = pdbIds.Select(x => new PdbDataReqs { iPdbId = x }).ToList();
            if (_rtaMaster != null)
            {
                _logger.LogInformation($"PDB 파일 요청 전송({string.Join(",", pdbIds)})");
                var response = _rtaMaster.SendListData<PdbDataReqs>(KdmsCodeInfo.KdmsPdbSyncReqs, KdmsCodeInfo.KdmsPdbSyncReqs, pdbDatas);
                while (true)
                {
                    if (response != null && response.RequestCode == KdmsCodeInfo.KdmsPdbSyncStart)
                    {
                        if (response.RecvDatas != null)
                        {
                            int pdbid = (response as KdmsPdbDataResponse).PdbId;
                            _logger.LogInformation($"PDB:{pdbid} => 파일 수신 및 처리1");
                            //var rcvDatas = KdmsValueConverter.ByteToStructArray<PdbInfoAnalog>(pdbDataponse.RecvDatas);
                        }
                    }

                    response = _rtaMaster.Recv();
                    if (response != null)
                    {
                        if (response.RequestCode == KdmsCodeInfo.KdmsPdbSyncComp)
                        {
                            _logger.LogInformation($"PDB => 파일 수신 완료");
                            break;
                        }
                    }
                }

                //KemsCTLReceive();
                //KemsCTLReceive();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"PDB FILE RCV FAIL(ex:{ex.Message})");
        }
    }


    public void KemsEVTReceive()
    {
        try
        {
            if (_evtMaster != null)
            {
                var response = _evtMaster.Recv();
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
                if (response != null)
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
            if (_rtaMaster != null)
            {
                var response = _rtaMaster.Recv();
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

    public void CtlDispose()
    {
        if(_rtaMaster != null )
        {
            _rtaMaster.Dispose();
            _rtaMaster = null;
        }
    }
}
