using FluentAssertions;

namespace TalkRailwayProgramming.MaitreD.Railway;

public class MaitreDShould
{
    [Fact]
    public async Task RegisterReservationWithoutName()
    {
        var fakeRepository = new FakeRepository();
        var sut = new MaitreD(fakeRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, new None<string>());
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Ok<Unit, Errors>(new Unit()));
        var expectedReservation = new Reservation(DateTime.Today, "mark@example.com", 5, "");
        fakeRepository.CreatedReservation.Should().Be(expectedReservation);
    }
    
    [Fact]
    public async Task RegisterReservationWithName()
    {
        var fakeRepository = new FakeRepository();
        var sut = new MaitreD(fakeRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Ok<Unit, Errors>(new Unit()));
        var expectedReservation = new Reservation(DateTime.Today, "mark@example.com", 5, "Mark");
        fakeRepository.CreatedReservation.Should().Be(expectedReservation);
    }
    
    [Fact]
    public async Task FailWhenNotEnoughSeats()
    {
        var fakeRepository = new FakeRepository(
            new Reservation(DateTime.Today, "bob@example.com", 2, "Bob"),
            new Reservation(DateTime.Today, "alice@example.com", 6, "Alice"));
        var sut = new MaitreD(fakeRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 4, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Error<Unit, Errors>(Errors.NotEnoughSeats));
    }
    
    [Fact]
    public async Task FailWhenEmailIsMissing()
    {
        var fakeRepository = new FakeRepository();
        var sut = new MaitreD(fakeRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "", 4, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Error<Unit, Errors>(Errors.InvalidEmail));
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task FailWhenQuantityIsLowerThan1(int quantity)
    {
        var fakeRepository = new FakeRepository();
        var sut = new MaitreD(fakeRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", quantity, new Some<string>("Mark"));
        var result = await sut.RegisterReservation(command);

        result.Should().Be(new Error<Unit, Errors>(Errors.InvalidQuantity));
    }
}

public class FakeRepository : IRepository
{
    private readonly IReadOnlyCollection<Reservation> _reservations;
    public FakeRepository(params Reservation[] reservations) => _reservations = reservations;

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