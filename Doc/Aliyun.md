# 阿里云短信使用教程

阿里云短信服务是阿里云提供的短信发送平台，支持验证码、通知、营销等多种短信类型。

## 📦 安装

```bash
dotnet add package Pek.Sms.Aliyun
```

## 🔑 获取凭证

1. 登录 [阿里云控制台](https://www.aliyun.com/)
2. 进入 **短信服务** 控制台
3. 获取以下信息：
   - **AccessKeyId**：访问密钥 ID
   - **AccessKeySecret**：访问密钥密文
   - **签名名称**：短信签名（需要审核通过）
   - **模板 CODE**：短信模板代码（需要审核通过）

## ⚙️ 配置

### 方式一：配置文件（推荐）

在 `appsettings.json` 或 NewLife 配置中心添加：

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
        "AccessKey": "LTAI5txxxxxxxxxxxxxxxx",
        "AccessSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        "SignName": "沛柯智能",
        "Timeout": 60000,
        "Security": true,
        "RetryTimes": 3
      },
      {
        "Code": "aliyun_marketing",
        "Name": "aliyun",
        "DisplayName": "阿里云营销短信",
        "SmsType": 2,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "LTAI5txxxxxxxxxxxxxxxx",
        "AccessSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        "SignName": "沛柯智能营销",
        "Timeout": 60000,
        "Security": true,
        "RetryTimes": 3
      }
    ]
  }
}
```

### 方式二：代码配置

```csharp
var config = new SmsData
{
    Code = "aliyun_notify",
    Name = "aliyun",
    DisplayName = "阿里云通知短信",
    SmsType = 0,
    AccessKey = "LTAI5txxxxxxxxxxxxxxxx",
    AccessSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    SignName = "沛柯智能",
    Timeout = 60000,
    Security = true,
    RetryTimes = 3
};
```

## 📝 基础用法

### 发送验证码短信

```csharp
using Pek.Sms;
using Pek.Sms.Aliyun;
using Pek.Sms.Aliyun.Models;

// 获取配置
var settings = SmsSettings.Current;
var config = settings.FindByNameAndType("aliyun", 0);

// 创建客户端
var client = new AliyunDysmsClient(config);

// 构建消息
var message = new AliyunDysmsMessage
{
    Phone = ["13800138000"],
    TemplateCode = "SMS_123456789",
    TemplateParams = new Dictionary<String, String>
    {
        ["code"] = "123456"
    }
};

// 发送短信
var result = await client.SendAsync(message);

if (result.IsSuccess())
{
    Console.WriteLine($"发送成功，请求ID：{result.RequestId}");
    Console.WriteLine($"业务ID：{result.BizId}");
}
else
{
    Console.WriteLine($"发送失败：{result.Code} - {result.Message}");
}
```

### 发送通知短信

```csharp
var message = new AliyunDysmsMessage
{
    Phone = ["13800138000"],
    TemplateCode = "SMS_987654321",
    TemplateParams = new Dictionary<String, String>
    {
        ["name"] = "张三",
        ["time"] = "2026-01-13 10:30",
        ["location"] = "会议室A"
    }
};

var result = await client.SendAsync(message);
```

### 批量发送短信

阿里云支持批量发送，多个手机号添加到列表中（单次最多 1000 个）：

```csharp
var message = new AliyunDysmsMessage
{
    Phone = ["13800138000", "13800138001", "13800138002"],
    TemplateCode = "SMS_123456789",
    TemplateParams = new Dictionary<String, String>
    {
        ["content"] = "系统升级通知"
    }
};

var result = await client.SendAsync(message);
```

### 发送国际短信

```csharp
// 配置国际短信（SmsType = 1）
var config = settings.FindByNameAndType("aliyun", 1);
var client = new AliyunDysmsClient(config);

var message = new AliyunDysmsMessage
{
    Phone = ["+85298765432"],  // 需要带国家/地区码
    TemplateCode = "SMS_INT_123456",
    TemplateParams = new Dictionary<String, String>
    {
        ["code"] = "654321"
    }
};

var result = await client.SendAsync(message);
```

## 🔧 高级功能

### 自定义扩展参数

阿里云支持扩展参数，可以通过配置传递：

```json
{
  "Code": "aliyun_notify",
  "Name": "aliyun",
  "ExtendData": "{\"OutId\": \"business_id_123\", \"SmsUpExtendCode\": \"extend_code\"}"
}
```

在消息中使用：

```csharp
var message = new AliyunDysmsMessage
{
    Phone = ["13800138000"],
    TemplateCode = "SMS_123456789",
    TemplateParams = new Dictionary<String, String>
    {
        ["code"] = "123456"
    },
    OutId = "business_id_123"  // 业务自定义 ID，用于关联业务数据
};
```

### 错误处理

```csharp
try
{
    var result = await client.SendAsync(message);
    
    if (result.IsSuccess)
    {
        // 成功处理
        SaveToDatabase(result.BizId, result.RequestId);
    }
    else
    {
        // 失败处理
        switch (result.Code)
        {
            case "isv.MOBILE_NUMBER_ILLEGAL":
                Console.WriteLine("手机号格式错误");
                break;
            case "isv.BUSINESS_LIMIT_CONTROL":
                Console.WriteLine("触发业务限流");
                break;
            case "isv.TEMPLATE_MISSING_PARAMETERS":
                Console.WriteLine("模板参数缺失");
                break;
            default:
                Console.WriteLine($"发送失败：{result.Message}");
                break;
        }
    }
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"参数错误：{ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"系统异常：{ex.Message}");
}
```

### 常见错误码

| 错误码 | 说明 | 处理建议 |
|--------|------|----------|
| `isv.MOBILE_NUMBER_ILLEGAL` | 手机号格式错误 | 检查手机号格式，国际号码需要带国家码 |
| `isv.BUSINESS_LIMIT_CONTROL` | 触发业务限流 | 降低发送频率或升级套餐 |
| `isv.TEMPLATE_MISSING_PARAMETERS` | 模板参数缺失 | 检查模板参数是否完整 |
| `isv.INVALID_PARAMETERS` | 参数无效 | 检查所有参数格式和内容 |
| `isv.DAY_LIMIT_CONTROL` | 触发日发送限额 | 等待第二天或升级套餐 |
| `SignatureDoesNotMatch` | 签名验证失败 | 检查 AccessKey 和 AccessSecret |

## 🎯 最佳实践

### 1. 验证码场景

```csharp
/// <summary>发送验证码</summary>
/// <param name="phone">手机号</param>
/// <param name="code">验证码</param>
/// <param name="expireMinutes">过期时间（分钟）</param>
public async Task<Boolean> SendVerifyCodeAsync(String phone, String code, Int32 expireMinutes = 5)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("aliyun", 0);
    var client = new AliyunDysmsClient(config);

    var message = new AliyunDysmsMessage
    {
        Phone = [phone],
        TemplateCode = "SMS_123456789",
        TemplateParams = new Dictionary<String, String>
        {
            ["code"] = code,
            ["minutes"] = expireMinutes.ToString()
        }
    };

    var result = await client.SendAsync(message);
    return result.IsSuccess;
}
```

### 2. 通知场景

```csharp
/// <summary>发送订单通知</summary>
public async Task<Boolean> SendOrderNotificationAsync(String phone, String orderNo, Decimal amount)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("aliyun", 0);
    var client = new AliyunDysmsClient(config);

    var message = new AliyunDysmsMessage
    {
        Phone = [phone],
        TemplateCode = "SMS_987654321",
        TemplateParams = new Dictionary<String, String>
        {
            ["order_no"] = orderNo,
            ["amount"] = amount.ToString("F2")
        }
    };

    var result = await client.SendAsync(message);
    return result.IsSuccess;
}
```

### 3. 使用重试机制

配置文件中设置 `RetryTimes` 可以自动重试失败的请求：

```json
{
  "RetryTimes": 3,
  "Timeout": 60000
}
```

### 4. 日志记录

```csharp
var result = await client.SendAsync(message);

// 记录日志
Log.Info($"[阿里云短信] 发送至 {message.GetPhoneString()}，模板：{message.TemplateCode}，" +
         $"结果：{(result.IsSuccess ? "成功" : "失败")}，RequestId：{result.RequestId}");

if (!result.IsSuccess)
{
    Log.Error($"[阿里云短信] 发送失败，错误码：{result.Code}，错误信息：{result.Message}");
}
```

## 📌 注意事项

1. **签名和模板**：必须在阿里云控制台申请并审核通过后才能使用
2. **发送频率**：默认限制同一手机号 1 分钟内最多发送 1 条，1 小时内最多发送 5 条
3. **内容规范**：不得发送违法违规内容，营销类短信需要明确退订方式
4. **费用**：按条计费，验证码 0.045 元/条，通知 0.045 元/条，营销 0.055 元/条
5. **国际短信**：费用较高，需要单独配置和申请

## 🔗 相关链接

- [阿里云短信服务官方文档](https://help.aliyun.com/product/44282.html)
- [阿里云短信控制台](https://dysms.console.aliyun.com/)
- [短信服务 API 参考](https://help.aliyun.com/document_detail/101414.html)
