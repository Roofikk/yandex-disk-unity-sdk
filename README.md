
# Yandex Disk SDK

Yandex Disk SDK is an open source plugin whose goal is to provide the developer with a simple interaction with the Yandex.Disk API to integrate it into the game in Unity. However, not all API requests have been implemented yet, but they are still at the development stage.

## Overview

The plugin provides support for the following features of the Yandex Disk API:

- authorization
- get disk and person information
- get files of folder
- get information about folder
- download/upload files to folder

Requirements:
- Unity 2021.3 (maybe the plugin works in earlier versions like 2019 or 2020, but I haven't checked)
- Newtonsoft.Json

**WARNING!!**

This plugin has been tested only on Windows build. I can't be sure that it will work on Android, MAC, iOS, etc. It definitely won't work on WebGL. In the future, all this will be corrected and supplemented. I will add it to WebGL soon.

## Install Yandex Disk SDK

Open **Package Manager** (Window --> Package Manager) and choose **Add package from git URL...** Paste this git repository and this is the finish.

## Configure Your Application

To use the plugin, you must first [create and configure application](https://yandex.ru/dev/oauth/) in the [Yandex OAuth](https://oauth.yandex.ru/client/new/id/). After creating yandex application, you see Client ID in your application.

![Client ID in Yandex application](Screenshots~/chrome_fExswEoUlX.png)

Create empty game object in scene and add component [YandexDiskClient](https://github.com/Roofikk/yandex-disk-sdk/blob/master/Runtime/YandexDisk/YandexDiskClient.cs). Paste Client ID from yandex application in component's field.

![Screenshot of component](Screenshots~/Unity_QZDvLnxFPp.png)

