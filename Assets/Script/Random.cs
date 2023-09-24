using System;
using System.Runtime.ConstrainedExecution;

public class Random
{
    UInt64 seed;
    UInt64 key;

    public Random(UInt64 seed, UInt64 key = 42)
    {
        this.seed = seed;
        this.key = key;
    }
    // Algorithme, variant 64 bit, basé sur l'algorithme SquareRNG : https://arxiv.org/pdf/2004.06278.pdf
    public UInt64 NextUInt64()
    {
        UInt64 t, x, y, z;

        // Init -> Premiere modification
        y = x = seed * key;
        z = y * key;

        // Passe 1
        x = x * x + y;
        x = (x >> 32) | (x << 32);

        // Passe 2
        x = x * x + z;
        x = (x >> 32) | (x << 32);

        // Passe 3
        x = x * x + y;
        x = (x >> 32) | (x << 32);

        // Passe 4
        t = x = x * x + z;
        x = (x >> 32) | (x << 32);

        // Passe Final
        seed = t ^ ((x * x + y) >> 32);
        return seed;
    }

    // Algorithme, variant 32 bit, basé sur l'algorithme SquareRNG : https://arxiv.org/pdf/2004.06278.pdf
    public uint NextUInt()
    {
        UInt64 x, y, z;

        // Init -> Premiere modification
        y = x = seed * key;
        z = y + key;

        // Passe 1
        x = x * x + y;
        x = (x >> 32) | (x << 32);

        // Passe 2
        x = x * x + z;
        x = (x >> 32) | (x << 32);

        // Passe 3
        x = x * x + y;
        x = (x >> 32) | (x << 32);

        // Passe 4
        seed = (x * x + z) >> 32;
        return (uint)seed;
    }

    // Int64
    public Int64 NextInt64()
    {
        return (Int64)NextUInt64();
    }

    // Int
    public int NextInt()
    {
        return (int)NextUInt();
    }

    // Bool
    public bool NextBool()
    {
        return (NextUInt64() % 2) == 0;
    }

    // Todo -> Implement Float, and Double

    // Todo -> 16bit and 8bit-ify that algorithm
}