cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
copy "C:\workspace\BoldDesk\helpdesk-omnichannel-api\build\RSAKeys.xml" "C:\Windows\Microsoft.NET\Framework\v4.0.30319\"
aspnet_regiis.exe -pi "RSAKeys" "RSAKeys.xml" -exp
aspnet_regiis.exe -pa "RSAKeys" "syncfusion" -full