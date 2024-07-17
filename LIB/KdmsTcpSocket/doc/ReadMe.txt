1. 인터페이스 작성 순서
   1) ITcpSocketMaster  => TcpSocketMaster 

2. 전송 함수
   1) Device/TcpSocketMaster.cs
   2) IO/TcpSocketIpTransport.cs

3. 소켓 콜 정리
   1) 로그인 = > FC_OPER_LOGIN_REQS =>rt
                FC_OPER_LOGIN_REPS
   2) PDB LIST = > FC_PDB_LIST_REQS =>  ctrl
                   FC_PDB_LIST_REPS
   3) PDB SYNC = > FC_PDB_SYNC_REQS ==> ctrl 
                   FC_PDB_SYNC_START
                   FC_PDB_SYNC_END
                   FC_PDB_SYNC_COMPLET
                   FC_PDB_SYNC_REPS=> 사용하지 않음.
   4) RT AI/DMC => FC_RT_AI_REQS   => rtsd
                   FC_RT_DATA_AI
                   FC_RT_DMC_REQS
                   FC_RT_DATA_DMC
   5) 이벤트  = >   FC_AE_UPDATE   ==> aesd

   6) H

4. ENUM 정리 : TcpSoketEnums
5. ValueConverter : KdmsValueConverter
6. 함수코드 : KdmsCodeInfo
7. 구조체 : KdmsSocketData

