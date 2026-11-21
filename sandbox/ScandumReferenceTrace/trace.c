// Prints the expected comparison traces of ScandumReferenceTraceTests from the C sources of quadsort and fluxsort 1.2.1.3,
// crumsort 1.2.1.3 and blitsort 1.2.1.2. Keep the input generator and the case lists in step with that test.
//
// Build in a folder holding copies of fluxsort.c, fluxsort.h, quadsort.c and quadsort.h from the fluxsort repository, crumsort.c and
// crumsort.h from the crumsort repository, and blitsort.c and blitsort.h from the blitsort repository; the three repositories carry
// the same quadsort 1.2.1.3. Fluxsort and crumsort start their pivot sample at an offset taken from a stack address. FluxSort starts
// it at div / 2 and CrumSort at the middle of the last stride, so the copies need the same changes:
//
//   sed -i 's/\r$//' *.c *.h
//   sed -i 's|pta = ptx + (size_t) &div / 16 % div;|pta = ptx + div / 2;|' fluxsort.c
//   sed -i 's|pta = array + nmemb - 1 - (size_t) &div / 64 % div;|pta = array + nmemb - 1 - div / 2;|' crumsort.c
//   gcc -O2 -w -I. -o trace <path to this file> && ./trace
//
// Work in a directory on the WSL file system rather than under /mnt/c, where sed -i once left NUL bytes in a source file.

#define CAPPED_N 4500000
#define CAPPED_SAW 2097152

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

static uint64_t trace_count, trace_hash;
#define FNV_PRIME 0x100000001b3ULL
#define FNV_OFFSET 14695981039346656037ULL

static inline int traced_greater(int64_t a, int64_t b)
{
	trace_count++;
	trace_hash = (trace_hash ^ (uint64_t) a) * FNV_PRIME;
	trace_hash = (trace_hash ^ (uint64_t) b) * FNV_PRIME;
	return (a >> 28) > (b >> 28);
}

#define cmp(a,b) traced_greater((int64_t) *(a), (int64_t) *(b))
#include "fluxsort.h"
#include "crumsort.h"
#include "blitsort.h"

static uint64_t rng;

static inline uint64_t next_random(void)
{
	uint64_t z = (rng += 0x9E3779B97F4A7C15ULL);
	z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ULL;
	z = (z ^ (z >> 27)) * 0x94D049BB133111EBULL;
	return z ^ (z >> 31);
}

static inline int64_t below(int64_t m)
{
	return (int64_t) (next_random() % (uint64_t) m);
}

static void generate(int shape, int64_t p, int64_t p2, int64_t n, int64_t *a)
{
	rng = 1000003ULL + (uint64_t) shape * 257ULL + (uint64_t) p * 65537ULL + (uint64_t) p2 * 17ULL + (uint64_t) n;

	int64_t half1 = n / 2, quad1 = half1 / 2, quad2 = half1 - quad1, half2 = n - half1, quad3 = half2 / 2, quad4 = half2 - quad3;
	int64_t start[4] = { 0, quad1, half1, half1 + quad3 }, length[4] = { quad1, quad2, quad3, quad4 };
	int64_t i, j, q;

	switch (shape)
	{
		case 0: for (i = 0; i < n; i++) a[i] = below(1LL << 32); break;
		case 1: for (i = 0; i < n; i++) a[i] = below(p); break;
		case 2: for (i = 0; i < n; i++) a[i] = i; break;
		case 3: for (i = 0; i < n; i++) a[i] = n - i; break;
		case 4: for (i = 0; i < n; i++) a[i] = (n - i) / p; break;
		case 5: for (i = 0; i < n; i++) a[i] = 7; break;
		case 6:
		{
			int64_t base = 16 * n;
			for (q = 0; q < 4; q++)
			{
				int mode = (int) ((p >> (2 * q)) & 3);
				for (j = 0; j < length[q]; j++)
				{
					int64_t v = mode == 0 ? below(n) : mode == 1 ? j : mode == 2 ? length[q] - j : below(4);
					a[start[q] + j] = base + v;
				}
				base += ((p2 >> q) & 1) ? -2 * n : 2 * n;
			}
			break;
		}
		case 7: for (i = 0; i < n; i++) a[i] = i < n * p / 100 ? below(n / 10 + 1) : n + below(n); break;
		case 8: for (i = 0; i < n; i++) a[i] = i % p; break;
		case 9: for (i = 0; i < n; i++) a[i] = p - i % p; break;
		case 10: for (i = 0; i < n; i++) a[i] = i < n - i ? i : n - i; break;
		case 11:
			for (i = 0; i < n; i++) a[i] = i;
			for (i = 0; i < n / p; i++) { int64_t x = below(n), y = below(n), t = a[x]; a[x] = a[y]; a[y] = t; }
			break;
		case 12: for (i = 0; i < n; i++) a[i] = i < n - n / p ? i : below(n); break;
		case 13: for (i = 0; i < n; i++) a[i] = i < n / p ? below(n) : i; break;
		case 14: for (i = 0; i < n; i++) a[i] = below(100) < p ? n / 2 : below(n); break;
		case 15: for (q = 0; q < 4; q++) for (j = 0; j < length[q]; j++) a[start[q] + j] = j < length[q] * p / 100 ? j : below(n); break;
		case 16: for (q = 0; q < 4; q++) for (j = 0; j < length[q]; j++) a[start[q] + j] = j < length[q] * p / 100 ? length[q] - j : below(n); break;
		case 17: for (i = 0; i < n; i++) a[i] = i < n / 2 ? 5 : below(p); break;
		case 18: { int64_t b = 0; for (i = 0; i < n; i++) { if (i % p == 0) b = below(n); a[i] = b + i % p; } break; }
		case 19: for (i = 0; i < n; i++) a[i] = (i & 1) ? n + i : i; break;
		case 20: for (i = 0; i < n; i++) { int64_t b = i / p; a[i] = b * p * 2 + ((b & 1) ? p - i % p : i % p); } break;
		case 21: for (i = 0; i < n; i++) { int64_t b = i / p; a[i] = (n / p - b + 1) * p * 2 + ((b & 1) ? p - i % p : i % p); } break;
		case 22: for (i = 0; i < n; i++) a[i] = i < 8 ? i - 1000 : -((i - 7) / 2); break;
		case 23: for (i = 0; i < n; i++) a[i] = i < 8 ? 1000 - i : -((i - 7) / 2); break;
		case 24: for (i = 0; i < n; i++) a[i] = -((i + 1) / 2); break;
		case 25: { int64_t b = start[p]; for (i = 0; i < n; i++) a[i] = n - i + (i >= b ? 1 : 0); break; }
		case 26: { int64_t z = n / 9 ? n / 9 : 1; for (i = 0; i < n; i++) a[i] = i % z == 0 ? 100 + i / z : below(100) < p ? below(100) : 1000 + below(n); break; }
		case 27: { int64_t z = n / 9 ? n / 9 : 1; for (i = 0; i < n; i++) a[i] = i % z == 0 || below(100) < p ? n : below(n); break; }
		case 28: { int64_t z = n / 9 ? n / 9 : 1, s = n / p ? n / p : 1; for (i = 0; i < n; i++) a[i] = i % z == 0 ? 500 : (i + 7) % s == 0 ? below(400) : 10000 + below(n); break; }
		case 29: { int64_t z = n / 9 ? n / 9 : 1, s = n / p ? n / p : 1; for (i = 0; i < n; i++) a[i] = i % z == 0 ? 500 : (i + 7) % s == 0 ? 10000 + below(n) : below(400); break; }
	}
	for (i = 0; i < n; i++) a[i] = (a[i] << 28) | i;
}

typedef struct { int shape; int64_t p; } shape_t;

static const shape_t all_shapes[] =
{
	{0,0},{1,2},{1,4},{1,16},{1,100},{1,1000},{2,0},{3,0},{4,2},{4,3},{4,16},{5,0},{7,10},{7,30},{7,50},{7,90},
	{8,7},{8,32},{8,64},{8,128},{8,256},{8,1000},{9,7},{9,32},{9,64},{9,128},{9,256},{9,1000},{10,0},{11,4},{11,16},{11,100},
	{12,2},{12,4},{12,10},{13,2},{13,4},{13,10},{14,50},{14,90},{14,99},{15,25},{15,45},{15,50},{15,55},{15,75},
	{16,25},{16,45},{16,50},{16,55},{16,75},{17,2},{17,6},{18,33},{18,100},{18,500},{19,0},{20,8},{20,64},{20,1024},
	{21,8},{21,64},{21,1024},{22,0},{23,0},{24,0},{25,1},{25,2},{25,3},{26,2},{27,4},{28,51},{29,60},
};

static const shape_t core_shapes[] =
{
	{0,0},{1,4},{1,1000},{7,30},{8,1024},{11,100},{12,4},{14,90},{15,50},{15,60},{16,50},{20,64},
};

static const shape_t capped_shapes[] = { {8,CAPPED_SAW}, };

enum { PLAN_CORE, PLAN_ALL, PLAN_ALL_SOME_QUARTERS, PLAN_ALL_EVERY_QUARTER, PLAN_CAPPED };
enum { SORT_QUAD, SORT_FLUX, SORT_CRUM, SORT_BLIT };

typedef struct { int sort; int64_t first, last, step, swap; int plan; } row_t;

static const row_t rows[] =
{
	{SORT_QUAD,2,128,1,0,PLAN_ALL},{SORT_QUAD,129,1100,29,0,PLAN_ALL},{SORT_QUAD,2047,2049,1,0,PLAN_ALL},{SORT_QUAD,4095,4097,1,0,PLAN_ALL},
	{SORT_QUAD,8192,8192,1,0,PLAN_ALL},{SORT_QUAD,20000,20000,1,0,PLAN_CORE},
	{SORT_QUAD,97,1097,100,32,PLAN_ALL},{SORT_QUAD,97,1097,100,64,PLAN_ALL},{SORT_QUAD,97,1097,100,100,PLAN_ALL},{SORT_QUAD,97,1097,100,512,PLAN_ALL},
	{SORT_QUAD,5000,5000,1,32,PLAN_CORE},{SORT_QUAD,5000,5000,1,64,PLAN_CORE},{SORT_QUAD,5000,5000,1,512,PLAN_ALL},{SORT_QUAD,20000,20000,1,2048,PLAN_CORE},{SORT_QUAD,1536,1536,1,512,PLAN_CORE},
	{SORT_QUAD,300000,300000,1,70000,PLAN_CORE},{SORT_QUAD,CAPPED_N,CAPPED_N,1,0,PLAN_CAPPED},
	{SORT_FLUX,2,132,13,0,PLAN_ALL},{SORT_FLUX,133,140,1,0,PLAN_ALL_EVERY_QUARTER},{SORT_FLUX,256,264,1,0,PLAN_ALL_SOME_QUARTERS},
	{SORT_FLUX,380,392,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_FLUX,510,524,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_FLUX,1000,1000,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_FLUX,2000,2000,1,0,PLAN_ALL},
	{SORT_FLUX,2044,2052,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_FLUX,4095,4097,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_FLUX,8190,8200,1,0,PLAN_ALL_SOME_QUARTERS},
	{SORT_FLUX,20000,20000,1,0,PLAN_CORE},{SORT_FLUX,32768,32769,1,0,PLAN_CORE},{SORT_FLUX,100000,100000,1,0,PLAN_CORE},
	{SORT_CRUM,2,256,17,0,PLAN_ALL},{SORT_CRUM,257,264,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_CRUM,500,520,4,0,PLAN_ALL_SOME_QUARTERS},
	{SORT_CRUM,1000,1000,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_CRUM,2044,2052,4,0,PLAN_ALL_SOME_QUARTERS},{SORT_CRUM,4095,4097,1,0,PLAN_ALL},
	{SORT_CRUM,8190,8200,5,0,PLAN_ALL_SOME_QUARTERS},{SORT_CRUM,20000,20000,1,0,PLAN_CORE},{SORT_CRUM,100000,100000,1,0,PLAN_CORE},
	{SORT_CRUM,300000,300000,1,0,PLAN_CORE},
	{SORT_BLIT,2,132,13,0,PLAN_ALL},{SORT_BLIT,133,140,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_BLIT,500,524,4,0,PLAN_ALL_SOME_QUARTERS},
	{SORT_BLIT,1000,1000,1,0,PLAN_ALL_SOME_QUARTERS},{SORT_BLIT,2044,2052,4,0,PLAN_ALL_SOME_QUARTERS},{SORT_BLIT,4095,4097,1,0,PLAN_ALL},
	{SORT_BLIT,8190,8200,5,0,PLAN_ALL_SOME_QUARTERS},{SORT_BLIT,20000,20000,1,0,PLAN_CORE},{SORT_BLIT,100000,100000,1,0,PLAN_CORE},
	{SORT_BLIT,300000,300000,1,0,PLAN_CORE},
};

#define COUNT_OF(x) (sizeof(x) / sizeof((x)[0]))

static int64_t *input, *swap_memory;
static int64_t capacity;

static void run(const row_t *row, int shape, int64_t p, int64_t p2, int64_t n)
{
	if (n > capacity)
	{
		free(input);
		free(swap_memory);
		input = malloc(n * sizeof(int64_t));
		swap_memory = malloc(n * sizeof(int64_t));
		capacity = n;
	}
	generate(shape, p, p2, n, input);

	if (row->sort == SORT_FLUX) fluxsort_int64(input, n, NULL);
	else if (row->sort == SORT_CRUM) crumsort_int64(input, n, NULL);
	else if (row->sort == SORT_BLIT) blitsort_int64(input, n, NULL);
	else if (row->swap == 0) quadsort_int64(input, n, NULL);
	else quadsort_swap_int64(input, swap_memory, row->swap, n, NULL);

	for (int64_t i = 1; i < n; i++)
	{
		// Crumsort is unstable, so only its keys must be in order; the others must also keep equal keys in input order.
		if (row->sort == SORT_CRUM ? (input[i - 1] >> 28) > (input[i] >> 28) : input[i - 1] >= input[i]) { fprintf(stderr, "unsorted: shape %d p %lld n %lld\n", shape, (long long) p, (long long) n); exit(1); }
	}
}

static void run_shapes(const row_t *row, const shape_t *shapes, size_t count, int64_t n)
{
	for (size_t i = 0; i < count; i++) run(row, shapes[i].shape, shapes[i].p, 0, n);
}

int main(void)
{
	for (size_t r = 0; r < COUNT_OF(rows); r++)
	{
		const row_t *row = &rows[r];
		trace_count = 0;
		trace_hash = FNV_OFFSET;

		for (int64_t n = row->first; n <= row->last; n += row->step)
		{
			switch (row->plan)
			{
				case PLAN_CORE: run_shapes(row, core_shapes, COUNT_OF(core_shapes), n); break;
				case PLAN_CAPPED: run_shapes(row, capped_shapes, COUNT_OF(capped_shapes), n); break;
				default: run_shapes(row, all_shapes, COUNT_OF(all_shapes), n); break;
			}
			if (row->plan == PLAN_ALL_SOME_QUARTERS)
			{
				for (int m = 0; m < 256; m += 17) for (int b = 0; b <= 5; b += 5) run(row, 6, m, b, n);
			}
			else if (row->plan == PLAN_ALL_EVERY_QUARTER)
			{
				for (int m = 0; m < 256; m++) for (int b = 0; b < 8; b++) run(row, 6, m, b, n);
			}
		}
		printf("        (%s, %lld, %lld, %lld, %lld, Plan.%s, %lluUL, 0x%016llXUL),\n",
			row->sort == SORT_QUAD ? "Sorter.Quad" : row->sort == SORT_FLUX ? "Sorter.Flux" : row->sort == SORT_CRUM ? "Sorter.Crum" : "Sorter.Blit", (long long) row->first, (long long) row->last, (long long) row->step, (long long) row->swap,
			row->plan == PLAN_CORE ? "Core" : row->plan == PLAN_ALL ? "All" : row->plan == PLAN_ALL_SOME_QUARTERS ? "AllSomeQuarters" : row->plan == PLAN_ALL_EVERY_QUARTER ? "AllEveryQuarter" : "Capped",
			(unsigned long long) trace_count, (unsigned long long) trace_hash);
	}
	return 0;
}
