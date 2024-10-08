﻿using KdmsSample;
using KdmsTcpSocket;
using KdmsTcpSocket.KdmsTcpStruct;
using KdmsTcpSocket.Message;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Test
{
    private static async Task<int> Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();

        try
        {
            CancellationToken stoppingToken = cts.Token;
            //await Task.Run(async () => {

            //    while (!stoppingToken.IsCancellationRequested)
            //    {
            //        Console.WriteLine("1111111");
            //        await Task.Delay(1000, stoppingToken);
            //        //cts.Cancel();
            //    }
            //});

            await KdermsTcpTest1(stoppingToken);


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        return 0;
    }


    public static async Task KdermsTcpTest1(CancellationToken stoppingToken)
    {
        try
        {
            // 29001 : RT => 로그인
            // 29002 : CTL
            // 29003 : EVT
            using (TcpClient client = new TcpClient("127.0.0.1", 22011))
            {
                var master = KdmsTcpClient.CreateKdmsSocketMaster(client);
                /////////////////////////
                ///
                while (!stoppingToken.IsCancellationRequested)
                {
                    master.Transport.NoResponseData((byte)eActionCode.rt_req, KdmsCodeInfo.KdmsPdbListReqs);
                    await Task.Delay(1000);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }


    public static async Task KdermsTcpTest()
    {
        try
        {
            // 29001 : RT => 로그인
            // 29002 : CTL
            // 29003 : EVT
            using (TcpClient client = new TcpClient("192.168.1.172", 29001))
            {
                var master = KdmsTcpClient.CreateKdmsSocketMaster(client);
                /////////////////////////
                Console.WriteLine("로그인 요청 시작");
                var response = master.SendData<OperLogReq>(KdmsCodeInfo.kdmsOperLoginReqs, KdmsCodeInfo.KdmsOperLoginReps
                    , new OperLogReq { szUserId = "Admin", szUserPw = "1111" });

                var loginResult = KdmsValueConverter.ByteToStruct<OperLogRes>(response.RecvDatas);
                 Console.WriteLine($"로그인 => st:{loginResult.usSt} res:{loginResult.usRes}");

                await Task.Run(() =>
                {
                    using (TcpClient client = new TcpClient("192.168.1.172", 29002))
                    {
                        var master = KdmsTcpClient.CreateKdmsSocketMaster(client);

                        Console.WriteLine("PDB 목록 요청 시작");
                        var pdbResponse = master.SendData<TcpNoData>(KdmsCodeInfo.KdmsPdbListReqs, KdmsCodeInfo.KdmsPdbListReps, null);
                        var pdbResult = KdmsValueConverter.ByteToStructArray<PdbListRes>(pdbResponse.RecvDatas);
                        for (int i = 0; i < pdbResponse.DataCount; i++)
                        {
                            Console.WriteLine($"PDB => ID:{pdbResult[i].iPdbId} PDB:{pdbResult[i].szPdbName} MD5:{pdbResult[i].szPdbMd5}");
                        }

                        var pdbDatas = pdbResult.Take((int)pdbResponse.DataCount).Select(x => new PdbDataReqs { iPdbId = x.iPdbId }).ToList();

                        var pdbDataponse = master.SendListData<PdbDataReqs>(KdmsCodeInfo.KdmsPdbSyncReqs, KdmsCodeInfo.KdmsPdbSyncReqs, pdbDatas);
                        if(pdbDataponse.RequestCode == KdmsCodeInfo.KdmsPdbSyncStart)
                        {

                        }





                        //KdmsValueConverter.StructArrayToByte<int>(list);
                        //foreach (PdbListRes item in pdbResult)
                        //{
                        //    Console.WriteLine($"PDB => ID:{item.iPdbId} PDB:{item.szPdbName} MD5:{item.szPdbMd5}");
                        //}
                    }
                });

                Console.WriteLine($"================================");


                //PdbListRes
                //////////////////
                //var request = new KdmsDataRequest<OperLogReq>(KdmsCodeInfo.kdmsOperLoginReqs, KdmsCodeInfo.KdmsOperLoginReps
                //    , new OperLogReq { szUserId = "1111", szUserPw = "1111" });
                //var loginResponse2 = master.Send(request);


                //// request (요청)데이터 없이 데이터 수신(이벤트 데이터)
                var eventRecvData = master.Recv();



                //master.KdmsLogin("kdms", "1234");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}






