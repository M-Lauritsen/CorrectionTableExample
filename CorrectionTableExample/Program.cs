using CorrectionTableExample.Data;
using CorrectionTableExample.Models;
using CorrectionTableExample.Models.CorrectionModels;
using CorrectionTableExample.Service;
using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GenericCorrectionDB")
            .Options;

using (var context = new DataContext(options))
{
    var productService = new EntityService<Product>(context);
    var customerService = new EntityService<Customer>(context);

    // Opret et produkt
    var product = new Product { Name = "Laptop", Price = 1000m };
    context.Products.Add(product);
    Console.WriteLine($"Produkt oprettet: Id = {product.Id}, Navn = {product.Name}, Pris = {product.Price}");

    // Opret en kunde
    var customer = new Customer { Name = "John Doe", Email = "john@example.com", DateOfBirth = new DateTime(1990, 1, 1) };
    context.Customers.Add(customer);
    Console.WriteLine($"Kunde oprettet: Id = {customer.Id}, Navn = {customer.Name}, Email = {customer.Email}, Fødselsdato = {customer.DateOfBirth}");

    context.SaveChanges();
    Console.WriteLine("Ændringer gemt i databasen.\n");

    // Anvend korrektioner på produkt
    Console.WriteLine("Anvender korrektioner på produkt...");
    productService.ApplyCorrection(product.Id, new Dictionary<string, string>
            {
                { "Name", "Updated Laptop" },
                { "Price", "1200" }
            });
    Console.WriteLine(); // Tilføj en tom linje for bedre læsbarhed

    // Anvend korrektioner på kunde
    Console.WriteLine("Anvender korrektioner på kunde...");
    customerService.ApplyCorrection(customer.Id, new Dictionary<string, string>
            {
                { "Email", "johndoe@example.com" },
                { "DateOfBirth", "1991-02-02" }
            });
    Console.WriteLine(); // Tilføj en tom linje for bedre læsbarhed

    // Hent og vis korrektionshistorik for produkt
    PrintCorrectionHistory(context, product, "produkt");
    var correctedProduct = productService.GetEntityWithCorrections(product.Id);
    Console.WriteLine($"Produkt efter korrektion: Id = {correctedProduct.Id}, Navn = {correctedProduct.Name}, Pris = {correctedProduct.Price}\n");


    // Hent og vis korrektionshistorik for kunde
    PrintCorrectionHistory(context, customer, "kunde");

    var correctedCustomer = customerService.GetEntityWithCorrections(customer.Id);
    Console.WriteLine($"Kunde efter korrektion: Id = {correctedCustomer.Id}, Navn = {correctedCustomer.Name}, Email = {correctedCustomer.Email}, Fødselsdato = {correctedCustomer.DateOfBirth}\n");

}

static void PrintCorrectionHistory<TEntity>(DataContext context, TEntity entity, string entityName)
        where TEntity : class, IEntity
{
    Console.WriteLine($"Korrektionshistorik for {entityName}:");
    var corrections = context.Set<EntityCorrection<TEntity>>()
        .Include(ec => ec.PropertyCorrections)
        .Where(ec => ec.OriginalEntityId == entity.Id)
        .OrderBy(ec => ec.CorrectionDate)
        .ToList();

    if (!corrections.Any())
    {
        Console.WriteLine($"  Ingen korrektioner fundet for {entityName} med Id {entity.Id}");
        return;
    }

    foreach (var correction in corrections)
    {
        Console.WriteLine($"  Korrektion dato: {correction.CorrectionDate}");
        foreach (var propCorrection in correction.PropertyCorrections)
        {
            Console.WriteLine($"    {propCorrection.PropertyName}: {propCorrection.OldValue} -> {propCorrection.NewValue}");
        }
    }
    Console.WriteLine(); // Tilføj en tom linje for bedre læsbarhed
}