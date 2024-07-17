namespace KdmsTcpSocket;

public enum eNodeCode
{
    nc_inner_comm = 0x00,  // NC_INNER_COMM
    nc_kdms_comm,          // NC_KDMS_COMM
    nc_fep_comm,            // NC_FEP_COMM
    nc_hmi_comm,            // NC_HMI_COMM
    nc_offdb_comm,          // NC_OFFDB_COMM
    nc_kbuilder_comm        // NC_KBUILDER_COMM
}

public enum eCompress
{
    uncompress,             // FC_UNCOMPRESS
    compress,               // FC_COMPRESS
}

public enum eActionCode
{
    no_req = 0x01,          // AC_NO_REQ
    rt_req,                 // AC_RT_REQ
    ack_packet,             // AC_ACK_PACKET
    health_check,           // AC_HEAL_CHK
    no_ack_packet,          // AC_NACK_PACKET
}


public enum eErrorCode
{
    error_none,
    error_data_not_exist,
    error_ack,
    error_invalid_sequence,
    error_interface_end
}