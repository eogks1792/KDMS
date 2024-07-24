using KdmsTcpSocket;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.Message;
using KdnTcpClient.Shared.Interface;
using System.Net.Sockets;

namespace KdnTcpClient;

public class KdmsHmiTcpSoket : ISingletonService
{
    private readonly IApiLogger _logger;
    private readonly IConfiguration _configuration;

    private ITcpSocketMaster? _rtaMaster = null;

    public KdmsHmiTcpSoket(IApiLogger logger
            , IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }



    public bool KdmsTcpServerLogin(string username, string password)
    {
        // config에서 IP, 포트 가져옴
        string serverAddr = "127.0.0.1";
        int rtPort = 22011;
        bool isLogin = false;
        try
        {
            TcpClient rtaClient = new TcpClient(serverAddr, rtPort);
            _rtaMaster = KdmsTcpClient.CreateKdmsSocketMaster(rtaClient);
            _rtaMaster.Transport.ReadTimeout = TcpSocket.ReadTimeOut;

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
                }
            }

        }
        catch(TcpSocketTimeoutException ex)
        {
            _logger.LogError(ex.Message);
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
            if (_rtaMaster is not null)
            {
                var response = _rtaMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsPdbListReqs, KdmsCodeInfo.KdmsPdbListReps, null);
                if (response is not null && response.RecvDatas is not null)
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
                            if (response is KdmsPdbDataResponse pdbResponse)
                            {
                                _logger.LogInformation($"PDB:{pdbResponse.PdbId} => 파일 수신 및 처리1");
                            }
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

    public void CtlDispose()
    {
        if (_rtaMaster != null)
        {
            _rtaMaster.Dispose();
            _rtaMaster = null;
        }
    }
}
