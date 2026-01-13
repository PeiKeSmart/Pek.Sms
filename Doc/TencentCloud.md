# 腾讯云短信使用教程

腾讯云短信服务是腾讯云提供的短信发送平台，支持验证码、通知、营销等多种短信类型。

##  安装

```bash
dotnet add package Pek.Sms.TencentCloud
```

##  实现状态

**已完成**：腾讯云短信功能已实现，可以正常使用！

已实现的功能：
-  TencentSmsClient - 短信客户端
-  TencentSmsResult - 发送结果
-  SendAsync - 异步发送方法
-  TC3-HMAC-SHA256 签名算法
-  错误处理和重试机制
-  批量发送支持（单次最多200个号码）
-  国际短信支持

##  获取凭证

1. 登录 [腾讯云控制台](https://cloud.tencent.com/)
2. 进入 **短信服务** 控制台
3. 获取以下信息：
   - **SecretId**：访问密钥 ID（对应 AccessKey）
   - **SecretKey**：访问密钥密文（对应 AccessSecret）
   - **SDK AppId**：短信应用 ID
   - **签名内容**：短信签名（需要审核通过）
   - **模板 ID**：短信模板 ID（需要审核通过）

##  配置示例

在 `sms.config` 或 `appsettings.json` 中配置：

```json
{
  "Sms": [
    {
      "Name": "tencent",
      "Type": "sms",
      "AccessKey": "AKIDxxxxxxxxxxxxxxxx",
      "AccessSecret": "xxxxxxxxxxxxxxxx",
      "SignName": "您的短信签名",
      "Security": true,
      "Timeout": 5000,
      "RetryTimes": 3,
      "Data": {
        "SdkAppId": "1400000000",
        "TemplateId": "123456",
        "CountryCode": "+86"
      }
    }
  ]
}
```

**配置说明**：
- `SdkAppId`：短信应用 ID（必填）
- `TemplateId`：模板 ID（必填）
- `CountryCode`：国家码（可选，默认 +86）

##  使用说明

### 快速示例

```csharp
using Pek.Sms;
using Pek.Sms.TencentCloud;

// 获取配置并创建客户端
var settings = SmsSettings.Current;
var config = settings.FindByNameAndType("tencent", 0);
var client = new TencentSmsClient(config);

// 发送短信（单个手机号）
var result = await client.SendAsync(
    mobiles: "13800138000",
    templateParams: ["123456", "5"]  // 模板参数
);

// 批量发送（多个手机号，逗号分隔）
var result = await client.SendAsync(
    mobiles: "13800138000,13900139000",
    templateParams: ["123456", "5"]
);

// 完整参数示例
var result = await client.SendAsync(
    mobiles: "13800138000",
    templateParams: ["123456", "5"],
    templateId: "789012",  // 可选，不传则使用配置中的 TemplateId
    extendCode: "01",      // 可选，短信码号扩展号
    sessionContext: "订单123",  // 可选，用户的 session 内容
    senderId: null         // 可选，国际/港澳台短信 SenderId
);

if (result.IsSuccess)
{
    Console.WriteLine("发送成功！");
}
else
{
    Console.WriteLine($"发送失败：{result.Message}");
}
```

### 发送结果说明

```csharp
public class TencentSmsResult
{
    /// <summary>是否成功</summary>
    public Boolean IsSuccess { get; set; }

    /// <summary>请求ID</summary>
    public String? RequestId { get; set; }

    /// <summary>错误码</summary>
    public String? Code { get; set; }

    /// <summary>消息</summary>
    public String? Message { get; set; }

    /// <summary>发送状态集合</summary>
    public List<SendStatus>? SendStatusSet { get; set; }
}
```

##  注意事项

1. **手机号格式**：
   - 默认自动添加 +86 前缀（中国大陆）
   - 可通过配置 `CountryCode` 修改默认国家码（如 +852 香港、+1 美国）
   - 手机号已包含 + 前缀时，不会重复添加国家码
   - 示例：`13800138000` → `+8613800138000`，`+85298765432` → `+85298765432`
2. **签名和模板**：必须审核通过后才能使用
3. **发送频率**：30秒内同一号码不能重复发送相同内容
4. **批量发送**：单次最多200个号码，多个号码使用逗号分隔
5. **配置要求**：ExtendData 中必须配置 SdkAppId 和 TemplateId
6. **国际短信**：发送国际短信需配置对应国家码，或直接在手机号前添加 + 和国家码

##  相关链接

- [腾讯云短信官方文档](https://cloud.tencent.com/document/product/382)
- [腾讯云短信控制台](https://console.cloud.tencent.com/smsv2)
- [API 参考](https://cloud.tencent.com/document/product/382/52077)
