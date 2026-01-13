# 连陆短信使用教程

连陆短信平台提供稳定的短信发送服务。

## 📦 安装

```bash
dotnet add package Pek.Sms.LianLu
```

## 🔑 获取凭证

联系连陆短信平台获取以下信息：
- **用户ID（AccessKey）**：平台分配的用户 ID
- **密钥（AccessSecret）**：平台分配的密钥
- **签名（SignName）**：短信签名
- **API 地址**：默认为 `http://47.110.199.86:8081`

## ⚙️ 配置

### 方式一：配置文件（推荐）

```json
{
  "Sms": {
    "Data": [
      {
        "Code": "lianlu_notify",
        "Name": "lianlu",
        "DisplayName": "连陆通知短信",
        "SmsType": 0,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "your_user_id",
        "AccessSecret": "your_secret",
        "SignName": "【沛柯智能】",
        "Timeout": 60000,
        "Security": false,
        "RetryTimes": 3
      },
      {
        "Code": "lianlu_marketing",
        "Name": "lianlu",
        "DisplayName": "连陆营销短信",
        "SmsType": 2,
        "IsDefault": true,
        "IsEnabled": true,
        "AccessKey": "your_user_id",
        "AccessSecret": "your_secret",
        "SignName": "【沛柯智能】",
        "Timeout": 60000,
        "Security": false,
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
    Code = "lianlu_notify",
    Name = "lianlu",
    DisplayName = "连陆通知短信",
    SmsType = 0,
    AccessKey = "your_user_id",
    AccessSecret = "your_secret",
    SignName = "【沛柯智能】",
    Timeout = 60000,
    Security = false,
    RetryTimes = 3
};
```

## 📝 基础用法

### 发送单条短信

```csharp
using Pek.Sms;
using Pek.Sms.LianLu;

// 获取配置
var settings = SmsSettings.Current;
var config = settings.FindByNameAndType("lianlu", 0);

// 创建客户端
var client = new LianLuSmsClient(config);

// 发送短信（不需要手动添加签名，会自动添加）
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

连陆支持批量发送，多个手机号用逗号分隔（单次最多 1000 个）：

```csharp
var mobiles = "13800138000,13800138001,13800138002";
var content = "系统升级通知：系统将于今晚22:00-24:00进行升级维护。";

var result = await client.SendAsync(mobiles, content);

if (result.Success)
{
    Console.WriteLine($"批量发送成功，共发送 {mobiles.Split(',').Length} 条");
}
```

### 发送国际短信

```csharp
// 使用国际短信配置（SmsType = 1）
var config = settings.FindByNameAndType("lianlu", 1);
var client = new LianLuSmsClient(config);

// 国际号码需要带国家码
var result = await client.SendAsync("+85298765432", "Your verification code is 123456.");
```

## 🎯 最佳实践

### 1. 验证码场景

```csharp
/// <summary>发送验证码</summary>
public async Task<Boolean> SendVerifyCodeAsync(String phone, String code)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("lianlu", 0);
    var client = new LianLuSmsClient(config);

    // 签名会自动添加，无需手动拼接
    var content = $"您的验证码是{code}，5分钟内有效。如非本人操作，请忽略此短信。";
    var result = await client.SendAsync(phone, content);
    
    if (result.Success)
    {
        Log.Info($"验证码发送成功：{phone}");
        return true;
    }
    else
    {
        Log.Error($"验证码发送失败：{phone}，{result.Message}");
        return false;
    }
}
```

### 2. 通知场景

```csharp
/// <summary>发送系统通知</summary>
public async Task<Boolean> SendNotificationAsync(String phone, String title, String content)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("lianlu", 0);
    var client = new LianLuSmsClient(config);

    var message = $"{title}：{content}";
    var result = await client.SendAsync(phone, message);
    
    return result.Success;
}
```

### 3. 营销短信

```csharp
/// <summary>批量发送营销短信</summary>
public async Task<(Int32 Success, Int32 Failed)> SendMarketingSmsAsync(
    List<String> phones, 
    String activity)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("lianlu", 2);  // SmsType = 2 表示营销短信
    var client = new LianLuSmsClient(config);

    // 营销短信必须包含退订提示
    var content = $"{activity}正在火热进行中！立即登录参与活动。回复TD退订。";

    // 分批发送，每批 500 个号码
    var batchSize = 500;
    var successCount = 0;
    var failedCount = 0;

    for (var i = 0; i < phones.Count; i += batchSize)
    {
        var batch = phones.Skip(i).Take(batchSize).ToList();
        var mobiles = String.Join(",", batch);

        try
        {
            var result = await client.SendAsync(mobiles, content);
            
            if (result.Success)
            {
                successCount += batch.Count;
                Log.Info($"批次 {i / batchSize + 1} 发送成功：{batch.Count} 条");
            }
            else
            {
                failedCount += batch.Count;
                Log.Error($"批次 {i / batchSize + 1} 发送失败：{result.Message}");
            }

            // 批次间延迟，避免触发限流
            if (i + batchSize < phones.Count)
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            failedCount += batch.Count;
            Log.Error($"批次 {i / batchSize + 1} 发送异常：{ex.Message}", ex);
        }
    }

    return (successCount, failedCount);
}
```

### 4. 错误处理和重试

```csharp
/// <summary>发送短信，支持自动重试</summary>
public async Task<Boolean> SendWithRetryAsync(String phone, String content, Int32 maxRetries = 3)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("lianlu", 0);
    var client = new LianLuSmsClient(config);

    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            var result = await client.SendAsync(phone, content);
            
            if (result.Success)
            {
                Log.Info($"短信发送成功：{phone}，尝试次数：{i + 1}");
                return true;
            }
            else
            {
                Log.Warn($"短信发送失败：{phone}，第 {i + 1} 次尝试，错误：{result.Message}");
                
                // 某些错误不需要重试
                if (result.Message.Contains("手机号码格式错误") || 
                    result.Message.Contains("账号不存在"))
                {
                    Log.Error($"短信发送永久失败：{phone}，{result.Message}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"短信发送异常：{phone}，第 {i + 1} 次尝试", ex);
        }

        // 重试前等待
        if (i < maxRetries - 1)
        {
            await Task.Delay((i + 1) * 1000);  // 递增等待时间
        }
    }

    Log.Error($"短信发送失败：{phone}，已达到最大重试次数 {maxRetries}");
    return false;
}
```

### 5. 发送频率控制

```csharp
// 使用分布式缓存限制发送频率
public class SmsRateLimiter
{
    private readonly IDistributedCache _cache;

    public SmsRateLimiter(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>检查是否允许发送</summary>
    public async Task<Boolean> CanSendAsync(String phone)
    {
        var cacheKey = $"sms_ratelimit_{phone}";
        var value = await _cache.GetStringAsync(cacheKey);
        
        return String.IsNullOrEmpty(value);
    }

    /// <summary>记录发送</summary>
    public async Task RecordSendAsync(String phone, Int32 intervalSeconds = 60)
    {
        var cacheKey = $"sms_ratelimit_{phone}";
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(intervalSeconds)
        };
        
        await _cache.SetStringAsync(cacheKey, "1", options);
    }

    /// <summary>发送短信（带频率限制）</summary>
    public async Task<SmsResult> SendWithRateLimitAsync(
        LianLuSmsClient client, 
        String phone, 
        String content)
    {
        if (!await CanSendAsync(phone))
        {
            return new SmsResult
            {
                Success = false,
                Message = "发送过于频繁，请稍后再试"
            };
        }

        var result = await client.SendAsync(phone, content);
        
        if (result.Success)
        {
            await RecordSendAsync(phone);
        }

        return result;
    }
}
```

### 6. 日志和监控

```csharp
/// <summary>发送短信并记录详细日志</summary>
public async Task<Boolean> SendWithLoggingAsync(String phone, String content, String businessType)
{
    var settings = SmsSettings.Current;
    var config = settings.FindByNameAndType("lianlu", 0);
    var client = new LianLuSmsClient(config);

    var startTime = DateTime.Now;
    
    try
    {
        Log.Info($"[连陆短信] 开始发送，手机号：{phone}，业务类型：{businessType}");
        
        var result = await client.SendAsync(phone, content);
        var duration = (DateTime.Now - startTime).TotalMilliseconds;
        
        if (result.Success)
        {
            Log.Info($"[连陆短信] 发送成功，手机号：{phone}，耗时：{duration}ms");
            
            // 记录到数据库
            await SaveSmsLogAsync(new SmsLog
            {
                Phone = phone,
                Content = content,
                Provider = "lianlu",
                BusinessType = businessType,
                Status = "success",
                Duration = duration,
                SendTime = startTime
            });
            
            return true;
        }
        else
        {
            Log.Error($"[连陆短信] 发送失败，手机号：{phone}，错误：{result.Message}，耗时：{duration}ms");
            
            await SaveSmsLogAsync(new SmsLog
            {
                Phone = phone,
                Content = content,
                Provider = "lianlu",
                BusinessType = businessType,
                Status = "failed",
                ErrorMessage = result.Message,
                Duration = duration,
                SendTime = startTime
            });
            
            return false;
        }
    }
    catch (Exception ex)
    {
        var duration = (DateTime.Now - startTime).TotalMilliseconds;
        Log.Error($"[连陆短信] 发送异常，手机号：{phone}，异常：{ex.Message}，耗时：{duration}ms", ex);
        
        await SaveSmsLogAsync(new SmsLog
        {
            Phone = phone,
            Content = content,
            Provider = "lianlu",
            BusinessType = businessType,
            Status = "exception",
            ErrorMessage = ex.Message,
            Duration = duration,
            SendTime = startTime
        });
        
        return false;
    }
}
```

## 🔒 安全建议

### 1. 签名验证

连陆平台使用时间戳和 MD5 签名验证：

```
sign = MD5(userid + timestamp + secret).ToLower()
```

客户端会自动计算签名，无需手动处理。

### 2. 内容检查

```csharp
/// <summary>检查短信内容</summary>
private (Boolean IsValid, String Message) CheckContent(String content)
{
    // 长度检查
    if (String.IsNullOrWhiteSpace(content))
    {
        return (false, "内容不能为空");
    }

    if (content.Length > 500)
    {
        return (false, "内容过长，最多支持500字符");
    }

    // 敏感词检查
    var sensitiveWords = LoadSensitiveWords();
    foreach (var word in sensitiveWords)
    {
        if (content.Contains(word))
        {
            return (false, $"内容包含敏感词：{word}");
        }
    }

    return (true, String.Empty);
}
```

## 📌 注意事项

1. **签名格式**：签名会自动添加到内容前面，格式为【签名】，无需手动拼接
2. **内容规范**：不得发送违法违规内容，营销类短信必须提供退订方式
3. **发送频率**：建议同一手机号间隔 60 秒以上
4. **字数计费**：
   - 70 字符以内（含签名）：1 条
   - 超过 70 字符：按 67 字符/条计费
5. **批量发送**：单次最多 1000 个号码，建议分批发送并控制频率
6. **国际短信**：需要单独配置，费用较高

## 🔗 相关链接

- 连陆短信平台官网：联系服务商获取
- API 文档：联系服务商获取
- 技术支持：联系服务商获取
