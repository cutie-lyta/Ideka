using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Random
{
    ulong seed;
    ulong key;

    uint seed_32bit;
    uint key_32bit;

    public Random(ulong seed, ulong key = 42)
    {
        this.seed = seed;
        this.key = key;

        this.seed_32bit = (uint)seed;
        this.key_32bit = (uint)key;
    }

    // Algorithme, variant 64 bit, basé sur l'algorithme SquareRNG : https://arxiv.org/pdf/2004.06278.pdf
    public ulong NextUInt64()
    {
        ulong t, x, y, z;

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
        uint x, y, z;

        // Init -> Premiere modification
        y = x = seed_32bit * key_32bit;
        z = y + key_32bit;

        // Passe 1
        x = x * x + y; 
        x = (x >> 32) | (x << 32);

        // Passe 2
        x = x * x + z; 
        x = (x >> 32) | (x << 32);

        // Passe 3
        x = x * x + y; 
        x = (x >> 32) | (x << 32);

        // Passe Final
        seed_32bit = (x * x + z) >> 32; 
        return seed_32bit;
    }

    // Int64
    public Int64 NextInt64()
    {
        return Math.Abs((Int64)NextUInt64());
    }

    // Int
    public int NextInt()
    {
        return Math.Abs((int)NextUInt());
    }

    // Bool
    public bool NextBool()
    {
        return (NextUInt64() % 2) == 0;
    }

    // Todo -> Implement Float, and Double

    // Todo -> 16bit and 8bit-ify that algorithm
}