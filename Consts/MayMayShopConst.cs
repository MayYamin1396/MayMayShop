using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MayMayShop.API.Facade;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MayMayShop.API.Const
{
    public static class MayMayShopConst
    {
        public readonly static string PARAM_APPLICATION = "MayMayShop";        
        public readonly static string PARAM_ORDERSTATUS = PARAM_APPLICATION + ".orderStatus";
        public readonly static string PARAM_PLATFORM = PARAM_APPLICATION + ".platform";
        public readonly static string PARAM_SEARCH_KEYWORD = PARAM_APPLICATION + ".searchKeyword";
        public readonly static string PARAM_ACTIVITY_TYPE = PARAM_APPLICATION + ".activityType";
        public readonly static string PARAM_TAX = PARAM_APPLICATION + ".tax";
        public readonly static string PARAM_SEARCHTYPE = PARAM_APPLICATION + ".searchType";
        public readonly static string PARAM_NOTI_REDIRECT_ACTION = PARAM_APPLICATION + ".notiRedirectAction";
        public readonly static string PARAM_PAYMENT_SERVICE = PARAM_APPLICATION + ".paymentService";
        public readonly static string PARAM_PAYMENT_STATUS = PARAM_APPLICATION + ".paymentStatus";
        public readonly static string PARAM_MESSAGETEMPLATES = PARAM_APPLICATION + ".smsMessages";
        public readonly static string PARAM_EMAIL_TEMPLATES = PARAM_APPLICATION + ".emailMessges";
        public readonly static string PARAM_AWS = PARAM_APPLICATION + ".aws";
        public readonly static string PARAM_FCM = PARAM_APPLICATION + ".fcm";
        public readonly static string PARAM_COMPANY_PROFILE = PARAM_APPLICATION + ".companyProfile";
        public readonly static string PARAM_KBZ_GATEWAY = PARAM_APPLICATION + ".KBZGateway";

        public static readonly Regex MOBILE_NO_REGEX = new Regex("((^(9|8))[0-9]{7})$");
        public const string DATE_TIME_FORMAT = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";
        public const string TIME_FORMAT = @"hh\:mm\:ss";
        public const int MAX_ALLOWED_BIT_SIZE = 1120;
        public const int CH_BIT_SIZE = 16;
        public const int ENG_BIT_SIZE = 7;

        public static string DB_CONNECTION = string.Empty;
        public static string ENVIRONMENT = string.Empty;

        public static int CUSTOM_DELIVERY_SERVICE_ID;

        public static string TOKEN_SECRET = string.Empty;
        public static double TOKEN_EXPIRATION_MINUTES;
        public static string TOKEN_ALG = string.Empty;
        public static bool TOKEN_VALIDATEISSUERSIGNINGKEY;
        public static bool TOKEN_VALIDATEAUDIENCE;
        public static bool TOKEN_VALIDATEISSUER;
        public static bool TOKEN_VALIDATELIFETIME;
        public static string TOKEN_ISSUER;
        public static int DELIVERY_COUNT;
        public static string MESSAGE_API_KEY;
        public static string MESSAGE_SENDPACKAGE_SENDER_SENT;
        public static string MESSAGE_SENDPACKAGE_RECIPIENT_SENT;
        public static string MESSAGE_SENDPACKAGE_RECIPIENT_ARRIVED;

        public static string EMAIL_SENDER;
        public static string EMAIL_PASSWORD;

        public static string EMAIL_HOST;

        public static string EMAIL_PORT;

        public static string AWS_KEY;

        public static string AWS_SECRET;

        public static string AWS_STATIC_IMG_PATH;
        public static string AWS_KEY_PATH;

        public static string AWS_PRODUCT_PATH { get; set; }
        public static string AWS_ORDER_PATH { get; set; }
        public static string AWS_BANNER_PATH {get;set;}
        public static string AWS_USER_PROFILE_PATH {get;set;}
        public static string AWS_IMG_HOSTED {get;set;}
        public static string FCM_TOKEN_KEY_BUYER;
        public static string FCM_SENDER_ID_BUYER { get; set; }
        public static string FCM_TOKEN_KEY_SELLER;
        public static string FCM_SENDER_ID_SELLER { get; set; }
        public static string USER_SERVICE_PATH { get; set; }
        public static string MEMBERPOINT_SERVICE_PATH { get; set; }

        public static string USER_TOKEN;
        public static int ORDER_STATUS_ORDER;
        public static int ORDER_STATUS_TAKE;
        public static int ORDER_STATUS_SENDING;
        public static int ORDER_STATUS_SENT;
        public static int ORDER_STATUS_CANCEL;
        public static int PLATFORM_ANDROID;
        public static int PLATFORM_IOS;
        public static int PLATFORM_WEB;

        public static int ACTIVITY_TYPE_SEARCH {get;set;}
        public static int ACTIVITY_TYPE_ADD_TO_CART;
        public static int ACTIVITY_TYPE_REMOVE_FROM_CART;
        public static int ACTIVITY_TYPE_ORDER;
        public static int ACTIVITY_TYPE_ORDER_CANCEL;
        public static int ACTIVITY_TYPE_IP;
        public static int ACTIVITY_TYPE_ACTIVE;
        public static int ACTIVITY_TYPE_REGISTER;
        public static int ACTIVITY_TYPE_MAKE_PAYMENT;
        public static int ACTIVITY_TYPE_PAYMENT_STATUS;
        public static int ACTIVITY_TYPE_ORDER_STATUS;

        public static int SEARCH_KEYWORD_ALL;
        public static int SEARCH_KEYWORD_WITH_RESULT;
        public static int SEARCH_KEYWORD_WITHOUT_RESULT;

        public static int TAX_TAXPAYER_ID;
        public static int TAX_COMMERCIAL_TAX;
        public static int SEARCHTYPE_NAME;
        public static int SEARCHTYPE_CATEGORY;
        public static int SEARCHTYPE_TAG;
        public static int SEARCHTYPE_LATEST;
        public static int SEARCHTYPE_PROMOTION;
        public static int SEARCHTYPE_SUB_CATEGORY;
        public static int SEARCHTYPE_BEST_SELLER;

        public static int SELLER_USER_ID;
        public static int USER_TYPE_ADMIN;
        public static int USER_TYPE_SELLER;
        public static int USER_TYPE_BUYER;

        public static int PAYMENT_SERVICE_WAVE_MONEY;
        public static int PAYMENT_SERVICE_KPAY;
        public static int PAYMENT_SERVICE_OK_DOLLAR;
        public static int PAYMENT_SERVICE_MASTER;
        public static int PAYMENT_SERVICE_BANK;
        public static int PAYMENT_SERVICE_VISA;
        public static int PAYMENT_SERVICE_COD;

        public static int PAYMENT_STATUS_CHECK;
        public static int PAYMENT_STATUS_SUCCESS;
        public static int PAYMENT_STATUS_FAIL;

        public static string COMPANY_PHONE_NO { get; set; }
        public static string COMPANY_WEBSITE { get; set; }
        public static string COMPANY_SHARED_LINK { get; set; }
        public static string COMPANY_SHOP_NAME { get; set; }
        public static string COMPANY_SHOP_URL { get; set; }
        public static string COMPANY_SHOP_ADDRESS { get; set; }
        public static string COMPANY_ORDER_DETAIL_URL { get; set; }

        public static string XMLFILEPATH;
        public static string KBZ_GATEWAY_NOTIFY_URL { get; set; }

        public static string KBZ_GATEWAY_METHOD { get; set; }

        public static string KBZ_GATEWAY_SIGN_TYPE { get; set; }

        public static string KBZ_GATEWAY_VERSION { get; set; }

        public static string KBZ_GATEWAY_APP_ID { get; set; }

        public static string KBZ_GATEWAY_TRADE_TYPE_MOBILE { get; set; }
        public static string KBZ_GATEWAY_TRADE_TYPE_WEB { get; set; }

        public static string KBZ_GATEWAY_MERCH_CODE { get; set; }

        public static string KBZ_GATEWAY_CURRENCY { get; set; }

        public static string KBZ_GATEWAY_KEY { get; set; }

        public static string KBZ_GATEWAY_URI { get; set; }
        public static string KBZ_GATEWAY_QUERYORDER_URI {get;set;}
        public static string KBZ_GATEWAY_QUERYORDER_METHOD{get;set;}
        public static string KBZ_GATEWAY_QUERYORDER_VERSION{get;set;}

        public static int APPLICATION_CONFIG_ID=0;
        
        public static int BEST_SELLER_DURATION;

        public static string NOTI_REDIRECT_ACTION_ORDER_DETAIL;

        public static string DELIVERY_SERVICE_PATH { get; set; }

        public static string[] NonKeyword = new string[]
                                            {
                                                "a",
                                                "an",
                                                "the",
                                                "for",
                                                "of",
                                                "in",
                                                "at",
                                                "on",
                                                "by",
                                                "to"
                                            };
        public static string[] AndroidDevice = new string[]
                                            {
                                                "Android",
                                                "android"
                                            };
        public static string[] IosDevice = new string[]
                                            {
                                                "IOS",
                                                "ios",
                                                "Iphone",
                                                "iphone",
                                                "IPAD",
                                                "IPad",
                                                "ipad",
                                            };

        public static void loadConfigData()
        {
            ENVIRONMENT = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".environment");

            DB_CONNECTION = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".dbConnection");

            CUSTOM_DELIVERY_SERVICE_ID = int.Parse(ConfigFacade.ApplicationConfig
                .GetParameter(PARAM_APPLICATION + ".customDeliveryServiceId"));

            USER_SERVICE_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".userService");
            MEMBERPOINT_SERVICE_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".memberPointService");

            TOKEN_SECRET = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".tokenSecret");
            TOKEN_ALG = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".alg");
            TOKEN_ISSUER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".tokenIssuer");
            DELIVERY_COUNT = int
            .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".deliveryCount"));
            TOKEN_VALIDATEISSUERSIGNINGKEY = bool
                .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".issuerSigningKey"));
            TOKEN_VALIDATEAUDIENCE = bool
                .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".validateAudience"));
            TOKEN_VALIDATEISSUER = bool
                .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".validateIssuer"));
            TOKEN_VALIDATELIFETIME = bool
                .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".validateLifetime"));

            MESSAGE_SENDPACKAGE_SENDER_SENT =
                ConfigFacade.ApplicationConfig.GetParameter(PARAM_MESSAGETEMPLATES + ".sendPackageSenderStausSent");
            MESSAGE_SENDPACKAGE_RECIPIENT_SENT =
                ConfigFacade.ApplicationConfig.GetParameter(PARAM_MESSAGETEMPLATES + ".sendPackageRecipientStatusSent");
            MESSAGE_SENDPACKAGE_RECIPIENT_ARRIVED =
                ConfigFacade.ApplicationConfig.GetParameter(PARAM_MESSAGETEMPLATES + ".sendPackageRecipientStatusArrived");
            MESSAGE_API_KEY =
                ConfigFacade.ApplicationConfig.GetParameter(PARAM_MESSAGETEMPLATES + ".apiKey");

            EMAIL_SENDER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_EMAIL_TEMPLATES + ".email");
            EMAIL_PASSWORD = ConfigFacade.ApplicationConfig.GetParameter(PARAM_EMAIL_TEMPLATES + ".password");
            EMAIL_HOST = ConfigFacade.ApplicationConfig.GetParameter(PARAM_EMAIL_TEMPLATES + ".host");
            EMAIL_PORT = ConfigFacade.ApplicationConfig.GetParameter(PARAM_EMAIL_TEMPLATES + ".port");

            AWS_KEY = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS + ".key");
            AWS_SECRET = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS + ".secret");
            AWS_STATIC_IMG_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS + ".imgStaticUrl");
            AWS_PRODUCT_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".productPath");
            AWS_ORDER_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".orderPath");
            AWS_BANNER_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".bannerPath");
            AWS_USER_PROFILE_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".userProfilePath");
            AWS_KEY_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".keyPath");
            AWS_IMG_HOSTED = ConfigFacade.ApplicationConfig.GetParameter(PARAM_AWS +  ".imgHosted");

            FCM_TOKEN_KEY_BUYER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_FCM + ".tokenKeyBuyer");
            FCM_SENDER_ID_BUYER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_FCM + ".senderIdBuyer");
            FCM_TOKEN_KEY_SELLER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_FCM + ".tokenKeySeller");
            FCM_SENDER_ID_SELLER = ConfigFacade.ApplicationConfig.GetParameter(PARAM_FCM + ".senderIdSeller");

            USER_TOKEN = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".userToken");

            SELLER_USER_ID = int
            .Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".sellerUserId"));

            COMPANY_PHONE_NO = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".phoneNo");
            COMPANY_WEBSITE = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".website");
            COMPANY_SHARED_LINK = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".sharedLink");
            COMPANY_SHOP_NAME = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".shopName");
            COMPANY_SHOP_URL = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".shopUrl");
            COMPANY_SHOP_ADDRESS = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".shopAddress");
            COMPANY_ORDER_DETAIL_URL = ConfigFacade.ApplicationConfig.GetParameter(PARAM_COMPANY_PROFILE + ".orderDetailUrl");
            
            XMLFILEPATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".xmlFilePath");        

            KBZ_GATEWAY_NOTIFY_URL = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".notifyUrl");
            KBZ_GATEWAY_METHOD = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".method");
            KBZ_GATEWAY_SIGN_TYPE = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".sign_type");
            KBZ_GATEWAY_VERSION = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".version");
            KBZ_GATEWAY_APP_ID = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".appid");
            KBZ_GATEWAY_TRADE_TYPE_MOBILE = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".trade_type_mobile");
            KBZ_GATEWAY_TRADE_TYPE_WEB = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".trade_type_web");
            KBZ_GATEWAY_MERCH_CODE = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".merch_code");
            KBZ_GATEWAY_CURRENCY = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".trans_currency");
            KBZ_GATEWAY_KEY = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".key");
            KBZ_GATEWAY_URI = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".uri");
            KBZ_GATEWAY_QUERYORDER_URI = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".queryorderuri");
            KBZ_GATEWAY_QUERYORDER_METHOD = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".queryordermethod");
            KBZ_GATEWAY_QUERYORDER_VERSION = ConfigFacade.ApplicationConfig.GetParameter(PARAM_KBZ_GATEWAY + ".queryorderversion");

            ORDER_STATUS_ORDER =int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ORDERSTATUS + ".order"));
            ORDER_STATUS_TAKE = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ORDERSTATUS + ".take"));
            ORDER_STATUS_SENDING = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ORDERSTATUS + ".sending"));
            ORDER_STATUS_SENT = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ORDERSTATUS + ".sent"));
            ORDER_STATUS_CANCEL = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ORDERSTATUS + ".cancel"));

            PLATFORM_ANDROID = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PLATFORM + ".android"));
            PLATFORM_IOS = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PLATFORM + ".ios"));
            PLATFORM_WEB = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PLATFORM + ".web"));

            ACTIVITY_TYPE_SEARCH = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".search"));
            ACTIVITY_TYPE_ADD_TO_CART = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".addToCart"));
            ACTIVITY_TYPE_REMOVE_FROM_CART = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".removeFromCart"));
            ACTIVITY_TYPE_ORDER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".order"));
            ACTIVITY_TYPE_ORDER_CANCEL = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".orderCancel"));
            ACTIVITY_TYPE_IP = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".ip"));
            ACTIVITY_TYPE_ACTIVE = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".active"));
            ACTIVITY_TYPE_REGISTER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".register"));
            ACTIVITY_TYPE_MAKE_PAYMENT = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".makePayment"));
            ACTIVITY_TYPE_PAYMENT_STATUS = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".paymentStatus"));
            ACTIVITY_TYPE_ORDER_STATUS = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_ACTIVITY_TYPE + ".orderStatus"));

            SEARCH_KEYWORD_ALL = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCH_KEYWORD + ".all"));
            SEARCH_KEYWORD_WITH_RESULT = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCH_KEYWORD + ".withResult"));
            SEARCH_KEYWORD_WITHOUT_RESULT = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCH_KEYWORD + ".withoutResult"));

            TAX_TAXPAYER_ID = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_TAX + ".taxpayerId"));
            TAX_COMMERCIAL_TAX = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_TAX + ".commercialTax"));

            USER_TYPE_ADMIN = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".userTypeAdmin"));
            USER_TYPE_SELLER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".userTypeSeller"));
            USER_TYPE_BUYER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".userTypeBuyer"));
    
            APPLICATION_CONFIG_ID = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".applicationConfigId"));

            BEST_SELLER_DURATION = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".bestSellerDuration"));

            PAYMENT_SERVICE_WAVE_MONEY = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".waveMoney"));
            PAYMENT_SERVICE_KPAY = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".kpay"));
            PAYMENT_SERVICE_OK_DOLLAR = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".okDollar"));
            PAYMENT_SERVICE_MASTER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".master"));
            PAYMENT_SERVICE_BANK = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".bank"));
            PAYMENT_SERVICE_VISA = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".visa"));
            PAYMENT_SERVICE_COD = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_SERVICE + ".cod"));

            PAYMENT_STATUS_CHECK = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_STATUS + ".check"));
            PAYMENT_STATUS_SUCCESS = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_STATUS + ".success"));
            PAYMENT_STATUS_FAIL = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_PAYMENT_STATUS + ".fail"));

            NOTI_REDIRECT_ACTION_ORDER_DETAIL = ConfigFacade.ApplicationConfig.GetParameter(PARAM_NOTI_REDIRECT_ACTION + ".orderDetail");
        
            DELIVERY_SERVICE_PATH = ConfigFacade.ApplicationConfig.GetParameter(PARAM_APPLICATION + ".deliveryService");

            SEARCHTYPE_NAME = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".name"));
            SEARCHTYPE_CATEGORY = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".category"));
            SEARCHTYPE_TAG = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".tag"));
            SEARCHTYPE_LATEST = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".latest"));
            SEARCHTYPE_PROMOTION = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".promotion"));
            SEARCHTYPE_SUB_CATEGORY = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".subCategory"));
            SEARCHTYPE_BEST_SELLER = int.Parse(ConfigFacade.ApplicationConfig.GetParameter(PARAM_SEARCHTYPE + ".bestSeller"));
    
        }
    }
}
