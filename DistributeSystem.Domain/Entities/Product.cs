﻿using DistributeSystem.Domain.Abstractions.Aggregates;
using DistributeSystem.Domain.Abstractions.Entities;
using static DistributeSystem.Domain.Exceptions.ProductException;

namespace DistributeSystem.Domain.Entities;

public class Product : AggregateRoot<Guid>, IAuditableEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }

    public static Product CreateProduct(Guid id, string name, decimal price, string description)
    {
        var product = new Product(id, name, price, description);

        product.RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductCreated(Guid.NewGuid(), product.Id,
            product.Name, product.Price,
            product.Description
            ));

        return product;
    }

    public Product(Guid id, string name, decimal price, string description)
    {
        if (name.Length > 10)
            throw new ProductFieldException(nameof(Name));

        Id = id;
        Name = name;
        Price = price;
        Description = description;
    }

    public void Update(string name, decimal price, string description)
    {
        Name = name;
        Price = price;
        Description = description;

        RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductUpdated(Guid.NewGuid(), Id, name, price, description));
    }

    public void Delete()
        => RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductDeleted(Guid.NewGuid(), Id));
}
