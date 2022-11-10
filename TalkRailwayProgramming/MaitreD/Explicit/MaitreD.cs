namespace TalkRailwayProgramming.MaitreD.Explicit;
#nullable enable

public class MaitreD
{
    private const int MaxCapacity = 10;
    private readonly IRepository _repository;
    public MaitreD(IRepository repository) => _repository = repository;

    public async Task<Result<Unit, Errors>> RegisterReservation(RegisterReservationCommand command)
    {
        var reservationResult = CreateReservation(command.At, command.Email, command.Quantity, command.Name);
        if (reservationResult is Error<Reservation, Errors> reservationError) return new Error<Unit, Errors>(reservationError.Value);
        var reservation = ((Ok<Reservation, Errors>)reservationResult).Value;
        
        var reservations = await _repository.ReadReservations(command.At);
        
        var validationResult = EnsureEnoughSeatsAvailable(reservations, reservation);
        if (validationResult is Error<Reservation, Errors> validationError) return new Error<Unit, Errors>(validationError.Value);
        var validatedReservation = ((Ok<Reservation, Errors>)validationResult).Value;
        
        await _repository.Create(validatedReservation);
        
        return new Ok<Unit, Errors>(new Unit());
    }

    private static Result<Reservation, Errors> CreateReservation(DateTime at, string email, int quantity, Option<string> name)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new Error<Reservation, Errors>(Errors.InvalidEmail);
        if (quantity < 1)
            return new Error<Reservation, Errors>(Errors.InvalidQuantity);

        var nameValue = name.Match(
            some => some,
            () => string.Empty);
        
        var reservation = new Reservation(at, email, quantity, nameValue);
        return new Ok<Reservation, Errors>(reservation);
    }

    private static Result<Reservation, Errors> EnsureEnoughSeatsAvailable(IReadOnlyCollection<Reservation> reservations, Reservation reservation)
    {
        var reservedSeats = reservations.Sum(r => r.Quantity);
        if (MaxCapacity < reservedSeats + reservation.Quantity)
            return new Error<Reservation, Errors>(Errors.NotEnoughSeats);
        return new Ok<Reservation, Errors>(reservation);
    }
}

public enum Errors { NotEnoughSeats, InvalidQuantity, InvalidEmail }

public sealed record RegisterReservationCommand(DateTime At, string Email, int Quantity, Option<string> Name);
public sealed record Reservation(DateTime At, string Email, int Quantity, string Name);

public interface IRepository
{
    Task<IReadOnlyCollection<Reservation>> ReadReservations(DateTime date);
    Task Create(Reservation reservation);
}