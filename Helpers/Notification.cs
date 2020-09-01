using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MayMayShop.API.Const;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MayMayShop.API.Helpers
{
    public class Notification
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task SendFCMNotification(string topic, string title, string body, int userId, string redirectAction, int redirectAttribute, int notificationId,bool isSentSeller)
        {  
            try
            {
                var fcmToken="";
                var fcmSenderId="";
                if(isSentSeller)//seller
                { 
                    fcmToken= MayMayShopConst.FCM_TOKEN_KEY_SELLER;
                    fcmSenderId= MayMayShopConst.FCM_SENDER_ID_SELLER;
                }
                else{
                    fcmToken= MayMayShopConst.FCM_TOKEN_KEY_BUYER;
                    fcmSenderId= MayMayShopConst.FCM_SENDER_ID_BUYER;                   
                }  
                 WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "POST"; 
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmToken));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", fcmSenderId));
                    tRequest.ContentType = "application/json";
                    var payload = new
                    {
                        to = "/topics/"+ topic,
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            title = title,
                            body = body
                        },
                        data = new Dictionary<string, string>()
                        {
                            ["RedirectAction"] = redirectAction,
                            ["RedirectAttribute"] = redirectAttribute.ToString(),
                            ["NotificationId"] = notificationId.ToString(),
                            ["UserId"] = userId.ToString(),
                        }
                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = await tRequest.GetResponseAsync())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                    }
                            }
                        }
                    }               
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }            
        }
    }
}

