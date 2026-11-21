// Prints the expected rows of SimdQuickSortReferenceTraceTests from Bramas's sort512.hpp (https://github.com/berenger-eu/avx-512-sort, MIT).
// Each row partitions one generated int array with the reference's Partition512 and records the boundary it returns and an FNV-1a hash
// of the array afterwards. Keep the input generator and the case lists in step with that test.
//
// Build on a CPU with AVX-512F, in a folder holding a copy of sort512.hpp:
//
//   g++ -O2 -mavx512f -mavx512cd -I. -o trace <path to this file> && ./trace

#include <cstdint>
#include <cstdio>
#include <vector>
#include "sort512.hpp"

static uint64_t rng;

static inline uint64_t next_random()
{
    uint64_t z = (rng += 0x9E3779B97F4A7C15ULL);
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ULL;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBULL;
    return z ^ (z >> 31);
}

static void generate(int shape, int n, std::vector<int>& a)
{
    a.resize(n);
    for (int i = 0; i < n; i++)
    {
        switch (shape)
        {
            case 0: a[i] = (int)(next_random() % 2000001) - 1000000; break;
            case 1: a[i] = i; break;
            case 2: a[i] = n - i; break;
            default: a[i] = (int)(next_random() % 4); break;
        }
    }
}

int main()
{
    const int sizes[] = { 32, 33, 40, 47, 48, 49, 63, 64, 65, 100, 255, 256, 257, 1000, 4096, 10007 };
    for (int shape = 0; shape < 4; shape++)
    {
        for (int n : sizes)
        {
            rng = (uint64_t)shape * 1000003 + (uint64_t)n;
            std::vector<int> a;
            generate(shape, n, a);
            const int pivot = a[n / 3];
            const size_t bound = Sort512::Partition512<size_t>(a.data(), 0, (size_t)n - 1, pivot);
            uint64_t hash = 14695981039346656037ULL;
            for (int i = 0; i < n; i++)
            {
                hash = (hash ^ (uint32_t)a[i]) * 0x100000001b3ULL;
            }
            std::printf("        (%d, %d, %zu, 0x%016llXUL),\n", shape, n, bound, (unsigned long long)hash);
        }
    }
    return 0;
}
