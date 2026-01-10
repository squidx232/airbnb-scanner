using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AirbnbKeywordFinder.Core.Extensions;
using AirbnbKeywordFinder.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAirbnbKeywordFinderWasm();

await builder.Build().RunAsync();
