namespace TalkRailwayProgramming.MaitreD.Initial;
#nullable disable

public class MaitreD
{
    private const int MaxCapacity = 10;
    private readonly IRepository _repository;
    public MaitreD(IRepository repository) => _repository = repository;

    public async Task RegisterReservation(RegisterReservationCommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        var reservation = CreateReservation(command.At, command.Email, command.Quantity, command.Name);

        var reservations = await _repository.ReadReservations(command.At);
        
        EnsureEnoughSeatsAvailable(reservations, command.Quantity);

        await _repository.Create(reservation);
    }

    private static Reservation CreateReservation(DateTime at, string email, int quantity, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException($"Invalid {nameof(email)} value");
        if (quantity < 1)
            throw new ArgumentException($"Invalid {nameof(quantity)} value");

        return new Reservation(at, email, quantity, name ?? "");
    }

    private static void EnsureEnoughSeatsAvailable(IReadOnlyCollection<Reservation> reservations, int quantity)
    {
        var reservedSeats = reservations.Sum(r => r.Quantity);
        if (MaxCapacity < reservedSeats + quantity)
            throw new InvalidOperationException("Not enough seats available");
    }
}

public sealed record RegisterReservationCommand(DateTime At, string Email, int Quantity, string Name);
public sealed record Reservation(DateTime At, string Email, int Quantity, string Name);

public interface IRepository
{
    Task<IReadOnlyCollection<Reservation>> ReadReservations(DateTime date);
    Task Create(Reservation reservation);
}