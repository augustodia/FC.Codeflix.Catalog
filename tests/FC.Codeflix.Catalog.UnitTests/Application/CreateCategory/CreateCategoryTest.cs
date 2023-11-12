using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategoryTest - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        var input = _fixture.GetInput();
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify((repository) => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify((unitOfWork) => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategoryTest - Use Cases")]
    public async void CreateCategoryWithOnlyName()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        var input = new CreateCategoryInput(_fixture.GetValidCategoryName());
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify((repository) => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify((unitOfWork) => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Application", "CreateCategoryTest - Use Cases")]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
            );
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify((repository) => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify((unitOfWork) => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateAggreagate))]
    [Trait("Application", "CreateCategoryTest - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void ThrowWhenCantInstantiateAggreagate(
        CreateCategoryInput input, 
        string exceptionMessage
    )
    {

        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);

    }

    private static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();

        //Name not be small
        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        invalidInputList.Add(new object[] { invalidInputShortName, "Name should be at leats 3 characters long" });

        //Name not be long
        var invalidInputLongName = fixture.GetInput();
        while (invalidInputLongName.Name.Length <= 255)
        {
            invalidInputLongName.Name = $"{invalidInputLongName.Name} {fixture.Faker.Commerce.ProductName()}";
        }

        invalidInputList.Add(new object[] { invalidInputLongName, "Name should be less or equal 255 characters long" });
        
        // Description not be null
        var invalidInputDescriptionNull = fixture.GetInput();
        invalidInputDescriptionNull.Description = null;
        invalidInputList.Add(new object[] { invalidInputDescriptionNull, "Description should not be null" });

        //Description not be long
        var invalidInputTooLongDescription = fixture.GetInput();
        while (invalidInputTooLongDescription.Description.Length <= 10_000)
        {
            invalidInputTooLongDescription.Description = $"{invalidInputTooLongDescription.Description} {fixture.Faker.Commerce.ProductDescription()}";
        }

        invalidInputList.Add(new object[] { invalidInputTooLongDescription, "Description should be less or equal 10000 characters long" });

        return invalidInputList;
    }
}
