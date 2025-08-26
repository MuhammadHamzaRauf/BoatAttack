using UnityEngine;

namespace BoatAttack.BoatInput
{
    /// <summary>
    /// Interface for boat input providers (desktop, mobile, AI)
    /// </summary>
    public interface IBoatInput
    {
        float Throttle { get; }   // -1..+1  (reverse/forward)
        float Steer { get; }      // -1..+1  (left/right)
        bool Brake { get; }       // true when braking
        bool Boost { get; }       // optional turbo/boost if present
        void Tick();              // call each Update() to refresh values
    }
}
