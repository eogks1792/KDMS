using KdmsTcpSocket;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Utils.SafeXml;

namespace KDMSServer.Model
{

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PdbListRes
    {
        public Int32 iPdbId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szPdbName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string szPdbMd5;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }



    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PdbDataReqs
    {
        public Int32 iPdbId;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OperLogReq
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szUserId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szUserPw;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OperLogRes
    {
        public UInt16 usSt;
        public UInt16 usRes;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct rtdb_Analog
    {
        public UInt32 pid;
        public double value;
        public UInt32 tlq;
        public UInt32 last_update;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct rtdb_Dmc
    {
        public UInt32 pid;
        public double value;
        public UInt32 tlq;
        public UInt32 last_update;

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct rtdb_Alarm
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string szevtid;                     //timestamp(16) + poDT_INTno(7) : 24Byte
        public UInt32 uiRTUTime;                  //RTU Event 발생시간 Seconds
        public UInt32 uiRTUMics;                  //RTU Event 발생시간 Micro Seconds
        public UInt32 uiSVRTime;                  //SERVER Event 생성 시간 Seconds
        public UInt32 uiSVRMics;                  //SERVER Event 생성 시간 Microseconds
        public UInt32 uiStation;                  //소속 변전소 ID
        public UInt32 uiDL;                       //소속 DL ID
        public UInt32 uiEqid;                     //Equipment ID 
        public UInt32 uiRtuid;                    //RTU ID
        public UInt32 uiPtType;                   //PoDT_INT Type(1:BI,2:BO,3:AI,4:AO,5,PI,6:DMC
        public UInt32 uiPid;                      //PoDT_INT ID
        public double fVal;                       //PoDT_INT Value
        public UInt32 uiTlq;                      //PoDT_INT Tlq(Tag,Limit,Quality)
        public UInt32 uiAC;                       //Alarm Category ID
        public UInt32 uiAClass;                   //Alarm Class ID
        public UInt32 uiPri;                      //Alarm Class 우선순위
        public UInt32 uiColor;                    //Color 
        public UInt32 uiAudio;                    //Audio Volume
        public UInt32 uiArea;                     //AcessArea ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szDesc;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        //public string szDesc;				        //Alarm 동작 설명

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_Discrete
    {
        public UInt32 pid;                              // Discrete ID            
        public UInt32 datalink_fk;                      // DataLink ID          
        public UInt32 st_fk;                            // 소속 변전소 ID                
        public UInt32 rtu_fk;                           // RTU ID           
        public UInt32 ceq_fk;                           // ConductingEquipment ID
        public UInt32 dl_fk;                            // 소속 D/L ID         
        public UInt32 tm_fk;                            // Terminal ID           
        public UInt32 vas_fk;                           // 상태 유닛 Set ID      
        public UInt32 bo_fk;                            // 제어 ID(BO point) 
        public UInt32 dp_fk;                            // DataPointInfo ID   
        public UInt32 ac_fk;                            // Alarm Catagory ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                             // Point Name            
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                        // Point AliasName        
        public UInt32 position;                          // DNP Index     
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string object_variation;                 // DNP ObjectVariation
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string dnp_class;                        // DNP Use Class
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string measurement_type;                 // IED LN Code  
        public UInt32 asdu_addr;                        // ASDU Address(60870)    
        public UInt32 group_addr;                        // GROUP Address(60870)  
        public UInt32 circuit_no;                        // Circuit No
        public UInt32 use_ems;                          // Application Use             
        public UInt32 abnormal_value;                    // 비정상 값 
        public UInt32 init_value;                        // 초기 값            
        public UInt32 data_type;                        // Value Type(1:Integer, 2:DT_FLOAT)  
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_Command
    {
        public UInt32 pid;                              // Discrete ID            
        public UInt32 datalink_fk;                      // DataLink ID          
        public UInt32 st_fk;                            // 소속 변전소 ID                
        public UInt32 rtu_fk;                           // RTU ID           
        public UInt32 ceq_fk;                           // ConductingEquipment ID
        public UInt32 dl_fk;                            // 소속 D/L ID         
        public UInt32 tm_fk;                            // Terminal ID           
        public UInt32 vas_fk;                           // 상태 유닛 Set ID      
        public UInt32 bi_fk;                            // BI Point ID 
        public UInt32 dp_fk;                            // DataPointInfo ID   
        public UInt32 alarmcatagory_fk;                 // Alarm Catagory ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                             // Point Name            
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                        // Point AliasName        
        public UInt32 position;                          // DNP Index     
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string object_variation;                 // DNP ObjectVariation
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string dnp_class;                        // DNP Use Class
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string measurement_type;                 // IED LN Code  
        public UInt32 asdu_addr;                        // ASDU Address(60870)    
        public UInt32 group_addr;                        // GROUP Address(60870)  
        public UInt32 circuit_no;                        // Circuit No
        public UInt32 use_ems;                          // Application Use              
        public UInt32 init_value;                        // 초기 값            
        public UInt32 data_type;                        // Value Type(1:Integer, 2:DT_FLOAT)  
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_Analog
    {
        public UInt32 pid;                                      // Discrete ID            
        public UInt32 datalink_fk;                              // DataLink ID          
        public UInt32 st_fk;                                    // �Ҽ� ������ ID                
        public UInt32 rtu_fk;                                   // RTU ID           
        public UInt32 ceq_fk;                                   // ConductingEquipment ID
        public UInt32 dl_fk;                                    // �Ҽ� D/L ID         
        public UInt32 tm_fk;                                    // Terminal ID        
        public UInt32 us_fk;                                    // Unit Symbol ID  
        public UInt32 dp_fk;                                    // DataPointInfo ID   
        public UInt32 ac_fk;                                    // Alarm Catagory ID              
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                                     // Point Name            
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                                // Point AliasName        
        public UInt32 position;                                 // DNP Index     
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string object_variation;                         // DNP ObjectVariation
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string dnp_class;                                // DNP Use Class
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string measurement_type;                         // IED LN Code  
        public UInt32 asdu_addr;                                // ASDU Address(60870)    
        public UInt32 group_addr;                               // GROUP Address(60870)  
        public UInt32 circuit_no;                               // Circuit No
        public UInt32 use_ems;                                  // Application Use         
        public UInt32 sf_fk;                                    // ScaleFactor ID             
        public double sf_val;                                   // ���� ���             
        public double sf_offset;                                // ���� ���  
        public UInt32 maxmin_limitset_fk;                       // Limit Set ID  
        public double max_value;                                // Limit �ִ� ��         
        public UInt32 max_alarmclass_fk;                        // Max AlarmClass ID
        public double min_value;                                // Limit �ּ� ��         
        public UInt32 min_alarmclass_fk;                        // Min  AlarmClass ID 
        public UInt32 highlow_limitset_fk;                      // Limit Set ID            
        public double high_value;                               // Limit �ִ� ��         
        public UInt32 high_alarmclass_fk;                       // Max AlarmClass ID
        public double low_value;                                // Limit �ּ� ��         
        public UInt32 low_alarmclass_fk;                        // Min  AlarmClass ID
        public double init_value;                               // �ʱ� ��            
        public UInt32 data_type;                                // Value Type(1:Integer, 2:float)            
        public UInt32 sw_group;                                 // MSW�� LSW�� grouping        
        public UInt32 sw_type;                                  // (1:MSW,2:LSW)            
        public UInt32 link_ai_fk;                               // MSW��LSW ��ȣ���� POINT ID  	

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_SetPoint
    {
        public UInt32 pid;                                      // Discrete ID            
        public UInt32 datalink_fk;                              // DataLink ID          
        public UInt32 st_fk;                                    // 소속 변전소 ID                
        public UInt32 rtu_fk;                                   // RTU ID           
        public UInt32 ceq_fk;                                   // ConductingEquipment ID
        public UInt32 dl_fk;                                    // 소속 D/L ID         
        public UInt32 tm_fk;                                    // Terminal ID        
        public UInt32 vas_fk;                        	        // 상태 유닛 Set ID      
        public UInt32 us_fk;                                    // Unit Symbol ID  
        public UInt32 ai_fk;                         	        // AI Point ID  
        public UInt32 dp_fk;                                    // DataPointInfo ID   
        public UInt32 ac_fk;                                    // Alarm Catagory ID              
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                                     // Point Name            
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                                // Point AliasName        
        public UInt32 position;                                 // DNP Index     
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string object_variation;                         // DNP ObjectVariation
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string dnp_class;                                // DNP Use Class
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string measurement_type;                         // IED LN Code  
        public UInt32 asdu_addr;                                // ASDU Address(60870)    
        public UInt32 group_addr;                               // GROUP Address(60870)  
        public UInt32 circuit_no;                               // Circuit No
        public UInt32 use_ems;                                  // Application Use         
        public UInt32 sf_fk;                                    // ScaleFactor ID             
        public double sf_val;                                   // 보정 계수              
        public double sf_offset;                                // 보정 계수    
        public double step_value;                               // 설정 증가 단계 값  
        public UInt32 maxmin_limitset_fk;                       // Limit Set ID  
        public double max_value;                                // Limit 최대 값         
        public UInt32 max_alarmclass_fk;                        // Max AlarmClass ID
        public double min_value;                                // Limit 최소 값         
        public UInt32 min_alarmclass_fk;                        // Min  AlarmClass ID 
        public double init_value;                               // �ʱ� ��            
        public UInt32 data_type;                                // Value Type(1:Integer, 2:float)            
        public UInt32 sw_group;                                 // MsW와 LSW를 grouping하는컬럼       
        public UInt32 sw_type;                                  // (1:MSW,2:LSW)            
        public UInt32 setpoint_swfk;                            //  MSW와 LSW가 상호 참조할 수 있도록 상호 FK

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_Accumulator
    {
        public UInt32 pid;                                      // Discrete ID            
        public UInt32 datalink_fk;                              // DataLink ID          
        public UInt32 st_fk;                                    // 소속 변전소 ID                
        public UInt32 rtu_fk;                                   // RTU ID           
        public UInt32 ceq_fk;                                   // ConductingEquipment ID
        public UInt32 dl_fk;                                    // 소속 D/L ID         
        public UInt32 tm_fk;                                    // Terminal ID        
        public UInt32 dp_fk;                                    // DataPointInfo ID   
        public UInt32 ac_fk;                                    // Alarm Catagory ID              
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                                     // Point Name            
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                                // Point AliasName        
        public UInt32 position;                                 // DNP Index     
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string object_variation;                         // DNP ObjectVariation
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string dnp_class;                                // DNP Use Class
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string measurement_type;                         // IED LN Code  
        public UInt32 asdu_addr;                                // ASDU Address(60870)    
        public UInt32 group_addr;                               // GROUP Address(60870)  
        public UInt32 circuit_no;                               // Circuit No
        public UInt32 use_ems;                                  // Application Use
        public UInt32 maxmin_limitset_fk;                       // Limit Set ID  
        public double max_value;                                // Limit 최대 값         
        public UInt32 max_alarmclass_fk;                        // Max AlarmClass ID
        public double min_value;                                // Limit 최소 값         
        public UInt32 min_alarmclass_fk;                        // Min  AlarmClass ID 
        public UInt32 highlow_limitset_fk;                      // Limit Set ID            
        public double high_value;                               // Limit 최대 값         
        public UInt32 high_alarmclass_fk;                       // Max AlarmClass ID
        public double low_value;                                // Limit 최소 값         
        public UInt32 low_alarmclass_fk;                        // Min  AlarmClass ID
        public UInt32 init_value;						        // 초기 값  

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_Dmc
    {
        public UInt32 pid;                      // DMC ID
        public UInt32 dmctype;                  // DMC 구분(AP, DCP, HCI/WorkStation, RTU, Channel, System Resource, DataLink
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //public string name;                     // DMC 이름(AP01, AP02, DCP01, DCP02, HCI01)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aliasname;                // Alias
        public UInt32 init_state;               // 초기값
        public UInt32 abnormal;                 // 비정상 상태
        public UInt32 indicationset;            // State 표시시 valuealiasset(1:online, 0:offline)
        public UInt32 controlset;               // control시	ValueAliasSet 사용
        public UInt32 gid;                      // AP Node, DCP Group ID
        public UInt32 alarmcatagory_fk;         // Alarm Catagory

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_ConductingEquipment
    {
        public UInt32 ceqid;                                    //  ConductionEquipment ID 
        public UInt32 eq_fk;                                    //  Equipment ID           
        public UInt32 ec_fk;                                    //  Equipment Container ID(Transformer, Composit, etc)  
        public UInt32 st_fk;                                    //  SubStation ID            
        public UInt32 dl_fk;                                    //  Distribution Line ID          
        public UInt32 link_st_fk;                               //  Link SubStation ID           
        public UInt32 link_dl_fk;                               //  Link Distribution Line ID           
        public UInt32 psrtype;                                  //  PowerSystemResource Type ID             
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] ec_name;                                  //  parent Name 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] mesh_no;                                  //	Equipment Mesh Number
        public UInt32 sw_type;                                  //  단말/상시연계 상태 표시      
        public UInt32 rtu_type;                                 //  Equipment RTU Type(1:NormalEquipment,2:MTR, 3:SVR, 4:Transformer 가공, 5:Transformer 지중, etc)    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] dev_no;                                   //  Equipment Device Number        
        public UInt32 dtr_fk;                                   //  distributionTransformer ID(Transformer 지중)        
        public UInt32 ptr_fk;                                   //  PowerTransformer ID(변전 설비-Busbarsection, Breaker, Disconnect, etc)        
        public UInt32 link_cb_fk;                               //  Link Breaker ID(Disconnect)        
        public UInt32 link_bbs_fk;                              //  Link BusBarSection ID(Disconnect)
        public double base_voltage;                             //  기준 전압
        public UInt32 phases;                                   //  상연결정보(A,B,C,N)
        public UInt32 busbar_order;                             //  Busbarsection Order(1차측,2차측)	 
        public UInt32 ai_cnt;                                   // Analog Input Point Count
        public UInt32 ai_pid;                                   // Start Analog Point ID
        public UInt32 ao_cnt;                                   // Analog Output Point Count
        public UInt32 ao_pid;                                   // Start SetPoint ID
        public UInt32 bi_cnt;                                   // Binary Input Point Count
        public UInt32 bi_pid;                                   // Start Discrete Point ID
        public UInt32 bo_cnt;                                   // Binary Output Point Count
        public UInt32 bo_pid;                                   // Start Command Point ID
        public UInt32 pi_cnt;                                   // Counter Point Count
        public UInt32 pi_pid;								    // Start Accumulator Point ID

        public int GetSize()
        {
            return Marshal.SizeOf(this);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_DistributionLineSegment
    {
        public UInt32 pid;                                          /*DistributionLineSegment ID		*/
        public UInt32 ceq_fk;                                       /*CondutingEquipment ID				*/
        public UInt32 dl_fk;                                        /*소속된 DistributionLine ID			*/
        public UInt32 plsiid_a_fk;                                  /*PerLengthSequenceImpedance ID		*/
        public UInt32 plsiid_b_fk;                                  /*PerLengthSequenceImpedance ID		*/
        public UInt32 plsiid_c_fk;                                  /*PerLengthSequenceImpedance ID		*/
        public UInt32 plsiid_n_fk;                                  /*PerLengthSequenceImpedance ID		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;                                         /*DistributionLineSegment Name		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] aliasname;                                    /*DistributionLineSegment AliasName	*/
        public double length;                                       /*Line 길이							*/
        public UInt32 length_us_fk;                                 // UnitSymbol                       
        public UInt32 ceq_f_fk;                                     /*전단 CondutingEquipment ID			*/
        public UInt32 sgr_f_fk;                                     /*전단 SGRegion ID					*/
        public UInt32 ceq_b_fk;                                     /*후단 CondutingEquipment ID			*/
        public UInt32 sgr_b_fk;                                     /*후단 SGRegion ID					*/
        public double secload;                                                // Section 부하 
        public UInt32 secload_us_fk;                                             //UnitSymbol
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_GeographicalRegion
    {
        public UInt32 ggrid;                                        /*지사(본부)ID			*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_SubGeographicalRegion
    {
        public UInt32 sgrid;                                        /*지점 ID				*/
        public UInt32 ggr_fk;                                       /*소속 지사(본부) ID		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;                                         /*지점 명					*/
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_SubStation
    {
        public UInt32 stid;                                         /*변전소 ID				*/
        public UInt32 sgr_fk;                                       /*소속 지점 ID			*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;							                /*변전소 명				*/
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_DistributionLine
    {
        public UInt32 dlid;                                         /*DistributionLine ID	*/
        public UInt32 st_fk;                                        /*변전소 ID				*/
        public UInt32 ptr_fk;                                       /*MTR ID				*/
        public UInt32 sw_fk;                                        /*OCB ID				*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;                                         /*DistributionLine 명	*/
        public UInt32 dlno;                                         /*회선번호				*/
        public UInt32 reliability;                                  /*회선 가용량			*/
        public UInt32 priority;                                     /*회선우선순위			*/
        public UInt32 rated_s;                                      /*회선기준용량			*/
        public UInt32 rated_s_usfk;									// UnitSymbol FK
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pdb_PowerTransformer
    {
        public UInt32 ptrid;                                        /*PowerTransformer  ID		*/
        public UInt32 st_fk;                                        /*소속변전소 ID */
        public UInt32 tap_fk;                                       /*TabChanger ID */
        public UInt32 trw_1st_fk;                                   /*TransformerWinding ID		*/
        public UInt32 trw_2nd_fk;                                   /*TransformerWinding ID		*/
        public UInt32 bbs_1st_fk;                                   /*BusbarSection 1차측 ID		*/
        public UInt32 bbs_2nd_fk;                                   /*BusbarSection 2차측 ID		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] name;                                         /*PowerTransformer  Name	*/
        public UInt32 bank_no;                                      /*Bank Number				*/
        public UInt32 mtr_imp;                                      /*MTR%임피던스				*/
        public UInt32 mtr_imp_us_fk;                                //   UnitSymbol
        public UInt32 disp_pos;                                     /*Display Position(Left->Right Order	*/
        public UInt32 st_type;										/*Station Type(GIS,MCSG,GIS+<CSG,ETC	*/
    }



}
