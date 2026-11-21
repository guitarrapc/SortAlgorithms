// Prints the expected rows of Ips4oReferenceTraceTests from the sequential ips4o. Keep the input generator, the shapes and the sizes
// in step with that test.
//
// Build in a folder that holds the include directory of https://github.com/ips4o/ips4o at 08a5b926ee65cef19139057c6bde02bb5542c1cb
// (BSD 2-Clause License). The reference seeds its generator from std::random_device and maps draws with
// std::uniform_int_distribution, whose mapping differs between standard libraries. Ips4oSort starts the same generator from a seed and
// maps a draw by the high 64 bits of its product with the range, so both lines are patched to do the same:
//
//   find include -name '*.hpp' -exec sed -i 's/\r$//' {} +
//   sed -i 's|random_generator.seed(seed);|random_generator.seed(IPS4O_TRACE_SEED);|' include/ips4o/memory.hpp
//   perl -0pi -e 's|std::uniform_int_distribution<\s*typename std::iterator_traits<It>::difference_type>\(0, --n\)\(gen\)|(typename std::iterator_traits<It>::difference_type)(((unsigned __int128)gen() * (unsigned __int128)(uint64_t)(--n + 1)) >> 64)|' include/ips4o/sampling.hpp
//   mkdir -p stub/tbb && echo 'namespace tbb { template <class T> class concurrent_queue {}; }' > stub/tbb/concurrent_queue.h
//   g++ -O2 -std=c++17 -w -Iinclude -Istub -o trace <path to this file> && ./trace
//
// Without -pthread the library takes its sequential path. scheduler.hpp includes a TBB header even then, but only its parallel
// scheduler uses it, so an empty stub stands in for TBB. Work in a directory on the WSL file system rather than under /mnt/c or
// /mnt/d, where copies have come out with NUL bytes.
//
// Each element carries its key above KEY_SHIFT and its position below, so the hash tells equal keys apart. Each comparison folds the
// smaller and then the larger of the two identities into the hash, so the hash records which pairs were compared and in what order,
// but not which operand came first.

#include <algorithm>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <vector>

static uint64_t trace_count, trace_hash;
static const uint64_t FNV_PRIME = 0x100000001b3ULL;
static const uint64_t FNV_OFFSET = 14695981039346656037ULL;
static const int KEY_SHIFT = 24;

static inline void trace(int64_t a, int64_t b)
{
    trace_count++;
    int64_t lo = a < b ? a : b, hi = a < b ? b : a;
    trace_hash = (trace_hash ^ (uint64_t)lo) * FNV_PRIME;
    trace_hash = (trace_hash ^ (uint64_t)hi) * FNV_PRIME;
}

// An 8-byte element: blocks of 256.
struct LessId
{
    bool operator()(int64_t a, int64_t b) const { trace(a, b); return (a >> KEY_SHIFT) < (b >> KEY_SHIFT); }
};

// A 16-byte element: blocks of 128.
struct Pair
{
    int64_t key;
    int64_t id;
};

struct LessPair
{
    bool operator()(const Pair& a, const Pair& b) const { trace(a.id, b.id); return a.key < b.key; }
};

#define IPS4O_TRACE_SEED 0x9E3779B97F4A7C15ULL
#include "ips4o.hpp"

static uint64_t rng;

static inline uint64_t next_random()
{
    uint64_t z = (rng += 0x9E3779B97F4A7C15ULL);
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ULL;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBULL;
    return z ^ (z >> 31);
}

static inline int64_t below(int64_t m) { return (int64_t)(next_random() % (uint64_t)m); }

// Keys stay below 2^39 in magnitude, so a key and a position fit in one 64-bit identity.
static void fill(std::vector<int64_t>& a, int shape, int p)
{
    int n = (int)a.size();
    rng = 1000003ULL + (uint64_t)shape * 257ULL + (uint64_t)p * 65537ULL + (uint64_t)n;
    switch (shape)
    {
        case 0: for (int i = 0; i < n; i++) a[i] = (int64_t)(next_random() >> 26); break;
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

static const int shapes[][2] = { {0, 0}, {1, 1}, {1, 16}, {2, 0}, {3, 0}, {4, 0}, {5, 50}, {6, 200}, {7, 0}, {8, 1 << 20} };
static const int sizes[] = { 2, 3, 32, 33, 100, 256, 257, 300, 1000, 3000, 4096, 4097, 10007, 65536, 100000, 1100000 };

static void row_id()
{
    for (int n : sizes)
    {
        trace_count = 0;
        trace_hash = FNV_OFFSET;
        for (auto& shape : shapes)
        {
            std::vector<int64_t> keys(n);
            fill(keys, shape[0], shape[1]);
            std::vector<int64_t> a(n);
            for (int i = 0; i < n; i++) a[i] = keys[i] * ((int64_t)1 << KEY_SHIFT) + i;
            ips4o::sort(a.begin(), a.end(), LessId{});
            for (int i = 1; i < n; i++)
                if ((a[i] >> KEY_SHIFT) < (a[i - 1] >> KEY_SHIFT)) { std::printf("not sorted: Int64 n=%d shape=%d\n", n, shape[0]); std::exit(1); }
        }
        std::printf("        (Element.Int64, %d, %lluUL, 0x%016llXUL),\n", n, (unsigned long long)trace_count, (unsigned long long)trace_hash);
    }
}

static void row_pair()
{
    for (int n : sizes)
    {
        trace_count = 0;
        trace_hash = FNV_OFFSET;
        for (auto& shape : shapes)
        {
            std::vector<int64_t> keys(n);
            fill(keys, shape[0], shape[1]);
            std::vector<Pair> a(n);
            for (int i = 0; i < n; i++) a[i] = Pair{ keys[i], keys[i] * ((int64_t)1 << KEY_SHIFT) + i };
            ips4o::sort(a.begin(), a.end(), LessPair{});
            for (int i = 1; i < n; i++)
                if (a[i].key < a[i - 1].key) { std::printf("not sorted: Pair n=%d shape=%d\n", n, shape[0]); std::exit(1); }
        }
        std::printf("        (Element.Pair, %d, %lluUL, 0x%016llXUL),\n", n, (unsigned long long)trace_count, (unsigned long long)trace_hash);
    }
}

int main()
{
    row_id();
    row_pair();
}
