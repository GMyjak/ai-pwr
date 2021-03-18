using System;
using System.Collections.Generic;
using System.Text;

namespace Genetics
{
    abstract class UniversalRandom
    {
        public static int Seed { get; set; } = 2020;

        private static Random _rng;
        public static Random Rng
        {
            get => _rng ??= new Random(Seed);
            set => _rng = value;
        }
    }
}
