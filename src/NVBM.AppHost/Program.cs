var builder = DistributedApplication.CreateBuilder(args);

var groceryDb = builder.AddConnectionString("grocerydb");
var cache = builder.AddRedis("cache");

var rabbitmq = builder.AddRabbitMQ("messaging");

var catalogApi = builder.AddProject<Projects.NVBM_Catalog_API>("catalog-api")
       .WithReference(groceryDb)
       .WithReference(cache)
       .WithReference(rabbitmq);

var barcodeApi = builder.AddProject<Projects.NVBM_Barcode_API>("barcode-api")
       .WithReference(groceryDb)
       .WithReference(cache)
       .WithReference(rabbitmq);

var inventoryApi = builder.AddProject<Projects.NVBM_Inventory_API>("inventory-api")
       .WithReference(groceryDb)
       .WithReference(cache)
       .WithReference(rabbitmq);

builder.AddProject<Projects.NVBM_Gateway>("gateway")
       .WithReference(catalogApi)
       .WithReference(barcodeApi)
       .WithReference(inventoryApi)
       .WithExternalHttpEndpoints();

builder.Build().Run();
