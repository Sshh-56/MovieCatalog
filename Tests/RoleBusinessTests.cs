using NUnit.Framework;
using Moq;
using Business.BusinessLogic;
using Data.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.EntityFrameworkCore.Metadata;

[TestFixture]
public class RoleBusinessTests
{
    private Mock<DbSet<Role>> _mockRoles;
    private Mock<DbSet<Movie>> _mockMovies;
    private Mock<DbSet<Actor>> _mockActors;
    private Mock<MovieCatalogDbContext> _mockContext;
    private RoleBusiness _roleBusiness;

    [SetUp]
    public void Setup()
    {
        _mockRoles = new Mock<DbSet<Role>>();
        _mockMovies = new Mock<DbSet<Movie>>();
        _mockActors = new Mock<DbSet<Actor>>();
        _mockContext = new Mock<MovieCatalogDbContext>();

        _mockContext.Setup(c => c.Roles).Returns(_mockRoles.Object);
        _mockContext.Setup(c => c.Movies).Returns(_mockMovies.Object);
        _mockContext.Setup(c => c.Actors).Returns(_mockActors.Object);

        _roleBusiness = new RoleBusiness(_mockContext.Object);
    }

    [Test]
    public void AddRole_Should_AddRoleSuccessfully()
    {
        var roles = new List<Role>().AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        var role = new Role { Id = 1, MovieId = 1, ActorId = 1, CharacterName = "Batman" };

        _roleBusiness.AddRole(role);

        mockSet.Verify(r => r.Add(It.IsAny<Role>()), Times.Once);
    }

    [Test]
    public void AddRole_Should_ThrowException_When_DuplicateCharacterName()
    {
        var roles = new List<Role>
    {
        new Role{ Id = 1, MovieId = 1, ActorId = 1, CharacterName = "Iron Man" }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        var duplicateRole = new Role{ Id = 2, MovieId = 2, ActorId = 2, CharacterName = "Iron Man" };

        Assert.Throws<InvalidOperationException>(() => _roleBusiness.AddRole(duplicateRole));
    }

    [Test]
    public void AddRole_Should_ThrowException_When_Null()
    {
        Role? nullRole = null; 
        Assert.Throws<ArgumentNullException>(() => _roleBusiness.AddRole(nullRole!)); 
    }

    [Test]
    public void DeleteRole_Should_RemoveRoleSuccessfully()
    {
        var roles = new List<Role>
    {
        new Role{ Id = 1, MovieId = 1, ActorId = 1, CharacterName = "Batman" }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        _roleBusiness.DeleteRoleByCharacterName("Batman");

        mockSet.Verify(r => r.Remove(It.Is<Role>(r => r.CharacterName == "Batman")), Times.Once);
    }

    [Test]
    public void DeleteRole_Should_ReturnFalse_When_NotFound()
    {
        var roles = new List<Role>().AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        var result = _roleBusiness.DeleteRoleByCharacterName("NonExistent");

        Assert.That(result, Is.False);
    }

    [Test]
    public void GetRolesByActorName_Should_ReturnCorrectRoles()
    {
        var actors = new List<Actor>
    {
        new Actor{Id = 1, FullName = "Robert Downey Jr.", /*new DateTime(1965, 4, 4),*/ Nationality = "American" }
    }.AsQueryable(); 

        var mockActorSet = new Mock<DbSet<Actor>>();
        mockActorSet.As<IQueryable<Actor>>().Setup(m => m.Provider).Returns(actors.Provider);
        mockActorSet.As<IQueryable<Actor>>().Setup(m => m.Expression).Returns(actors.Expression);
        mockActorSet.As<IQueryable<Actor>>().Setup(m => m.ElementType).Returns(actors.ElementType);
        mockActorSet.As<IQueryable<Actor>>().Setup(m => m.GetEnumerator()).Returns(actors.GetEnumerator());

        _mockContext.Setup(c => c.Actors).Returns(mockActorSet.Object);

        var roles = new List<Role>
    {
        new Role{Id = 1, MovieId = 1, ActorId = actors.First().Id, CharacterName = "Iron Man" },
        new Role{ Id = 2, MovieId = 2, ActorId = actors.First().Id, CharacterName = "Sherlock Holmes" }
    }.AsQueryable(); 

        var mockRoleSet = new Mock<DbSet<Role>>();
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.Provider).Returns(roles.Provider);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.Expression).Returns(roles.Expression);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.ElementType).Returns(roles.ElementType);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockRoleSet.Object);

        var result = _roleBusiness.GetRolesByActorName("Robert Downey Jr.");

        Assert.That(result.Count, Is.EqualTo(2)); 
    }

    [Test]
    public void GetByCharacterName_Should_ReturnCorrectRoles()
    {
        var roles = new List<Role>
    {
        new Role{ Id = 1, MovieId = 1, ActorId = 1, CharacterName = "Iron Man" },
        new Role{ Id = 2, MovieId = 2, ActorId = 1, CharacterName = "Iron Man" },
        new Role{ Id = 3, MovieId = 3, ActorId = 2, CharacterName = "Sherlock Holmes" }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        var result = _roleBusiness.GetByCharacterName("Iron Man");

        Assert.That(result.Count, Is.EqualTo(2)); 
    }

    [Test]
    public void GetRolesByMovieTitle_Should_ReturnCorrectRoles()
    {
        var movies = new List<Movie>
    {
        new Movie{Id = 1, Title = "Inception", ReleaseYear = 2010, GenreId = 1 }
    }.AsQueryable(); 

        var mockMovieSet = new Mock<DbSet<Movie>>();
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(movies.Provider);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(movies.Expression);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(movies.ElementType);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(movies.GetEnumerator());

        _mockContext.Setup(c => c.Movies).Returns(mockMovieSet.Object);

        var roles = new List<Role>
    {
        new Role{Id = 1, MovieId = 1, ActorId = 1, CharacterName =  "Cobb" },
        new Role{ Id = 2, MovieId = 2, ActorId = 3, CharacterName = "Arthur" }
    }.AsQueryable(); 

        var mockRoleSet = new Mock<DbSet<Role>>();
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.Provider).Returns(roles.Provider);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.Expression).Returns(roles.Expression);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.ElementType).Returns(roles.ElementType);
        mockRoleSet.As<IQueryable<Role>>().Setup(r => r.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockRoleSet.Object);

        var result = _roleBusiness.GetRolesByMovieTitle("Inception");

        Assert.That(result.Exists(r => r.CharacterName == "Cobb"));
        Assert.That(result.Exists(r => r.CharacterName == "Arthur"));
    }

    [Test]
    public void GetRolesByMovieTitle_Should_ReturnEmptyList_WhenMovieNotFound()
    {
        var movies = new List<Movie>().AsQueryable(); 

        var mockSet = new Mock<DbSet<Movie>>();
        mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(movies.Provider);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(movies.Expression);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(movies.ElementType);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(movies.GetEnumerator());

        _mockContext.Setup(c => c.Movies).Returns(mockSet.Object);

        var result = _roleBusiness.GetRolesByMovieTitle("Nonexistent Movie");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetByCharacterName_Should_ReturnEmpty_When_NotFound()
    {
        var roles = new List<Role>().AsQueryable(); 

        var mockSet = new Mock<DbSet<Role>>();
        mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

        var result = _roleBusiness.GetByCharacterName("Unknown");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRolesByActorName_Should_ReturnEmpty_When_ActorNotFound()
    {
        var actors = new List<Actor>().AsQueryable();

        var mockSet = new Mock<DbSet<Actor>>();
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Provider).Returns(actors.Provider);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.Expression).Returns(actors.Expression);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.ElementType).Returns(actors.ElementType);
        mockSet.As<IQueryable<Actor>>().Setup(m => m.GetEnumerator()).Returns(actors.GetEnumerator());

        _mockContext.Setup(c => c.Actors).Returns(mockSet.Object);

        var result = _roleBusiness.GetRolesByActorName("Unknown Actor");

        Assert.That(result, Is.Empty);
    }
}

