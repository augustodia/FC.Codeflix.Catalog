using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true) : base()
    {
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

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;

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

        DomainValidation.NotNullOrEmpty(Name, fieldName);
        DomainValidation.MinLength(Name, 3, fieldName);
        DomainValidation.MaxLength(Name, 255, fieldName);
    }

    private void validateDescription()
    {
        string fieldName = nameof(Description);

        DomainValidation.NotNull(Description, fieldName);
        DomainValidation.MaxLength(Description, 10_000, fieldName);
    }
}
