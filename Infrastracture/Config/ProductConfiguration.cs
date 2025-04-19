using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastracture.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    //Manually declaring the properties of each field
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}
