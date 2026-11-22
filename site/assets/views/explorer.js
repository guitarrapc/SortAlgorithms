import { d3, PATTERNS, fmtT, fmtX, esc, cvBadge, allocBadge, refChip, selectEl, caseKey } from "../lib.js";

// Every case of one (category, size, pattern) slice, sortable. "All categories" shows the whole size league.
export function render(root, { run, params, setParams }) {
  const cat = params.c && (params.c === "*" || run.categories.includes(params.c)) ? params.c : (run.categories.includes("Partition") ? "Partition" : run.categories[0]);
  const sizes = run.allSizes;
  const size = sizes.includes(+params.n) ? +params.n : sizes.at(-1);
  const pattern = PATTERNS.includes(params.p) ? params.p : "Random";
  root.innerHTML = `
    <section class="panel">
      <h2>Explorer</h2>
      <p class="lead">Mean time per sort with its 99.9% confidence interval (Error), run-to-run variation (CV = StdDev / Mean) and managed allocation. Choose <i>All categories</i> to rank every algorithm measured at that size.</p>
      <div class="filters">
        ${selectEl("x-c", "Category", [["*", "All categories"], ...run.categories.map(c => [c, c])], cat)}
        ${selectEl("x-n", "Size", sizes.map(n => [n, "n = " + n]), size)}
        ${selectEl("x-p", "Pattern", PATTERNS, pattern)}
        <label for="x-q">Filter by name<input id="x-q" type="search" placeholder="e.g. Heap"></label>
      </div>
      <div class="vscroll" id="x-t"></div>
      <p class="note">Rank and "vs best" exclude the reference row. Click a method to open its history.</p>
    </section>`;
  const f = root.querySelector(".filters");
  let sortKey = "mean", dir = 1;
  const draw = () => {
    const c = f.querySelector("#x-c").value, n = +f.querySelector("#x-n").value, p = f.querySelector("#x-p").value;
    const q = f.querySelector("#x-q").value.trim().toLowerCase();
    setParams({ c, n, p });
    const host = root.querySelector("#x-t");
    let list = run.slice(c, n, p).filter(r => !q || r.m.toLowerCase().includes(q));
    if (c === "*") list = list.filter(r => !r.ref || r.c === "Partition" || !run.methodsOf("Partition").includes(run.reference));
    if (!list.length) {
      host.innerHTML = `<p class="empty">Not measured. ${c !== "*" ? `${esc(c)} is measured at n = ${run.sizesOf(c).join(", ")}.` : ""}</p>`;
      return;
    }
    const ranked = list.filter(r => !r.ref).sort((a, b) => a.mean - b.mean);
    const rank = new Map(ranked.map((r, i) => [r, i + 1]));
    const best = ranked.length ? ranked[0].mean : d3.min(list, r => r.mean);
    list.sort((a, b) => dir * (sortKey === "m" ? a.m.localeCompare(b.m) : a[sortKey] - b[sortKey]));
    const x = d3.scaleLog().domain([d3.min(list, r => Math.max(1e-3, r.mean - r.err)) * 0.92, d3.max(list, r => r.mean + r.err) * 1.05]).range([0, 200]);
    const arrow = k => sortKey === k ? (dir > 0 ? " ↑" : " ↓") : "";
    let h = `<table class="st"><thead><tr><th>#</th><th data-sort="m">Method${arrow("m")}</th>${c === "*" ? "<th>Category</th>" : ""}<th data-sort="mean">Mean${arrow("mean")}</th><th>Mean ± Error (log)</th><th data-sort="cv">CV${arrow("cv")}</th><th>Allocated</th><th>vs best</th></tr></thead><tbody>`;
    for (const r of list) {
      const e0 = x(Math.max(x.domain()[0], r.mean - r.err)), e1 = x(r.mean + r.err);
      const hist = `#history?${new URLSearchParams({ k: caseKey(r.c, r.m, r.n, r.p) })}`;
      h += `<tr class="${r.ref ? "ref" : ""}"><td class="num">${r.ref ? "" : rank.get(r)}</td><td><a href="${hist}">${esc(r.m)}</a>${refChip(r)}</td>${c === "*" ? `<td><span class="cat">${esc(r.c)}</span></td>` : ""}<td class="num">${fmtT(r.mean)}</td>
        <td><svg width="210" height="14" viewBox="0 0 210 14" role="img" aria-label="${fmtT(r.mean)} ± ${fmtT(r.err)}" style="display:block"><rect x="0" y="3" width="${x(r.mean)}" height="8" rx="2" fill="${r.ref ? "var(--v-axis)" : "var(--s1)"}" fill-opacity=".85"/><line x1="${e0}" x2="${e1}" y1="7" y2="7" stroke="var(--v-ink)" stroke-width="1.5"/><line x1="${e1}" x2="${e1}" y1="3" y2="11" stroke="var(--v-ink)" stroke-width="1.5"/></svg></td>
        <td>${cvBadge(r.cv)}</td><td>${allocBadge(r.alloc)}</td><td class="num">${fmtX(r.mean / best)}</td></tr>`;
    }
    host.innerHTML = h + "</tbody></table>";
    host.querySelectorAll("th[data-sort]").forEach(th => th.addEventListener("click", () => {
      const k = th.dataset.sort;
      if (k === sortKey) dir = -dir; else { sortKey = k; dir = 1; }
      draw();
    }));
  };
  f.addEventListener("input", draw);
  draw();
}
