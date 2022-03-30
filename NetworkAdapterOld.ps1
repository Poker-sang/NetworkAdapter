param([int] $inputNum)

$wmi = Get-WmiObject Win32_NetworkAdapterConfiguration -Filter "IPEnabled = True"

function CheckReturn {
    param ($obj)
    if ($obj.ReturnValue -eq 0) {
        "Successed"
    }
    else {
        "Failed"
    }
}

if ($inputNum -eq 0)
{
    CheckReturn($wmi.EnableDHCP())
    CheckReturn($wmi.SetDNSServerSearchOrder())
}
elseif (($inputNum -gt 0) -and ($inputNum -lt 254))
{
    CheckReturn($wmi.EnableStatic("10.200.200." + $inputNum.ToString(), "255.255.255.0"))
    CheckReturn($wmi.SetGateways("10.200.200.254"))
    CheckReturn($wmi.SetDNSServerSearchOrder("114.114.114.114"))
}
else 
{
    "Input number between 1 to 253 to set network adapter, or 0 to disable network adapter"
}