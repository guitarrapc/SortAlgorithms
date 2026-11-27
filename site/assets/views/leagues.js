import { d3, PATTERNS, PATTERN_SHORT, fmtX, esc, logTicks, geo, anchorBase, anchorFactor, pct, cvBadge, leagueSizes as sizesForLeagues, defaultLeague, SCALING, homeOf, shortN, intro, foldFilters, KEYED } from "../lib.js";

// Overall ranking per size. Each league holds only algorithms measured at that size, so
// O(n log n) and linear sorts are never ranked against each other at a size too small to separate them.
// Times are divided by the reference sort, a baseline readers recognize, instead of by the fastest algorithm,
// which a reader would first have to look up. Rows are HTML so they reflow to any width; selecting a row
// shows its ratio for every pattern.
export function render(root, { run, params, setParams }) {
  const leagueSizes = sizesForLeagues(run);
  let n = leagueSizes.includes(+params.n) ? +params.n : defaultLeague(run);
  const base = anchorBase(run);
  const ref = run.reference;
  // A row measured by the Scaling class names the class the algorithm belongs to.
  const cat = d => d.c === SCALING ? homeOf(run, d.m) : d.c;
  let corrected = params.raw !== "1" && !!base;
  // Distribution sorts need integer keys, so a reader choosing a general-purpose sort can leave them out.
  let comparisonOnly = params.cmp === "1";
  let showAll = false;
  const open = new Set();
  root.innerHTML = `
    <section class="panel">
      <h2>Leagues</h2>
      ${intro(`Overall ranking at one input size, across all six input patterns. Left of the ×1 line is faster than ${esc(ref)}.`,
        `Score = geometric mean, over the six input patterns, of the algorithm's time divided by ${esc(ref)}'s time for that pattern. ×0.5 means half the time of ${esc(ref)} on average. The tick marks the algorithm's worst pattern, because a good average can hide an adversarial one. Select a row to see every pattern.`,
        `A league holds only the algorithms measured at that size. ${esc(ref)} stays at the top with the scale while the list scrolls, and is not ranked; a dashed line marks where it would rank. Every benchmark class measures it; ratios use the measurement in ${esc(base ?? "Partition")}, or in ${SCALING} at the sizes only that class measures. Drift correction scales each class by one factor so that its ${esc(ref)} matches that one.`)}
      <div class="filters">
        <div class="fl wide"><span>League (n)</span><div class="seg" role="group" id="l-n">${leagueSizes.map(s => `<button type="button" data-v="${s}" aria-pressed="${s === n}"><span class="sm-hide">n = </span>${shortN(s)}</button>`).join("")}</div></div>
        <label class="check"><input type="checkbox" id="l-cmp"${comparisonOnly ? " checked" : ""}> Comparison sorts only</label>
        <label class="check" data-sum="Drift corrected"><input type="checkbox" id="l-corr"${corrected ? " checked" : ""}${base ? "" : " disabled"}> Correct drift between benchmark classes</label>
      </div>
      <div class="legend" id="l-k"></div>
      <div id="l-c"></div>
      <div id="l-more"></div>
      <div id="l-note" class="note"></div>
    </section>`;
  foldFilters(root.querySelector(".panel"));

  const scores = () => {
    const cats = run.categories.filter(c => run.sizesOf(c).includes(n));
    const factors = new Map(cats.map(c => [c, corrected ? anchorFactor(run, c, base) : 1]));
    const all = cats.flatMap(c => run.methodsOf(c).filter(m => m !== ref).map(m => ({ c, m, rows: PATTERNS.map(p => run.get(c, m, n, p)) })))
      .filter(d => d.rows.every(Boolean));
    const keyed = all.filter(d => cat(d) === KEYED).length;
    const list = all.filter(d => !comparisonOnly || cat(d) !== KEYED)
      .map(d => ({ ...d, adj: d.rows.map(r => r.mean * (factors.get(d.c) ?? 1)) }));
    if (!list.length) return { list, cats, drift: [], byRef: false, fastest: [], keyed };
    // The reference of the base class, or of the class that measures this size; older imported runs lack it
    // at some sizes, and those leagues fall back to the fastest per pattern.
    const refCat = [base, ...cats.filter(c => c !== base)].find(c => c && PATTERNS.every(p => run.get(c, ref, n, p)));
    const byRef = !!refCat;
    const fastest = PATTERNS.map((p, i) => d3.least(list, d => d.adj[i]));
    const denom = byRef ? PATTERNS.map(p => run.get(refCat, ref, n, p).mean * (factors.get(refCat) ?? 1)) : fastest.map((d, i) => d.adj[i]);
    for (const d of list) {
      d.ratios = d.adj.map((v, i) => ({ p: PATTERNS[i], v: v / denom[i] }));
      d.score = geo(d.ratios.map(r => r.v));
      d.worst = d.ratios.reduce((a, b) => b.v > a.v ? b : a);
      d.cvMax = d3.max(d.rows, r => r.cv);
    }
    const drift = corrected ? cats.filter(c => c !== base && factors.get(c) != null).map(c => ({ c, f: factors.get(c) })) : [];
    return { list: list.sort((a, b) => a.score - b.score), cats, drift, byRef, refCat, keyed, fastest: fastest.map((d, i) => ({ p: PATTERNS[i], d, v: d.adj[i] / denom[i] })) };
  };

  const draw = () => {
    setParams({ n, raw: corrected ? undefined : "1", cmp: comparisonOnly ? "1" : undefined });
    const { list, cats, drift, byRef, refCat, fastest, keyed } = scores();
    const legend = root.querySelector("#l-k");
    if (!list.length) {
      legend.innerHTML = "";
      root.querySelector("#l-c").innerHTML = `<p class="empty">No algorithm has all six patterns at n = ${n}.</p>`;
      root.querySelector("#l-more").innerHTML = root.querySelector("#l-note").innerHTML = "";
      return;
    }
    legend.innerHTML = `<span><span class="sw bar"></span>Score</span><span><span class="sw tick"></span>Worst pattern</span>` +
      `<span><span class="sw one"></span>×1 = ${byRef ? `${esc(ref)} (.NET Span.Sort)` : "fastest for each pattern"}</span>`;
    const missing = corrected ? cats.filter(c => c !== base && !run.methodsOf(c).includes(ref)) : [];
    const shownCats = cats.filter(c => list.some(d => d.c === c));
    const shownDrift = drift.filter(d => shownCats.includes(d.c));
    root.querySelector("#l-note").innerHTML = (byRef ? "" : `This run has no ${esc(ref)} at n = ${n}, so ratios are against the fastest algorithm for each pattern. `) +
      `Fastest per pattern: ${fastest.map(f => `${PATTERN_SHORT[f.p]} <b>${esc(f.d.m)}</b> ${fmtX(f.v)}`).join(", ")}. ` +
      (cats.length === 1 && cats[0] === SCALING
        ? `${list.length} algorithms, measured at this size by the ${SCALING} class; each row names its own class. Ratios use the ${esc(ref)} of that class.`
        : `${list.length} algorithms from ${shownCats.length} categories: ${shownCats.map(esc).join(", ")}.` + (byRef && refCat !== base ? ` Ratios use the ${esc(ref)} of ${esc(refCat)}.` : "")) +
      (corrected && shownDrift.length ? ` Drift correction against ${esc(base)}: ${shownDrift.map(d => `${esc(d.c)} ${d.f >= 1 ? "+" : "−"}${pct(Math.abs(d.f - 1))}`).join(", ")}.` : "") +
      (missing.length ? ` No reference in ${missing.map(esc).join(", ")}; those classes are not corrected.` : "") +
      (keyed ? (comparisonOnly ? ` ${keyed} ${KEYED} sorts are hidden: they read integer keys instead of comparing.` : ` ${keyed} of these are ${KEYED} sorts, which read integer keys instead of comparing.`) : "");

    const shown = showAll ? list : list.slice(0, 20);
    const values = [1, ...shown.map(d => d.score), ...shown.map(d => d.worst.v)];
    const x = d3.scaleLog().domain([byRef ? d3.min(values) / 1.15 : 1, Math.max(byRef ? 1.2 : 2, d3.max(values) * 1.1)]).range([0, 100]);
    const at = v => x(v).toFixed(2) + "%";
    const tick = v => `<span style="left:${at(v)}${x(v) < 4 ? ";transform:none" : x(v) > 96 ? ";transform:translateX(-100%)" : ""}">×${v}</span>`;
    // Bars grow from the ×1 line: to the left when faster than the baseline, to the right when slower.
    const span = v => `left:${at(Math.min(v, 1))};width:${(x(Math.max(v, 1)) - x(Math.min(v, 1))).toFixed(2)}%`;
    const row = (d, rank) => {
      const id = d.c + "|" + d.m;
      return `<div class="lg-row${open.has(id) ? " open" : ""}" role="listitem" tabindex="0" aria-expanded="${open.has(id)}" data-id="${esc(id)}">
        <span class="lg-rk">${rank}</span>
        <span class="lg-nm"><b>${esc(d.m)}</b><span class="cat">${esc(cat(d))}</span></span>
        <span class="lg-bar" role="img" aria-label="score ${fmtX(d.score)}, worst ${fmtX(d.worst.v)} on ${PATTERN_SHORT[d.worst.p]}"><b style="${span(d.score)}"></b><i style="left:calc(${at(d.worst.v)} - 1px)"></i></span>
        <span class="lg-sc">${fmtX(d.score)}</span>
        <span class="lg-wr"><span class="sm-hide">worst </span>${fmtX(d.worst.v)} ${PATTERN_SHORT[d.worst.p]}</span>
        <span class="lg-det"><span class="cat sm-only">${esc(cat(d))}</span>${d.ratios.map(r => `<span>${PATTERN_SHORT[r.p]} ${fmtX(r.v)}</span>`).join("")}<span>max CV ${cvBadge(d.cvMax)}</span></span>
      </div>`;
    };
    // The axis and the reference stay at the top of the list while it scrolls, so every row can be read against them.
    const refRow = byRef ? `<div class="lg-row ref" role="listitem"><span class="lg-rk"></span><span class="lg-nm"><b>${esc(ref)}</b><span class="cat">reference</span></span>` +
      `<span class="lg-bar" aria-hidden="true"></span><span class="lg-sc">${fmtX(1)}</span><span class="lg-wr">.NET Span.Sort</span></div>` : "";
    const rows = shown.map((d, i) => row(d, i + 1));
    // Where the reference would rank, the list is marked; the note gives the position when it is below the rows shown.
    const pos = list.filter(d => d.score < 1).length;
    if (byRef && pos > 0 && pos < shown.length) rows.splice(pos, 0, `<div class="lg-cut" aria-hidden="true">↑ faster than ${esc(ref)} · slower ↓</div>`);
    if (byRef) root.querySelector("#l-note").insertAdjacentHTML("afterbegin", `${esc(ref)} falls ${pos === 0 ? "above #1" : pos === list.length ? `below #${pos}` : `between #${pos} and #${pos + 1}`}. `);
    root.querySelector("#l-c").innerHTML = `<div class="lg" role="list" style="--one:${at(1)}"><div class="lg-top"><div class="lg-row head" aria-hidden="true"><span class="lg-bar axis">${logTicks(x, 7).map(tick).join("")}</span></div>${refRow}</div>${rows.join("")}</div>`;
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
  root.querySelector("#l-cmp").addEventListener("change", e => { comparisonOnly = e.target.checked; draw(); });
  draw();
}
