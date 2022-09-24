using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageBestSellersMyTask : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IOrderReportService _orderReportService;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;

        public HomepageBestSellersMyTask(
            IStoreContext storeContext, 
            IOrderReportService orderReportService, 
            IProductService productService, 
            IProductModelFactory productModelFactory)
        {
            _storeContext = storeContext;
            _orderReportService = orderReportService;
            _productService = productService;
            _productModelFactory = productModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var bestSellersReport = await _orderReportService.BestSellersReportAsync(storeId: store.Id);

            var products = await (await _productService.GetProductsByIdsAsync(bestSellersReport.Select(bs => bs.ProductId).Take(3).ToArray())).ToListAsync();

            if (products.Any() is false)
                return Content("");

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true)).ToList();

            return View(model);
        }
    }
}
