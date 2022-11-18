using FluentAssertions;

namespace TalkRailwayProgramming.Railway;

public class MaitreDShould
{
    [Fact]
    public async Task RegisterReservationWithoutName()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, new None<string>());
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Ok<Unit, Errors>(new Unit()));
        var expectedReservation = new Reservation(DateTime.Today, "mark@example.com", 5, "");
        spyRepository.CreatedReservation.Should().Be(expectedReservation);
    }
    
    [Fact]
    public async Task RegisterReservationWithName()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Ok<Unit, Errors>(new Unit()));
        var expectedReservation = new Reservation(DateTime.Today, "mark@example.com", 5, "Mark");
        spyRepository.CreatedReservation.Should().Be(expectedReservation);
    }
    
    [Fact]
    public async Task FailWhenNotEnoughSeats()
    {
        var spyRepository = new SpyRepository(
            new Reservation(DateTime.Today, "bob@example.com", 2, "Bob"),
            new Reservation(DateTime.Today, "alice@example.com", 6, "Alice"));
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 4, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        var expected = new Error<Unit, Errors>(Errors.NotEnoughSeats);
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task FailWhenEmailIsMissing()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "", 4, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        var expected = new Error<Unit, Errors>(Errors.InvalidEmail);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task FailWhenQuantityIsLowerThan1(int quantity)
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", quantity, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        var expected = new Error<Unit, Errors>(Errors.InvalidQuantity);
        result.Should().Be(expected);
    }
}

public class SpyRepository : IRepository
{
    private readonly IReadOnlyCollection<Reservation> _reservations;
    public SpyRepository(params Reservation[] reservations) => _reservations = reservations;

    public Reservation? CreatedReservation { get; private set; }

    public Task<IReadOnlyCollection<Reservation>> ReadReservations(DateTime date)
    {
        return Task.FromResult(_reservations);
    }

    public Task Create(Reservation reservation)
    {
        CreatedReservation = reservation;
        return Task.CompletedTask;
    }
}