namespace Coats.Crafts.Configuration
{
    using Castle.Components.DictionaryAdapter;

    public interface IAppSettings
    {
        string About { get; }

        string AboutUs { get; }

        string AccessDenied { get; }

        string Accessibility { get; }

        string AddressBookId { get; }

        string App404Path { get; }

        string AppErrorPath { get; }

        string Brands { get; }

        string CatsInsertSQL { get; }

        string CatsThankYou { get; }

        bool CheckEmailNewsletterOption { get; }

        string CoatsStrapline { get; }

        string CommentModeration { get; }

        string CommentsAdminUsers { get; }

        string ContactUs { get; }

        string ContentTypeActiveDiscussion { get; }

        string ContentTypeActiveDiscussionTitle { get; }

        string ContentTypeArticle { get; }

        string ContentTypeArticleTitle { get; }

        string ContentTypeBlog { get; }

        string ContentTypeBlogTitle { get; }

        string ContentTypeBrand { get; }

        string ContentTypeBrandTitle { get; }

        string ContentTypeCareRepair { get; }

        string ContentTypeCareRepairTitle { get; }

        string ContentTypeDesigner { get; }

        string ContentTypeDesignerTitle { get; }

        string ContentTypeEvent { get; }

        string ContentTypeEventTitle { get; }

        string ContentTypeFAQ { get; }

        string ContentTypeFAQTitle { get; }

        string ContentTypeGenericGeneral { get; }

        string ContentTypeGenericPromo { get; }

        string ContentTypeIntroduction { get; }

        string ContentTypeMoodboard { get; }

        string ContentTypeMoodboardTitle { get; }

        string ContentTypeProduct { get; }

        string ContentTypeProductHaberdashery { get; }

        string ContentTypeProductTitle { get; }

        string ContentTypeProject { get; }

        string ContentTypeProjectTitle { get; }

        string ContentTypeTutorial { get; }

        string ContentTypeTutorialTitle { get; }

        string CookieAccept { get; }

        string CookieInfo { get; }

        string CraftListCategory { get; }

        string Culture { get; }

        string DebugJson { get; }

        string DefaultCommentListing { get; }

        string DefaultLat { get; }

        string DefaultLong { get; }

        string DefaultPage { get; }

        string DefaultSiteEmailAddress { get; }

        bool DisableWishList { get; }

        string EditProfile { get; }

        string EmailNewsLetterConfirmation { get; }

        string EmailNewsletterListCategory { get; }

        string EmailNewsletterYes { get; }

        int EventsNearYouMaxResults { get; }

        int EventsNearYouRadius { get; }

        string EventsUrl { get; }

        string FabricPage { get; }

        string FacebookAppId { get; }

        string FacebookAppSecret { get; }

        string FacebookNumberToReturn { get; }

        string FaceBookType { get; }

        string FooterLinksComponents { get; }

        string GeneralContentSchemaTitle { get; }

        string GeoData { get; }

        string GoogleAPIsMapUrl { get; }

        string GoogleMapsAPIKey { get; }

        string HomepageItemsPerPage { get; }

        string HomepageSort { get; }

        bool IsDoubleOpt { get; }

        bool IsPrivacyEnable { get; }

        string KnowledgeServiceId { get; }

        string Legal { get; }

        string LocationId { get; }

        string Login { get; }

        string Logo { get; }

        string Logout { get; }

        [Key("ModeratorODataEndPoint.URL")]
        string ModeratorODataEndPoint { get; }

        [Key("ModeratorODataEndPoint.Security.ClientId")]
        string ModeratorODataEndPointSecurityClientId { get; }

        [Key("ModeratorODataEndPoint.Security.ClientSecret")]
        string ModeratorODataEndPointSecurityClientSecret { get; }

        [Key("ModeratorODataEndPoint.Security.Endpoint")]
        string ModeratorODataEndPointSecurityEndpoint { get; }

        string MyProfile { get; }

        string NestedLevelDelimiter { get; }

        string NewsletterCategory { get; }

        [Key("ODataEndPoint.URL")]
        string ODataEndPoint { get; }

        [Key("ODataEndPoint.Security.ClientId")]
        string ODataEndPointSecurityClientId { get; }

        [Key("ODataEndPoint.Security.ClientSecret")]
        string ODataEndPointSecurityClientSecret { get; }

        [Key("ODataEndPoint.Security.Endpoint")]
        string ODataEndPointSecurityEndpoint { get; }

        string OurBrandsOpenInNewTab { get; }

        string OurBrandsSpotlightRandom { get; }

        string PageTitleBrandAppend { get; }

        string PageTitleProductAppend { get; }

        string PageTitleProfileAppend { get; }

        string PasswordReminderEmailFrom { get; }

        string PasswordReminderEmailTemplate { get; }

        string PasswordReminderPath { get; }

        string PrivacyAndCookie { get; }

        string PrivacyPolishPath { get; }

        string ProductExplorer { get; }

        string ProductFree { get; }

        string ProductGroupFormat { get; }

        string ProductGroups { get; }

        string ProductServiceId { get; }

        string ProfileVisibleListCategory { get; }

        string PromoSchemaTitle { get; }

        string PublicasterApiPassword { get; }

        string PublicasterDataHeader { get; }

        string PublicasterDelimiter { get; }

        string PublicasterEncryptedAccountID { get; }

        string PublicasterGetSubscriberIdByEmailUrl { get; }

        string PublicasterGetSubscribeUserInfoUrl { get; }

        string PublicasterGlobalStatus { get; }

        string PublicasterJsonFormatResponse { get; }

        string PublicasterListActiveStatus { get; }

        string PublicasterListImportUrl { get; }

        string PublicasterListPendingStatus { get; }

        string PublicasterMailingListId { get; }

        string PublicasterPrimaryKey { get; }

        string PublicasterSource { get; }

        string PublicasterSubscribeUrl { get; }

        string PublicasterUnsubscribeToSubscribeUrl { get; }

        string PublicasterUnSubscribeUserUrl { get; }

        string PublicasteruserID { get; }

        [Key("DD4T.PublicationId")]
        int PublicationId { get; }

        string RegisterAddSP { get; }

        string RegisterSuccess { get; }

        string RegisterThankYou { get; }

        string RegisterWelcome { get; }

        string Registration { get; }

        string ReleaseVersion { get; }

        string RemoteImagePath { get; }

        [Key("DD4T.ResourceName")]
        string ResourceName { get; }

        [Key("DD4T.ResourcePath")]
        string ResourcePath { get; }

        string Scrapbook { get; }

        string ServiceGroup { get; }

        string ServiceId { get; }

        string SessionKey { get; }

        string ShareACreationEmailAddress { get; }

        string ShoppingList { get; }

        string ShoppingListEmailFrom { get; }

        string ShoppingListEmailTemplate { get; }

        string ShoppingListPrint { get; }

        string SiteIdentifier { get; }

        string SiteMap { get; }

        string SiteMapFile { get; }

        string SiteUrl { get; }

        string StoreFinder { get; }

        string StoreLocatorDefaultValue { get; }

        string StoreLocatorDisplayMsgWhenMaxResults { get; }

        string StoreLocatorDistanceUnitName { get; }

        string StoreLocatorDistanceValues { get; }

        string StoreLocatorFormPath { get; }

        string StoreLocatorGoogleGeoCodeBaseAddress { get; }

        string StoreLocatorGoogleMapsBaseAddress { get; }

        string StoreLocatorLatitude { get; }

        string StoreLocatorLongitude { get; }

        string StoreLocatorMaxDisambigResults { get; }

        string StoreLocatorMaxEventsPerRetailer { get; }

        string StoreLocatorMaxResults { get; }

        string StoreLocatorNoResults { get; }

        string StoreLocatorPostcodeNotFound { get; }

        string StoreLocatorShowEventsInNextDays { get; }

        string StoreLocatorUseMilesForDistanceUnit { get; }

        string StoreLocatorWithinVals { get; }

        string TimeZone { get; }

        string TimeZoneAbbr { get; }

        string ToolGroup { get; }

        string ToolId { get; }
    }
}
