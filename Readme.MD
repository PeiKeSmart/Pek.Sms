# Pek.Sms - 短信操作类库

基于 DH 框架的短信操作类库，支持多家短信服务商，提供统一的短信发送接口。

## 📦 支持的服务商

| 服务商 | 包名 | 支持类型 | 状态 |
|--------|------|----------|------|
| 阿里云 | `Pek.Sms.Aliyun` | 国内/国际 通知/营销 | ✅ 稳定 |
| 腾讯云 | `Pek.Sms.TencentCloud` | 国内/国际 通知/营销 | ✅ 稳定 |
| 烽火 | `Pek.Sms.FengHuo` | 国内/国际 通知/营销 | ✅ 稳定 |
| 连陆 | `Pek.Sms.LianLu` | 国内/国际 通知/营销 | ✅ 稳定 |
| 聚合包 | `Pek.Sms.Extensions` | 一键安装所有客户端 | ✅ 元包 |

## 🚀 快速开始

### 安装

```bash
# 安装单个服务商
dotnet add package Pek.Sms.Aliyun
dotnet add package Pek.Sms.FengHuo
dotnet add package Pek.Sms.LianLu
dotnet add package Pek.Sms.TencentCloud

# 或者一次性安装所有服务商（推荐）
dotnet add package Pek.Sms.Extensions
```

### 配置文件

在 `appsettings.json` 或使用 NewLife.Configuration 配置：

```json
{
  "Sms": {
    "Data": [
      {
        "Code": "aliyun_notify",
        "Name": "aliyun",
        "DisplayName": "阿里云通知短信",
        "SmsType": 0,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "your_access_key",
        "AccessSecret": "your_access_secret",
        "SignName": "你的签名",
        "Timeout": 60000,
        "Security": true,
        "RetryTimes": 3
      }
    ],
    "EnableProxy": false,
    "ProxyUrl": "",
    "ProxyToken": ""
  }
}
```

### 短信类型说明

| SmsType | 说明 |
|---------|------|
| 0 | 国内通知类短信 |
| 1 | 国际通知类短信 |
| 2 | 国内营销类短信 |
| 3 | 国际营销类短信 |

## 📖 使用教程

### 基础用法

```csharp
using Pek.Sms;
using Pek.Sms.Aliyun;

// 加载配置
var settings = SmsSettings.Current;

// 获取默认的国内通知类配置
var config = settings.FindDefault(0);

// 创建客户端
var client = new AliyunDysmsClient(config);

// 发送短信
var message = new AliyunDysmsMessage
{
    PhoneNumbers = "13800138000",
    TemplateCode = "SMS_123456789",
    TemplateParam = new Dictionary<String, Object>
    {
        ["code"] = "123456",
        ["product"] = "测试系统"
    }
};

var result = await client.SendAsync(message);
if (result.IsSuccess)
{
    Console.WriteLine("发送成功");
}
else
{
    Console.WriteLine($"发送失败：{result.Message}");
}
```

## 📚 详细文档

- [阿里云短信使用教程](Doc/Aliyun.md)
- [烽火短信使用教程](Doc/FengHuo.md)
- [连陆短信使用教程](Doc/LianLu.md)
- [腾讯云短信使用教程](Doc/TencentCloud.md)

## ⚙️ 高级功能

### 代理支持

当本地调试需要访问限制 IP 的短信平台时，可以启用代理：

```json
{
  "Sms": {
    "EnableProxy": true,
    "ProxyUrl": "https://proxy.example.com",
    "ProxyToken": "your_proxy_token"
  }
}
```

### 多配置管理

同一服务商可以配置多个账号，系统会按照 `Order` 字段排序执行，失败时自动切换到下一个：

```json
{
  "Sms": {
    "Data": [
      {
        "Code": "aliyun_1",
        "Name": "aliyun",
        "SmsType": 0,
        "Order": 1,
        "IsEnabled": true
      },
      {
        "Code": "aliyun_2",
        "Name": "aliyun",
        "SmsType": 0,
        "Order": 2,
        "IsEnabled": true
      }
    ]
  }
}
```

### 获取配置的多种方式

```csharp
var settings = SmsSettings.Current;

// 获取默认配置
var config1 = settings.FindDefault(0);

// 根据 Code 获取
var config2 = settings.FindByCode("aliyun_notify");

// 根据服务商名称和类型获取
var config3 = settings.FindByNameAndType("aliyun", 0);

// 获取某个服务商的所有配置
var configs = settings.FindByName("aliyun");
```

## 🔧 扩展配置

某些服务商需要额外的配置参数，可以通过 `ExtendData` 传递：

```json
{
  "Code": "fenghuo_notify",
  "Name": "fenghuo",
  "ExtendData": "{\"BaseUrl\": \"https://aisms.aipaas.com:8443\"}"
}
```

在代码中访问：

```csharp
var baseUrl = config.Data.TryGetValue("BaseUrl", out var url) 
    ? url?.ToString() 
    : "default_url";
```

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

MIT License

## 🔗 相关链接

- [PeiKeSmart 组织](https://github.com/PeiKeSmart)
- [NewLife 框架](https://github.com/NewLifeX)
- [官方网站](https://www.haocoding.com)