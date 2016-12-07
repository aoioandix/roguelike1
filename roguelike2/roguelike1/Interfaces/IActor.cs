using RLNET;
using RogueSharp;


namespace roguelike1.Interfaces
{
    public interface IActor
    {
        string Name { get; set; }
        int Awareness { get; set; }
    }
}
