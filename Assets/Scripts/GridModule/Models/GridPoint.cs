using UnityEngine;

namespace GridModule.Models
{
    public struct GridPoint
    {
        public int X;
        public int Y;
        
        public override string ToString()
        {
            return $"(x: {X}, y: {Y})";
        }

        public Vector2 ToVector()
        {
            return new Vector2(X, Y);
        }
    }
}