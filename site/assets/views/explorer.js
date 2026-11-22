import { d3, PATTERNS, fmtT, fmtX, esc, cvBadge, allocBadge, refChip, selectEl, caseKey, intro, defaultLeague, SCALING, homeOf } from "../lib.js";

// Every case of one (category, size, pattern) slice, sortable. "All categories" shows the whole size league.
// On phones and tablets the table rows become two-line cards and a select replaces the sortable headers.
export function render(root, { run, params, setParams }) {
  const cat = params.c && (params.c === "*" || run.categories.includes(params.c)) ? params.c : (run.categories.includes("Partition") ? "Partition" : run.categories[0]);
  const sizes = run.allSizes;
  const size = sizes.includes(+params.n) ? +params.n : cat === "*" ? defaultLeague(run) : run.maxSize(cat);
  const pattern = PATTERNS.includes(params.p) ? params.p : "Random";
  root.innerHTML = `
    <section class="panel">
      <h2>Explorer</h2>
      ${intro(`Every algorithm for one category, size and input pattern, fastest first. The ratio is the time relative to ${esc(run.reference)} (.NET Span.Sort): below ×1 is faster.`,
        "Mean is the time per sort; the bar shows it on a log scale with its 99.9% confidence interval (Error). CV = StdDev / Mean is the run-to-run variation: ● below 2%, ▲ below 5%, ■ 5% or more. Allocated is managed bytes per sort.",
        `The ratio divides by the reference row of the table: the category's own ${esc(run.reference)}, or the one in Partition for <i>All categories</i>, which ranks every algorithm measured at that size. Rank excludes the reference row. Select a method to open its history.`)}
      <div class="filters">
        ${selectEl("x-c", "Category", [["*", "All categories"], ...run.categories.map(c => [c, c])], cat)}
        ${selectEl("x-n", "Size", sizes.map(n => [n, "n = " + n]), size)}
        ${selectEl("x-p", "Pattern", PATTERNS, pattern)}
        ${selectEl("x-s", "Sort by", [["mean", "Mean"], ["cv", "CV"], ["m", "Name"]], "mean", "md-only")}
        <label for="x-q">Filter by name<input id="x-q" type="search" placeholder="e.g. Heap"></label>
      </div>
      <div class="tbox" id="x-t"></div>
    </section>`;
  const f = root.querySelector(".filters");
  let sortKey = "mean", dir = 1;
  const draw = () => {
    const c = f.querySelector("#x-c").value, n = +f.querySelector("#x-n").value, p = f.querySelector("#x-p").value;
    const q = f.querySelector("#x-q").value.trim().toLowerCase();
    setParams({ c, n, p });
    const host = root.querySelector("#x-t");
    let list = run.slice(c, n, p);
    // All categories keep one reference row: Partition's, or the one of the class that measures this size.
    if (c === "*") { const refs = list.filter(r => r.ref), keep = refs.find(r => r.c === "Partition") ?? refs[0]; list = list.filter(r => !r.ref || r === keep); }
    // The ratio divides by the reference row; a name filter can hide that row, so it is found first.
    const ref = list.find(r => r.ref);
    const catOf = r => r.c === SCALING && !r.ref ? homeOf(run, r.m) : r.c;
    list = list.filter(r => !q || r.m.toLowerCase().includes(q));
    if (!list.length) {
      host.innerHTML = `<p class="empty">Not measured. ${c !== "*" ? `${esc(c)} is measured at n = ${run.sizesOf(c).join(", ")}.` : ""}</p>`;
      return;
    }
    const ranked = list.filter(r => !r.ref).sort((a, b) => a.mean - b.mean);
    const rank = new Map(ranked.map((r, i) => [r, i + 1]));
    // Older imported runs lack the reference at some sizes; those tables fall back to the fastest row.
    const base = ref ? ref.mean : ranked.length ? ranked[0].mean : d3.min(list, r => r.mean);
    list.sort((a, b) => dir * (sortKey === "m" ? a.m.localeCompare(b.m) : a[sortKey] - b[sortKey]));
    const x = d3.scaleLog().domain([d3.min(list, r => Math.max(1e-3, r.mean - r.err)) * 0.92, d3.max(list, r => r.mean + r.err) * 1.05]).range([0, 100]).clamp(true);
    const arrow = k => sortKey === k ? (dir > 0 ? " ↑" : " ↓") : "";
    let h = `<table class="st cards"><thead><tr><th>#</th><th data-sort="m">Method${arrow("m")}</th>${c === "*" ? "<th>Category</th>" : ""}<th data-sort="mean">Mean${arrow("mean")}</th><th>Mean ± Error (log)</th><th data-sort="cv">CV${arrow("cv")}</th><th>Allocated</th><th>${ref ? "vs " + esc(run.reference) : "vs best"}</th></tr></thead><tbody>`;
    for (const r of list) {
      const e0 = x(r.mean - r.err), e1 = x(r.mean + r.err);
      const hist = `#history?${new URLSearchParams({ k: caseKey(r.c, r.m, r.n, r.p) })}`;
      h += `<tr class="${r.ref ? "ref" : ""}"><td class="num rk">${r.ref ? "" : rank.get(r)}</td><td class="nm"><a href="${hist}">${esc(r.m)}</a>${refChip(r)}</td>${c === "*" ? `<td class="ct"><span class="cat">${esc(catOf(r))}</span></td>` : ""}<td class="num mn">${fmtT(r.mean)}</td>
        <td class="br">${c === "*" ? `<span class="cat md-only">${esc(catOf(r))}</span>` : ""}<div class="xbar${r.ref ? " ref" : ""}" role="img" aria-label="${fmtT(r.mean)} ± ${fmtT(r.err)}"><b style="width:${x(r.mean).toFixed(1)}%"></b><i style="left:${e0.toFixed(1)}%;width:${Math.max(0.5, e1 - e0).toFixed(1)}%"></i></div></td>
        <td class="cv">${cvBadge(r.cv)}</td><td class="al">${allocBadge(r.alloc)}</td><td class="num vb">${fmtX(r.mean / base)}</td></tr>`;
    }
    host.innerHTML = h + "</tbody></table>";
    host.querySelectorAll("th[data-sort]").forEach(th => th.addEventListener("click", () => {
      const k = th.dataset.sort;
      if (k === sortKey) dir = -dir; else { sortKey = k; dir = 1; }
      f.querySelector("#x-s").value = sortKey;
      draw();
    }));
  };
  f.addEventListener("input", e => {
    if (e.target.id === "x-s") { sortKey = e.target.value; dir = 1; }
    draw();
  });
  draw();
}
