
<img src="https://user-images.githubusercontent.com/7417301/170061921-fee5feb8-b348-40a8-9cd4-8db46d1aceec.png" />

# Wg Server for Windows
# 适用于 Windows 的 Wg 服务器
WS4W is a desktop application that allows running and managing a WireGuard server endpoint on Windows.
WS4W 是一个桌面应用程序，允许在窗口上运行和管理WireGuard server端点。
Inspired by Henry Chang's post, [How to Setup Wireguard VPN Server On Windows](https://www.henrychang.ca/how-to-setup-wireguard-vpn-server-on-windows/), my goal was to create an application that automated and simplified many of the complex steps. While still not quite a plug-and-play solution, the idea is to be able to perform each of the prerequisite steps, one-by-one, without running any scripts, modifying the Registry, or entering the Control Panel.
受到Henry Chang的帖子[如何在Windows上设置Wireguard VPN服务器”的启发](https://www.henrychang.ca/how-to-setup-wireguard-vpn-server-on-windows/)，我的目标是创建一个自动化和简化许多复杂步骤的应用程序。虽然仍然不是一个即插即用的解决方案，但这个想法是能够逐个执行每个先决条件步骤，而无需运行任何脚本，修改注册表或进入控制面板。

# Getting Started 开始
The latest release is available [here](https://github.com/micahmo/WgServerforWindows/releases/latest). Download the installer and run.
最新版本可在此处获取。[下载](https://github.com/micahmo/WgServerforWindows/releases/latest)，安装程序并运行
> **Note**: The application will request to run as Administrator. Due to all the finagling of the registry, Windows services, wg.exe calls, etc., it is easier to run the whole application elevated.
> **注意**：应用程序将请求以管理员身份运行。由于注册表，Windows服务，wg.exe调用等的所有混乱，因此更容易运行提升的整个应用程序。

#### Upgrade from 1.5.2
#### 从 1.5.2 升级
Before introducing an installer, WS4W was distributed as a portable application. The portable versions (1.5.2 and earlier) have no automatic upgrade path to the installer version. To upgrade, simply delete the downloaded portable version and download the installer. No configuration settings will be lost.
在引入安装程序之前，WS4W 是作为可移植应用程序分发的。可移植版本（1.5.2 及更早版本）没有安装程序版本的自动升级路径。要升级，只需删除下载的便携式版本并下载安装程序即可。不会丢失任何配置设置。

# What Does It Do?
# 它有什么作用？
Below are the tasks that can be performed automatically using this application.
以下是可以使用此应用程序自动执行的任务。

## Before
## 以前
![BeforeScreenshot](https://user-images.githubusercontent.com/7417301/170070964-b43b88c8-fa87-4123-a7dc-2345046a7baa.png)

### WireGuard.exe
This step downloads and runs the latest version of WireGuard for Windows from https://download.wireguard.com/windows-client/wireguard-installer.exe. Once installed, it can be uninstalled directly from WS4W, too.
此步骤从 https://download.wireguard.com/windows-client/wireguard-installer.exe 下载并运行最新版本的用于 Windows 的 WireGuard。安装后，也可以直接从WS4W卸载它。

### Server Configuration
### 服务器配置
![ServerConfiguration](https://user-images.githubusercontent.com/7417301/170072344-598a8b9c-bec8-4f34-9a85-ee95765520e3.png)

Here you can configure the server endpoint. See the WireGuard documentation for the meaning of each of these fields. The Private Key and Public Key are generated by calling `wg genkey` and `wg pubkey [private key]` respectively. (You can optionally supply your own Private Key.)
您可以在此处配置服务器终端节点。请参阅电线防护文档，了解每个字段的含义。私钥和公钥分别通过调用和生成。（您可以选择提供自己的私钥。wg genkeywg pubkey [private key]

> **Note**: It is important that the server's network range not conflict with the host system's IP address or LAN network range.
> **注意**：服务器的网络范围不得与主机系统的 IP 地址或 LAN 网络范围冲突，这一点很重要。

In addition to creating/udpating the configuration file for the server endpoint, editing the server configuration will also update the `ScopeAddress` registry value (under `HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters`). This is the IP address that is used for the WireGuard adapter when using the Internet Sharing feature (explained [here](#internet-sharing)). Thus, the Address property of the server configuration serves to determine the allowable addresses for clients, as well as the IP that Windows will assign to the WireGuard adapter when performing Internet Sharing. Note the IP address is grabbed from the `ScopeAddress` at the time when Internet Sharing is first performed. That means that if the server's IP address is changed in the configuration (and thus the `ScopeAddress` registry value is updated), the WireGuard interface will no longer accurately reflect the desired server IP. Therefore, WS4W will prompt to re-share internet. If canceled, Internet Sharing will be disabled and will have to be re-enabled manually.

> **Important**: You must configure port forwarding on your router. Forward all UDP traffic that is destined for your server endpoint port (default `51820`) to the LAN IP of your server. Every router is different, so it is difficult to give specific guidance here. As an example, here is what the port forwarding rule would look like on a Verizon Quantum Gateway router.
> 
> ![](https://user-images.githubusercontent.com/7417301/127727564-0d666c41-4998-4c5d-8d2a-e7b730e545c8.png)

You should set the Endpoint property to your public IPv4, IPv6, or domain address, followed by whatever port you have forwarded. The `Detect Public IP Address` button will attempt to detect your public address automatically using the [ipify.org](https://ipify.org) API. However, if possible, it is recommended that you use a domain name with DDNS. That way, if your public IP address changes, your clients will be able to find your server endpoint without reconfiguration.

### Client Configuration
![ClientConfiguration](https://user-images.githubusercontent.com/7417301/173172783-5a16da01-f725-4a47-b738-1e52f79f49cd.png)

Here you can configure the client(s). The Address can be entered manually or calculated based on the server's network range. For example, if the server's network is `10.253.0.0/24`, the client config can determine that `10.253.0.2` is a valid address. Note that the first address in the range (in this example, `10.253.0.1`) is reserved for the server. DNS is optional, but recommended. You may add DNS Search Domains (also known as DNS Suffixes, [read more](https://en.wikipedia.org/wiki/Search_domain)). Lastly, the Private Key, Public Key, and Preshared Key are generated using `wg genkey`, `wg pubkey [private key]`, and `wg genpsk`. (You may specify your own Private Key. Preshared Keys are optional, generated uniquely per-client, and shared with the server's configuration. See [#34](https://github.com/micahmo/WgServerforWindows/issues/34) for more info.)

> Due to a bit of a quirk in WireGuard, if you were to remove a client Preshared Key and sync the server configuration, WireGuard would still expect the client to connect with a PSK. Therefore, WS4W does not allow you to clear the Preshared Key field from clients. Instead, delete and recreate a client to remove the PSK.

Once configured, it's easy to import the configuration into your client app of choice via QR code or by exporting the `.conf` file.

![ClientQrCode](https://user-images.githubusercontent.com/7417301/170073360-628712b3-90e2-4ea5-a759-2dd6c9d5dc4a.png)

For security, you may not want to keep the clients' private keys on the server. In that case, you may clear the private key field before saving a client configuration. However, there are two things to keep in mind.
1. You should export the client config (via QR code or file) before removing the private key and saving.
2. If you ever need to import the config to your client again, you will have to re-generate both the private and public keys.

### Tunnnel Service
Once the server and client(s) are configured, you may install the tunnel service, which creates a new network interface for WireGuard using the `wireguard /installtunnelservice` command. After installation, the tunnel may be also removed directly within WS4W. This uses the `wireguard /uninstalltunnelservice` command.

After completing this step, WireGuard clients should be able to get as far as performing a successful handshake with the server.

> **Note:** If the server configuration is edited after the tunnel service is installed, the tunnel service will automatically be updated via the `wg syncconf` command (if the newly saved server configuration is valid). This is also true of the client configurations, updates to which often cause the server configuration to be updated (e.g., if a new client is added, the server configuration must be aware of this new peer).

### Private Network
Even after the tunnel service is installed, some protocols may be blocked. It is recommended to change the network profile to `Private`, which eases Windows restrictions on the network.

This step also creates a Windows Task to make the network Private automatically on boot. You may disable the Task via the dropdown. 

> **Note**: On a system where the shared internet connection originates from a domain network, this step is not necessary, as the WireGuard interfaces picks up the profile of the shared domain network.


### Routing

The last step is to allow requests made over the WireGuard interface to be routed to your private network or the Internet. To do so, the connection of the "real" network adapter on the Windows machine must be shared with the virtual WireGuard adapter. This can be done in one of two ways.
* NAT Routing
* Internet Sharing + Persistent Internet Sharing

The first option is only available on some systems (see more below). The second options may be used as necessary, but have some caveats (such as, if the Internet Connection is shared with the WireGuard adapter, it cannot be shared with any other adapter; see [#18](https://github.com/micahmo/WgServerforWindows/issues/18)). There have also been multiple issues reported with Internet Sharing, so NAT Routing should be used if available.

These options are mutually exclusive.

#### NAT Routing

Here you can create a NAT routing rule on the WireGuard interface to allow it to interact with your private/public network. Specifically, the following commands are invoked.

* `New-NetIPAddress` is called on the WireGuard adapter to assign a static IP in the range of the Server Configuration's Address property.
* `New-NetNat` is called to create a new NAT rule on the WireGuard adapter.
* A Windows Task is created to call `New-NetIPAddress` on boot.
  * If you do not wish to have the Windows Task automatically configure the WireGuard interface on boot, you can press the dropdown and choose "Disable Automatic NAT Routing".

> NAT Routing requires at least Windows 10, and the option to enable it will not even appear in the application on older versions of Windows. However, even with Windows 10, NAT Routing does not always work. Sometimes it requires Hyper-V to be enabled, which the application will prompt for, but that also requires a Pro or higher (i.e., not Home) version of Windows. Ultimately, if the application is unable to enable NAT Routing, it will recommend using Internet Connection Sharing instead (below). See [#30](https://github.com/micahmo/WgServerforWindows/issues/30) for a full discussion about NAT Routing support.

#### Internet Sharing
![InternetSharing](https://user-images.githubusercontent.com/7417301/170073850-fde3a685-79d5-4ea9-a2b6-acb9b08c58d0.png)

If NAT Routing is not available, you can use internet sharing to provide network connection to the WireGuard interface. When configuring this option, you may select any of your network adapters to share. Note that it will likely only work for adapters whose status is `Connected`, and it will only be useful for adapters which provide internet or LAN access. When choosing the adapter to share, hover over the menu item to get more details, including the adapter's assigned IP address, to determine if it's the one you want to share.

> **Note:** When performing internet sharing, the WireGuard adapter is assigned an IP from the `ScopeAddress` registry value (under `HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters`). This value is automatically set when updating the Address property of the server configuration. See more [here](#server-configuration).

#### Persistent Internet Sharing
There are issues in Windows that cause Internet Sharing to become disabled after a reboot. If the WireGuard server is intended to be left unattended, it is recommended to enable Persistent Internet Sharing so that no interaction is required after rebooting.

When enabling this feature, two actions are performed in Windows:
1. The `Internet Connection Sharing` service startup mode is changed from `Manual` to `Automatic`.
2. The value of the `EnableRebootPersistConnection` regstry value in `HKLM\Software\Microsoft\Windows\CurrentVersion\SharedAccess` is set to `1` (it is created if not found).

Even with these workarounds, Internet Sharing can become disabled after a reboot. Therefore, one more action is performed. A Scheduled Task is created that disables and re-enables Internet Sharing using the WS4W CLI upon system boot. This should be sufficient to guarantee that sharing remains enabled.

### View Server Status
![ServerStatus](https://user-images.githubusercontent.com/7417301/170075139-0647c35d-ac30-4296-93c1-985f1310c051.png)

Once the tunnel is installed, the status of the WireGuard interface may be viewed. This is accomplished via the `wg show` command. It will be continually updated as long as `Update Live` is checked.

## After

![AfterScreenshot](https://user-images.githubusercontent.com/7417301/170075433-e3e27369-30de-4404-a0ca-19d3a57d52f8.png)

## CLI
There is also a CLI bundled in the portable download called `ws4w.exe` which can be invoked from a terminal or called from a script. In addition to messages written to standard out, the CLI will also set the exit code based on the success of executing the given command. In PowerShell, for example, the exit code can be printed with `echo $lastexitcode`.

> **Note**: The CLI must also be run as an Administrator for the same reasons as above.

### Usage
The CLI uses verbs, or top-level commands, each of which has its own set of options. You can run `ws4w.exe --help` for a list of all verbs or `ws4w.exe verb --help` to see the list of options for a particular verb.

#### List of Supported Verbs
* ```ws4w.exe restartinternetsharing [--network <NETWORK_TO_SHARE>]```
	* This will tell WS4W to attempt to restart the Internet Sharing feature.
	* The `--network` option may be passed to specify which network WS4W should share.
	* If Internet Sharing is already enabled, WS4W will attempt to reshare the same network (unless `--network` is passed).
	* If multiple networks are already shared, it is not possible to tell which one is shared with the WireGuard network, so the `--network` option must be passed to specify.
	* If Internet Sharing is not already enabled, the `--network` option must be passed, otherwise there is no way to know which network to share.
	* The exit code will be 0 if the requested or previously shared network was successfully reshared.
      > This command is used by the Scheduled Task that is created when Persistent Internet Sharing is enabled.
* ```ws4w.exe setpath```
    * This will tell WS4W to add the current executing directory to the system's `PATH` environment variable.
    * This verb has no options.
      > This command is used by the installer when the "Add CLI to PATH" option is selected.
* ```ws4w.exe setnetipaddress --serverdatapath <PATH_TO_SERVER_CONFIG>```
    * This will tell WS4W to call `Set-NetIPAddress` on the WireGuard interface, using the network Address as defined in the given WireGuard server configuration file.
      > This command is used by the Scheduled Task that is created when NAT Routing is enabled.
* ```ws4w.exe privatenetwork```
    * This will set the category of the WireGuard network interface to Private.
    * This verb has no options.
      > This command is used by the Windows Task that is created when Private Network is enabled.

# Known Issues

### Inability to Enable Internet Sharing

First, it is recommended to use NAT Routing if available.

However, if you experience the following error message when enabling Internet Sharing, please perform the following manual steps.

![image](https://user-images.githubusercontent.com/7417301/170076429-d08685ef-3eae-4433-978f-1adc722763c0.png)

 - Open Network Connections in the Control Panel.
 - Right-click > Properties on the network interface that you want to share.
    - Go to the Sharing tab and check "Allow other network users to connect through this computer's Internet connection".
	- From the "Home networking connection" dropdown, choose `wg_server`.
	- Press OK.
 - Close and reopen WS4W. It should now show Internet Sharing enabled, and subsequent attempts to disable/re-enable should be sucessful going forward.

> Note: This issue is often triggered after creating a new virtual switch for a VM. The manual workaround should only be needed once after that and does not affect the virtual switch.

# Attribution

WireGuard is a registered trademark of Jason A. Donenfeld.

[Icon](https://www.flaticon.com/free-icon/sign_28310) made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/).
