param([int] $inputNum)
 
$cim = Get-CimInstance Win32_NetworkAdapterConfiguration -Filter "IPEnabled = True"

function CheckReturn {
    param ($obj)
    if ($obj.ReturnValue -eq 0) {
        "Successed"
    }
    else {
        "Failed"
    }
}

if ($inputNum -eq 0) {
    CheckReturn(Invoke-CimMethod $cim -MethodName EnableDHCP)
    CheckReturn(Invoke-CimMethod $cim -MethodName SetDNSServerSearchOrder)
}
elseif (($inputNum -gt 0) -and ($inputNum -lt 254)) {
    CheckReturn(Invoke-CimMethod $cim -MethodName EnableStatic -Arguments @{
            IPAddress  = @("10.200.200." + $inputNum.ToString());
            SubnetMask = @("255.255.255.0")
        })
    CheckReturn(Invoke-CimMethod $cim -MethodName SetGateways -Arguments @{
            DefaultIPGateway = @("10.200.200.254")
        })
    CheckReturn(Invoke-CimMethod $cim -MethodName SetDNSServerSearchOrder -Arguments @{
            DNSServerSearchOrder = @("114.114.114.114")
        })
}
else {
    "Input number between 1 to 253 to set network adapter, or 0 to disable network adapter"
}