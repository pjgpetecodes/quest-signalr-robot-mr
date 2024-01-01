# Raspberry Pi Remote Controlled Meta Quest VR Robot Arm with SignalR

## Quest 3 Unity 2022 version with Mixed Reality!

This version of the project is built from the ground up using Unity 2022.3.16f1 and version 8 of the SignalrR Nuget Packages.

## SignalR Not Working on Quest

I found that I could run the project using the play button and Quest Link perfectly fine and have SignalR working - As the whole process is running locally on the machine.

However, when I deployed to the headset SignalR wouldn't connect.

Doing some debugging, I saw `InvalidOperationException: Sequence contains no matching element` in the log file;

```log
01-01 01:00:47.108 11878 11902 I Unity   : SignalRController:StartSignalR()
01-01 01:00:47.108 11878 11902 I Unity   : SignalRController:Start() (at C:\repos\quest-signalr-robot-mr\Assets\Scripts\SignalRController.cs:46)
01-01 01:00:47.108 11878 11902 I Unity   :
01-01 01:00:47.114 11878 11902 E Unity   : InvalidOperationException: Sequence contains no matching element
01-01 01:00:47.114 11878 11902 E Unity   :   at System.Linq.Enumerable.Single[TSource] (System.Collections.Generic.IEnumerable`1[T] source, System.Func`2[T,TResult] predicate) [0x00000] in <00000000000000000000000000000000>:0
01-01 01:00:47.114 11878 11902 E Unity   :   at Microsoft.AspNetCore.SignalR.Client.HubConnection..cctor () [0x00000] in <00000000000000000000000000000000>:0
01-01 01:00:47.114 11878 11902 E Unity   :   at System.Reflection.RuntimeConstructorInfo.InternalInvoke (System.Object obj, System.Object[] parameters, System.Boolean wrapExceptions) [0x00000] in <00000000000000000000000000000000>:0
01-01 01:00:47.114 11878 11902 E Unity   :   at System.Reflection.RuntimeConstructorInfo.DoInvoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x00000] in <00000000000000000000000000000000>:0
01-01 01:00:47.114 11878 11902 E Unity   :   at System.Reflection.RuntimeConstructorInfo.Invoke (System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo cult
01-01 01:00:47.116 11878 11902 I Unity   : OVRControllerHelp: Active controller type: TouchPlus for product Oculus Headset2 (headset Meta_Quest_3, hand HandRight)
01-01 01:00:47.116 11878 11902 I Unity   : UnityEngine.DebugLogHandler:LogFormat(LogType, Object, String, Object[])
01-01 01:00:47.116 11878 11902 I Unity   : UnityEngine.Logger:LogFormat(LogType, String, Object[])
01-01 01:00:47.116 11878 11902 I Unity   : UnityEngine.Debug:LogFormat(String, Object[])
01-01 01:00:47.116 11878 11902 I Unity   : OVRControllerHelper:InitializeControllerModels() (at .\Library\PackageCache\com.meta.xr.sdk.core@60.0.0\Scripts\Util\OVRControllerHelper.cs:200)
01-01 01:00:47.116 11878 11902 I Unity   : OVRControllerHelper:Start() (at .\Library\PackageCache\com.meta.xr.sdk.core@60.0.0\Scripts\Util\OVRControllerHelper.cs:126)
```

Which relates to the fact that SignalR can';t find some dependant dll files.

This is due to Unity stripping dlls it thinks aren't needed.

## Steps performed to get SignalR working

- Install SignalR;

    - Create a `tmp` directory at `Assets`
    - Create a `dll` directory at `Assets`
    - Run `.\install-signalr.ps1` from `Assets`.

- Update the AndroidManifest.xml file from within Unity at `Oculus->Tools->Update AndroidManifest.xml`

- Add the following to the AndroidManifest.xml file at `Assets\Plugins\Android`;

```
<uses-permission android:name="android.permission.INTERNET" />
```

- Create a `link.xml` file in Assets, Plugins and Assets\dll with the following content;

```
<linker>
    <assembly fullname="System.Core">
        <type fullname="System.Linq.Expressions.Interpreter.LightLambda" preserve="all" />
    </assembly>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Client.Core" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Client" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Common" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Protocols.Json" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Protocols.MessagePack" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR.StackExchangeRedis" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.SignalR" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.Sockets.Client" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.Sockets.Common" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.Sockets" preserve="all"/>
    <assembly fullname="Microsoft.AspNetCore.WebSockets" preserve="all"/>
    
</linker>
```

- Perform a Clean Build at `File->Build Settings` and Drop Down the "Build" button at the bottom right and select `Clean Build` (This can take 15 minutes or more)

- Make `Internet` required instead of `Auto` at `Edit->Project Settings->Player->Other Settings->Configuration->Internet Access`

- Set `Allow Downloads over HTTP` to `Always Allowed` at `Edit->Project Settings->Player->Other Settings->Configuration->Allow Downloads over HTTP`

- The SignalR Hub address is set against the SignalR Script attached to the UR3 robot in the scene. If this doesn't work, then try replacing the `signalRHubURL` variable in the `SignalRController.cs` file in the `StartSignalR` function. 

## For Debugging

- Create a Debug Build of the APK and deploy it at `File->Build Settings`. Set `Development Build` and `Script Debugging`.

- Run the following command from `C:\Program Files\Unity\Hub\Editor\2022.3.16f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools`;

```
adb logcat -s Unity ActivityManager PackageManager dalvikvm DEBUG > c:\temp\out.log
```

## Thanks.

Thanks to `Evan Lindsey` with the repo here for installing SignalR;

`https://github.com/evanlindsey/Unity-WebGL-SignalR`