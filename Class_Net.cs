
using System.Net.NetworkInformation;

/// <summary>
/// 获取本机的IPv4地址
/// </summary>
/// <returns>IPV4地址</returns>
  private string GetLocalIpv4()
        {
            try
            {
                // 获得网络接口，网卡，拨号器，适配器都会有一个网络接口。 
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface network in networkInterfaces)
                {
                    // 获得当前网络接口属性。
                    IPInterfaceProperties properties = network.GetIPProperties();
                    // 每个网络接口可能会有多个IP地址 。
                    foreach (IPAddressInformation address in properties.UnicastAddresses)
                    {
                        // 如果此IP不是ipv4，则进行下一次循环。
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        { continue; }
                        // 忽略127.0.0.1。
                        if (IPAddress.IsLoopback(address.Address))
                        { continue; }
                        if (address.Address.ToString().Substring(0, 3) == "169")
                        {
                            continue;
                        }
                        return address.Address.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return null;
        }
