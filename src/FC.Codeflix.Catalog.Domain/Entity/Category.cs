﻿using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.Now;
        IsActive = isActive;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;

        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;

        Validate();
    }

    private void Validate()
    {
        validateName();
        validateDescription();
    }


    private void validateName()
    {
        string fieldName = nameof(Name);
        bool nameIsNullOrWhiteSpace = String.IsNullOrWhiteSpace(Name);

        if (nameIsNullOrWhiteSpace) throw new EntityValidationException($"{fieldName} should not be empty or null");
        if (Name.Length < 3) throw new EntityValidationException($"{fieldName} should be at leats 3 characters long");
        if (Name.Length > 255) throw new EntityValidationException($"{fieldName} should be less or equal 255 characters long");
    }

    private void validateDescription()
    {
        string fieldName = nameof(Description);

        if (Description == null) throw new EntityValidationException($"{fieldName} should not be empty or null");
        if (Description.Length > 10_000) throw new EntityValidationException($"{fieldName} should be less or equal 10.000 characters long");
    }
}
