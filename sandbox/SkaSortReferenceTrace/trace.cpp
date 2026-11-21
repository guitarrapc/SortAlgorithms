// Prints the expected rows of SkaSortReferenceTraceTests from ska_sort.hpp. Keep the input generator, the shapes and the sizes in step
// with that test.
//
// Build in a folder that holds ska_sort.hpp from https://github.com/skarupke/ska_sort at 2c14d4bf7b667032ed28a481065f721385e33849
// (Boost Software License 1.0), with its two element exchanges and its std::sort fallback routed through the hooks below. The
// exchange that reorders the list of unfinished buckets stays as it is:
//
//   sed -i 's/\r$//' ska_sort.hpp
//   sed -i 's|std::iter_swap(it, begin + offset);|trace_swap(\&*it, \&*(begin + offset));|' ska_sort.hpp
//   sed -i 's|    std::sort(begin, end, \[&\](auto && l, auto && r){ return extract_key(l) < extract_key(r); });|    trace_std_sort(\&*begin, end - begin);\n&|' ska_sort.hpp
//   g++ -O2 -std=c++17 -w -I. -o trace <path to this file> && ./trace
//
// Work in a directory on the WSL file system rather than under /mnt/c or /mnt/d, where copies have come out with NUL bytes.
//
// The trace records every exchange of the radix levels and every range handed to std::sort, in order. It does not record what
// std::sort does inside a range: the standard library's std::sort is not the one SkaSort calls, but both leave equal integers in
// the same places, so the levels that follow see the same input either way.

#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <vector>

static uint64_t swap_count, std_sort_count, trace_hash;
static const void* trace_base;
static const uint64_t FNV_PRIME = 0x100000001b3ULL;
static const uint64_t FNV_OFFSET = 14695981039346656037ULL;

static inline void fold(uint64_t v) { trace_hash = (trace_hash ^ v) * FNV_PRIME; }

template<typename T>
inline void trace_swap(T* a, T* b)
{
    swap_count++;
    fold(1);
    fold((uint64_t)(a - (const T*)trace_base));
    fold((uint64_t)(b - (const T*)trace_base));
    std::iter_swap(a, b);
}

template<typename T>
inline void trace_std_sort(T* first, std::ptrdiff_t n)
{
    std_sort_count++;
    fold(2);
    fold((uint64_t)(first - (const T*)trace_base));
    fold((uint64_t)n);
}

#include "ska_sort.hpp"

static uint64_t rng;

static inline uint64_t next_random()
{
    uint64_t z = (rng += 0x9E3779B97F4A7C15ULL);
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ULL;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBULL;
    return z ^ (z >> 31);
}

static inline int64_t below(int64_t m) { return (int64_t)(next_random() % (uint64_t)m); }

// Values are made as 64-bit integers and truncated to the element type.
static void fill(std::vector<int64_t>& a, int shape, int p)
{
    int n = (int)a.size();
    rng = 1000003ULL + (uint64_t)shape * 257ULL + (uint64_t)p * 65537ULL + (uint64_t)n;
    switch (shape)
    {
        case 0: for (int i = 0; i < n; i++) a[i] = (int64_t)next_random(); break;
        case 1: for (int i = 0; i < n; i++) a[i] = below(p); break;
        case 2: for (int i = 0; i < n; i++) a[i] = i; break;
        case 3: for (int i = 0; i < n; i++) a[i] = n - i; break;
        case 4: for (int i = 0; i < n; i++) a[i] = i < n - i ? i : n - i; break;
        case 5: for (int i = 0; i < n; i++) a[i] = i % p; break;
        case 6:
            for (int i = 0, run = 0; i < n; run++)
            {
                int len = 1 + (int)below(p);
                int64_t base = below(1 << 30);
                for (int k = 0; k < len && i < n; k++, i++) a[i] = (run & 1) ? base - 3 * k : base + 3 * k;
            }
            break;
        case 7:
            for (int i = 0; i < n; i++) a[i] = i;
            for (int k = 0; k < n / 100 + 1; k++) { int x = (int)below(n), y = (int)below(n); int64_t t = a[x]; a[x] = a[y]; a[y] = t; }
            break;
        default: for (int i = 0; i < n; i++) a[i] = below(p) - p / 2; break;
    }
}

static const int shapes[][2] = { {0, 0}, {1, 16}, {2, 0}, {3, 0}, {4, 0}, {5, 50}, {6, 200}, {7, 0}, {8, 1 << 20} };
static const int sizes[] = { 2, 5, 6, 23, 24, 100, 127, 128, 129, 200, 1000, 1023, 1024, 1025, 3000, 10000, 100000 };

template<typename T>
static void row(const char* name)
{
    for (int n : sizes)
    {
        swap_count = 0;
        std_sort_count = 0;
        trace_hash = FNV_OFFSET;
        for (auto& shape : shapes)
        {
            std::vector<int64_t> values(n);
            fill(values, shape[0], shape[1]);
            std::vector<T> a(n);
            for (int i = 0; i < n; i++) a[i] = (T)values[i];
            trace_base = a.data();
            ska_sort(a.begin(), a.end());
            if (!std::is_sorted(a.begin(), a.end())) { std::printf("not sorted: %s n=%d shape=%d\n", name, n, shape[0]); std::exit(1); }
        }
        std::printf("        (Key.%s, %d, %lluUL, %lluUL, 0x%016llXUL),\n", name, n,
                    (unsigned long long)swap_count, (unsigned long long)std_sort_count, (unsigned long long)trace_hash);
    }
}

int main()
{
    row<int8_t>("SByte");
    row<int16_t>("Int16");
    row<int32_t>("Int32");
    row<int64_t>("Int64");
}
