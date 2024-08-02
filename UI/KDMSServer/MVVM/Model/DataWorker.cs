using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Mask.Internal;
using DevExpress.Data.ODataLinq;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Editors.Validation;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSServer.Features;
using KDMSServer.MVVM.Model;
using KDMSServer.ViewModel;
using KdmsTcpSocket;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using KdmsTcpSocket.Message;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace KDMSServer.Model
{
    public class DataWorker
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly CommonDataModel _commonData;

        private ITcpSocketMaster? rtaMaster = null;
        //private ITcpSocketMaster? ctlMaster = null;
        private ITcpSocketMaster? evtMaster = null;

        private SocketConnectionType socketConnectionType { get; set; }

        private List<PdbListModel> pdbLists = new List<PdbListModel>();

        private bool ThreadFlag { get; set; } = true;
        public DataWorker(ILogger logger, IMediator mediator, IConfiguration configuration, CommonDataModel commonData)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            _commonData = commonData;
        }
        public void Init()
        {
            Task.Run(() =>
            {
                DataBaseStatus();
            });

            Task.Run(() =>
            {
                SocketStatus();
            });

            Task.Run(() =>
            {
                TableCreateWorker();
            });

            Task.Run(() =>
            {
                SchudleDataWorker();
            });
        }

        public void ThreadClose()
        {
            ThreadFlag = false;
        }

        public void SocketClose()
        {
            if (rtaMaster != null)
            {
                rtaMaster.Dispose();
                rtaMaster = null;
            }

            //if (ctlMaster != null)
            //{
            //    ctlMaster.Dispose();
            //    ctlMaster = null;
            //}

            if (evtMaster != null)
            {
                evtMaster.Dispose();
                evtMaster = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////     통신     ////////////////////////////////////////////////////////////////////////////
        public void KdmsServerInit()    // 호출 하는 시점..
        {
            var serverInfos = _configuration.GetSection("ServerInfo");
            var primeIP = serverInfos.GetSection("PrimeServer").Value!;
            var backupIP = serverInfos.GetSection("BackupServer").Value!;

            bool retValue = KdmsTcpServerLogin(primeIP, SocketConnectionType.PRIME);
            if(!retValue)   // 연결 실패시 backup IP로 대체 다시 로그인 처리
            {
                retValue = KdmsTcpServerLogin(backupIP, SocketConnectionType.BACKUP);
                if (retValue)
                {
                    KdmsPdbListDownload();
                    KdmsPdbFileDownload();
                }
            }
            else
            {
                KdmsPdbListDownload();
                KdmsPdbFileDownload();
            }
        }
        
        public bool KdmsTcpServerLogin(string serverAddr, SocketConnectionType type)
        {
            socketConnectionType = type;

            var channelInfos = _configuration.GetSection("ChannelInfo");
            //var contorlPort = Convert.ToInt32(channelInfos.GetSection("Control").Value!); 
            var scanPort = Convert.ToInt32(channelInfos.GetSection("Scan").Value!);       // RT
            var alarmPort = Convert.ToInt32(channelInfos.GetSection("Alarm").Value!);

            var loginInfos = _configuration.GetSection("LoginInfo");
            var loginId = loginInfos.GetSection("LoginId").Value!;
            var loginPwd = loginInfos.GetSection("LoginPwd").Value!;

            // config에서 IP, 포트 가져옴
            bool isLogin = false;
            try
            {
                TcpClient rtaClient = new TcpClient(serverAddr, scanPort);
                rtaMaster = KdmsTcpClient.CreateKdmsSocketMaster(rtaClient);
                rtaMaster.Transport.ReadTimeout = 10 * 1000;

                //_logger.ServerLog($"KDMS SERVER LOGIN 정보 USER:{loginId} PWD:{loginPwd}");
                var response = rtaMaster.SendData<OperLogReq>(KdmsCodeInfo.kdmsOperLoginReqs, KdmsCodeInfo.KdmsOperLoginReps
                       , new OperLogReq { szUserId = loginId, szUserPw = loginPwd });

                if(response == null)
                    throw new Exception("Response DATA NULL");

                if (response.RecvDatas != null)
                {
                    var loginResult = KdmsValueConverter.ByteToStruct<OperLogRes>(response.RecvDatas);
                    if (loginResult.usSt == 1) 
                        isLogin = true;

                    if (isLogin)
                    {
                        // CTL/EVT 연결
                        //TcpClient ctlClient = new TcpClient(serverAddr, contorlPort);
                        //ctlMaster = KdmsTcpClient.CreateKdmsSocketMaster(ctlClient);
                        TcpClient evtClient = new TcpClient(serverAddr, alarmPort);
                        evtMaster = KdmsTcpClient.CreateKdmsSocketMaster(evtClient);
                        evtMaster.Transport.ReadTimeout = 10 * 1000;

                        _logger.ServerLog($"[로그인] KDMS SERVER: {serverAddr} 사용자: {loginId} 연결 성공");
                    }
                    else
                        _logger.ServerLog($"[로그인] KDMS SERVER: {serverAddr} 사용자: {loginId} 연결 실패");
                }
            }
            catch (TcpSocketTimeoutException ex)
            {
                _logger.Error($"[로그인] KDMS SERVER: {serverAddr} 연결 실패 타임아웃 발생(ex:{ex.Message})");
            }
            catch (Exception ex)
            {
                _logger.ServerLog($"[로그인] KDMS SERVER: {serverAddr} 연결 실패(ex:{ex.Message})");
                SocketClose();
            }
            return isLogin;
        }

        public void KdmsPdbListDownload()
        {
            try
            {
                if (rtaMaster != null)
                {
                    var response = rtaMaster.SendData<TcpNoData>(KdmsCodeInfo.KdmsPdbListReqs, KdmsCodeInfo.KdmsPdbListReps, null);
                    if (response.RecvDatas != null)
                    {
                        var pdbResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(response.RecvDatas);

                        for (int i = 0; i < response.DataCount; i++)
                        {
                            _logger.Debug($"[PDB 목록] ID:{pdbResult[i].iPdbId} PDB:{pdbResult[i].szPdbName} MD5:{pdbResult[i].szPdbMd5}");
                            var find = pdbLists.FirstOrDefault(p => p.PdbId == pdbResult[i].iPdbId);
                            if(find != null)
                            {
                                if (find.PdbMd5 != pdbResult[i].szPdbMd5)
                                {
                                    find.PdbMd5 = pdbResult[i].szPdbMd5;
                                    find.IsModify = true;
                                }
                                else
                                    find.IsModify = false;
                            }
                            else
                            {
                                pdbLists.Add(new PdbListModel
                                {
                                    PdbId = pdbResult[i].iPdbId,
                                    PdbName = pdbResult[i].szPdbName,
                                    PdbMd5 = pdbResult[i].szPdbMd5,
                                    IsModify = true
                                });
                            }
                        }
                        _commonData.PdbListSave(pdbLists);
                        _logger.ServerLog($"[PDB 목록] 수신 완료 CNT:{response.DataCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[PDB 목록] RCV FAIL(ex:{ex.Message})");
            }
        }

        public void KdmsPdbFileDownload()
        {
            try
            {
                var pdbDatas = pdbLists.Where(p => p.IsModify).Select(x => new PdbDataReqs { iPdbId = x.PdbId }).ToList();
                if(pdbDatas.Count <= 0)
                {
                    _logger.ServerLog($"[PDB 파일] 변경 사항 없음");
                    return;
                }

                Thread.Sleep(500);
                if (rtaMaster != null)
                {
                    var min = pdbDatas.Min(p => p.iPdbId);
                    var max = pdbDatas.Max(p => p.iPdbId);

                    //string.Join(",", pdbDatas.Select(p => p.iPdbId).ToList())
                    _logger.ServerLog($"[PDB 파일] 요청 전송(PDBID: {min} ~ {max})");
                    var response = rtaMaster.SendListData<PdbDataReqs>(KdmsCodeInfo.KdmsPdbSyncReqs, KdmsCodeInfo.KdmsPdbSyncReqs, pdbDatas);
                    while (true)
                    {
                        if (response != null && response.RequestCode == KdmsCodeInfo.KdmsPdbSyncStart)
                        {
                            if (response.RecvDatas != null)
                            {
                                int pdbid = (response as KdmsPdbDataResponse).PdbId;

                                var find = pdbLists.FirstOrDefault(p => p.PdbId == pdbid);
                                if (find != null)
                                {
                                    if (find.PdbName.Trim() == "pdb_Discrete")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_Discrete>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbDiscretes = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_Command")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_Command>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbCommands = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_Analog")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_Analog>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbAnalogs = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_SetPoint")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_SetPoint>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbSetPoints = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_Accumulator")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_Accumulator>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbAccumulators = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_DMC")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_Dmc>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbDmcs = data.ToList();
                                    }

                                    //////////// 이 아래 항목들은 PDB 수신 후 데이터베이스에 파일 입력 처리
                                    else if (find.PdbName.Trim() == "pdb_RemoteUnit")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_RemoteUnit>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbRemoteUnits = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_CompositSwitch")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_CompositeSwitch>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbCompositeSwitchs = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_ConductingEquipment")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_ConductingEquipment>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbConductingequipments = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_DistributionLineSegment")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_DistributionLineSegment>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbDistributionLineSegments = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_GeographicalRegion")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_GeographicalRegion>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbGeographicalRegions = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_SubGeographicalRegion")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_SubGeographicalRegion>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbSubGeographicalRegions = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_SubStation")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_SubStation>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbSubStations = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_DistributionLine")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_DistributionLine>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbDistributionLines = data.ToList();
                                    }
                                    else if (find.PdbName.Trim() == "pdb_PowerTransformer")
                                    {
                                        var data = KdmsValueConverter.ByteToStructArray<pdb_PowerTransformer>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                            _commonData.pdbPowerTransformers = data.ToList();
                                    }
                                }

                                // 파일 저장 처리
                                PDBFIleSave(pdbid, response.RecvDatas, response.RecvDatas.Length);
                            }
                        }

                        response = rtaMaster.Recv();
                        if (response != null)
                        {
                            if (response.RequestCode == KdmsCodeInfo.KdmsPdbSyncComp)
                            {
                                _logger.ServerLog($"[PDB 파일] 수신 완료(PDBID: {min} ~ {max})");
                                PdbFileUPDate();

                                // 동작 처리
                                Task.Run(() =>
                                {
                                    PdbRealTimeWorker();
                                });

                                Task.Run(() =>
                                {
                                    KemsEVTReceive();
                                });
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[PDB 파일] RCV FAIL(ex:{ex.Message})");
            }
        }

        public void KdmsRealTimePdbFile()
        {
            try
            {
                var pdbDatas = new List<PdbDataReqs>()
                {
                    new PdbDataReqs { iPdbId = 57},
                    new PdbDataReqs { iPdbId = 60}
                };

                if (rtaMaster != null)
                {
                    _logger.Debug($"[PDB 파일] 요청 전송(PDBID: 57, 60)");
                    _commonData.rtdbAnalogs = new List<rtdb_Analog>();
                    _commonData.rtdbDmcs = new List<rtdb_Dmc>();
                    var response = rtaMaster.SendListData<PdbDataReqs>(KdmsCodeInfo.KdmsPdbSyncReqs, KdmsCodeInfo.KdmsPdbSyncReqs, pdbDatas);
                    while (true)
                    {
                        if (response != null && response.RequestCode == KdmsCodeInfo.KdmsPdbSyncStart)
                        {
                            if (response.RecvDatas != null)
                            {
                                int pdbid = (response as KdmsPdbDataResponse).PdbId;
                                // 파일 저장 처리
                                switch(pdbid)
                                {
                                    case 57:
                                        {
                                            var data = KdmsValueConverter.ByteToStructArray<rtdb_Analog>(response.RecvDatas);
                                            if (data != null && data.Length > 0)
                                                _commonData.rtdbAnalogs = data.ToList();
                                        }
                                        break;
                                    case 60:
                                        {
                                            var data = KdmsValueConverter.ByteToStructArray<rtdb_Dmc>(response.RecvDatas);
                                            if (data != null && data.Length > 0)
                                                _commonData.rtdbDmcs = data.ToList();
                                        }
                                        break;
                                }
                            }
                        }

                        response = rtaMaster.Recv();
                        if (response != null)
                        {
                            if (response.RequestCode == KdmsCodeInfo.KdmsPdbSyncComp)
                            {
                                _logger.Debug($"[PDB 파일] 수신 완료(PDBID: 57, 60)");
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[PDB 파일] RCV FAIL(ex:{ex.Message})");
            }
        }

        private void PDBFIleSave(int pdbId, byte[] rcvData, int count)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}pdb";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = _commonData.PdbFileName(pdbId);
            using (FileStream stream = new FileStream($"{path}\\{fileName}.pdb", FileMode.Create))
            {
                stream.Write(rcvData, 0, count);
                stream.Close();
                _logger.Debug($"[PDB 파일] ID:{pdbId} NM:{fileName} 파일 저장");
            }
        }

        public async void KemsEVTReceive()
        {
            while (ThreadFlag)
            {
                try
                {
                    if (evtMaster != null)
                    {
                        var response = evtMaster.Recv();
                        if (response != null)
                        {
                            switch (response.RequestCode)
                            {
                                case KdmsCodeInfo.KdmsAlarmEvent:
                                    {
                                        _logger.Information($"[Alarm 수신] FC:0x{response.RequestCode.ToString("X2")} AE_UPDATE 수신");
                                        // 데이터 저장 처리
                                        if (response.RecvDatas == null)
                                        {
                                            _logger.Information($"[Alarm 수신] 알람/이벤트 데이터 없음");
                                            continue;
                                        }

                                        var data = KdmsValueConverter.ByteToStructArray<rtdb_Alarm>(response.RecvDatas);
                                        if (data != null && data.Length > 0)
                                        {
                                            var alarmList = data.ToList();
                                            var allAlarmList = alarmList.Where(p => p.uiPtType != (int)PointTypeCode.DMC).ToList();
                                            if (allAlarmList.Count > 0)
                                            {
                                                await Task.Run(() =>
                                                {
                                                    _commonData.FiAlarmDataSave(alarmList);
                                                });
                                            }

                                            var dmcAlarmList = alarmList.Where(p => p.uiPtType == (int)PointTypeCode.DMC).ToList();
                                            if (dmcAlarmList.Count > 0)
                                            {
                                                await Task.Run(() =>
                                                {
                                                    _commonData.CommStateLogDataSave(alarmList);
                                                });
                                            }
                                        }
                                    }
                                    break;
                                case KdmsCodeInfo.KdmsHeartBeat:
                                    {
                                        _logger.Debug($"[Alarm 수신] FC:0x{response.RequestCode.ToString("X2")} HEART_BEAT 수신");
                                    }
                                    break;
                            }

                        }
                    }
                }
                catch (TcpSocketTimeoutException ex)
                {
                    _logger.Error($"[Alarm 수신] 타임아웃 발생(ex:{ex.Message})");
                }
                catch (Exception ex)
                {
                    _logger.Error($"[Alarm 수신] FAIL(ex:{ex.Message})");
                }

                await Task.Delay(1000);
            }
        }

        private async void PdbRealTimeWorker()
        {
            Thread.Sleep(1000);

            DateTime pdbModifyInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour: 1, min: 0);
            _logger.ServerLog($"[PDB 목록] 다운로드 시간: {pdbModifyInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            string minString = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.MINDATA)!.SchduleValue.ToString();
            DateTime minDt = Convert.ToDateTime(DateTime.Now.ToString(minString));
            DateTime minDataInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Minute, min: minDt.Minute);
            _logger.ServerLog($"[1분 실시간] 데이터 생성 시간: {minDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            string min15String = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.STATISTICSMIN)!.SchduleValue.ToString();
            int addMin = 15 - (DateTime.Now.Minute % 15);
            DateTime min15DataInitialTime = Convert.ToDateTime(DateTime.Now.AddMinutes(addMin).AddMinutes(Convert.ToInt32(min15String)).ToString("yyyy-MM-dd HH:mm:00"));
            _logger.ServerLog($"[15분 실시간(평균부하전류)] 데이터 생성 시간: {min15DataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            string commstateString = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.COMMSTATE)!.SchduleValue.ToString();
            DateTime commstateDt = Convert.ToDateTime(DateTime.Now.ToString(commstateString));
            DateTime commstateDataInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.None, hour: commstateDt.Hour, min: commstateDt.Minute);
            _logger.ServerLog($"[통신 성공률] 데이터 생성 시간: {commstateDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            while (ThreadFlag)
            {
                try
                {
                    var nowTime = DateTime.Now;
                    if (minDataInitialTime <= nowTime)
                    {
                        KdmsRealTimePdbFile();
                        if (_commonData.rtdbAnalogs.Count > 0)
                        {
                            _logger.ServerLog($"[1분 실시간] {minDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 시작");
                            await Task.Run(() =>
                            {
                                _commonData.MinDataSave(minDataInitialTime);
                            });

                            if (min15DataInitialTime <= nowTime)
                            {
                                _logger.ServerLog($"[15분 실시간(평균부하전류)] {min15DataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 시작");
                                await Task.Run(() =>
                                {
                                    _commonData.StatisticsMinDataSave(min15DataInitialTime);
                                });

                                min15DataInitialTime = min15DataInitialTime.AddMinutes(15);
                                _logger.ServerLog($"[15분 실시간(평균부하전류)] NEXT 데이터 생성 시간: {min15DataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                            }
                        }

                        minDataInitialTime = minDataInitialTime.AddMinutes(1);
                        //_logger.ServerLog($"[1분 실시간] NEXT 데이터 생성 시간: {minDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (commstateDataInitialTime <= nowTime)
                    {
                        if(_commonData.rtdbDmcs.Count > 0)
                        {
                            _logger.ServerLog($"[통신 성공률] {commstateDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 시작");
                            await Task.Run(() =>
                            {
                                _commonData.CommStateDataSave(commstateDataInitialTime);
                            });
                        }

                        commstateDataInitialTime = commstateDataInitialTime.AddHours(12);
                        _logger.ServerLog($"[PDB 목록] NEXT 다운로드 시간: {commstateDataInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (pdbModifyInitialTime <= nowTime)
                    {
                        KdmsPdbListDownload();
                        KdmsPdbFileDownload();

                        await Task.Run(() =>
                        {
                            PdbFileUPDate();
                        });

                        pdbModifyInitialTime = pdbModifyInitialTime.AddDays(1);
                        _logger.ServerLog($"[PDB 목록] NEXT 다운로드 시간: {pdbModifyInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                await Task.Delay(1000);
            }
        }

        private void PdbFileUPDate()
        {
            // 데이터 베이스 저장 처리
            // PDB_CONDUCTINGEQUIPMENT [설비 정보]
            // PDB_DISTRIBUTIONLINESEGMENT [선로 정보]
            // GEOGRAPHICALREGION [조직 정보(지사)]
            // SUBGEOGRAPHICALREGION [조직 정보(지점)]
            // SUBSTATION [변전소 정보]
            // DISTRIBUTIONLINE [배전선로(DL) 정보
            // POWERTRANSFORMER [MTR 정보]

            var pdbModifyList = pdbLists.Where(p => p.IsModify).ToList();
            if (pdbModifyList.Count <= 0)
                return;

            foreach (var pdb in pdbModifyList)
            {
                try
                {
                    if (pdb.PdbName.Trim() == "pdb_RemoteUnit")
                        _commonData.RemoteUnitSave();
                    else if (pdb.PdbName.Trim() == "pdb_CompositSwitch")
                        _commonData.CompositSwitchSave();
                    else if (pdb.PdbName.Trim() == "pdb_ConductingEquipment")
                        _commonData.ConductingequipmentSave();
                    else if (pdb.PdbName.Trim() == "pdb_DistributionLineSegment")
                        _commonData.DistributionLineSegmentSave();
                    else if (pdb.PdbName.Trim() == "pdb_GeographicalRegion")
                        _commonData.GeographicalRegionSave();
                    else if (pdb.PdbName.Trim() == "pdb_SubGeographicalRegion")
                        _commonData.SubGeographicalRegionSave();
                    else if (pdb.PdbName.Trim() == "pdb_SubStation")
                        _commonData.SubStationSave();
                    else if (pdb.PdbName.Trim() == "pdb_DistributionLine")
                        _commonData.DistributionLineSave();
                    else if (pdb.PdbName.Trim() == "pdb_PowerTransformer")
                        _commonData.PowerTransformerSave();
                }
                catch (Exception ex)
                {
                    _logger.Error($"[PDB 파일] {pdb.PdbName.Trim()} 데이터 데이터베이스 입력중 에외발생(ex:{ex.Message})");
                }
            }
        }

        /////////////////////////////////////////////////////////////////////// 데이터베이스 ////////////////////////////////////////////////////////////////////////////
        public async void GetProcData(int type, DateTime time, bool manual = false)
        {
            switch (type)
            {
                case (int)ProcTypeCode.MINDATA:
                    {

                    }
                    break;
                case (int)ProcTypeCode.DAYSTATDATA:
                    {
                        _logger.DbLog($"[1일 통계(1분실시간전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        DaystatDataCalculation.Command request = new DaystatDataCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[1일 통계(1분실시간전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[1일 통계(1분실시간전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSMIN:
                    {
                        
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSHOUR:
                    {
                        _logger.DbLog($"[시간 통계(평균부하전류)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsHourCalculation.Command request = new StatisticsHourCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[시간 통계(평균부하전류)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[시간 통계(평균부하전류)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSDAY:
                    {
                        _logger.DbLog($"[일 통계(평균부하전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsDayCalculation.Command request = new StatisticsDayCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[일 통계(평균부하전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[일 통계(평균부하전류)] {time.ToString("yyyy-MM-dd")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSMONTH:
                    {
                        _logger.DbLog($"[월 통계(평균부하전류)] {time.ToString("yyyy-MM")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsMonthCalculation.Command request = new StatisticsMonthCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[월 통계(평균부하전류)] {time.ToString("yyyy-MM")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[월 통계(평균부하전류)] {time.ToString("yyyy-MM")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSYEAR:
                    {
                        _logger.DbLog($"[년 통계(평균부하전류)] {time.ToString("yyyy")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsYearCalculation.Command request = new StatisticsYearCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[년 통계(평균부하전류)] {time.ToString("yyyy")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[년 통계(평균부하전류)] {time.ToString("yyyy")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.FIALARM:
                    {
                       
                    }
                    break;
                case (int)ProcTypeCode.COMMSTATE:
                    {
                      
                    }
                    break;
                case (int)ProcTypeCode.COMMSTATELOG:
                    {
                        
                    }
                    break;
            }

            var model = App.Current.Services.GetService<MainViewModel>()!;
            if (model != null)
                model.IsInput = true;
        }
       

        private async void SchudleDataWorker()
        {
            Thread.Sleep(2500);

            string HourString = _commonData.SchduleInfos.FirstOrDefault(x=>x.SchduleId == (int)ProcTypeCode.STATISTICSHOUR)!.SchduleValue.ToString();
            string DayString = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.STATISTICSDAY)!.SchduleValue.ToString();
            string MonthString = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.STATISTICSMONTH)!.SchduleValue.ToString();
            string YearString = _commonData.SchduleInfos.FirstOrDefault(x => x.SchduleId == (int)ProcTypeCode.STATISTICSYEAR)!.SchduleValue.ToString();
            string DayStringForOneMinute = _commonData.SchduleInfos.FirstOrDefault(p => p.SchduleId == (int)ProcTypeCode.DAYSTATDATA)?.SchduleValue.ToString();
            DateTime HourDt = Convert.ToDateTime(DateTime.Now.ToString(HourString));
            DateTime hourInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Hour, min:HourDt.Minute);
            _logger.DbLog($"[시간 통계(평균부하전류)] 데이터 생성 시간: {hourInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime DayDt = Convert.ToDateTime(DateTime.Now.ToString(DayString));
            DateTime dayInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour:DayDt.Hour);
            _logger.DbLog($"[일 통계(평균부하전류)] 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime MonthDt = Convert.ToDateTime(DateTime.Now.ToString(MonthString));
            DateTime monthInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Month, day:MonthDt.Day, hour:MonthDt.Hour);
            _logger.DbLog($"[월 통계(평균부하전류)] 데이터 생성 시간: {monthInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime YearDt = Convert.ToDateTime(DateTime.Now.ToString(YearString));
            DateTime yearhInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Year, month:YearDt.Month, day:YearDt.Day, hour:YearDt.Hour);
            _logger.DbLog($"[년 통계(평균부하전류)] 데이터 생성 시간: {yearhInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime DayForOneMinute = Convert.ToDateTime(DateTime.Now.ToString(DayStringForOneMinute));
            DateTime dayInitialTimeForOneMinute = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour: DayForOneMinute.Hour, min: DayForOneMinute.Minute, sec: DayForOneMinute.Second);
            _logger.DbLog($"[1일 통계(1분실시간전류)] 데이터 생성 시간: {dayInitialTimeForOneMinute.ToString("yyyy-MM-dd HH:mm:ss")}");

            while (ThreadFlag)
            {
                try
                {
                    if (hourInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSHOUR, hourInitialTime.AddHours(-1));
                            else
                                _logger.DbLog($"{hourInitialTime.AddHours(-1).ToString("yyyy-MM-dd HH:00:00")} [시간 통계(평균부하전류)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        hourInitialTime = hourInitialTime.AddHours(1);
                        _logger.DbLog($"[시간 통계(평균부하전류)] NEXT 데이터 생성 시간: {hourInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (dayInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSDAY, dayInitialTime.AddDays(-1));
                            else
                                _logger.DbLog($"{dayInitialTime.AddDays(-1).ToString("yyyy-MM-dd")} [일 통계(평균부하전류)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        dayInitialTime = dayInitialTime.AddDays(1);
                        _logger.DbLog($"[일 통계(평균부하전류)] NEXT 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (monthInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSMONTH, monthInitialTime.AddMonths(-1));
                            else
                                _logger.DbLog($"{monthInitialTime.AddMonths(-1).ToString("yyyy-MM")} [월 통계(평균부하전류)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        monthInitialTime = monthInitialTime.AddMonths(1);
                        _logger.DbLog($"[월 통계(평균부하전류)] NEXT 데이터 생성 시간: {monthInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (yearhInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSYEAR, yearhInitialTime.AddYears(-1));
                            else
                                _logger.DbLog($"{yearhInitialTime.AddYears(-1).ToString("yyyy")} [년 통계(평균부하전류)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        yearhInitialTime = yearhInitialTime.AddYears(1);
                        _logger.DbLog($"[년 통계(평균부하전류)] NEXT 데이터 생성 시간: {yearhInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (dayInitialTimeForOneMinute <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.DAYSTATDATA, dayInitialTimeForOneMinute.AddDays(-1));
                            else
                                _logger.DbLog($"{dayInitialTimeForOneMinute.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")} [1일 통계(1분활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        dayInitialTimeForOneMinute = dayInitialTimeForOneMinute.AddDays(1);
                        _logger.DbLog($"[1일 통계(1분활용)] NEXT 데이터 생성 시간: {dayInitialTimeForOneMinute.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                await Task.Delay(1000);
            }
        }

        private async void TableCreateWorker()
        {
            Thread.Sleep(1000);

            DateTime yearInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now.AddYears(-1), TimeDivisionCode.Year, month:12, day:31, hour:1);
            _logger.DbLog($"[1일 통계(1분실시간전류)] 테이블 생성 시간: {yearInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            DateTime dayInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour: 23, min:55);
            _logger.DbLog($"[1분 실시간] 테이블 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            while (ThreadFlag)
            {
                try
                {
                    if (dayInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if(model.IsDBConnetion)
                            {
                                bool retval = _commonData.MinDataTableCreate(dayInitialTime);
                                if (retval)
                                {
                                    _logger.DbLog($"{dayInitialTime.AddDays(1).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 성공");
                                    _logger.DbLog($"{dayInitialTime.AddDays(2).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 성공");
                                }
                            }
                            else
                            {
                                _logger.DbLog($"{dayInitialTime.AddDays(1).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 실패 (DB 접속 실패) ");
                                _logger.DbLog($"{dayInitialTime.AddDays(2).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 실패 (DB 접속 실패) ");
                            }
                        }
                        dayInitialTime = dayInitialTime.AddDays(1);
                        _logger.DbLog($"[1분 실시간] NEXT 테이블 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (yearInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                            {
                                bool retval = _commonData.DayStatTableCreate(yearInitialTime);
                                if (retval)
                                {
                                    _logger.DbLog($"{yearInitialTime.AddYears(1).ToString("yyyy")} [1일 통계(1분실시간전류)] 테이블 생성 성공");
                                }
                            }
                            else
                                _logger.DbLog($"{yearInitialTime.AddYears(1).ToString("yyyy")} [1일 통계(1분실시간전류)] 테이블 생성 실패 (DB 접속 실패) ");
                        }
                        yearInitialTime = yearInitialTime.AddYears(1);
                        _logger.DbLog($"[1일 통계(1분실시간전류)] NEXT 테이블 생성 시간: {yearInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                await Task.Delay(1000);
            }
        }

        /////////////////////////////////////////////////////////////////////// 상태 체크 ////////////////////////////////////////////////////////////////////////////

        private async void SocketStatus()
        {
            int checkSocketCnt = 0;
            string checkSocketText = string.Empty;
            while (ThreadFlag)
            {
                try
                {
                    var model = App.Current.Services.GetService<MainViewModel>()!;
                    if (model != null)
                    {
                        // 소켓 통신 상태
                        try
                        {
                            if (!string.IsNullOrEmpty(model.IsSocketConnetionText))
                                checkSocketText = model.IsSocketConnetionText;

                            if (checkSocketText == model.IsSocketConnetionText)
                                checkSocketCnt++;

                            if (checkSocketCnt == 6)
                            {
                                checkSocketCnt = 0;
                                checkSocketText = string.Empty;
                                model.IsSocketConnetionText = string.Empty;
                            }

                            if (rtaMaster == null || evtMaster == null)     // 소켓 상태가 한개라도 NULL 이면 전체 재연결 처리
                            {
                                model.ScanState = false;
                                model.EventState = false;

                                model.IsSocketConnetion = false;
                                model.IsSocketConnetionState = "실패";

                                SocketClose();      // 현재 소켓이 연결되어 있으면 연결 종료 처리
                                Thread.Sleep(500);
                                KdmsServerInit();   // 소켓 연결 처리
                            }
                            else
                            {
                                model.IsSocketConnetion = true;
                                model.IsSocketConnetionState = "성공";

                                if (rtaMaster != null)
                                    model.ScanState = true;

                                if (evtMaster != null)
                                    model.EventState = true;
                            }
                        }
                        catch
                        {
                            model.IsSocketConnetion = false;
                            model.IsSocketConnetionState = "실패";
                        }
                    }
                    await Task.Delay(1000);
                }
                catch
                {

                }
            }
        }

        private async void DataBaseStatus()
        {
            int checkDBCnt = 0;
            string checkDBText = string.Empty;
            while (ThreadFlag)
            {
                try
                {
                    var model = App.Current.Services.GetService<MainViewModel>()!;
                    if (model != null)
                    {
                        // DB 상태
                        try
                        {
                            if (!string.IsNullOrEmpty(model.IsDBConnetionText))
                                checkDBText = model.IsDBConnetionText;

                            if (checkDBText == model.IsDBConnetionText)
                                checkDBCnt++;

                            if (checkDBCnt == 6)
                            {
                                checkDBCnt = 0;
                                checkDBText = string.Empty;
                                model.IsDBConnetionText = string.Empty;
                            }

                            bool retValue = new MySqlMapper(_configuration).IsConnection();
                            if (retValue)
                                model.IsDBConnetionState = "성공";
                            else
                                model.IsDBConnetionState = "실패";

                            model.IsDBConnetion = retValue;
                        }
                        catch
                        {
                            model.IsDBConnetion = false;
                            model.IsDBConnetionState = "실패";
                        }
                    }
                    await Task.Delay(1000);
                }
                catch
                {

                }
            }
        }

    }
}
