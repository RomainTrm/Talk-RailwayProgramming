using FluentAssertions;

namespace TalkRailwayProgramming.Initial;

public class MaitreDShould
{
    [Fact]
    public async Task RegisterReservationWithoutName()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, null);
        await sut.RegisterReservation(command);

        var expectedReservation = new Reservation(DateTime.Today, "mark@example.com", 5, "");
        spyRepository.CreatedReservation.Should().Be(expectedReservation);
    }
    
    [Fact]
    public async Task RegisterReservationWithName()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 5, "Mark");
        await sut.RegisterReservation(command);

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

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", 4, "Mark");
        var exception = await Record.ExceptionAsync(() => sut.RegisterReservation(command));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>()
            .Which.Message.Should().Be("Not enough seats available");
    }
    
    [Fact]
    public async Task FailWhenEmailIsMissing()
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "", 4, "Mark");
        var exception = await Record.ExceptionAsync(() => sut.RegisterReservation(command));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentException>()
            .Which.Message.Should().Be("Invalid email value");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task FailWhenQuantityIsLowerThan1(int quantity)
    {
        var spyRepository = new SpyRepository();
        var sut = new MaitreD(spyRepository);

        var command = new RegisterReservationCommand(DateTime.Today, "mark@example.com", quantity, "Mark");
        var exception = await Record.ExceptionAsync(() => sut.RegisterReservation(command));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentException>()
            .Which.Message.Should().Be("Invalid quantity value");
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

    public Task Register(Reservation reservation)
    {
        CreatedReservation = reservation;
        return Task.CompletedTask;
    }
}