using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmaconnect.Scraper.Lib
{
    public class StoreScraper
    {
        private const string URL_STORE_DETAILS = "https://pharmaconnect.ca/Pharmacies/Detail?storeId={0}&isPharmacyView=False&displaySkip=False&isLinkingMode=True";

        private const string SELECTOR_STORE_NAME = "h2.b-pharmacy-detail__name";
        private const string SELECTOR_STORE_ADDRESS = "div.b-pharmacy-detail__info";
        private const string SELECTOR_STORE_CONTACT = "div.b-pharmacy-detail__contact";

        private const string SELECTOR_APPOINTMENT_AVAILABILITY_DAY_ITEM = "div.appointment-availability__days-item";
        private const string SELECTOR_APPOINTMENT_AVAILABILITY_TIME_ITEM = "div#{0} div.appointment-availability__period-text";

        private const string CONTACT_TYPE_PHONE = "Phone:";

        public Guid StoreUID { get; private set; }

        public StoreScraper(Guid storeUID)
        {
            StoreUID = storeUID;
        }

        public async Task<Store> Load()
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = await htmlWeb.LoadFromWebAsync(string.Format(URL_STORE_DETAILS, StoreUID));
            var docNode = htmlDoc.DocumentNode;

            var name = docNode.QuerySelector(SELECTOR_STORE_NAME).InnerText.Trim();
            var address = string.Join(" ", docNode.QuerySelectorAll(SELECTOR_STORE_ADDRESS).Select(n => n.InnerText.Trim()).Where(t => !t.Contains("Note:", StringComparison.InvariantCultureIgnoreCase)));
            var phone = docNode.QuerySelectorAll(SELECTOR_STORE_CONTACT).Where(n => n.ChildNodes[1].InnerText.Trim().Equals(CONTACT_TYPE_PHONE)).FirstOrDefault().ChildNodes[3].InnerText.Trim();

            return new Store
            {
                ID = StoreUID,
                Name = name,
                Address = address,
                Phone = phone
            };
        }
    }
}
