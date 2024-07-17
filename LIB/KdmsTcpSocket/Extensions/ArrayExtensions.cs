namespace KdmsTcpSocket.Extensions;

public static class ArrayExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] arr, int size)
    {
        return arr.Select((s, i) => arr.Skip(i * size).Take(size)).Where(a => a.Any());
    }
}
