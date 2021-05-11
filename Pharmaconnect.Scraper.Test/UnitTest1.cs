using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pharmaconnect.Scraper.Lib;

namespace Pharmaconnect.Scraper.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async System.Threading.Tasks.Task TestStoreScrape()
        {
            var storeId = new System.Guid("8ab18efb-b158-4ca1-8103-34792852814d");
            var storeScraper = new StoreScraper(storeId);
            var store = await storeScraper.Load();

            Assert.AreEqual(store, new Store
            {
                ID = storeId,
                Name = "TELUS Pharmacy Demo",
                Address = "25 York Street Toronto ON M5J 2V5",
                Phone = "(416) 245-6153"
            });
        }
    }
}
