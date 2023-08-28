using Bogus;
using Xunit;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Xunit.Sdk;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();

        Action action  = () => DomainValidation.NotNull(value, "Value");

        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;

        Action action = () => DomainValidation.NotNull(value, "FieldName");

        action.Should().Throw<EntityValidationException>().WithMessage("FieldName should not be null");
    }


    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenNullOrEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenNullOrEmpty(string? target)
    {
        Action action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().Throw<EntityValidationException>().WithMessage("FieldName should not be empty or null");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        string target = Faker.Commerce.ProductName();

        Action action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().NotThrow();
    }

    [Theory(DisplayName =nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        Action action = () => DomainValidation.MinLength(target, minLength, "FieldName");

        action.Should().Throw<EntityValidationException>().WithMessage($"FieldName should be at leats {minLength} characters long");
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 10)]
    public void MinLengthOk(string target, int minLength)
    {
        Action action = () => DomainValidation.MinLength(target, minLength, "FieldName");

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
    {
        var Faker = new Faker();

        for(int i  = 0; i < numberOfTests; i++)
        {
            var example = Faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 5);

            yield return new object[] { example, minLength };
        }
    } 
    
    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
    {
        var Faker = new Faker();

        for(int i  = 0; i < numberOfTests; i++)
        {
            var example = Faker.Commerce.ProductName();
            var minLength = example.Length - (new Random()).Next(1, 5);

            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        Action action = () => DomainValidation.MaxLength(target, maxLength, "FieldName");

        action.Should().Throw<EntityValidationException>().WithMessage($"FieldName should be less or equal {maxLength} characters long");
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    public void MaxLengthOk(string target, int maxLength)
    {
        Action action = () => DomainValidation.MaxLength(target, maxLength, "FieldName");

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
    {
        var Faker = new Faker();

        for (int i = 0; i < numberOfTests; i++)
        {
            var example = Faker.Commerce.ProductName();
            var maxLength = example.Length - (new Random()).Next(1, 5);

            yield return new object[] { example, maxLength };
        }
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
    {
        var Faker = new Faker();

        for (int i = 0; i < numberOfTests; i++)
        {
            var example = Faker.Commerce.ProductName();
            var maxLength = example.Length + (new Random()).Next(0, 5);

            yield return new object[] { example, maxLength };
        }
    }
}
