using System;
using System.Threading;

namespace CloudEntity.Test.Utils;

/// <summary>
/// Guid生成器
/// </summary>
public class GuidHelper
{
    /// <summary>
    /// 时间戳
    /// </summary>
    private static long _counter;

    /// <summary>
    /// 初始化
    /// </summary>
    static GuidHelper()
    {
        _counter = DateTime.UtcNow.Ticks;
    }
    /// <summary>
    /// 获取有序的Guid
    /// </summary>
    /// <returns>有序的Guid</returns>
    public static Guid NewOrdered()
    {
        //获取计算好的字节
        byte[] counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref _counter));
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);
        //生成排好序的Guid
        byte[] guidBytes = Guid.NewGuid().ToByteArray();
        guidBytes[08] = counterBytes[1];
        guidBytes[09] = counterBytes[0];
        guidBytes[10] = counterBytes[7];
        guidBytes[11] = counterBytes[6];
        guidBytes[12] = counterBytes[5];
        guidBytes[13] = counterBytes[4];
        guidBytes[14] = counterBytes[3];
        guidBytes[15] = counterBytes[2];
        return new Guid(guidBytes);
    }
}
