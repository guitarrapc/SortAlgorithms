global using System;
global using System.Collections.Generic;
global using System.Linq;

// Skip zero-initialization of locals and stackalloc buffers.
// All stackalloc sites that need zero values call .Clear()/.Fill() explicitly.
[module: System.Runtime.CompilerServices.SkipLocalsInit]
