﻿using NewLife.Serialization;

using Pek.Sms.Aliyun.Core;
using Pek.Sms.Exceptions;

namespace Pek.Sms.Aliyun.Models;

public class AliyunDysmsMessage
{
    /// <summary>
    /// 短信模板Code，应严格按"模板CODE"填写, 请参考: https://dysms.console.aliyun.com/dysms.htm#/develop/template ，必填
    /// </summary>
    public String TemplateCode { get; set; } = String.Empty;

    public List<String> Phone { get; set; } = [];

    public Dictionary<String, String> TemplateParams { get; set; } = [];

    public String? OutId { get; set; }

    public String GetPhoneString() => String.Join(",", Phone);

    public String GetTemplateParamsString() => TemplateParams.ToJson();

    public Boolean HasTemplateParams() => TemplateParams.Count != 0;

    public void FixParameters(IDictionary<String, Object?> config)
    {
        if (String.IsNullOrWhiteSpace(TemplateCode))
            TemplateCode = config["TemplateCode"].SafeString();
    }

    public void CheckParameters()
    {
        if (String.IsNullOrWhiteSpace(TemplateCode))
        {
            throw new InvalidArgumentException("短信模板 Code 不能为空", AliyunDysmsConstants.ServiceName, 401);
        }

        var phoneCount = Phone?.Count;
        if (phoneCount == 0)
        {
            throw new InvalidArgumentException("收信人为空", AliyunDysmsConstants.ServiceName, 401);
        }

        if (phoneCount > AliyunDysmsConstants.MaxReceivers)
        {
            throw new InvalidArgumentException("收信人超过限制", AliyunDysmsConstants.ServiceName, 401);
        }
    }
}