namespace AirportAPI.Enums
{
    [Flags]
    public enum FlightStatus
    {
        None = 0,
        Landing = 1 << 0,  // 1
        Arrival = 1 << 1,  // 2
        Departure = 1 << 2// 4
    }
}
