using System.Configuration;

namespace Coats.Crafts.Configuration
{
    public class CtaTemplateIdSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public CtaTemplateIdCollection Instances
        {
            get { return (CtaTemplateIdCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class CtaTemplateIdCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CtaTemplateIdElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CtaTemplateIdElement)element).TcmId;
        }
    }

    public class CtaTemplateIdElement : ConfigurationElement
    {
        // Using constants to cut down typos
        private const string _tcmId = "tcmId";
        //private const string _useView = "useView";
        private const string _description = "description";
        private const string _ctaTemplateIdList = "CtaTemplateIdList";

        [ConfigurationProperty(_tcmId, IsKey=true, IsRequired = true)]
        public string TcmId
        {
            get { return (string)base[_tcmId]; }
            set { base[_tcmId] = value; }
        }

        //[ConfigurationProperty(_useView, IsRequired = true)]
        //[StringValidator(InvalidCharacters = "  ~!@#$%^&*()[]{}/;’\"|\\", MinLength = 1, MaxLength = 256)]
        //public string UseView
        //{
        //    get { return (string)base[_useView]; }
        //    set { base[_useView] = value; }
        //}

        [ConfigurationProperty(_description, IsRequired = false)]
        public string Description
        {
            get { return (string)base[_description]; }
            set { base[_description] = value; }
        }
    
    }
}

