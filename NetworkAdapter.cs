#nullable enable
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace NetworkAdapter;

[SupportedOSPlatform(nameof(OSPlatform.Windows))]
public static class NetworkAdapter
{
    public static void Main()
    {
        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out var temp))
                continue;
            switch (temp)
            {
                case 0:
                    DisableNetworkAdapter();
                    return;
                case > 0 and < 254:
                    SetNetworkAdapter("10.200.200." + temp, "255.255.255.0", "10.200.200.254", "114.114.114.114");
                    return;
                default:
                    Console.WriteLine("输入错误：请输入机房电脑序号（1-253），输入0恢复默认网络设置");
                    break;
            }
        }
    }
    private static void SetNetworkAdapter(string ipAddress, string subnetMask, string gateway, string dns)
    {
        if (GetManagementObject() is { } mo)
        {
            var inPar = mo.GetMethodParameters("EnableStatic");
            //设置ip地址和子网掩码
            inPar["IPAddress"] = new[] { ipAddress };
            inPar["SubnetMask"] = new[] { subnetMask };
            _ = mo.InvokeMethod("EnableStatic", inPar, null!);

            //设置网关地址
            inPar = mo.GetMethodParameters("SetGateways");
            inPar["DefaultIPGateway"] = new[] { gateway };
            _ = mo.InvokeMethod("SetGateways", inPar, null!);

            //设置DNS
            inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
            inPar["DNSServerSearchOrder"] = new[] { dns };
            _ = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null!);
        }
    }

    private static void DisableNetworkAdapter()
    {
        if (GetManagementObject() is { } mo)
        {
            _ = mo.InvokeMethod("SetDNSServerSearchOrder", Array.Empty<object>());
            _ = mo.InvokeMethod("EnableDHCP", Array.Empty<object>());
        }
    }

    private static ManagementObject? GetManagementObject()
    {
        var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        var moc = mc.GetInstances();
        foreach (var o in moc)
            if ((bool)o["IPEnabled"])
                return (ManagementObject)o;

        return null;
    }
}
