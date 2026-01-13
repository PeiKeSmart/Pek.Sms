# 烽火短信使用教程

烽火短信平台提供稳定的短信发送服务，支持国内外短信发送。

## 📦 安装

```bash
dotnet add package Pek.Sms.FengHuo
```

## 🔑 获取凭证

联系烽火短信平台获取以下信息：
- **用户名（AccessKey）**：平台分配的用户名
- **密码（AccessSecret）**：平台分配的密码
- **签名（SignName）**：短信签名
- **API 地址（BaseUrl）**：平台提供的 API 基础地址

## ⚙️ 配置

### 方式一：配置文件（推荐）

```json
{
  "Sms": {
    "Data": [
      {
        "Code": "fenghuo_notify",
        "Name": "fenghuo",
        "DisplayName": "烽火通知短信",
        "SmsType": 0,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "your_username",
        "AccessSecret": "your_password",
        "SignName": "【沛柯智能】",
        "Timeout": 60000,
        "Security": true,
        "RetryTimes": 3,
        "ExtendData": "{\"BaseUrl\": \"https://aisms.aipaas.com:8443\"}"
      },
      {
        "Code": "fenghuo_marketing",
        "Name": "fenghuo",
        "DisplayName": "烽火营销短信",
        "SmsType": 2,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "your_username",
        "AccessSecret": "your_password",
        "SignName": "【沛柯智能】",
        "Timeout": 60000,
        "Security": true,
        "RetryTimes": 3,
        "ExtendData": "{\"BaseUrl\": \"https://aisms.aipaas.com:8443\"}"
      }
    ]
  }
}
```

### 方式二：代码配置

```csharp
var config = new SmsData
{
    Code = "fenghuo_notify",
    Name = "fenghuo",
    DisplayName = "烽火通知短信",
    SmsType = 0,
    AccessKey = "your_username",
    AccessSecret = "your_password",
    SignName = "【沛柯智能】",
    Timeout = 60000,
    Security = true,
    RetryTimes = 3,
    ExtendData = "{\"BaseUrl\": \"https://aisms.aipaas.com:8443\"}"
};
```

## 📝 基础用法

### 发送单条短信

```csharp
using Pek.Sms;
using Pek.Sms.FengHuo;

// 获取配置
var settings = SmsSettings.Current;
var config = settings.FindByNameAndType("fenghuo", 0);

// 创建客户端
var client = new FengHuoSmsClient(config);

// 发送短信
var result = await client.SendAsync("13800138000", "您的验证码是123456，5分钟内有效。");

if (result.Success)
{
    Console.WriteLine("发送成功");
}
else
{
    Console.WriteLine($"发送失败：{result.Message}");
}
```

### 批量发送短信

烽火支持批量发送，多个手机号用逗号分隔（单次最多 1000 个）：

```csharp
var mobiles = "13800138000,13800138001,13800138002";
var content = "系统升级通知：系统将于今晚22:00-24:00进行升级维护，期间服务暂停，请合理安排使用时间。";

var result = await client.SendAsync(mobiles, content);
```

### 使用模板发送

```csharp
// 模板内容：您的验证码是{code}，{minutes}分钟内有效。
var content = $"您的验证码是123456，5分钟内有效。";
var result = await client.SendAsync("13800138000", content);
```

### 发送国际短信

```csharp
// 使用国际短信配置（SmsType = 1）
var config = settings.FindByNameAndType("fenghuo", 1);
var client = new FengHuoSmsClient(config);

// 国际号码需要带国家码
var result = await client.SendAsync("+85298765432", "Your verification code is 123456.");
```

## 🔧 高级功能

### 查询余额

```csharp
var balance = await client.GetBalanceAsync();
Console.WriteLine($"当前余额：{balance} 条");
```

### 查询发送记录

```csharp
// 查询指定日期的发送记录
var date = DateTime.Now.AddDays(-1);
var records = await client.QueryRecordsAsync(date);

foreach (var record in records)
{
    Console.WriteLine($"手机号：{record.Mobile}，状态：{record.Status}，时间：{record.SendTime}");
}
```

### 查询发送状态

```csharp
// 根据消息 ID 查询发送状态
var msgId = "msg_123456789";
var status = await client.QueryStatusAsync(msgId);

Console.WriteLine($"发送状态：{status.StatusText}");
Console.WriteLine($"提交时间：{status.SubmitTime}");
Console.WriteLine($"完成时间：{status.CompleteTime}");
```

## 🎯 最佳实践

### 1. 验证码场景

```csharp
/// <summary>发送验证码</summary>
public async Task<Boolean> SendVerifyCodeAsync(String phone, String code)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("fenghuo", 0);
    var client = new FengHuoSmsClient(config);

    var content = $"您的验证码是{code}，5分钟内有效。如非本人操作，请忽略此短信。";
    var result = await client.SendAsync(phone, content);
    
    return result.Success;
}
```

### 2. 通知场景

```csharp
/// <summary>发送订单通知</summary>
public async Task<Boolean> SendOrderNotificationAsync(String phone, String orderNo)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("fenghuo", 0);
    var client = new FengHuoSmsClient(config);

    var content = $"您的订单{orderNo}已发货，预计3-5个工作日送达。物流信息请登录系统查询。";
    var result = await client.SendAsync(phone, content);
    
    return result.Success;
}
```

### 3. 营销短信

```csharp
/// <summary>发送营销短信</summary>
public async Task<Boolean> SendMarketingSmsAsync(List<String> phones, String activity)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("fenghuo", 2);  // SmsType = 2 表示营销短信
    var client = new FengHuoSmsClient(config);

    var mobiles = String.Join(",", phones);
    var content = $"{activity}正在火热进行中！立即登录参与活动，回复TD退订。";
    
    var result = await client.SendAsync(mobiles, content);
    return result.Success;
}
```

### 4. 错误处理

```csharp
try
{
    var result = await client.SendAsync(phone, content);
    
    if (result.Success)
    {
        Log.Info($"烽火短信发送成功：{phone}");
    }
    else
    {
        Log.Error($"烽火短信发送失败：{phone}，错误信息：{result.Message}");
        
        // 根据错误码进行处理
        if (result.Message.Contains("余额不足"))
        {
            // 发送告警通知管理员充值
            NotifyAdmin("短信余额不足，请及时充值");
        }
        else if (result.Message.Contains("签名错误"))
        {
            // 检查签名配置
            Log.Error("签名配置错误，请检查配置文件");
        }
    }
}
catch (Exception ex)
{
    Log.Error($"烽火短信发送异常：{ex.Message}", ex);
}
```

### 5. 定时检查余额

```csharp
/// <summary>定时检查短信余额</summary>
public async Task CheckBalanceAsync()
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("fenghuo", 0);
    var client = new FengHuoSmsClient(config);

    var balance = await client.GetBalanceAsync();
    
    Log.Info($"当前短信余额：{balance} 条");
    
    // 余额不足 100 条时告警
    if (balance < 100)
    {
        Log.Warn($"短信余额不足：当前剩余 {balance} 条");
        await NotifyAdminAsync($"短信余额不足，当前剩余 {balance} 条，请及时充值！");
    }
}
```

## 🔒 安全建议

### 1. 签名验证

烽火平台使用 MD5 签名验证请求合法性：

```
sign = MD5(userName + timestamp + MD5(password))
```

客户端会自动计算签名，无需手动处理。

### 2. 防刷机制

```csharp
// 使用缓存限制同一手机号的发送频率
public async Task<Boolean> SendWithRateLimitAsync(String phone, String content)
{
    var cacheKey = $"sms_ratelimit_{phone}";
    
    // 检查是否在限制时间内
    if (Cache.TryGetValue(cacheKey, out _))
    {
        throw new InvalidOperationException("发送过于频繁，请稍后再试");
    }
    
    var result = await client.SendAsync(phone, content);
    
    if (result.Success)
    {
        // 设置 60 秒的发送间隔
        Cache.Set(cacheKey, true, TimeSpan.FromSeconds(60));
    }
    
    return result.Success;
}
```

### 3. 内容过滤

```csharp
/// <summary>检查短信内容是否合规</summary>
private Boolean CheckContentCompliance(String content)
{
    // 敏感词过滤
    var sensitiveWords = new[] { "违禁词1", "违禁词2" };
    
    foreach (var word in sensitiveWords)
    {
        if (content.Contains(word))
        {
            Log.Warn($"短信内容包含敏感词：{word}");
            return false;
        }
    }
    
    // 长度检查（70字符以内按1条计费，超出按67字符/条计费）
    if (content.Length > 500)
    {
        Log.Warn($"短信内容过长：{content.Length} 字符");
        return false;
    }
    
    return true;
}
```

## 📌 注意事项

1. **签名格式**：签名必须使用【】包裹，例如：【沛柯智能】
2. **内容规范**：不得发送违法违规内容，营销类短信必须提供退订方式（回复TD退订）
3. **发送频率**：建议同一手机号间隔 60 秒以上，避免触发平台限流
4. **字数计费**：
   - 70 字符以内（含签名）：1 条
   - 超过 70 字符：按 67 字符/条计费
5. **国际短信**：需要单独配置，费用较高
6. **BaseUrl 配置**：不同客户可能使用不同的 API 地址，需要在 ExtendData 中配置

## 🔗 相关链接

- 烽火短信平台官网：联系服务商获取
- API 文档：联系服务商获取
- 技术支持：联系服务商获取
