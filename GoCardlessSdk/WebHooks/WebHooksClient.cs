﻿using System.IO;
using GoCardlessSdk.Api.Json;
using Newtonsoft.Json;

namespace GoCardlessSdk.WebHooks
{
    public class WebHooksClient
    {
        /// <summary>
        /// Parse request from GoCardless into objects, and checks the signature of the request.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Payload ParseRequest(string content)
        {
            // deserialize request content. (ensure content type is set to JSON in GoCardless setup)
            var serializer = new JsonSerializer
            {
                ContractResolver = new UnderscoreToCamelCasePropertyResolver(),
            };
            var payload = serializer.Deserialize<GoCardlessRequest>(new JsonTextReader(new StringReader(content))).Payload;

            // validate the HMAC digest by resigning the received parameters
            var signature = payload.Signature;
            payload.Signature = null;

            if (signature != Utils.GetSignatureForParams(payload.ToHashParams(), GoCardless.AccountDetails.AppSecret))
            {
                throw new SignatureException("Signature was invalid!");
            }

            return payload;
        }
    }
}
