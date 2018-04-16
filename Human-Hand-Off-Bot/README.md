# Human Hand Off Bot

## 修改 web.config

請修改 `web.config` 中的 appsettings 段落，加入註冊 Azure Bot Service 時，所設定的機器人名稱及 Microsoft 應用程式識別碼相關資訊。

```xml
<appSettings>
  <add key="BotId" value="YOUR_BOT_ID" />
  <add key="MicrosoftAppId" value="YOUR_MICROSOFT_APP_ID" />
  <add key="MicrosoftAppPassword" value="YOUR_MICROSOFT_APP_PASSWORD" />
</appSettings>
```

* `YOUR_BOT_ID` 請輸入您的機器人名稱，例如 `Human-Hand-Off-Bot`
* `YOUR_MICROSOFT_APP_ID` 請輸入 Microsoft 應用程式識別碼
* `YOUR_MICROSOFT_APP_PASSWORD` 請輸入 Microsoft 應用程式密碼

## 修改 demo-hand-off.html 範例程式碼

`demo-hand-off.html` 範例程式碼使用以下兩種頻道：

1. [Web Chat](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-webchat)
2. [Direct Line](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-directline)

分別有各自的頻道密鑰，請至 Azure Bot Service 服務後台建立，並修改範例程式碼的以下位置。

```html
<iframe src="https://webchat.botframework.com/embed/Human-Hand-Off-Bot?s=YOUR_WEBCHAT_SECRET_GOES_HERE"></iframe>
```

* `YOUR_WEBCHAT_SECRET_GOES_HERE` 請輸入 Azure Bot Service 的 Web Chat 頻道密鑰

```javascript
var botConnection = new BotChat.DirectLine({
  //secret: params['s'],
  secret: 'YOUR_DIRECT_LINE_SECRET_GOES_HERE',
  token: params['t'],
  domain: params['domain'],
  webSocket: params['webSocket'] && params['webSocket'] === "true" // defaults to true
});
```

* `YOUR_DIRECT_LINE_SECRET_GOES_HERE` 請輸入 Azure Bot Service 的 Direct Line 頻道密鑰

> 正式環境請避免**頻道密鑰**，建議改用 Token 的方式替代，詳請參考[此份官方文件](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication)。
