using Castle.Components.DictionaryAdapter;

namespace Coats.Crafts.Configuration
{
    public static class WebConfiguration
    {
        static IAppSettings _settings;

        public static void InitSettings(IAppSettings settings)
        {
            _settings = settings;
        }

        public static IAppSettings Current
        {
            get
            {
                return _settings;
            }
        }

    }

    /// <summary>
    /// Interface for application settings in web.config
    /// </summary>
    public interface IAppSettings
    {
        string CatsThankYou { get; }
        string RegisterWelcome { get; }
        string RegisterThankYou { get; }
        string ProductGroupFormat { get; }
        string NestedLevelDelimiter { get; }
        string SessionKey { get; }

        string ReleaseVersion { get; }

        string DefaultCommentListing { get; }
        string CommentModeration { get; }

        [Key("ODataEndPoint.URL")]
        string ODataEndPoint { get; }

        [Key("ModeratorODataEndPoint.URL")]
        string ModeratorODataEndPoint { get; }

        [Key("DD4T.PublicationId")]
        int PublicationId { get; }
        [Key("DD4T.ResourceName")]
        string ResourceName { get; }
        [Key("DD4T.ResourcePath")]
        string ResourcePath { get; }
        string GoogleMapsAPIKey { get; }
        string StoreLocatorUseMilesForDistanceUnit { get; }
        string StoreLocatorDistanceUnitName { get; }
        string StoreLocatorDistanceValues { get; }
        string StoreLocatorDefaultValue { get; }
        string StoreLocatorGoogleMapsBaseAddress { get; }
        string StoreLocatorGoogleGeoCodeBaseAddress { get; }
        string StoreLocatorPostcodeNotFound { get; }
        string StoreLocatorNoResults { get; }
        string StoreLocatorShowEventsInNextDays { get; }
        string StoreLocatorMaxResults { get; }
        string StoreLocatorDisplayMsgWhenMaxResults { get; }
        string StoreLocatorMaxEventsPerRetailer { get; }
        string StoreLocatorFormPath { get; }
        string StoreLocatorMaxDisambigResults { get; }
        string StoreLocatorWithinVals { get; }

        [Key("ODataEndPoint.Security.Endpoint")]
        string ODataEndPointSecurityEndpoint { get; }

        [Key("ODataEndPoint.Security.ClientId")]
        string ODataEndPointSecurityClientId { get; }

        [Key("ODataEndPoint.Security.ClientSecret")]
        string ODataEndPointSecurityClientSecret { get; }


        [Key("ModeratorODataEndPoint.Security.Endpoint")]
        string ModeratorODataEndPointSecurityEndpoint { get; }

        [Key("ModeratorODataEndPoint.Security.ClientId")]
        string ModeratorODataEndPointSecurityClientId { get; }

        [Key("ModeratorODataEndPoint.Security.ClientSecret")]
        string ModeratorODataEndPointSecurityClientSecret { get; }

        string App404Path { get; }
        string AppErrorPath { get; }

        string DebugJson { get; }

        string AddressBookId { get; }

        string LocationId { get; }
        string ToolId { get; }
        string ServiceId { get; }
        string ToolGroup { get; }
        string ServiceGroup { get; }

        string DefaultSiteEmailAddress { get; }
        string KnowledgeServiceId { get; }
        string ProductServiceId { get; }

        string RegisterSuccess { get; }
        string AccessDenied { get; }

        string GeneralContentSchemaTitle { get; }
        string PromoSchemaTitle { get; }

        string SiteIdentifier { get; }

        string DefaultPage { get; }
        string Registration { get; }
        string Login { get; }
        string Logout { get; }
        string GeoData { get; }
        string Culture { get; }
        string CraftListCategory { get; }
        string EmailNewsletterListCategory { get; }
        string ProfileVisibleListCategory { get; }

        string ProductGroups { get; }

        string GoogleAPIsMapUrl { get; }
        string EditProfile { get; }

        int EventsNearYouRadius { get; }
        int EventsNearYouMaxResults { get; }
        string ShoppingListEmailFrom { get; }
        string ShoppingListEmailTemplate { get; }
        string ShoppingListPrint { get; }

        string StoreFinder { get; }
        string ContactUs { get; }
        string About { get; }
        string MyProfile { get; }
        string Scrapbook { get; }
        string ShoppingList { get; }

        string AboutUs { get; }
        string SiteMap { get; }
        string Accessibility { get; }
        string PrivacyAndCookie { get; }
        string Legal { get; }
        string ProductExplorer { get; }
        string Brands { get; }



        string RemoteImagePath { get; }
        string OurBrandsOpenInNewTab { get; }
        string CommentsAdminUsers { get; }

        string OurBrandsSpotlightRandom { get; }

        string ContentTypeArticle { get; }
        string ContentTypeProject { get; }
        string ContentTypeMoodboard { get; }
        string ContentTypeBlog { get; }
        string ContentTypeTutorial { get; }
        string ContentTypeGenericGeneral { get; }
        string ContentTypeIntroduction { get; }
        string ContentTypeDesigner { get; }
        string ContentTypeFAQ { get; }
        //
        string ContentTypeCareRepair { get; }
        string ContentTypeEvent { get; }
        string ContentTypeBrand { get; }
        string ContentTypeProduct { get; }
        string ContentTypeProductHaberdashery { get; }
        string ContentTypeActiveDiscussion { get; }

        string ContentTypeArticleTitle { get; }
        string ContentTypeProjectTitle { get; }
        string ContentTypeMoodboardTitle { get; }
        string ContentTypeBlogTitle { get; }
        string ContentTypeTutorialTitle { get; }
        string ContentTypeDesignerTitle { get; } // ??? Needed ???
        string ContentTypeFAQTitle { get; }
        //
        string ContentTypeCareRepairTitle { get; }
        string ContentTypeEventTitle { get; }
        string ContentTypeBrandTitle { get; }
        string ContentTypeProductTitle { get; }
        string ContentTypeActiveDiscussionTitle { get; } // ??? Needed ???

        string ContentTypeGenericPromo { get; }

        string CookieAccept { get; }
        string CookieInfo { get; }

        string FacebookAppId { get; }
        string FacebookAppSecret { get; }
        string FacebookNumberToReturn { get; }

        string DefaultLong { get; }
        string DefaultLat { get; }

        string SiteUrl { get; }
        string PasswordReminderEmailFrom { get; }
        string PasswordReminderEmailTemplate { get; }

        string PasswordReminderPath { get; }
        //Added by Ajaya for double opt 
        string PrivacyPolishPath { get; }
        bool CheckEmailNewsletterOption { get; }
        string EmailNewsletterYes { get; }
        string ShareACreationEmailAddress { get; }

        string EmailNewsLetterConfirmation { get; }

        string FooterLinksComponents { get; }

        string EventsUrl { get; }
        //API NewsLetter
        string PublicasterPrimaryKey { get; }
        string PublicasterMailingListId { get; }
        string PublicasteruserID { get; }
        string PublicasterDataHeader { get; }

        string PublicasterDelimiter { get; }
        string PublicasterEncryptedAccountID { get; }
        string PublicasterApiPassword { get; }
        string PublicasterSource { get; }

        string PublicasterListImportUrl { get; }
        string PublicasterUnSubscribeUserUrl { get; }
        string PublicasterSubscribeUrl { get; }
        string PublicasterGetSubscriberIdByEmailUrl { get; }
        string PublicasterGetSubscribeUserInfoUrl { get; }
        string PublicasterUnsubscribeToSubscribeUrl { get; }
        string PublicasterJsonFormatResponse { get; }
        string PublicasterListActiveStatus { get; }
        string PublicasterListPendingStatus { get; }
        string PublicasterGlobalStatus { get; }

        string CatsInsertSQL { get; }
        //Added by Ajaya for double opt
        string RegisterAddSP { get; }
        string CoatsStrapline { get; }

        string PageTitleProfileAppend { get; }

        string PageTitleProductAppend { get; }

        string PageTitleBrandAppend { get; }

        string ProductFree { get; }

        string SiteMapFile { get; }

        string FaceBookType { get; }
        //string FaceBookStatusType { get; }

        string StoreLocatorLongitude { get; }
        string StoreLocatorLatitude { get; }
        string Logo { get; }


        string TimeZoneAbbr { get; }

        string TimeZone { get; }

        string HomepageSort { get; }
        string HomepageItemsPerPage { get; }

        bool DisableWishList { get; }

    }
}