using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmaconnect.Scraper.Lib
{
    public class AppointmentScraper
    {
        private const string SELECTOR_APPOINTMENT_AVAILABILITY_DAY_ITEM = "div.appointment-availability__days-item";
        private const string SELECTOR_APPOINTMENT_AVAILABILITY_TIME_ITEM = "div#{0} div.appointment-availability__period-text";

        private const string ATTRIBUTE_DATA_SELECTED_ID = "data-selected-id";

        public Guid StoreUID { get; private set; }

        public AppointmentScraper(Guid storeUID)
        {
            StoreUID = storeUID;
        }

        public async Task Load()
        {
            //using var httpClient = new HttpClient();
            //var html = await httpClient.GetStringAsync("https://gist.githubusercontent.com/andrew-from-toronto/69b87a099237f207c23767b4c1531558/raw/74bc8742c763cf41583bf96c9318be6dd1d69af5/output.html");

            //var htmlDoc = new HtmlDocument();
            //htmlDoc.LoadHtml(html);

            var htmlWeb = new HtmlWeb();
            //var doc = htmlWeb.Load("https://gist.githubusercontent.com/andrew-from-toronto/69b87a099237f207c23767b4c1531558/raw/74bc8742c763cf41583bf96c9318be6dd1d69af5/output.html");
            var htmlDoc = await htmlWeb.LoadFromWebAsync("https://pharmaconnect.ca/Appointment/8ab18efb-b158-4ca1-8103-34792852814d/Slots?serviceType=ImmunizationCovid");
            var docNode = htmlDoc.DocumentNode;

            var days = docNode.QuerySelectorAll(SELECTOR_APPOINTMENT_AVAILABILITY_DAY_ITEM).Select(x => x.Attributes[ATTRIBUTE_DATA_SELECTED_ID].Value).ToArray();
            var dayAppointments = (from day in days
                                   from appointment in docNode.QuerySelectorAll(string.Format(SELECTOR_APPOINTMENT_AVAILABILITY_TIME_ITEM, day))
                                   let dayParsed = DateTime.Parse(day)
                                   select new
                                   {
                                       Day = dayParsed,
                                       time = appointment.InnerText
                                   }).ToArray();
        }
    }
}
