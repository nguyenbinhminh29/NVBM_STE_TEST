var builder = DistributedApplication.CreateBuilder(args);

var groceryDb = builder.AddConnectionString("grocerydb");
var cache = builder.AddRedis("cache");

var catalogApi = builder.AddProject<Projects.NVBM_Catalog_API>("catalog-api")
       .WithReference(groceryDb)
       .WithReference(cache);

var barcodeApi = builder.AddProject<Projects.NVBM_Barcode_API>("barcode-api")
       .WithReference(groceryDb)
       .WithReference(cache);

builder.AddProject<Projects.NVBM_Gateway>("gateway")
       .WithReference(catalogApi)
       .WithReference(barcodeApi)
       .WithExternalHttpEndpoints();

builder.Build().Run();
