# Coding Style Guidelines

## Modern C# Features

- Use the latest C# syntax available (C# 13 or later)
- Use `using` declarations for automatic disposal
- Use file-scoped namespaces to reduce indentation
- Use collection literals where applicable
- Use pattern matching and switch expressions

## Naming Conventions

- Follow .NET coding guidelines for base conventions
- Use `PascalCase` for constant names
- Do NOT use `_` or `s_` prefixes for fields
- Omit the `private` modifier (it's the default)
- Prefer the use of `var` for local variables

## Unit Tests

- All sorting algorithms must have comprehensive unit tests
- Tests should cover edge cases (empty arrays, single elements, duplicates, etc.)
- Test file naming: `{AlgorithmName}Tests.cs`
- Use xUnit framework conventions

## Comments and Documentation

- Write comments based on observable facts, not general claims or assumptions.
- Avoid strong or definitive phrasing. Instead of "X guarantees Y" or "X achieves Y", use "X is intended to Y" or "X preserves the reference's Y-oriented structure".
- When referencing a runtime or JIT behavior (e.g., inlining, branchless codegen, ILP), describe what the code does structurally, not what the runtime will do. Example: instead of "runs simultaneously for ILP", write "preserves the reference's ILP-oriented interleaving (actual ILP depends on the JIT)".
- Performance claims (e.g., "zero allocation", "O(n) best case") are acceptable only when verified against the actual implementation.
- Avoid hedged generalizations like "generally faster" or "typically better". State the specific condition under which the behavior holds.
