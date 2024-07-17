using KdmsTcpSocket.IO;
using System;

namespace KdmsTcpSocket.Interfaces
{
    public interface ITcpSocketTransport : IDisposable
    {
        //uint RetryOnOldResponseThreshold { get; set; }

        //bool SlaveBusyUsesRetryCount { get; set; }

        int WaitToRetryMilliseconds { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        ITcpSocketMessage ResponseData(ITcpSocketMessage message);
        ITcpSocketMessage ResponseData();

        void Write(ITcpSocketMessage message);

        byte[] Read(out bool isCompress);

        IStreamResource StreamResource { get; }
    }
}