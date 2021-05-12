using AzureMapsToolkit;
using AzureMapsToolkit.Search;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
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

        private readonly AzureMapsServices _azureMaps;

        public StoreScraper(string azureMapsKey)
        {
            _azureMaps = new AzureMapsServices(azureMapsKey);
        }

        public async Task<Store> Load(Store store)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = await htmlWeb.LoadFromWebAsync(string.Format(URL_STORE_DETAILS, store.ID));
            var docNode = htmlDoc.DocumentNode;

            var name = docNode.QuerySelector(SELECTOR_STORE_NAME).InnerText.Trim();
            var address = string.Join(" ", docNode.QuerySelectorAll(SELECTOR_STORE_ADDRESS).Select(n => n.InnerText.Trim()).Where(t => !t.Contains("Note:", StringComparison.InvariantCultureIgnoreCase)));
            var phone = docNode.QuerySelectorAll(SELECTOR_STORE_CONTACT).Where(n => n.ChildNodes[1].InnerText.Trim().Equals(CONTACT_TYPE_PHONE)).FirstOrDefault().ChildNodes[3].InnerText.Trim();

            store.Name = name;
            store.Address = address;
            store.Phone = phone;

            return store;
        }

        public async Task<Store> Geolocate(Store store)
        {
            var searchAddressRequest = new SearchAddressRequest
            {
                Query = WebUtility.UrlEncode(store.Address),
                Limit = 5
            };
            var searchResp = await _azureMaps.GetSearchAddress(searchAddressRequest);
            if (searchResp.Error != null)
            {
                //Handle error
            }

            var resultLatLng = searchResp.Result.Results[0].Position;

            var timezoneResp = await _azureMaps.GetTimezoneByCoordinates(new AzureMapsToolkit.Timezone.TimeZoneRequest
            {
                Query = $"{resultLatLng.Lat},{resultLatLng.Lon}"
            });

            if (timezoneResp.Error != null)
            {
                //handle error?
            }

            store.Latitude = resultLatLng.Lat;
            store.Longitude = resultLatLng.Lon;
            store.Timezone = timezoneResp.Result.TimeZones[0].Id;

            return store;
        }

        public async Task ScrapStoreDB()
        {
            var storeDbJson = await File.ReadAllTextAsync("pharmaconnect_storedb.json");
            var stores = JsonSerializer.Deserialize<Store[]>(storeDbJson);

            for (var i = 0; i < stores.Length; i++)
            {
                var store = stores[i];

                store = await Load(store);
                store = await Geolocate(store);
            }

            storeDbJson = JsonSerializer.Serialize(stores);
            await File.WriteAllTextAsync("pharmaconnect_storedb.json", storeDbJson);
        }
    }
}
