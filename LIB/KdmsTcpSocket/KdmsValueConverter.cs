using System.Runtime.InteropServices;

namespace KdmsTcpSocket;

public class KdmsValueConverter
{
    public static T ByteToStruct<T>(byte[] buffer) where T : struct
    {
        int num = Marshal.SizeOf(typeof(T));
        if (num > buffer.Length)
        {
            throw new Exception(string.Format($"SIZE ERR(len:{buffer.Length} sz:{num})"));
        }

        IntPtr intPtr = Marshal.AllocHGlobal(num);
        Marshal.Copy(buffer, 0, intPtr, num);
        T result = (T)Marshal.PtrToStructure(intPtr, typeof(T));
        Marshal.FreeHGlobal(intPtr);
        return result;
    }

    public static byte[] StructToByte<T>(T obj) where T : struct
    {
        byte[] array = null;
        int num = Marshal.SizeOf(typeof(T));
        array = new byte[num];
        IntPtr intPtr = Marshal.AllocHGlobal(num);
        Marshal.StructureToPtr(obj, intPtr, fDeleteOld: true);
        Marshal.Copy(intPtr, array, 0, num);
        return array;
    }

    public static byte[] StructArrayToByte<T>(T[] obj) where T : struct
    {
        byte[] array = null;
        int num = Marshal.SizeOf(typeof(T));
        array = new byte[num * obj.Length];
        IntPtr intPtr = Marshal.AllocHGlobal(num);
        for (int i = 0; i < obj.Length; i++)
        {
            Marshal.StructureToPtr(obj[i], intPtr, fDeleteOld: true);
            Marshal.Copy(intPtr, array, i * num, num);
        }

        Marshal.FreeHGlobal(intPtr);
        return array;
    }

    public static T[] ByteToStructArray<T>(byte[] buffer) where T : struct
    {
        int num = buffer.Length / Marshal.SizeOf(typeof(T));
        if (num > 0)
        {
            T[] array = new T[num];
            int num2 = Marshal.SizeOf(typeof(T));
            if (num2 * num > buffer.Length)
            {
                throw new Exception($"SIZE ERR(buf_len:{buffer.Length} cnt:{num} size={num2} total_rcv_size:{num2 * num})");
            }

            for (int i = 0; i < num; i++)
            {
                IntPtr intPtr = Marshal.AllocHGlobal(num2);
                Marshal.Copy(buffer, i * num2, intPtr, num2);
                T val = (T)Marshal.PtrToStructure(intPtr, typeof(T));
                array[i] = val;
                Marshal.FreeHGlobal(intPtr);
            }

            return array;
        }

        return null;
    }

    public static DateTime TimeTToDateTime(uint time_t)
    {
        return DateTime.FromFileTime(10000000L * (long)time_t + 116444736000000000L);
    }

    public static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
        return Math.Floor((date.ToUniversalTime() - dateTime).TotalSeconds);
    }
}
