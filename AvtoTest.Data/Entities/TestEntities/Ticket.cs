namespace AvtoTest.Data.Entities.TestEntities;

public class Ticket
{
    public byte Id { get; set; }
    public ushort StartIndex => (ushort)((Id - 1) * 20 + 1);
    public ushort EndIndex => (ushort)(Id * 20);

    //  (id-1) * 20 + 1 to id * 20
    // id = 3; (3-1) * 20 + 1;
    // id = 3; 3 * 20;

}