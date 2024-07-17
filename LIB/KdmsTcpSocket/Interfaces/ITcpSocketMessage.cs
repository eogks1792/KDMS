using KdmsTcpSocket.KdmsTcpStruct;
using System.Diagnostics.CodeAnalysis;

namespace KdmsTcpSocket.Interfaces;

/// <summary>
///     A message built by the master (client) that initiates a Modbus transaction.
/// </summary>
public interface ITcpSocketMessage
{
    DateTime SendTime { get; set; }
    UInt16 RequestCode { get; set; }

    UInt16 ResponseCode { get; set; }

    uint DataCount { get; set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
    byte[] DataHeader { get; }

    /// <summary>
    ///     Composition of the function code and message data.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
    byte[] SendDatas { get; }

    byte[]? RecvDatas { get; set; }

    //bool IsCompress { get; set; }
}
