// Prints the expected rows of MergeReferenceTraceTests from the C++ sources of powersort, timsort and ShiftSort.
// Keep the input generator, the shapes and the sizes in step with that test.
//
// Build in a folder that holds these files at the paths shown:
//   src/algorithms.h, src/sorts/powersort.h, src/sorts/merging.h, src/sorts/insertionsort.h
//     from https://github.com/sebawild/powersort at 1d078b6be9023e134c4f8f6de88e2406dc681e89 (MIT)
//   ShiftSort.hpp, the file C++/ShiftSort.hpp
//     from https://github.com/JamesQuintero/ShiftSort at b9681f612f6e367540a8066e331341db466b3455 (MIT)
//   timsort.hpp, the file include/gfx/timsort.hpp
//     from https://github.com/timsort/cpp-TimSort at tag v2.1.0, 60d53f54f9ecf2547a25a056e4c8ea61b9a2124f (MIT)
//
// gfx::timsort needs two changes to be the TimSort that TimSort.cs implements. binarySort finds the insertion point with
// std::upper_bound, whose probes differ from the halving loop of CPython's binarysort, and the merges end with
// minGallop_ = min(minGallop, 1), which resets the shared galloping threshold to 1 instead of keeping it at least 1:
//
//   sed -i 's/\r$//' timsort.hpp
//   sed -i 's|iter_t const pos = std::upper_bound(lo, start, pivot, compare);|iter_t l = lo, r = start; do { iter_t p = l + ((r - l) >> 1); if (compare(pivot, *p)) r = p; else l = p + 1; } while (l < r); iter_t const pos = l;|' timsort.hpp
//   sed -i 's|minGallop_ = (std::min)(minGallop, 1);|minGallop_ = minGallop < 1 ? 1 : minGallop;|' timsort.hpp
//   g++ -O2 -std=c++20 -w -I. -o trace <path to this file> && ./trace
//
// powersort is used as published, with merging_methods::COPY_SMALLER, the merge PowerSort.cs implements.
// ShiftSort.hpp is used as published. It defines sort, split and merge at global scope, so it is included inside a namespace.

#include <cmath>
#include <cstdint>
#include <cstdio>
#include <limits>
#include <vector>

static uint64_t trace_count, trace_hash;
static const uint64_t FNV_PRIME = 0x100000001b3ULL;
static const uint64_t FNV_OFFSET = 14695981039346656037ULL;

// Each comparison folds the smaller and then the larger of its two values into the hash, so the hash records which
// pairs were compared and in what order, but not which operand came first.
static inline void trace(int32_t a, int32_t b)
{
    trace_count++;
    int32_t lo = a < b ? a : b, hi = a < b ? b : a;
    trace_hash = (trace_hash ^ (uint64_t)(uint32_t)lo) * FNV_PRIME;
    trace_hash = (trace_hash ^ (uint64_t)(uint32_t)hi) * FNV_PRIME;
}

struct E
{
    int32_t v;
    bool operator<(const E& o) const { trace(v, o.v); return v < o.v; }
    bool operator<=(const E& o) const { trace(v, o.v); return v <= o.v; }
    bool operator>(const E& o) const { trace(v, o.v); return v > o.v; }
};

// powersort.h instantiates every merging method, including the sentinel one, which asks for numeric limits.
template<> struct std::numeric_limits<E>
{
    static constexpr bool is_specialized = true;
    static constexpr bool has_infinity = false;
    static E max() { return E{0x7fffffff}; }
    static E infinity() { return max(); }
};

#include "src/sorts/powersort.h"
#include "timsort.hpp"
#include <iostream>
#include <iterator>
namespace shiftsort
{
#include "ShiftSort.hpp"
}

static uint64_t rng;

static inline uint64_t next_random()
{
    uint64_t z = (rng += 0x9E3779B97F4A7C15ULL);
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ULL;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBULL;
    return z ^ (z >> 31);
}

static inline int32_t below(int64_t m) { return (int32_t)(next_random() % (uint64_t)m); }

static void fill(std::vector<int32_t>& a, int shape, int p)
{
    int n = (int)a.size();
    rng = 1000003ULL + (uint64_t)shape * 257ULL + (uint64_t)p * 65537ULL + (uint64_t)n;
    switch (shape)
    {
        case 0: for (int i = 0; i < n; i++) a[i] = below(1LL << 31); break;
        case 1: for (int i = 0; i < n; i++) a[i] = below(p); break;
        case 2: for (int i = 0; i < n; i++) a[i] = i; break;
        case 3: for (int i = 0; i < n; i++) a[i] = n - i; break;
        case 4: for (int i = 0; i < n; i++) a[i] = i < n - i ? i : n - i; break;
        case 5: for (int i = 0; i < n; i++) a[i] = i % p; break;
        case 6:
            for (int i = 0, run = 0; i < n; run++)
            {
                int len = 1 + below(p);
                int base = below(1 << 30);
                for (int k = 0; k < len && i < n; k++, i++) a[i] = (run & 1) ? base - 3 * k : base + 3 * k;
            }
            break;
        default:
            for (int i = 0; i < n; i++) a[i] = i;
            for (int k = 0; k < n / 100 + 1; k++) { int x = below(n), y = below(n); int t = a[x]; a[x] = a[y]; a[y] = t; }
            break;
    }
}

static const int shapes[][2] = { {0, 0}, {1, 16}, {2, 0}, {3, 0}, {4, 0}, {5, 50}, {6, 200}, {7, 0} };
static const int sizes[] = { 2, 5, 20, 23, 24, 25, 31, 32, 33, 63, 64, 65, 100, 1000, 10000, 100000 };

int main()
{
    for (int sorter = 0; sorter < 3; sorter++)
    {
        for (int n : sizes)
        {
            trace_count = 0;
            trace_hash = FNV_OFFSET;
            for (auto& shape : shapes)
            {
                std::vector<int32_t> input(n);
                fill(input, shape[0], shape[1]);
                std::vector<E> a(n);
                for (int i = 0; i < n; i++) a[i].v = input[i];
                if (sorter == 0)
                {
                    algorithms::powersort<std::vector<E>::iterator, 24, algorithms::merging_methods::COPY_SMALLER> ps;
                    ps.sort(a.begin(), a.end());
                }
                else if (sorter == 1)
                {
                    gfx::timsort(input.begin(), input.end(), [](int32_t x, int32_t y) { trace(x, y); return x < y; });
                }
                else
                {
                    shiftsort::sort(a.begin(), n);
                }
            }
            std::printf("        (Sorter.%s, %d, %lluUL, 0x%016llXUL),\n", sorter == 0 ? "Power" : sorter == 1 ? "Tim" : "Shift", n,
                        (unsigned long long)trace_count, (unsigned long long)trace_hash);
        }
    }
}
