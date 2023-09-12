using Dirigera.Lib.Api.Dto;

namespace Dirigera.Lib.Devices
{
    public class Room
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }

        public Room(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
