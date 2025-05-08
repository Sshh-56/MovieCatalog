using NUnit.Framework;
using Moq;
using Business.BusinessLogic;
using Data.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Data;

[TestFixture]
public class ActorBusinessTests
{
   
    
    private Mock<DbSet<Actor>> _mockActors;
    private Mock<MovieCatalogDbContext> _mockContext;
    private ActorBusiness _actorBusiness;

    [SetUp]
    public void Setup()
    {
        _mockActors = new Mock<DbSet<Actor>>();
        _mockContext = new Mock<MovieCatalogDbContext>();

        var actors = new List<Actor>
        {
            new Actor{ Id = 1, FullName = "Leonardo DiCaprio", /*new DateTime(1974, 11, 11),*/ Nationality = "American" }
        }.AsQueryable();

        _mockActors.As<IQueryable<Actor>>().Setup(m => m.Provider).Returns(actors.Provider);
        _mockActors.As<IQueryable<Actor>>().Setup(m => m.Expression).Returns(actors.Expression);
        _mockActors.As<IQueryable<Actor>>().Setup(m => m.ElementType).Returns(actors.ElementType);
        _mockActors.As<IQueryable<Actor>>().Setup(m => m.GetEnumerator()).Returns(actors.GetEnumerator());

        _mockContext.Setup(c => c.Actors).Returns(_mockActors.Object);

        _actorBusiness = new ActorBusiness(_mockContext.Object);
    }

    [Test]
    public void AddActor_Should_AddActorSuccessfully()
    {
        var actor = new Actor{ Id = 2, FullName = "Brad Pitt", Nationality = "American" };

        _actorBusiness.AddActor(actor);

        _mockActors.Verify(a => a.Add(It.IsAny<Actor>()), Times.Once);
    }

    [Test]
    public void DeleteActor_Should_RemoveActorSuccessfully()
    {
        var actor = new Actor{ Id = 1, FullName = "Leonardo DiCaprio", Nationality = "American" };

        var actors = new List<Actor> { actor }.AsQueryable(); // ✅ Convert list to IQueryable

        var mockSet = new Mock<DbSet<Actor>>();
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Provider).Returns(actors.Provider);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Expression).Returns(actors.Expression);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.ElementType).Returns(actors.ElementType);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.GetEnumerator()).Returns(actors.GetEnumerator());

        _mockContext.Setup(c => c.Actors).Returns(mockSet.Object);

        _actorBusiness.DeleteActor("Leonardo DiCaprio");

        mockSet.Verify(m => m.Remove(It.Is<Actor>(x => x.FullName == "Leonardo DiCaprio")), Times.Once);
    }

    [Test]
    public void UpdateActor_Should_UpdateActorDetails()
    {
        var actor = new Actor{ Id = 1, FullName = "Leonardo DiCaprio", Nationality = "American" };

        var actors = new List<Actor> { actor }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Actor>>();
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Provider).Returns(actors.Provider);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Expression).Returns(actors.Expression);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.ElementType).Returns(actors.ElementType);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.GetEnumerator()).Returns(actors.GetEnumerator());

        _mockContext.Setup(c => c.Actors).Returns(mockSet.Object);

        var updatedActor = new Actor{ Id = 1, FullName = "Leonardo DiCaprio", Nationality = "Canadian" };

        _actorBusiness.UpdateActor("Leonardo DiCaprio", updatedActor);

        //Assert.That(actor.Birthdate, Is.EqualTo(new DateTime(1974, 11, 12)));
        Assert.That(actor.Nationality, Is.EqualTo("Canadian"));
    }

}

