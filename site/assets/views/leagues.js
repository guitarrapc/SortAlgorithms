import { d3, PATTERNS, PATTERN_SHORT, fmtX, esc, logTicks, geo, anchorBase, anchorFactor, pct, cvBadge, leagueSizes as sizesForLeagues, intro } from "../lib.js";

// Overall ranking per size. Each league holds only algorithms measured at that size, so
// O(n log n) and linear sorts are never ranked against each other at a size too small to separate them.
// Rows are HTML so they reflow to any width; selecting a row shows its ratio for every pattern.
export function render(root, { run, params, setParams }) {
  const leagueSizes = sizesForLeagues(run);
  let n = leagueSizes.includes(+params.n) ? +params.n : leagueSizes[0];
  const base = anchorBase(run);
  let corrected = params.raw !== "1" && !!base;
  let showAll = false;
  const open = new Set();
  root.innerHTML = `
    <section class="panel">
      <h2>Leagues</h2>
      ${intro("Overall ranking at one input size, across all six input patterns. Shorter bars are faster.",
        "Score = geometric mean, over the six input patterns, of the algorithm's time divided by the fastest algorithm in the league for that pattern. ×1 means fastest on every pattern. The tick marks the algorithm's worst pattern, because a good average can hide an adversarial one.",
        "A league holds only the algorithms measured at that size. The reference sort (DotnetSort) is not ranked. Drift correction scales each benchmark class by one factor so that its reference matches the reference in Partition.")}
      <div class="filters">
        <div class="fl wide"><span>League</span><div class="seg" role="group" id="l-n">${leagueSizes.map(s => `<button type="button" data-v="${s}" aria-pressed="${s === n}">n = ${s}</button>`).join("")}</div></div>
        <label class="check"><input type="checkbox" id="l-corr"${corrected ? " checked" : ""}${base ? "" : " disabled"}> Correct drift between benchmark classes</label>
      </div>
      <div id="l-c"></div>
      <div id="l-more"></div>
      <div id="l-note" class="note"></div>
    </section>`;

  const scores = () => {
    const cats = run.categories.filter(c => run.sizesOf(c).includes(n));
    const factors = new Map(cats.map(c => [c, corrected ? anchorFactor(run, c, base) : 1]));
    const list = cats.flatMap(c => run.methodsOf(c).filter(m => m !== run.reference).map(m => ({ c, m, rows: PATTERNS.map(p => run.get(c, m, n, p)) })))
      .filter(d => d.rows.every(Boolean))
      .map(d => ({ ...d, adj: d.rows.map(r => r.mean * (factors.get(d.c) ?? 1)) }));
    const best = PATTERNS.map((p, i) => d3.min(list, d => d.adj[i]));
    for (const d of list) {
      d.ratios = d.adj.map((v, i) => ({ p: PATTERNS[i], v: v / best[i] }));
      d.score = geo(d.ratios.map(r => r.v));
      d.worst = d.ratios.reduce((a, b) => b.v > a.v ? b : a);
      d.cvMax = d3.max(d.rows, r => r.cv);
    }
    const drift = corrected ? cats.filter(c => c !== base && factors.get(c) != null).map(c => ({ c, f: factors.get(c) })) : [];
    return { list: list.sort((a, b) => a.score - b.score), cats, drift };
  };

  const draw = () => {
    setParams({ n, raw: corrected ? undefined : "1" });
    const { list, cats, drift } = scores();
    const missing = corrected ? cats.filter(c => c !== base && !run.methodsOf(c).includes(run.reference)) : [];
    root.querySelector("#l-note").innerHTML = `${list.length} algorithms from ${cats.length} categories: ${cats.map(esc).join(", ")}.` +
      (corrected && drift.length ? ` Drift correction against ${esc(base)}: ${drift.map(d => `${esc(d.c)} ${d.f >= 1 ? "+" : "−"}${pct(Math.abs(d.f - 1))}`).join(", ")}.` : "") +
      (missing.length ? ` No reference in ${missing.map(esc).join(", ")}; those classes are not corrected.` : "");
    const shown = showAll ? list : list.slice(0, 20);
    const x = d3.scaleLog().domain([1, Math.max(2, d3.max(shown, d => d.worst.v) * 1.1)]).range([0, 100]);
    const at = v => x(v).toFixed(2) + "%";
    let h = `<div class="lg" role="list"><div class="lg-row head" aria-hidden="true"><span class="lg-bar axis">${logTicks(x, 6).map(v => `<span style="left:${at(v)}">×${v}</span>`).join("")}</span></div>`;
    shown.forEach((d, i) => {
      const id = d.c + "|" + d.m;
      h += `<div class="lg-row${open.has(id) ? " open" : ""}" role="listitem" tabindex="0" aria-expanded="${open.has(id)}" data-id="${esc(id)}">
        <span class="lg-rk">${i + 1}</span>
        <span class="lg-nm"><b>${esc(d.m)}</b><span class="cat">${esc(d.c)}</span></span>
        <span class="lg-bar" role="img" aria-label="score ${fmtX(d.score)}, worst ${fmtX(d.worst.v)} on ${PATTERN_SHORT[d.worst.p]}"><b style="width:${at(d.score)}"></b><i style="left:calc(${at(d.worst.v)} - 1px)"></i></span>
        <span class="lg-sc">${fmtX(d.score)}</span>
        <span class="lg-wr">worst ${fmtX(d.worst.v)} ${PATTERN_SHORT[d.worst.p]}</span>
        <span class="lg-det"><span class="cat sm-only">${esc(d.c)}</span>${d.ratios.map(r => `<span>${PATTERN_SHORT[r.p]} ${fmtX(r.v)}</span>`).join("")}<span>max CV ${cvBadge(d.cvMax)}</span></span>
      </div>`;
    });
    root.querySelector("#l-c").innerHTML = h + "</div>";
    const more = root.querySelector("#l-more");
    more.innerHTML = list.length > 20 ? `<button class="btn" type="button">${showAll ? "Show top 20" : `Show all ${list.length}`}</button>` : "";
    const btn = more.querySelector("button");
    if (btn) btn.onclick = () => { showAll = !showAll; draw(); };
  };

  const toggle = row => {
    const id = row.dataset.id;
    if (open.has(id)) open.delete(id); else open.add(id);
    row.classList.toggle("open", open.has(id));
    row.setAttribute("aria-expanded", String(open.has(id)));
  };
  const host = root.querySelector("#l-c");
  host.addEventListener("click", e => { const row = e.target.closest(".lg-row[data-id]"); if (row) toggle(row); });
  host.addEventListener("keydown", e => {
    const row = e.target.closest(".lg-row[data-id]");
    if (row && (e.key === "Enter" || e.key === " ")) { e.preventDefault(); toggle(row); }
  });
  root.querySelector("#l-n").addEventListener("click", e => {
    const b = e.target.closest("button");
    if (!b) return;
    root.querySelectorAll("#l-n button").forEach(x => x.setAttribute("aria-pressed", String(x === b)));
    n = +b.dataset.v; draw();
  });
  root.querySelector("#l-corr").addEventListener("change", e => { corrected = e.target.checked; draw(); });
  draw();
}
