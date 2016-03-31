namespace Coats.Crafts.Configuration
{
    using System;

    public static class WebConfiguration
    {
        private static IAppSettings _settings;

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
}

